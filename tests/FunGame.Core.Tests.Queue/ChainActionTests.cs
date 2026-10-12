using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

public class ChainActionTests
{
    [Fact]
    public void GetAvailableChainActors_OnlyConsecutiveTeammates()
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

    [Fact]
    public void GetAvailableChainActors_IsEmptyOutsideNormalTurn()
    {
        BattleFixture f = new();
        Assert.Empty(f.Queue.GetAvailableChainActors(f.A));

        f.DuringTurn(() => Assert.Single(f.Queue.GetAvailableChainActors(f.A)));
    }

    [Fact]
    public void SubmitAndConfirm_ConsumesActorBudgetOnly()
    {
        BattleFixture f = new();
        DecisionPoints points = f.Points(f.B);
        points.CurrentDecisionPoints = 3;
        points.ActionsTaken = 2;
        points.DecisionPointsCost = 4;
        points.ActionsHardnessTime.Add(99);

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B);
            Assert.Equal(3, points.CurrentDecisionPoints);

            ChainActionResult result = f.Queue.ConfirmChainAction(f.A, request.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, points.CurrentDecisionPoints);
            Assert.Equal(1, points.ActionTypes[CharacterActionType.NormalAttack]);
            Assert.Equal(2, points.ActionsTaken);
            Assert.Equal(4, points.DecisionPointsCost);
            Assert.Equal(new[] { 99d }, points.ActionsHardnessTime);
        });
    }

    [Fact]
    public void ConfirmChainAction_IsRejectedOnRepeat()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.Equal(1, f.Points(f.B).ActionTypes[CharacterActionType.NormalAttack]);
        });
    }

    [Fact]
    public void WithdrawChainAction_OnlyRequesterMayWithdraw()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B);
            Assert.False(f.Queue.WithdrawChainAction(f.C, request.Id));
            Assert.True(f.Queue.WithdrawChainAction(f.B, request.Id));
        });
    }

    [Fact]
    public void SubmitChainAction_ReplacesPreviousPendingRequest()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            ChainActionRequest first = f.Submit(f.B);
            ChainActionRequest second = f.Submit(f.B);

            Assert.False(f.Queue.ConfirmChainAction(f.A, first.Id).IsSuccess);
            Assert.True(f.Queue.ConfirmChainAction(f.A, second.Id).IsSuccess);
        });
    }

    [Fact]
    public void ConfirmChainAction_ExpiresWhenTurnEnds()
    {
        BattleFixture f = new();
        Guid expired = Guid.Empty;

        f.DuringTurn(() => expired = f.Submit(f.B).Id);

        Assert.False(f.Queue.ConfirmChainAction(f.A, expired).IsSuccess);
        Assert.Null(f.Queue.ChainActionTurnId);
    }

    [Fact]
    public void ConfirmChainAction_RechecksBudget()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B);
            f.Points(f.B).CurrentDecisionPoints = 0;

            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
            Assert.Equal(0, f.Points(f.B).ActionTypes.GetValueOrDefault(CharacterActionType.NormalAttack));
        });
    }

    [Fact]
    public void ConfirmChainAction_RechecksSkillCooldownBeforeExecuting()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Skill, f.B);

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B, CharacterActionType.CastSkill, skill);
            skill.CurrentCD = 2;

            Assert.False(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
        });
    }

    [Fact]
    public void SubmitChainAction_RejectsForeignSkillAndAllyTarget()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            Skill foreign = new TestSkill(SkillType.Skill) { Level = 1 };

            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.CastSkill, [f.D], out _, foreign));
            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.NormalAttack, [f.C], out _));
        });
    }

    [Fact]
    public void SubmitChainAction_RejectsExhaustedQuota()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            f.Points(f.B).ActionTypes[CharacterActionType.NormalAttack] = f.Points(f.B)[CharacterActionType.NormalAttack];

            Assert.Null(f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.NormalAttack, [f.D], out _));
        });
    }

    [Fact]
    public void ChainAction_IsRecordedWithSharedRequestId()
    {
        BattleFixture f = new();
        f.DuringTurn(() => f.Queue.ConfirmChainAction(f.A, f.Submit(f.B).Id));

        ActionRecord confirm = f.Queue.LastRound.Actions.Single(a => a.ActionType == CharacterActionType.ChainAction);
        ActionRecord execute = f.Queue.LastRound.Actions.Single(a => a.Actor == f.B);

        Assert.Equal(f.A.Guid, confirm.Actor.Guid);
        Assert.Equal(f.B.Guid, execute.Actor.Guid);
        Assert.Equal(f.A.Guid, execute.TurnActor!.Guid);
        Assert.Equal(confirm.ChainActionRequestId, execute.ChainActionRequestId);
        Assert.Equal(new[] { f.B.Guid }, confirm.NonDamageAssistTargets.Select(c => c.Guid));
    }

    [Fact]
    public void ChainAction_SkillCastSharesCasterResources()
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(SkillType.Skill, f.B);
        skill.EPCost = 7;
        double ep = f.B.EP;
        f.Points(f.B).CurrentDecisionPoints = 5;

        f.DuringTurn(() =>
        {
            ChainActionRequest request = f.Submit(f.B, CharacterActionType.CastSkill, skill);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request.Id).IsSuccess);
        });

        Assert.Equal(ep - 7, f.B.EP, 3);
        Assert.Equal(3, f.Points(f.B).CurrentDecisionPoints);
    }

    [Fact]
    public void SummonCanChainForItsMasterTeam()
    {
        BattleFixture f = new();
        Character summon = QueueTestHelper.NewCharacter("summon");
        summon.Master = f.B;
        f.Queue.AddCharacter(summon, .6, false);

        f.DuringTurn(() =>
        {
            Assert.Equal(new[] { f.B, summon }, f.Queue.GetAvailableChainActors(f.A));
        });
    }
}
