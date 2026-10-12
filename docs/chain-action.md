# 协同行动

协同行动将当前正常回合的角色 A 与实际动作执行者 B 分开。A 确认申请，B 使用自己延续的预算执行动作，随后继续由 A 决策。

## 规则

| 项目 | 行为 |
| --- | --- |
| A 的确认动作 | `CharacterActionType.ChainAction`，消耗 1 决策点，配额无限 |
| A 的动作硬直 | 3，参与原有最大动作硬直计算；普攻后确认为 `max(普攻硬直, 3) + 2` |
| B 的可用动作 | 普攻、战技、魔法、爆发技、物品 |
| B 的预算 | 上一次正常回合以来持续扣减的决策点及对应配额，不刷新旧硬直统计 |
| 新角色和召唤物 | 入队时使用队列配置中的 `InitialDecisionPoints`，提交申请不补点 |
| B 的即时硬直 | 通过队列更新入口，在当前硬直上加本次动作硬直 |
| 协同魔法 | 当前队列时间替换为吟唱时间；释放后设置为原硬直加魔法动作硬直 |
| 魔法 MP | 开始吟唱时检查，释放时重新检查并扣除，与普通魔法一致 |
| 协同爆发技 | 2 决策点及爆发配额；回合外预释放仍消耗 3 点 |
| 吟唱失败或打断 | 保留确认时的原硬直，加 3 的失败硬直；已消耗的 DP 不返还，不产生有效协调辅助 |
| 施法者死亡或移除 | 清除协同吟唱上下文，不再执行残留魔法 |
| 贡献 | B 拥有动作、资源消耗及击杀；A 通过辅助 B 获得非伤害助攻关联 |

确认成本和硬直可通过 `EquilibriumConstant.DecisionPointsCostChainAction` 和 `ChainActionHardnessTime` 配置。

框架在正常回合开始恢复预算；协同行动及协同吟唱结算均不恢复。临时配额在正常回合结束清除。协同吟唱结算只运行动作结算及完成事件，不调用正常回合奖励发放和正常决策入口，也不增加正常行动回合计数。

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

除上述回合内选择分支，接管回合的宿主也可以直接调用 `ConfirmChainAction(A, request.Id)`，读取 `ChainActionResult.IsSuccess`、`FailReason`、`IsCasting` 和执行记录。此入口同样记录 A 的确认成本与硬直。失败时可向玩家显示 `FailReason`。

公开入口在 `IGamingQueue` 上提供兼容的默认实现，未实现协同机制的自定义队列会返回“不支持”，原有实现者不需要立即增加方法。队列和协同入口沿用框架的同步执行模型；网络或 UI 线程应将操作串行派发到本局游戏循环。

## 记录、回放与 AI

每次成功确认保存两条 `ActionRecord`：A 的 `ChainAction` 确认记录和 B 的实际动作记录。两条记录共享 `ChainActionRequestId`，实际动作记录的 `Actor` 为 B、`TurnActor` 为 A；确认记录还保存 `ChainActor`。

即时动作在 A 的 `NonDamageAssistTargets` 中登记协作方 B。魔法在释放完成时才建立有效辅助，记录保留原 A 及申请 ID。实际致死效果在计算击杀贡献前登记协调关系，确保该次击杀就能识别 A 的助攻。旧团队回合辅助逻辑只读取当前角色实际执行的动作，避免把 B 的目标错误归给 A。

这些字段支持 `Snapshot()` 和 JSON 往返，并通过现有 `DefaultRoundRecordSink` 自动外发。`ActionRecord.ToString()` 显示确认者及实际执行者，现有 `RoundRecordRenderer` 直接使用该输出。回合级字典仍作为聚合数据；需要准确归属和双方成本的消费者应读取 `RoundRecord.Actions`。

AI 第一版会选择并确认第一份有效申请；预算保留策略及自动提交策略留给宿主或后续 AI 扩展。申请 ID 与回合 ID 用于生命周期校验，不参与战斗随机数源。

## 验证

无需额外测试包，使用独立控制台场景测试；失败会返回非零退出码，CI 已接入同一命令：

```powershell
dotnet run --project Tests/FunGame.Core.ChainAction.Tests --configuration Release
```

场景覆盖连续位置、控制态占位、申请替换撤回及到期、队列重验、预算隔离与跨回合扣减、双方硬直、配额及资源失败、重复确认与钩子重入、协同魔法成功/打断/目标失效/资源失败/死亡、预释放取消吟唱、召唤物、地图射程与空地魔法、物品消耗、击杀和辅助归属、记录快照及 JSON、AI 确认，以及普通技能/魔法回归。
