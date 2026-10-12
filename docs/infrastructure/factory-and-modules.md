# 工厂与模块

> 上级：[基础设施](README.md) ｜ 源码：`src/Api/Factory.cs`、`EntityModuleConfig.cs`
> 相关测试：`FactoryTests`、`JsonConverterTests`

## 一句话结论

`Factory` 解决「同一份逻辑、不同数值」：先查注册的委托，**查不到就静默回退成默认实例**。
这个回退让配置错误表现为「对象字段全是默认值」，而不是异常。

## 两类委托

| 委托 | 签名要点 | 用于 |
| --- | --- | --- |
| `EntityFactoryDelegate<T>(long id, string name, Dictionary<string, object> args)` | `T?`，返回 `null` 表示「不是我的」 | `Character` / `Skill` / `Item` / `Inventory` / `Room` / `User` |
| `EffectFactoryDelegate(long id, string name, Skill skill, Dictionary<string, object>? args)` | 特效多一个宿主技能参数 | **所有特效** |

```csharp
Factory.OpenFactory.RegisterFactory(custom);     // 注册
Factory.OpenFactory.UnRegisterFactory(custom);   // 注销（测试务必放 finally）
```

## 回退规则

| 请求类型 | 没注册时得到 |
| --- | --- |
| `Character` | `new Character()` |
| `Skill` | `new OpenSkill(id, name, args)` |
| `Item` | `new OpenItem(id, name, args)` |
| `Inventory` | `new Inventory(new User())` |
| `Room` / `User` | `new Room()` / `new User()` |
| **`Effect`（泛型路径）** | **抛 `NotSupportedInstanceClassException`** |
| 其他未知类型 | 抛异常 |

> **特效必须走专用委托**：`Factory.OpenFactory.GetInstance(id, name, skill, args)`。
> 用泛型 `GetInstance<Effect>(...)` 会直接抛（错误码 `#10037`）—— 因为特效需要宿主技能才能构造。

```csharp
Skill skill = new OpenSkill(2001, "木杖", []);
Effect effect = Factory.OpenFactory.GetInstance(1001, "", skill, new() { { "exatk", 20 } });
skill.Effects.Add(effect);
```

## 模块 JSON

| 入口 | 用途 |
| --- | --- |
| `Factory.GetGameModuleInstances<T>(module_name, file_name)` | 读取模块配置 → `Dictionary<string, T>` |
| `Factory.CreateGameModuleEntityConfig<T>(module_name, file_name, dict)` | 反向导出配置 |

内部走 `EntityModuleConfig<T>`。JSON 里的枚举写**整数**，未识别的属性进 `Effect.Values`
（见 [JSON 与转换器](json.md)）。仓库里的完整示例见
`src/Library/Module/Example/ExampleItem.cs` 的 `ExampleOpenItemByJson.CreateAJsonItem()`。

`Library/Module/` 下另有 `CharacterModule` / `SkillModule` / `ItemModule` 三个模块基类，
以及 `Example/` 里可直接参照的实现（非指向性技能、基于攻击力的伤害特效、被动技能、爆发技、装备品）。

## 坑

1. **注册顺序影响结果**：`GetInstance` 命中第一个返回非空值的委托，注册多个同 id 工厂时结果不确定——请让委托用 `id` 判归属。
2. 委托内部抛异常不会被打包成框架异常，而是原样向上抛（`catch { throw; }`），栈里会直接看到你的代码。
3. 注册是**全局单例**状态：测试里注册后必须在 `finally` 注销，且该项目关闭并行
   （`[assembly: CollectionBehavior(DisableTestParallelization = true)]`）。
4. 模块加载失败会抛 `ReadConfigException` / `Addon*Exception`，注意区分「文件没找到」与「JSON 类型不符」。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `GetInstance_WithoutRegistrationFallsBackToDefaultTypes` | `Character` / `Item` → `OpenItem` / `Skill` → `OpenSkill` |
| `GetInstance_RejectsEffectWithoutDedicatedFactory` | 泛型路径抛 `NotSupportedInstanceClassException`（`#10037`） |
| `RegisterFactory_TakesPrecedenceOverFallback` | 委托返回值优先于回退；注销后恢复回退 |
| `RegisterFactory_SupportsEntityDelegates` | 实体委托生效（自定义 `Item` 名字与开关） |
| `OpenFactory_IsASingleton` | `Factory.OpenFactory` 与 `General.OpenFactory` 同一实例 |
