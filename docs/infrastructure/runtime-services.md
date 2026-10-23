# 运行时服务

> 上级：[基础设施](README.md) ｜ 源码：`src/Library/Constant/`、`Model/EventCenter.cs`、`Api/RoundRecordRenderer.cs`、`BattleStatePredictor.cs`、`Library/Exception/`
> 相关测试：`ConstantSetTests`、`EventCenterTests`、`RoundRecordTests`、`ExceptionTests`

## 全局平衡常数

`General.GameplayEquilibriumConstant` 是**全局可写的单例**，而 `BaseEntity` 与 `DecisionPoints` 的
`GameplayEquilibriumConstant` 属性**默认指向它**：

```csharp
public EquilibriumConstant GameplayEquilibriumConstant { get; set; } = General.GameplayEquilibriumConstant;
```

> **两种改法，后果不同**：
> - 改**属性**（`character.GameplayEquilibriumConstant = new EquilibriumConstant { … }`）→ 只影响该实体；
> - 改**单例的字段**（`General.GameplayEquilibriumConstant.InitialHP = 0`）→ 影响之后创建的所有实体。
>
> 测试与模组请用第一种：换实例，别就地改字段。

## 名称映射家族（`Library/Constant/ConstantSet.cs`）

| 类 | 提供 |
| --- | --- |
| `SkillSet` | 技能类型 / 效果类型的**名称**、`GetSkillMaxLevel(type, constant)`；效果类型 → `CharacterState` / `DispelledType` / 豁免属性 / 是否 debuff 的映射 |
| `ItemSet` | 物品类型 / 武器类型 / 装备槽 / 品质的名称双向转换 |
| `CharacterSet` | 伤害类型、魔法类型、免疫类型、角色状态、定位、稀有度的展示名 |
| `RoomSet` | 房间模式名称与 `RoomType` 互转 |
| `CommonSet` / `UserSet` / `GamingSet` | 状态前缀、预留用户 Id 与名称、游戏类型 |

约定：`GetXxxName(T)` 与 `GetXxxFromName(string)` **成对往返**（测试里对所有枚举取值做了闭环校验）。
新增枚举成员时两边都要补，否则往返会拿到默认值。

## 事件中心

`EventCenter.Instance` 是单例注册表：

| 成员 | 说明 |
| --- | --- |
| `Events` / `this[string]` | 事件名 → `HashSet<Activity>`；**索引器读不存在的名字返回空集合**（不抛） |
| `Stores` / `GetStore` | 商店注册表 |
| `AddActivity(eventName, activity)` | ⚠️ 事件名**首次**出现时返回 `false`，但活动**已经加进去了**——别只看返回值判断成败 |
| `AddOrUpdateEvent` | 整体替换该事件名下的集合 |
| `RemoveEvent` / `RemoveActivity` / `RemoveStore` | 返回是否真的删掉了 |

`Activity` 自己带 `UserAccess` / `UserGetActivityInfo` 两个事件，`EventCenter.Register*EventHandler` 会
遍历该事件名下的所有 `Activity` 挂上处理器。

## 钩子缓存

`Api/EffectHookCache`（`internal`）：反射扫描 `Effect` 派生类型，缓存「重写了哪些钩子」。
`ScanAllAssemblies()` 在模组加载时预填充，未命中时**懒计算**（兼容后加载的程序集）。

它只被用来决定**回合记录是否登记**该钩子的触发，不影响钩子是否被调用。
**给 `Effect` 基类新增钩子时无需登记**——缓存以基类的虚方法清单为全集自动识别。

## 诊断三件套

| 类型 | 用途 | 注意 |
| --- | --- | --- |
| `RoundRecordRenderer` | `RenderRound` / `RenderAll` / `RenderCharacterActions` | 纯渲染，不重跑引擎；无动作的回合渲染 `RenderCharacterActions` 会返回空串 |
| `BattleStatePredictor` | `PredictAll(rounds, targetRound)` / `Predict(…)` | **依赖 `Checkpoint`**：目标回合之前没有检查点时返回空字典 / `null` |
| `CharacterStateSnapshot` | 检查点载体（装备/物品/技能/状态栏） | 只在总第 1 回合建立，见[回合记录](../queue/round-record.md) |

`BattleStatePredictor` 基于检查点副本推算。副本保留背包 `Items` 和装备明细 `EquipmentsDetail` 的全部字段，
并逐项新建快照对象；修改预测结果中的物品或装备名称、描述及列表，不会回写检查点。

## 异常与工具

- 框架异常都在 `FunGame.Core` 命名空间下，`Message` 里带错误码（`#10001` 起，按顺序递增，如
  `NotSupportedInstanceClassException` 是 `#10037`）。
- `Library/Exception/ExceptionExtension.GetErrorInfo()` 在有内层异常时加上 `InnerExceoption: …` 前缀
  （原文拼写如此，别当自己是笔误）。
- `Calculation.Round` 用 `MidpointRounding.AwayFromZero`，并把 `|x| < 1e-10` 归零；
  `PercentageCheck` 把值钳到 `[0, 1]`——**公式里的百分比一律过它**。
- `NetworkUtility.IsEmail` 等正则通过 `[GeneratedRegex]` 编译；改正则等于改对外契约，记得同步测试。

## 坑

1. 全局常数被就地修改后**没有还原机制**：测试里改了它，同进程的其他用例会一起遭殃。
2. `EventCenter.AddActivity` 的返回值语义（新事件名 → `false` 但已添加）容易写成错误的断言。
3. `BattleStatePredictor` 不做「无检查点时的降级推算」，用之前先确认 `Checkpoint` 存在。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `SkillSet_ResolvesTypeNamesAndLevelCaps` / `ItemSet_RoundTripsItemWeaponAndSlotNames` | 全部枚举取值的名称往返 |
| `RoomAndCharacterSets_ExposeReadableNames` | 各映射函数不返回空串 |
| `AddActivity_RegistersIntoNamedEventCollection` | 活动入表、可按名取回、可移除 |
| `Indexer_ReturnsEmptySetForUnknownEvent` | 未知事件名返回空集合而非异常 |
| `AddOrUpdateAndRemoveEvent_ReplacesWholeCollection` | 整体替换语义 |
| `Stores_AreRegisteredByName` | 商店注册与移除 |
| `BattleStatePredictor_NeedsCheckpointToRestoreState` | 无检查点 → 空字典 / `null` |
| `BattleStatePredictorTests.Predict_CopiesInventoryAndEquipmentDetailsWithoutMutatingCheckpoint` | 背包、装备明细完整保留且与检查点独立 |
| `RenderRound_ProducesHeaderForEmptyRound` | 渲染非空；无动作角色渲染为空串 |
| `FrameworkExceptions_CarryErrorCodes` / `ExceptionExtension_WrapsInnerException` | 错误码与 `InnerExceoption` 前缀 |
| `CalculationTests` / `NetworkUtilityTests` / `EncryptionTests` | 工具方法的边界与已知向量 |
