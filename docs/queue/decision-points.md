# 决策点

> 上级：[队列与回合](README.md) ｜ 源码：`src/Model/Framework/DecisionPoints.cs`、`Model/Framework/EquilibriumConstant.cs`
> 相关测试：`DecisionPointsTests`、`ChainActionRegressionTests`

## 一句话结论

决策点是「本回合还能做几件事」的货币：存量的 `CurrentDecisionPoints` 决定能不能花，配额
`ActionTypes` / `this[type]` 决定**同一类动作能做几次**。两者都要过。

## 三个数字

| 概念 | 字段 | 默认值 | 说明 |
| --- | --- | --- | --- |
| 初始/上限/每回合补充 | `InitialDecisionPoints` / `MaxDecisionPoints` / `RecoverDecisionPointsPerRound` | 1 / 7 / 1 | 来自 `General.GameplayEquilibriumConstant` |
| 存量 | `CurrentDecisionPoints` | = `InitialDecisionPoints` | 动作发生时扣减 |
| 配额 | `this[CharacterActionType]` | 见下表 | 与该类型**已用次数**比较 |

## 配额（`this[type]`）

| 动作类型 | 配额构成 |
| --- | --- |
| `NormalAttack` / `CastSkill` / `CastSuperSkill` / `UseItem` | 对应 `ActionQuota*` + 该类型临时加成 + **临时全能加成** |
| `PreCastSkill`（魔法） | 仅 `ActionQuotaMagic` —— **不吃任何临时配额** |
| `ChainAction` | `int.MaxValue`（无限） |
| 其他（`Move` / `None` …） | `ActionQuotaOther`，**默认为 0** |

默认配额表：普攻/战技/爆发技/魔法/物品 = `1/1/1/1/1`，其他 = `0`。

> **`PreCastSkill` 是个例外**：魔法配额不叠加临时全能加成。想给魔法加次数，只能改
> `EquilibriumConstant.ActionQuotaMagic`，或换实现思路。

## 成本（`GetActionPointCost`）

| 调用形态 | 取值 |
| --- | --- |
| `CastSkill` + `chainAction: true` | `DecisionPointsCostSkill`（协同按正常成本算） |
| `CastSuperSkill`（无 skill、非协同） | `DecisionPointsCostSuperSkillOutOfTurn` = 3（回合外预释放） |
| `PreCastSkill` + skill 类型 | 按 `SkillType` 取 `Magic` / `Skill` / `SuperSkill` 成本 |
| 其余 | 对应动作的 `DecisionPointsCost*` |

默认成本：普攻 1、战技 2、爆发技 2、燃烧外 3、魔法 2、物品 1、其他 1、协同 1。

## 记一笔：`AddActionType` vs `ConsumeChainAction`

| 方法 | 改 `ActionsTaken` | 改 `DecisionPointsCost` | 改 `ActionsHardnessTime` | 改 `ActionTypes` / `CurrentDecisionPoints` |
| --- | :---: | :---: | :---: | :---: |
| `AddActionType(type, skill, addActionTaken)` | ✅ | ✅（累加成本） | 由调用方自行 `Add` | ✅ |
| `ConsumeChainAction(type, skill)` | ❌ | ❌ | ❌ | ✅ |

**这张表是协同行动的核心不变量**：B 在 A 的回合里出手，只能消耗 B 自己的预算与配额，
不得污染 B 上一个正常回合的硬直统计与行动次数。

## 临时配额

`AddTempActionQuota(effect, type, add)` 以**特效为键**累加（同一特效重复加会叠加），
`TempActionQuota*` 是各字典的和。回合结束时由 `ClearTempActionQuota()` 统一清空
（`ProcessTurn` 的 `finally` 里调用）——**别把跨回合配额塞进临时配额**。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `DecisionPointsTests.Indexer_ReturnsQuotaPerActionType` | 各类型配额取值、`ChainAction == int.MaxValue` |
| `GetActionPointCost_MatchesActionType` | 全部类型-成本映射（含回合外爆发技 3） |
| `GetActionPointCost_ChainActionUsesNormalCost` | `chainAction: true` 走正常成本 |
| `ConsumeChainAction_KeepsTurnLocalCountersUntouched` | `ActionsTaken` / `DecisionPointsCost` / `ActionsHardnessTime` 均不变 |
| `CheckActionTypeQuota_StopsAtQuotaButChainActionIsUnlimited` | 配额耗尽后拒绝，协同恒可 |
| `TempActionQuota_AddsToQuotaThenClears` | 累加与清空，清空后回到基线 |
| `ChainActionRegressionTests.ValidationOfResourcesQuotaOwnershipAndTargets` | 提交/确认两段都重验预算与配额 |
