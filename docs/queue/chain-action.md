# 协同行动

> 上级：[队列与回合](README.md) ｜ 源码：`src/Model/Queue/GamingQueue.ChainAction.cs`、`GamingQueue.cs` 的回合循环挂钩
> 相关测试：`ChainActionTests`、`ChainActionRegressionTests`、`ChainActionAITests`、`ChainActionSettlementTests`

协同行动将当前正常回合的角色 A 与实际动作执行者 B 分开。A 确认申请，B 使用自己延续的预算执行动作，随后继续由 A 决策。

## 规则

| 项目 | 行为 |
| --- | --- |
| A 的确认动作 | `CharacterActionType.ChainAction`，消耗 1 决策点，配额无限 |
| A 的动作硬直 | 主循环将确认动作的硬直 3 入账一次，按通用规则参与回合末聚合；普攻后确认为 `max(普攻硬直, 3) + 2` |
| B 的可用动作 | 普攻、战技、魔法、爆发技、物品 |
| B 的预算 | 上一次正常回合以来持续扣减的决策点及对应配额，不刷新硬直统计 |
| 新角色和召唤物 | 入队时使用队列配置中的 `InitialDecisionPoints`，提交申请不补点 |
| B 的即时硬直 | 通过队列更新入口，在当前硬直上加本次动作硬直 |
| 协同魔法 | 当前队列时间替换为吟唱时间；结算是独立正常回合，触发回合开始、奖励、行动回合计数及特效钩子；回合末硬直为确认时原硬直加本次魔法动作硬直 |
| 魔法 MP | 开始吟唱时检查，释放时重新检查并扣除，与普通魔法一致 |
| 协同爆发技 | 2 决策点及爆发配额；回合外预释放仍消耗 3 点 |
| 吟唱失败或打断 | 保留确认时的原硬直，加 3 的失败硬直；已消耗的 DP 不返还，不产生有效协调辅助 |
| 施法者死亡或移除 | 清除协同吟唱上下文，不再执行残留魔法 |
| 贡献 | B 拥有动作、资源消耗及击杀；A 通过辅助 B 获得非伤害助攻关联 |

确认成本和硬直可通过 `EquilibriumConstant.DecisionPointsCostChainAction` 和 `ChainActionHardnessTime` 配置。

框架在非吟唱态进入回合时恢复预算；协同行动及协同吟唱结算均不恢复。回合结束只清理回合角色本人的临时配额。结算角色处于 `Casting` 时跳过主动决策，直接由主循环释放魔法。

协同结算向主循环注入确认时冻结的原硬直、记录归属、协调辅助和上下文清理。入口清空协作方的 `ActionsHardnessTime`、`ActionTypes`、`ActionsTaken` 与 `DecisionPointsCost`：上一正常回合的计数已参与结算，本次协同不再使用它们。回合末在通用聚合结果上加回原硬直，还原被吟唱时间替换的队列位置。

`TurnStartEvent` 返回 `false` 可以接管结算回合。未完成释放时保留施法上下文；宿主负责回合收尾和重新入队，后续回合继续释放。结算失败沿用普通魔法的原因文案，区分目标丢失与魔法值不足。

## 连续位置与申请生命周期

资格按实际行动顺序表判断。`NextCharacter()` 已将 A 移出队列，因此扫描从剩余队列头部开始，遇到第一个不同队角色停止。

- `A → B → C → D`：同队的 B、C 均可提交。
- `A → B → D → C`：只有 B 可提交。
- 同队但无法行动的角色仍占据区间，本人不能提交。
- 团队成员及其召唤物共享团队关系；召唤物作为 A 时也能识别主人的队友。

每个 B 最多保留一份申请。新提交替换旧申请；B 可撤回自己的申请。申请绑定一次 `ProcessTurn()` 的唯一 `TurnId`，包括事件接管、提前返回等情况，A 的回合结束统一清除。

提交及确认均检查状态、位置、预算、配额、持有技能或物品、冷却、资源、目标数量和地图射程。提交不扣费；确认失败不扣双方费用。确认成功后先移除申请并消耗预算，再执行效果，避免重复确认及动作钩子重入。成功发动后被免疫或未产生效果仍消耗资源。查询有效申请会移除失效申请；被移除的申请不会因后续队列变化重新出现。

## 接入示例

宿主可在 A 的 `TurnStartEvent` 或正常决策期间收集 B 的申请。目标由宿主先选择，提交时复制集合；确认时重新验证。`CastSkill` 表示战技，`PreCastSkill` 表示魔法，`CastSuperSkill` 表示协同爆发技。

```csharp
// A、B、enemy 为本局现有角色，A 当前正在正常回合内。
ChainActionRequest? request = queue.SubmitChainAction(
    A, B, CharacterActionType.NormalAttack, [enemy], out string failure);

if (request is null)
{
    Console.WriteLine(failure);
}

// A 的动作选择界面展示 ctx.ChainActions，选中协同行动后返回 ChainAction。
queue.DecideActionEvent += ctx =>
    ctx.ChainActions.Count > 0
        ? CharacterActionType.ChainAction
        : CharacterActionType.EndTurn;

// 将宿主中玩家选定的申请 ID 返回给框架。
queue.SelectChainActionEvent += (actor, pending) =>
    pending.FirstOrDefault()?.Id;
```

魔法或技能申请传入 `skill`；物品申请传入 `item`；非指向性地图技能传入 `targetGrids`。`targets` 可在允许空地施法时使用空集合。

确认必须发生在 A 的回合内。接管回合的宿主可直接调用 `ConfirmChainAction(A, request.Id)`，读取 `ChainActionResult.IsSuccess`、`FailReason`、`IsCasting` 和执行记录。公开入口扣减双方预算并记录确认成本，A 的确认硬直入账、确认记录硬直和回合收尾由宿主负责；通过 `ChainAction` 行动分支确认时由主循环统一处理。失败时可向玩家显示 `FailReason`。

公开入口在 `IGamingQueue` 上都有默认实现：未实现协同机制的自定义队列调用它们时得到「不支持」的结果。队列和协同入口沿用框架的同步执行模型；网络或 UI 线程应将操作串行派发到本局游戏循环。

## 记录、回放与 AI

每次成功确认保存两条 `ActionRecord`：A 的 `ChainAction` 确认记录和 B 的实际动作记录。两条记录共享 `ChainActionRequestId`，实际动作记录的 `Actor` 为 B、`TurnActor` 为 A；确认记录还保存 `ChainActor`。

即时动作在 A 的 `NonDamageAssistTargets` 中登记协作方 B。魔法在释放完成时才建立有效辅助，记录保留原 A 及申请 ID。实际致死效果在计算击杀贡献前登记协调关系，确保该次击杀就能识别 A 的助攻。正常回合的团队辅助逻辑只读取当前角色实际执行的动作。

`ActionRecord.HardnessTime` 与 `CastTime` 在外发前填写本次操作自身的硬直或吟唱时间。协同魔法的释放记录只包含魔法动作硬直，原队列硬直的还原包含在 `RoundRecord.HardnessTime` 中。

这些字段支持 `Snapshot()` 和 JSON 往返，并通过 `DefaultRoundRecordSink` 自动外发。`ActionRecord.ToString()` 显示确认者及实际执行者，`RoundRecordRenderer` 直接使用该输出。回合级字典作为聚合数据；需要准确归属和双方成本的消费者应读取 `RoundRecord.Actions`。

AI 在每次正常行动决策前调用 `WillSubmitChainActions`，为当前连续同队区间内的可行动队友（包括手动控制队友及召唤物）代交申请。已有有效申请保持不变；没有申请时复用 `AIController.DecideAIAction` 评估普攻、技能与物品，只评估协作方当前格，不移动。当前回合角色仍可作为协同辅助技能的目标。

宿主行动决策事件优先；宿主未选择行动时，AI 优先确认第一份有效申请，再评估自己的普通行动。每次行动后重新检查连续位置、状态、预算、配额、资源和射程，因此同一队友可以在仍符合资格时再次提交。申请与确认均不补充预算；技能预算恰好等于成本时也参与评估，协同爆发技按正常爆发技成本执行。关闭 `EnableAI`、手动回合、整回合事件接管及吟唱结算均不自动提交。预算保留策略由宿主或 AI 扩展自行实现。申请 ID 与回合 ID 只用于生命周期校验，不参与战斗随机数源。

## 相关测试

协同行动的场景都是 xUnit 用例，随解决方案一起跑：

```powershell
dotnet test tests/FunGame.Core.Tests.Queue/FunGame.Core.Tests.Queue.csproj -c Release
```

| 测试类 | 守什么 |
| --- | --- |
| `ChainActionTests`（14 条） | 申请提交与撤回、确认成本与硬直、预算隔离等精确数值 |
| `ChainActionRegressionTests`（25 条） | 连续位置与控制态占位、申请替换/撤回/到期、队列重验、预算隔离与跨回合扣减、双方硬直、配额与资源失败、重复确认与钩子重入、协同魔法的成功/打断/目标失效/资源失败/死亡、预释放取消吟唱、召唤物、地图射程与空地魔法、物品消耗、击杀与辅助归属、记录快照与 JSON、AI 确认，以及普通技能与魔法的回归 |
| `ChainActionAITests`（13 条） | AI 自动代交与确认、多次协同、技能类型与精确预算、连续位置与预算过滤、原地射程、宿主优先级和手动回合 |
| `ChainActionSettlementTests`（11 条） | 结算回合事件、计数、奖励、特效钩子、预算与旧计数隔离、整回合接管、公开确认契约，以及记录和外发的单次硬直/吟唱时间 |

改协同行动相关代码前，先看这些用例的期望值；决策点与配额的精确数值另见 `DecisionPointsTests`。
