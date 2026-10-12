using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

/// <summary>
/// 协同行动的端到端回归场景：每条用例走完整流程，断言硬直、预算与资源的精确数值。
/// 覆盖面比 <see cref="ChainActionTests"/> 的单点断言更宽，改动协同机制后应优先跑这一组。
/// </summary>
public class ChainActionRegressionTests
{
    // 连续同队区间与控制态占位
    [Fact]
    public void ContinuousTeammatesAndControlStatePlaceholder()
    {
        BattleFixture f = new();
        f.DuringTurn(() => Assert.Equal(new[] { f.B }, f.Queue.GetAvailableChainActors(f.A)));

        BattleFixture g = new(continuous: true);
        g.DuringTurn(() =>
        {
            Assert.Equal(new[] { g.B, g.C }, g.Queue.GetAvailableChainActors(g.A));
            g.B.CharacterState = CharacterState.NotActionable;
            Assert.Equal(new[] { g.C }, g.Queue.GetAvailableChainActors(g.A));
        });
    }

    // 申请替换、撤回及回合到期
    [Fact]
    public void RequestReplacementWithdrawalAndTurnExpiry()
    {
        BattleFixture f = new();
        Guid expired = Guid.Empty;

        f.DuringTurn(() =>
        {
            ChainActionRequest first = f.Submit(f.B);
            ChainActionRequest second = f.Submit(f.B);
            Assert.False(f.Queue.ConfirmChainAction(f.A, first.Id).IsSuccess);
            Assert.False(f.Queue.WithdrawChainAction(f.C, second.Id));
            Assert.True(f.Queue.WithdrawChainAction(f.B, second.Id));
            expired = f.Submit(f.B).Id;
        });

        Assert.False(f.Queue.ConfirmChainAction(f.A, expired).IsSuccess);
        Assert.Null(f.Queue.ChainActionTurnId);
    }

    // 队列更新使 C 的申请失效
    [Fact]
    public void QueueUpdateInvalidatesPendingRequest()
    {
        BattleFixture f = new(continuous: true);

        f.DuringTurn(() =>
        {
            ChainActionRequest b = f.Submit(f.B);
            ChainActionRequest c = f.Submit(f.C);
            Assert.True(f.Queue.ConfirmChainAction(f.A, b.Id).IsSuccess);

            f.Queue.ChangeCharacterHardnessTime(f.D, -.6, false, false);
            int before = f.Points(f.C).CurrentDecisionPoints;

            Assert.False(f.Queue.ConfirmChainAction(f.A, c.Id).IsSuccess);
            Assert.Equal(before, f.Points(f.C).CurrentDecisionPoints);
        });
    }

    // 即时普攻硬直、预算隔离与重复确认
    [Fact]
    public void InstantNormalAttackHardnessBudgetIsolationAndRepeatConfirm()
    {
        BattleFixture f = new();
        f.B.NormalAttack.ExHardnessTime += 6 - f.B.NormalAttack.RealHardnessTime;
        DecisionPoints points = f.Points(f.B);
        points.CurrentDecisionPoints = 3;
        points.ActionsTaken = 2;
        points.DecisionPointsCost = 4;
        points.ActionsHardnessTime.Add(99);

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B);
            Assert.Equal(3, points.CurrentDecisionPoints);

            Assert.True(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.Equal(6.5, f.Queue.HardnessTime[f.B], 3);
            Assert.Equal(2, points.CurrentDecisionPoints);
            Assert.Equal(2, points.ActionsTaken);
            Assert.Equal(4, points.DecisionPointsCost);
            Assert.Equal(new[] { 99d }, points.ActionsHardnessTime);

            int ownerBudget = f.Points(f.A).CurrentDecisionPoints;
            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.Equal(ownerBudget, f.Points(f.A).CurrentDecisionPoints);
        });
    }

    // 普攻后协同确认的最大硬直公式
    [Fact]
    public void ChainConfirmAfterNormalAttackUsesMaxHardness()
    {
        BattleFixture f = new();
        f.A.NormalAttack.ExHardnessTime += 6 - f.A.NormalAttack.RealHardnessTime;
        ChainActionRequest? request = null;
        f.Queue.TurnStartEvent += ctx => { request = f.Submit(f.B); return true; };

        int decision = 0;
        f.Queue.DecideActionEvent += ctx => ++decision switch
        {
            1 => CharacterActionType.NormalAttack,
            2 => CharacterActionType.ChainAction,
            _ => CharacterActionType.EndTurn
        };
        f.Queue.SelectNormalAttackTargetsEvent += ctx => [f.D];
        f.Queue.SelectChainActionEvent += (actor, pending) => request?.Id;

        f.Run(f.A);

        Assert.Equal(8d, f.Queue.LastRound.HardnessTime, 3);
        Assert.Equal(1, f.Queue.LastRound.Actions.Count(a => a.ActionType == CharacterActionType.ChainAction));
    }

    // 多次协同确认不限配额且不累加多个 3
    [Fact]
    public void MultipleChainConfirmsAreUnlimitedAndNotAccumulated()
    {
        BattleFixture f = new(continuous: true);

        f.DuringTurn(() =>
        {
            ChainActionRequest b = f.Submit(f.B);
            ChainActionRequest c = f.Submit(f.C);
            Assert.True(f.Queue.ConfirmChainAction(f.A, b.Id).IsSuccess);
            Assert.True(f.Queue.ConfirmChainAction(f.A, c.Id).IsSuccess);
            Assert.True(f.Points(f.A).CheckActionTypeQuota(CharacterActionType.ChainAction));
        });

        Assert.Equal(5d, f.Queue.LastRound.HardnessTime, 3);
    }

    // 跨队友回合持续消耗同一预算
    [Fact]
    public void BudgetKeepsDrainingAcrossTeammateTurns()
    {
        BattleFixture f = new();
        f.Queue.GameplayEquilibriumConstant = new EquilibriumConstant { ActionQuotaNormalAttack = 3 };
        DecisionPoints points = f.Points(f.B);
        points.GameplayEquilibriumConstant = f.Queue.GameplayEquilibriumConstant;
        points.CurrentDecisionPoints = 3;

        f.DuringTurn(() => Assert.True(f.Queue.ConfirmChainAction(f.A, f.Submit(f.B).Id).IsSuccess));

        f.Queue.RemoveCharacterFromQueue(f.C);
        f.Queue.AddCharacter(f.C, 0, false);
        f.Queue.ChangeCharacterHardnessTime(f.B, .5 - f.Queue.HardnessTime[f.B], false, false);

        f.DuringTurn(() =>
        {
            ChainActionRequest? request = f.Queue.SubmitChainAction(f.C, f.B, CharacterActionType.NormalAttack, [f.D], out string reason);
            Assert.True(request != null, reason);
            Assert.True(f.Queue.ConfirmChainAction(f.C, request!.Id).IsSuccess);
        }, f.C);

        Assert.Equal(1, points.CurrentDecisionPoints);
        Assert.Equal(2, points.ActionTypes[CharacterActionType.NormalAttack]);
    }

    // 校验资源、配额、技能归属与目标
    [Fact]
    public void ValidationOfResourcesQuotaOwnershipAndTargets()
    {
        BattleFixture f = new();

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B);
            f.Points(f.B).CurrentDecisionPoints = 0;
            int ownerBudget = f.Points(f.A).CurrentDecisionPoints;
            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.Equal(ownerBudget, f.Points(f.A).CurrentDecisionPoints);

            f.Points(f.B).CurrentDecisionPoints = 5;
            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.NormalAttack, [f.C], out _));

            Skill foreign = new TestSkill(SkillType.Skill) { Level = 1 };
            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.CastSkill, [f.D], out _, foreign));

            f.Points(f.B).ActionTypes[CharacterActionType.NormalAttack] = 1;
            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.NormalAttack, [f.D], out _));
        });
    }

    // 协同爆发技 2 点与预释放 3 点
    [Fact]
    public void ChainSuperSkillCostsTwoPointsAndPreReleaseThree()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.SuperSkill, f.B);
        DecisionPoints points = f.Points(f.B);
        points.CurrentDecisionPoints = 3;

        Assert.Equal(3, points.GetActionPointCost(CharacterActionType.CastSuperSkill));

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B, CharacterActionType.CastSuperSkill, skill);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.Equal(1, points.CurrentDecisionPoints);
            Assert.Equal(1, points.ActionTypes[CharacterActionType.CastSuperSkill]);
        });
    }

    // 协同魔法 0.5→3→5.5，释放扣 MP 且不刷新预算
    [Fact]
    public void ChainMagicCastingPaysMpOnReleaseWithoutRefreshingBudget()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Magic, f.B);
        skill.MPCost = 10;
        double mp = f.B.MP;
        DecisionPoints points = f.Points(f.B);
        points.CurrentDecisionPoints = 5;

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B, CharacterActionType.PreCastSkill, skill);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request.Id).IsCasting);
            Assert.Equal(3d, f.Queue.HardnessTime[f.B], 3);
            Assert.Equal(mp, f.B.MP, 3);
            Assert.False(f.Queue.AssistDetails[f.A].NotDamageAssistLastTime.ContainsKey(f.B));
        });

        int actionTurns = f.Queue.CharacterStatistics[f.B].ActionTurn;
        int turnStarts = 0;
        f.Queue.TurnStartEvent += ctx => { turnStarts++; return true; };

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(5.5, f.Queue.HardnessTime[f.B], 3);
        Assert.Equal(3, points.CurrentDecisionPoints);
        Assert.Equal(1, points.ActionTypes[CharacterActionType.PreCastSkill]);
        Assert.Equal(mp - 10, f.B.MP, 3);
        Assert.Equal(actionTurns, f.Queue.CharacterStatistics[f.B].ActionTurn);
        Assert.Equal(0, turnStarts);
        Assert.True(f.Queue.AssistDetails[f.A].NotDamageAssistLastTime.ContainsKey(f.B));
    }

    // 协同吟唱使用框架两位小数精度
    [Fact]
    public void ChainCastingKeepsTwoDecimalPrecision()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Magic, f.B);
        skill.CastTime = 3.3333;

        f.DuringTurn(() =>
        {
            ChainActionResult result = f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, skill).Id);
            Assert.True(result.IsCasting);
            Assert.Equal(3.33, f.Queue.HardnessTime[f.B], 3);
            Assert.Equal(f.Queue.HardnessTime[f.B], result.Record!.CastTime, 3);
        });
    }

    // 协同吟唱打断保留原硬直并加 3
    [Fact]
    public void ChainCastingInterruptKeepsOriginalHardnessPlusPenalty()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Magic, f.B);

        f.DuringTurn(() =>
        {
            f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, skill).Id);
            f.Queue.InterruptCasting(f.B, f.D);

            Assert.Equal(3.5, f.Queue.HardnessTime[f.B], 3);
            Assert.Equal(CharacterState.Actionable, f.B.CharacterState);
            Assert.False(f.Queue.AssistDetails[f.A].NotDamageAssistLastTime.ContainsKey(f.B));
        });
    }

    // 协同吟唱资源不足与目标失效收尾
    [Fact]
    public void ChainCastingResourceFailureAndTargetLoss()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Magic, f.B);
        skill.MPCost = 10;
        f.DuringTurn(() => f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, skill).Id));

        f.B.MP = 0;
        skill.MPCost = 10000;
        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(3.5, f.Queue.HardnessTime[f.B], 3);
        Assert.False(f.Queue.LastRound.Actions[0].IsSuccess);
        Assert.False(f.Queue.AssistDetails[f.A].NotDamageAssistLastTime.ContainsKey(f.B));

        BattleFixture g = new();
        Skill magic = g.NewSkill(SkillType.Magic, g.B);
        g.DuringTurn(() =>
        {
            g.Queue.ConfirmChainAction(g.A, g.Submit(g.B, CharacterActionType.PreCastSkill, magic).Id);
            g.Queue.InterruptCasting(g.D);
            Assert.Equal(3.5, g.Queue.HardnessTime[g.B], 3);
        });
    }

    // 协同物品扣次数与 MP/EP
    [Fact]
    public void ChainItemConsumesTimesAndResources()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Item, f.B);
        skill.MPCost = 5;
        skill.EPCost = 7;
        Item item = new() { Name = "药剂", RemainUseTimes = 1, IsReduceTimesAfterUse = true };
        item.Skills.Active = skill;
        f.B.Items.Add(item);
        double mp = f.B.MP, ep = f.B.EP;

        f.DuringTurn(() =>
        {
            ChainActionRequest? request = f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.UseItem, [f.D], out string reason, item: item);
            Assert.True(request != null, reason);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request!.Id).IsSuccess);

            Assert.Equal(0, item.RemainUseTimes);
            Assert.Equal(mp - 5, f.B.MP, 3);
            Assert.Equal(ep - 7, f.B.EP, 3);
            Assert.DoesNotContain(item, f.B.Items);
        });
    }

    // 协同技能能以当前回合角色为友方目标
    [Fact]
    public void ChainSkillCanTargetCurrentTurnActor()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Skill, f.B);
        skill.CanSelectEnemy = false;
        skill.CanSelectTeammate = true;

        f.DuringTurn(() =>
        {
            ChainActionRequest? request = f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.CastSkill, [f.A], out string reason, skill);
            Assert.True(request != null, reason);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request!.Id).IsSuccess);
        });
    }

    // 召唤物作为回合角色及协作方
    [Fact]
    public void SummonActsAsTurnActorAndChainPartner()
    {
        BattleFixture f = new();
        f.B.Master = f.C;
        f.DuringTurn(() => Assert.Contains(f.B, f.Queue.GetAvailableChainActors(f.A)));

        BattleFixture g = new();
        g.A.Master = g.C;
        g.DuringTurn(() => Assert.Contains(g.B, g.Queue.GetAvailableChainActors(g.A)));
    }

    // 记录归属、辅助去重、快照和 JSON
    [Fact]
    public void RecordOwnershipAssistSnapshotAndJson()
    {
        BattleFixture f = new();
        f.DuringTurn(() => f.Queue.ConfirmChainAction(f.A, f.Submit(f.B).Id));

        ActionRecord confirm = f.Queue.LastRound.Actions.Single(a => a.ActionType == CharacterActionType.ChainAction);
        ActionRecord execute = f.Queue.LastRound.Actions.Single(a => a.Actor == f.B);

        Assert.True(confirm.Actor == f.A && execute.Actor == f.B);
        Assert.True(execute.TurnActor == f.A && execute.ChainActionRequestId == confirm.ChainActionRequestId);
        Assert.Equal(new[] { f.B.Guid }, confirm.NonDamageAssistTargets.Select(c => c.Guid));

        ActionRecord snapshot = confirm.Snapshot();
        confirm.NonDamageAssistTargets.Clear();
        Assert.Single(snapshot.NonDamageAssistTargets);

        ActionRecord? decoded = JsonService.GetObject<ActionRecord>(JsonService.GetString(snapshot));
        Assert.True(decoded!.ChainActionRequestId == snapshot.ChainActionRequestId && decoded.TurnActor?.Guid == f.A.Guid &&
            decoded.ChainActor?.Guid == f.B.Guid && decoded.NonDamageAssistTargets.Single().Guid == f.B.Guid);
    }

    // 协同致死动作归 B，A 获得非伤害助攻
    [Fact]
    public void LethalChainActionCreditsActorAndAssist()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Skill, f.B);
        f.Queue.CharacterCastSkillEvent += ctx =>
        {
            if (ctx.Trigger != f.B) return;
            f.D.HP = 0;
            f.Queue.DeathCalculation(f.B, f.D);
        };

        f.DuringTurn(() => f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.CastSkill, skill).Id));

        Assert.Equal(1, f.Queue.CharacterStatistics[f.B].Kills);
        Assert.Equal(0, f.Queue.CharacterStatistics[f.A].Kills);
        Assert.Equal(1, f.Queue.CharacterStatistics[f.A].Assists);
    }

    // AI 识别申请并确认
    [Fact]
    public void AiRecognisesAndConfirmsPendingRequest()
    {
        BattleFixture f = new();
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);
        f.Queue.TurnStartEvent += ctx => { f.Submit(f.B); return true; };

        f.Run(f.A);

        Assert.Contains(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.ChainAction);
    }

    // 配置初始预算与新召唤物不临时补点
    [Fact]
    public void ConfiguredInitialBudgetAndNewSummon()
    {
        BattleFixture f = new();
        f.Queue.GameplayEquilibriumConstant = new EquilibriumConstant { InitialDecisionPoints = 4 };
        Unit summon = new() { Name = "召唤物", Master = f.A, InitialHP = 100 };
        summon.Recovery();
        f.Queue.AddCharacter(summon, .3, false);

        Assert.Equal(4, f.Points(summon).CurrentDecisionPoints);

        f.DuringTurn(() =>
        {
            ChainActionRequest? request = f.Queue.SubmitChainAction(f.A, summon, CharacterActionType.NormalAttack, [f.D], out string reason);
            Assert.True(request != null, reason);
            Assert.Equal(4, f.Points(summon).CurrentDecisionPoints);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request!.Id).IsSuccess);
            Assert.Equal(3, f.Points(summon).CurrentDecisionPoints);
        });
    }

    // 预释放取消协同吟唱后不残留魔法上下文
    [Fact]
    public void PreReleaseSuperSkillClearsPendingChainCast()
    {
        BattleFixture f = new();
        Skill magic = f.NewSkill(SkillType.Magic, f.B), burst = f.NewSkill(SkillType.SuperSkill, f.B);

        f.DuringTurn(() =>
        {
            f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, magic).Id);
            f.Queue.SetCharacterPreCastSuperSkill(f.B, burst);
            Assert.Equal(0, f.Points(f.B).CurrentDecisionPoints);
        });

        f.Queue.SelectSkillTargetsEvent += ctx => [f.D];
        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Same(burst, f.Queue.LastRound.Actions.Single().Skill);
        Assert.Null(f.Queue.LastRound.Actions.Single().ChainActionRequestId);
        Assert.Equal(5d, f.Queue.LastRound.HardnessTime, 3);
    }

    // 协同施法者死亡清理上下文
    [Fact]
    public void DeadChainCasterLeavesNoCastingContext()
    {
        BattleFixture f = new();
        Skill magic = f.NewSkill(SkillType.Magic, f.B);

        f.DuringTurn(() =>
        {
            f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, magic).Id);
            f.B.HP = 0;
            f.Queue.DeathCalculation(f.D, f.B);
            Assert.DoesNotContain(f.B, f.Queue.Queue);
        });

        f.Queue.SetCharacterRespawn(f.B);
        int turnStarts = 0;
        f.Queue.TurnStartEvent += ctx => { turnStarts++; return true; };
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(1, turnStarts);
    }

    // 地图射程重验与无角色格子魔法
    [Fact]
    public void MapRangeRevalidationAndEmptyGridMagic()
    {
        BattleFixture f = new(mapMode: true);
        Skill skill = f.NewSkill(SkillType.Skill, f.B);
        skill.CastRange = 1;

        f.DuringTurn(() =>
        {
            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.CastSkill, [f.D], out _, skill));

            Skill area = f.NewSkill(SkillType.Magic, f.B);
            area.IsNonDirectional = true;
            area.AllowSelectNoCharacterGrid = true;
            area.CastRange = 5;

            ChainActionRequest? request = f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.PreCastSkill, [], out string reason,
                area, targetGrids: [f.Queue.Map![3, 0]!]);
            Assert.True(request != null, reason);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request!.Id).IsCasting);
        });

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.True(f.Queue.LastRound.Actions.Single().IsSuccess);
        Assert.Equal(5.5, f.Queue.LastRound.HardnessTime, 3);
    }

    // 确认重验冷却、资源及目标并拒绝钩子重入
    [Fact]
    public void ConfirmRevalidatesCooldownResourcesAndTargets()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Skill, f.B);

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B, CharacterActionType.CastSkill, skill);
            skill.CurrentCD = 2;
            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);

            skill.CurrentCD = 0;
            request = f.Submit(f.B, CharacterActionType.CastSkill, skill);
            skill.EPCost = 1000;
            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);

            skill.EPCost = 0;
            request = f.Submit(f.B, CharacterActionType.CastSkill, skill);
            Guid id = request.Id;
            f.Queue.CharacterCastSkillEvent += ctx => Assert.False(f.Queue.ConfirmChainAction(f.A, id).IsSuccess);
            Assert.True(f.Queue.ConfirmChainAction(f.A, id).IsSuccess);
        });
    }

    // 普通技能与普通魔法回归
    [Fact]
    public void NormalSkillAndMagicRegression()
    {
        foreach (SkillType type in new[] { SkillType.Skill, SkillType.Magic })
        {
            BattleFixture f = new();
            Skill skill = new TestSkill(type) { Level = 1, HardnessTime = 5, CastTime = 3, MPCost = 10, EPCost = 7 };
            f.A.Skills.Add(skill);
            int decisions = 0;
            f.Queue.DecideActionEvent += ctx => ++decisions == 1 ? CharacterActionType.PreCastSkill : CharacterActionType.EndTurn;
            f.Queue.SelectSkillEvent += ctx => skill;
            f.Queue.SelectSkillTargetsEvent += ctx => [f.D];
            double mp = f.A.MP, ep = f.A.EP;

            f.Run(f.A);

            if (type == SkillType.Magic)
            {
                Assert.Equal(mp, f.A.MP, 3);
                f.MakeNext(f.A);
                f.Run(f.A);
                Assert.Equal(5d, f.Queue.LastRound.HardnessTime, 3);
            }
            else
            {
                Assert.Equal(ep - 7, f.A.EP, 3);
            }

            Assert.All(f.Queue.LastRound.Actions, a => Assert.Null(a.ChainActionRequestId));
        }
    }
}
