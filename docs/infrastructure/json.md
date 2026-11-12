# JSON 与转换器

> 上级：[基础设施](README.md) ｜ 源码：`src/Api/JsonService.cs`、`JsonTool.cs`、`Library/Common/JsonConverter/`
> 相关测试：`JsonServiceTests`、`JsonConverterTests`

## 一句话结论

序列化走 **`JsonService.GeneralOptions` 这一份全局单例选项**；需要独立配置时用 `JsonTool` 实例，
但两者的默认行为并不完全一致——这是本模块最容易误判的地方。

## 全局选项里注册了什么

`GeneralOptions` 注册 27 个转换器（含日期与实体转换器），并设定：

| 设置 | 值 | 影响 |
| --- | --- | --- |
| `WriteIndented` | `true` | 产出可读的多行 JSON |
| `PropertyNameCaseInsensitive` | `true` | 默认对象映射时属性名大小写不敏感；手写转换器按自身规则匹配 |
| `Encoder` | `JavaScriptEncoder.Create(UnicodeRanges.All)` | 中文**不**转义 |
| `ReferenceHandler` | `ReferenceHandler.IgnoreCycles` | 循环引用被忽略而不是抛异常 |

> 它是共享的静态单例选项。首次序列化或反序列化前修改配置会影响全局；被序列化器使用后选项变为只读，再改列表或配置会抛 `InvalidOperationException`。需要独立配置时，在使用前配置自己的 `JsonTool`。

## `JsonService` 的入口一览

| 方法 | 用途 |
| --- | --- |
| `GetString<T>(obj)` / `GetObject<T>(json)` | 常规往返 |
| `GetObject<T>(ref Utf8JsonReader, options)` | **给转换器内部递归读取用**，不吞异常 |
| `GetObject<T>(Hashtable/Dictionary/IEnumerable, …)` | 从容器里取出 `JsonElement` 再反序列化；缺失键或空值返回 `default`，非 JsonElement 值可能抛 `InvalidCastException`，目标 JSON 类型不符可能抛 `JsonException` |
| `GetObjects<T>(json)` | 多个对象：自行补数组括号（见下） |
| `IsCompleteJson<T>(json)` | 试解析判断完整性，失败返回 `false` |

### `GetObjects<T>` 的入参形态

它内部会把入参规整成数组，因此支持三种写法：

```
{"Name":"甲"}{"Name":"乙"}      → 首尾相接（会按 }\s*{ 统一分隔）
{"Name":"甲"}
{"Name":"乙"}                   → 换行分隔也可以
[{"Name":"甲"},{"Name":"乙"}]   → 已经是数组，原样处理
""                              → 空数组
```

## `JsonTool` 与静态入口的差异

| | `JsonService`（静态） | `JsonTool`（实例） |
| --- | --- | --- |
| 选项 | `GeneralOptions` | 每个实例一份 `Options` 和转换器列表，默认列表中的转换器实例共享 |
| 默认对象映射的大小写敏感性 | **不敏感** | **敏感**（构造函数没拷该标志） |
| 加转换器 | 使用前的修改影响全局，使用后配置只读 | `AddConverter` / `AddConverters` 对同一转换器实例去重，只修改自己的列表，需在选项使用前配置 |

> 需要相同的默认对象映射行为时，在使用前设置 `tool.Options.PropertyNameCaseInsensitive = true`。
> `CharacterConverter`、`UnitConverter` 等手写读取器按 `switch (propertyName)` 精确匹配；该开关不会替它们改写键名。实体字段应使用转换器规定的 PascalCase。

## 转换器：两段式读取

所有实体转换器都继承 `BaseEntityConverter<T>`：

```csharp
T NewInstance();                                             // 惰性创建（遇到第一个属性名时才 new）
void ReadPropertyName(ref Utf8JsonReader, string name,
                      JsonSerializerOptions, ref T result, Dictionary<string, object> context);
void AfterConvert(ref T result, Dictionary<string, object> context);   // 读完整对象后兜底
```

两点约定值得注意：

1. **读取顺序不可靠**，所以「依赖其他属性才能赋值的字段」要放进 `convertingContext`，在 `AfterConvert` 里回填。
   `CharacterConverter` 的 `HP` / `MP` / `EP` 就是这么处理的（否则会被 `MaxHP` 的钳制吃掉）。
2. `Write` 是**手写**的：新增可序列化字段时，`Read` 与 `Write` 两边都要改，漏一边不会报错。

## 角色与单位属性

`CharacterConverter` 与 `UnitConverter` 共用字段读写及资源回填逻辑，分别创建 Character 和 Unit。
固定点数保存为 `ExSTR2` / `ExAGI2` / `ExINT2` / `ExATK2` / `ExDEF2` / `ExHP2` / `ExMP2`，比例保存为对应 `Ex*Percentage`。
属性聚合 `Ex*`、基础值 `Base*`、百分比派生 `Ex*3` 与四个 `FromEx*` 来源属性不写入配置 JSON。
当前 HP / MP / EP 在配置读取完成后回填，字段顺序不影响资源钳制。
`JsonService`、`NetworkUtility` 与使用默认转换器的 `JsonTool` 都使用此口径。
回合快照中的 MaxHP / MaxMP 和 Attributes 是测量结果与展示记录，按快照自身 schema 保存。

## 动态特效的数据通道：`Effect.Values`

`EffectConverter` 的 `default` 分支会把**转换器不认识的属性**按 JSON token 类型塞进 `Effect.Values`
（数字 → `double`，字符串 → `string`，布尔 → `bool`）。模组特效借此接收动态参数：

```
{ "Id": 2001, "Name": "木杖", "SkillType": 3, "Effects": [ { "Id": 1001, "exatk": 20 } ] }
                                                                    └─ 进入 Values["exatk"]
```

特效构造函数里读取并解析它，参见 `ExampleOpenEffectExATK2`（`Library/Module/Example/ExampleItem.cs`）。

> **枚举在 JSON 里是整数**：`SkillType.Passive = 3`、`ItemType.Weapon = 1`。写模块 JSON 时别写名字。

## 时间格式

`DateTimeConverter` 使用 `General.GeneralDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff"`（毫秒精度），
非字符串 token 或解析失败都会**抛 `JsonException`**（不静默兜底）。

## 坑

1. 转换器里递归读对象使用 `JsonService.GetObject<T>(ref reader, options)`，或等价地传入当前 options 的 `JsonSerializer.Deserialize<T>(ref reader, options)`；省略 options 才会丢失当前转换器配置。
2. `IsCompleteJson<T>` 是「能解析且类型匹配」，不是「JSON 语法合法」——字段类型不符也会返回 `false`。
3. 在共享选项使用前修改转换器列表会影响其他调用者，使用后则会因只读而失败；测试和模组应在独立选项使用前完成配置。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `GeneralOptions_AreCaseInsensitiveIndentedAndChineseFriendly` | 三个开关的实际取值 |
| `GetString_KeepsChineseCharactersUnescaped` | 中文不转义、带缩进 |
| `GetObjectFromHashtable_ReadsJsonElementAndFallsBackToDefault` | 缺失 key 返回 `default` 而非抛异常 |
| `GetObjects_AcceptsConcatenatedAndArrayInput` | 三种入参形态 + 空串 |
| `JsonTool_UsesIndependentCaseSensitiveOptions` | 实例选项**敏感**（与静态入口的差异） |
| `JsonTool_AddConverterIsIdempotent` | 重复注册不重复添加 |
| `CharacterJson_RoundTripKeepsResources` | `HP`/`MP`/`EP` 经 `AfterConvert` 回填后不丢 |
| `EffectConverter_StoresUnknownPropertiesInValues` | 未知属性进 `Values` |
| `ItemConverter_ReadsDynamicItemFromJson` | 动态物品 JSON → 技能 → 特效 → `Values["exatk"]` |
| `DateTimeConverter_RejectsInvalidPayload` | 非法日期抛 `JsonException` |
| `RoundRecordCostTests` | 消耗引用延迟解析、动作引用回退、平铺格式与同名冲突 |
| `CharacterAttributeSerializationTests` | 核心属性点数与比例、最终属性往返 |
| `UnitAttributeSerializationTests` | Character / Unit 同一属性 schema、三个入口和 User / Inventory 嵌套往返 |
