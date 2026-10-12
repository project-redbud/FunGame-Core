# 队列与回合

> **面向**：要改 `src/Model/Queue/`、`Model/Framework/DecisionPoints.cs` 的开发者。
> **规则本身**（怎么玩）见文档站：[回合制系统](https://project-redbud.github.io/guide/turn-based)、[行动顺序表](https://project-redbud.github.io/guide/action-queue)。
> **测试**：[`tests/FunGame.Core.Tests.Queue`](../../tests/FunGame.Core.Tests.Queue)（70 个用例）

## 本模块文档

| 文档 | 讲什么 |
| --- | --- |
| [行动顺序表与时间流逝](action-queue.md) | 硬直排序、入队冲突、插队保护、`NextCharacter` / `TimeLapse` / `ProcessTurn` |
| [决策点](decision-points.md) | 预算、配额、行动成本、临时配额、协同预算隔离 |
| [回合记录](round-record.md) | `LastRound` / `Rounds`、快照语义、动作归属与辅助统计 |
| [协同行动](chain-action.md) | 队友/召唤物用剩余预算提前出手的规则、接入接口与验证方法 |

## 代码地图

`GamingQueue` 是 `partial class`，按职责拆分——**新增逻辑请放进对应的分部文件**：

| 文件 | 职责 |
| --- | --- |
| `GamingQueue.cs` | 回合循环（`NextCharacter` / `TimeLapse` / `ProcessTurn`）、行动顺序表、伤害与治疗的公开入口、事件声明 |
| `GamingQueue.ChainAction.cs` | 协同行动：申请、撤回、确认、失效判定 |
| `GamingQueue.EffectDispatch.cs` | 特效钩子分发（每个钩子一份手写触发器）+ 集合构建 |
| `GamingQueue.ActionExecution.cs` | 单个动作的执行与资源结算（技能/物品/普攻/移动/吟唱）、局内删物 |
| `GamingQueue.RoundReward.cs` | 回合奖励的获取、丢失与夺取 |
| `MixGamingQueue.cs` / `TeamGamingQueue.cs` | 混战 / 团队模式：胜负判定、队伍归属 |
| `../Framework/DecisionPoints.cs` | 决策点对象（预算、配额、成本） |
| `../Framework/GameMap.cs`、`Grid.cs` | 战棋地图与格子 |
| `../Framework/RoundRecord.cs`、`ActionRecord.cs` | 回合与动作记录（不可变快照的来源） |

## 坑速查

| 症状 | 原因 | 处理 |
| --- | --- | --- |
| `TimeLapse` 抛 `KeyNotFoundException` | 队列由**无参构造**创建，`_stats` 从未建立（`InitCharacters` 才建） | 用带角色列表的构造函数；测试夹具都这么做 |
| 改了硬直却没变化 | `ChangeCharacterHardnessTime` 对**不在队列中**的角色静默返回 | 先确认角色在 `Queue` 里；加完再加回去 |
| 以为「相同硬直」会保持相等 | `AddCharacter` 遇同值会 `+0.01` 递增到唯一 | 断言用容差，别比精确相等 |
| 插队保护完全不生效 | `UseQueueProtected` 默认 `false`，此时 `MaxCutQueueTimes` 返回 **0** | 显式打开；`-1` 表示取 `max(5, 队列长度)` |
| 历史回合记录被后续写入污染 | `Rounds` 里存的是 `Snapshot()`：**集合深拷贝、实体引用共享** | 改 `RoundRecord` 上的实体字段会污染历史；只改集合字段安全 |
| 测试里回合莫名提前结束 | `TurnStartEvent` 返回 `false` 表示「接管本回合」 | 事件处理器别随手返回 `false` |
| 重复 `AddCharacter` 出现两个同名角色 | 不会——`AddCharacter` 先 `RemoveAll` 再插入 | 但它会**重置该角色的硬直** |

## 一句话主线

```
AddCharacter / InitActionQueue   按硬直升序维护 _queue（值越小越先动）
        ↓
NextCharacter()                  队首硬直 == 0 才出队；否则递归 TimeLapse 直到有人能动
        ↓                        TotalRound++，归档上一回合 Snapshot
ProcessTurn(actor)               决策 → 执行动作 → 重算硬直 → 重新入队；返回 true 表示游戏结束
        ↓
TimeLapse()                      以队首硬直为单位推进全场：回复、减 CD、特效到期、复活
```

## 相关测试

| 测试类 | 守什么 |
| --- | --- |
| `ActionQueueTests` | 速度排序、硬直插入与冲突、`TimeLapse`、`ProcessTurn` 嵌套与返回值、事件接管 |
| `DecisionPointsTests` | 配额/成本/临时配额/协同扣点的精确数值 |
| `TeamQueueTests` | 队伍注册、召唤物归属主人队伍、回合记录里的 `TeamMap` |
| `ChainActionTests` + `ChainActionRegressionTests` | 协同行动全链路：前者做单点断言，后者走完整流程并断言硬直与预算的精确数值 |
