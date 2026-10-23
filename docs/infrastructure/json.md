# JSON 与转换器

> 上级：[基础设施](README.md) ｜ 源码：`src/Api/JsonService.cs`、`JsonTool.cs`、`Library/Common/JsonConverter/`
> 相关测试：`JsonServiceTests`、`JsonConverterTests`

## 一句话结论

序列化走 **`JsonService.GeneralOptions` 这一份全局单例选项**；需要独立配置时用 `JsonTool` 实例，
但两者的默认行为并不完全一致——这是本模块最容易误判的地方。

## 全局选项里注册了什么

`GeneralOptions` 一次性挂上 26 个实体转换器，并设定：

| 设置 | 值 | 影响 |
| --- | --- | --- |
| `WriteIndented` | `true` | 产出可读的多行 JSON |
| `PropertyNameCaseInsensitive` | `true` | 属性名大小写不敏感 |
| `Encoder` | `JavaScriptEncoder.Create(UnicodeRanges.All)` | 中文**不**转义 |
| `ReferenceHandler` | `ReferenceHandler.IgnoreCycles` | 循环引用被忽略而不是抛异常 |

> 它是静态只读单例：**就地改 `GeneralOptions.Converters` 会全局生效**（包括测试之间互相干扰）。
> 需要额外转换器时用 `JsonTool`。

## `JsonService` 的入口一览

| 方法 | 用途 |
| --- | --- |
| `GetString<T>(obj)` / `GetObject<T>(json)` | 常规往返 |
| `GetObject<T>(ref Utf8JsonReader, options)` | **给转换器内部递归读取用**，不吞异常 |
| `GetObject<T>(Hashtable/Dictionary/IEnumerable, …)` | 从容器里取出 `JsonElement` 再反序列化；缺失/类型不符返回 `default`，**不抛异常** |
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
| 选项 | `GeneralOptions` | 每个实例一份 `Options`（拷贝行为与转换器） |
| 大小写敏感 | **不敏感** | **敏感**（构造函数没拷该标志） |
| 加转换器 | 全局，影响所有人 | `AddConverter` / `AddConverters`，幂等且只影响自己 |

> 需要「和静态入口行为一致」时，显式设置 `tool.Options.PropertyNameCaseInsensitive = true`。

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

1. 转换器里递归读对象统一用 `JsonService.GetObject<T>(ref reader, options)`；直接 `JsonSerializer.Deserialize`
   会绕过当前选项与其它转换器。
2. `IsCompleteJson<T>` 是「能解析且类型匹配」，不是「JSON 语法合法」——字段类型不符也会返回 `false`。
3. 往 `GeneralOptions.Converters` 里加东西会**全局生效**，测试里尤其危险（并行时互相污染）。

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
