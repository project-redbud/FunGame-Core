# 回合记录

> 上级：[队列与回合](README.md) ｜ 源码：`src/Model/Framework/RoundRecord.cs`、`ActionRecord.cs`、`Entity/Character/AssistDetail.cs`
> 相关测试：`ChainActionRegressionTests.RecordOwnershipAssistSnapshotAndJson`

## 一句话结论

记录是**不可变快照**：每回合新建 `LastRound`，上一回合以 `Snapshot()` 归档进 `Rounds`；
快照只深拷贝集合，实体本身仍按引用共享。

## 三个持有者

| 成员 | 生命周期 |
| --- | --- |
| `LastRound` | 当前回合，动作执行时持续写入 |
| `CurrentAction` | 当前动作（一条 `ActionRecord`） |
| `Rounds` | 历史归档；在 `NextCharacter()` 中当 `TotalRound > 1` 时追加上一回合的快照 |

`Checkpoint`（`CharacterStateSnapshot` 列表）只在总第 1 回合建立，供回放端从任意回合重建状态。

## `Snapshot()` 的边界

**深拷贝**：`Actions`、`Targets`、`Damages`、`DamageDetails`、`Effects`、`ApplyEffects`、`Inquiries`、
`Assists`、`RoundRewards` 等集合与字典；`Dictionary` 的**值**（如 `Dictionary<Character, ...>` 的键）仍是实体引用。

**不拷贝**：`ActionRecord.Actor` / `Skill` / `Item` / `Targets` 里的 `Character` 等实体。

> **后果**：往历史回合的**集合**里塞东西不会影响当前回合；但改这些实体自身的字段（`HP`、`CharacterState`、
> `Skill.CurrentCD`）会同时改变历史记录看到的值。回放端要「冻结」实体状态，靠的是 `Checkpoint`。

## 动作归属（协同行动的关键）

| 记录 | `Actor` | `TurnActor` | `ChainActor` | `ChainActionRequestId` |
| --- | --- | --- | --- | --- |
| 普通动作 | 执行者 | 执行者（或省略） | — | `null` |
| 协同确认 | A（回合持有者） | — | — | 申请 ID |
| 协同执行 | **B（真正出手者）** | A | B | 与确认**同一个** ID |

资源与战绩归 B（`Kills`、消耗），A 通过 `NonDamageAssistTargets` 获得非伤害助攻关联。
两段记录共享 `ChainActionRequestId` 是回放时把「谁确认的」与「谁打的」拼起来的唯一线索。

## 一个容易误判的点：特效触发记录

`RoundRecord.AddEffectTriggered(character, skill)` 内部调用 `EffectHookCache.IsOverridden(...)`，
**只记录被重写过的钩子**。这是刻意的性能设计：没重写就等于没有行为，不必入账。

> 所以「回合记录里看不到某内置钩子触发」不是 bug。要让它可见，得在特效里显式重写该钩子。

## 统计与辅助

- `CharacterStatistics`：`Kills` / `Deaths` / `Assists` / `ActionTurn` / `UseDecisionPoints` / `TurnDecisions` /
  `TotalDamage` 及其分类型拆分、`Avg*` 系列。注意**没有** `DamageDealt` 字段，对应概念是 `TotalDamage`。
- `AssistDetail`（由队列持有）：`Damages`、`DamageLastTime`、`NotDamageAssistLastTime` 三张表，
  以「对某敌人的贡献」为粒度；`GetNotDamageAssistLastTime()` 无记录时返回 `double.MinValue` 而非 0。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `ChainActionRegressionTests.RecordOwnershipAssistSnapshotAndJson` | 共享 ID、归属、辅助集合、快照独立、JSON 往返 |
| `ChainActionRegressionTests.LethalChainActionCreditsActorAndAssist` | B 得击杀、A 得助攻 |
| `ChainActionRegressionTests.ChainMagicCastingPaysMpOnReleaseWithoutRefreshingBudget` | `ActionTurn` 不因协同吟唱结算而累计 |
| `RoundRecordTests.Snapshot_CopiesCollectionsButKeepsEntityReferences` | 集合独立、实体同引用 |
| `ActionQueueTests.NextCharacter_ArchivesPreviousRoundOnNewTurn` | 归档时机与 `Round`、`Seed` 字段 |
