# 基础设施

> **面向**：要改 `src/Api/`、`src/Library/` 的开发者。
> **测试**：[`tests/FunGame.Core.Tests.Infrastructure`](../../tests/FunGame.Core.Tests.Infrastructure)（77 个用例）

## 本模块文档

| 文档 | 讲什么 |
| --- | --- |
| [JSON 与转换器](json.md) | 全局序列化选项、`JsonTool` 的差异、转换器基类的两段式读取、动态特效的 `Values` 通道 |
| [工厂与模块](factory-and-modules.md) | 工厂委托与静默回退、特效专用委托、模块 JSON 加载与导出 |
| [运行时服务](runtime-services.md) | 常量映射家族、事件中心、钩子缓存、诊断三件套 |

## 代码地图

| 位置 | 职责 |
| --- | --- |
| `Api/Factory.cs` | 实体/特效工厂与模块实例加载 |
| `Api/JsonService.cs`、`JsonTool.cs` | 序列化入口（静态全局 / 实例独立） |
| `Api/General.cs` | 工具箱：`NetworkUtility`、`Encryption`、`Verification`、`Calculation`、`DateTimeUtility` |
| `Api/EntityModuleConfig.cs` | 模块配置文件读写 |
| `Api/RoundRecordRenderer.cs`、`BattleStatePredictor.cs` | 回放渲染与状态推算 |
| `Api/Attribute.cs` | `[InitRequired]` / `[InitOptional]` 标记（用于初始化流程识别） |
| `Api/EffectHookCache.cs` | 特效钩子重写缓存（`internal`） |
| `Library/Constant/` | 枚举与常量、名称映射家族（`SkillSet` / `ItemSet` / `CharacterSet` / `RoomSet`） |
| `Library/Common/JsonConverter/` | 各实体的 `JsonConverter`（基类 `BaseEntityConverter<T>`） |
| `Library/Exception/` | 框架异常（带 `#100xx` 错误码）与 `GetErrorInfo()` |
| `Library/Module/` | `CharacterModule` / `SkillModule` / `ItemModule` 与 `Example/` 示例实现 |
| `Model/EventCenter.cs` | 活动 / 商店的全局注册表 |

## 坑速查

| 症状 | 原因 | 处理 |
| --- | --- | --- |
| 改了序列化行为，测试全乱 | `JsonService.GeneralOptions` 是**全局单例** | 用 `JsonTool` 实例的独立 `Options` |
| 实例反序列化的默认行为与静态入口不一致 | `JsonTool.Options` 没开 `PropertyNameCaseInsensitive` | 需要一致就显式设置 |
| 造对象时字段全是默认值 | 工厂**静默回退**默认实例，不抛异常 | 检查工厂注册与模块 JSON |
| `GetInstance<Effect>` 抛异常 | 泛型路径不支持 `Effect` | 用 `EffectFactoryDelegate` 专用入口 |
| `GetObjects` 抛 `JsonException` | 入参必须是「首尾相接的对象序列」或已是数组 | 见 [JSON 与转换器](json.md) |
| 构建后 `bin/` 多出 `.nupkg` | `GeneratePackageOnBuild = true` | 属正常现象 |

## 相关测试

| 测试类 | 守什么 |
| --- | --- |
| `CalculationTests`、`NetworkUtilityTests`、`EncryptionTests`、`VerificationTests`、`DateTimeUtilityTests`、`LinqExtensionTests` | 工具箱的边界与已知向量（如 MD5/SHA256） |
| `JsonServiceTests`、`JsonConverterTests` | 全局选项、往返、转换器回填与 `Values` 通道 |
| `FactoryTests`、`ConstantSetTests` | 工厂注册与回退、常量名称映射往返 |
| `EventCenterTests`、`RoundRecordTests`、`SkillExtensionTests` | 事件中心增删、快照、渲染与推算 |
