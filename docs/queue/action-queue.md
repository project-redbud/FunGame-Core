# 行动顺序表与时间流逝

> 上级：[队列与回合](README.md) ｜ 源码：`src/Model/Queue/GamingQueue.cs`
> 相关测试：`ActionQueueTests`、`ChainActionRegressionTests`

## 一句话结论

顺序表不是「轮次表」，而是**一条按剩余硬直升序的队列**：谁的时间先到 0，谁先动；没人到 0 就推进时间。

## 入队：`AddCharacter(character, hardnessTime, isCheckProtected = true)`

`GamingQueue.cs:636 AddCharacter()` 的完整副作用，逐条都有人踩过：

1. 若该角色没有决策点对象，先按队列常数建一个（`CreateInitialDecisionPoints()`）；
2. `character.GamingQueue = this`；
3. **先把该角色从队列移除**（`_queue.RemoveAll`）→ 重复入队不会产生两个副本，但硬直会被覆盖；
4. 硬直冲突消解：`ResolveConflict` 里 `while` 到唯一，每次 **+0.01**（`Round2Digits`）；
5. 插队保护（见下）；
6. 插入到「第一个硬直严格大于自己」的位置，末尾 `Sort` 兜底；
7. 给所有被插队者累计 `_cutCount`。

> **注意第 3 步的后果**：拿旧硬直做「加一点」的运算时，先读 `HardnessTime[character]` 再入队，
> 否则你读到的是刚被覆盖后的值。跨回合累计硬直请用 `ChangeCharacterHardnessTime`。

## 插队保护

| 字段 | 语义 |
| --- | --- |
| `UseQueueProtected` | 总开关，**默认 `false`** |
| `MaxCutQueueTimes` | `UseQueueProtected == false` → 返回 0（= 关闭）；`-1` → `max(5, 队列长度)`；`< 5` → 5 |

被插队次数达到阈值后，该角色「获得保护」：新角色被压到它后面一位，硬直取保护者硬直 `+0.01` 再消解冲突。
调试时可用 `-1` 快速复现保护路径，不要写死在业务里。

## 出队：`NextCharacter()`

`GamingQueue.cs:805 NextCharacter()` 的判定非常严格：**只有队首硬直恰好为 0 才出队**，否则先 `TimeLapse()` 再递归。

出队时会：
- `TotalRound++`；
- `TotalRound > 1` 时把上一回合 `LastRound.Snapshot()` 追加进 `Rounds`；
- 第 1 回合额外建立状态检查点（`Checkpoint`）并触发 `GameStartEvent`；
- 把角色从 `_queue` 移出，并清除它的 `_cutCount`。

队列为空返回 `null`——**调用方负责退出循环**，`NextCharacter` 自己不判断胜负。

## 推进：`TimeLapse()`

`GamingQueue.cs:860 TimeLapse()` 的推进量 = 队首硬直；若存在复活倒计时且更小，则取复活时间。
推进后统一结算全场：

1. 所有角色硬直减去推进量（负值告警并归 0），重新排序；
2. 回血回蓝：`HP += HR * elapsed`、`MP += MR * elapsed`（受特效 `BeforeApplyRecoveryAtTimeLapsing` 干预）；
3. 技能 CD 递减、特效持续时间递减、临时驱散状态恢复；
4. 统计 `LiveRound` / `LiveTime` / 各项 `DamagePer*`；
5. 复活倒计时结算与重新入队。

队列为空时返回 `0`（不会抛异常，可安全用于空队列探测）。

## 处理回合：`ProcessTurn(character)`

`GamingQueue.cs:1118 ProcessTurn()`：

- 返回值**只**表示「游戏是否已结束」（内部 `_isGameEnd`）；正常回合返回 `false`；
- 若 `_isInRound || _executingChainAction` 为真，直接抛 `InvalidOperationException("不能嵌套处理角色回合。")`
  ——**不要在事件钩子里回调 `ProcessTurn`**；
- `finally` 中清理本回合的临时决策点配额（见[决策点](decision-points.md)）。

回合内的决策顺序：`TurnStartEvent` →（可被事件接管）→ `DecideActionEvent` → AI 控制器 → 概率兜底
→ 目标选择事件 → 执行 → `TurnEndEvent`。

## 相关测试

| 用例 | 断言要点 |
| --- | --- |
| `ActionQueueTests.InitActionQueue_OrdersBySpeedDescending` | 速度降序 + 硬直 `0 / .1 / .2 / .3` |
| `ActionQueueTests.AddCharacter_ResolvesHardnessConflictWithEpsilon` | 同值硬直 → `1.01` |
| `ActionQueueTests.NextCharacter_AdvancesTimeUntilHeadIsReady` | 硬直 3 → `TotalTime == 3`，队首出队后队列空 |
| `ActionQueueTests.NextCharacter_ArchivesPreviousRoundOnNewTurn` | 第 2 次出队时 `Rounds` 出现 `Round == 1` 的快照 |
| `ActionQueueTests.ProcessTurn_RejectsNestedTurnProcessing` | 嵌套调用抛 `InvalidOperationException` |
| `ActionQueueTests.TurnStartEvent_CanTakeOverTheWholeTurn` | 事件返回 `false` → 跳过决策、无动作记录 |
| `ChangeCharacterHardnessTime_AppliesAbsoluteAndPercentageValue` | 绝对值/百分比、下限归 0、队列外角色被忽略 |
