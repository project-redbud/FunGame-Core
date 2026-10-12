using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class ChainActionAITests
{
    [Fact]
    public void AiTurn_SubmitsAndConfirmsForConsecutiveTeammates()
    {
        BattleFixture f = new(continuous: true);
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);

        f.Run(f.A);

        Assert.Contains(f.Queue.LastRound.Actions, a => a.Actor == f.B && a.TurnActor == f.A);
        Assert.Contains(f.Queue.LastRound.Actions, a => a.Actor == f.C && a.TurnActor == f.A);
        Assert.Equal(2, f.Queue.LastRound.Actions.Count(a => a.ActionType == CharacterActionType.ChainAction));
        Assert.Empty(f.Queue.GetPendingChainActions(f.A));
    }

    [Fact]
    public void AiTurn_ReevaluatesAndSubmitsAgainAfterExecution()
    {
        BattleFixture f = new();
        f.Queue.GameplayEquilibriumConstant = new() { ActionQuotaNormalAttack = 3 };
        foreach (DecisionPoints points in f.Queue.CharacterDecisionPoints.Values) points.GameplayEquilibriumConstant = f.Queue.GameplayEquilibriumConstant;
        f.Points(f.A).MaxDecisionPoints = 5;
        f.Queue.AddCharacter(f.D, 100, false);
        f.Points(f.B).CurrentDecisionPoints = 3;
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);

        f.Run(f.A);

        Assert.Equal(3, f.Queue.LastRound.Actions.Count(a => a.Actor == f.B && a.TurnActor == f.A));
        Assert.Equal(0, f.Points(f.B).CurrentDecisionPoints);
    }

    [Theory]
    [InlineData(SkillType.Skill, CharacterActionType.CastSkill)]
    [InlineData(SkillType.Magic, CharacterActionType.PreCastSkill)]
    [InlineData(SkillType.SuperSkill, CharacterActionType.CastSuperSkill)]
    public void AiTurn_UsesSkillWithExactRemainingBudget(SkillType skillType, CharacterActionType actionType)
    {
        BattleFixture f = new();
        Skill skill = f.NewSkill(skillType, f.B);
        f.Points(f.B).ActionTypes[CharacterActionType.NormalAttack] = f.Points(f.B)[CharacterActionType.NormalAttack];
        f.Points(f.B).CurrentDecisionPoints = f.Points(f.B).GetActionPointCost(actionType, skill, chainAction: true);
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);

        f.Run(f.A);

        ActionRecord execution = Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.B);
        Assert.Equal(actionType, execution.ActionType);
        Assert.Same(skill, execution.Skill);
        Assert.Equal(0, f.Points(f.B).CurrentDecisionPoints);
        Assert.Equal(skillType == SkillType.Magic, f.B.CharacterState == CharacterState.Casting);
    }

    [Fact]
    public void AiTurn_SkipsExhaustedAndNonConsecutiveTeammates()
    {
        BattleFixture f = new();
        f.Points(f.B).CurrentDecisionPoints = 0;
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);

        f.Run(f.A);

        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.ChainAction);
        Assert.Equal(5, f.Points(f.C).CurrentDecisionPoints);
    }

    [Fact]
    public void AiTurn_DoesNotMoveChainActorToReachTarget()
    {
        BattleFixture f = new(mapMode: true);

        f.Queue.SetCharactersToAIControl(false, false, [f.A]);

        f.Run(f.A);

        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => a.Actor == f.B);
        Assert.Same(f.Queue.Map![1, 0], f.Queue.Map.GetCharacterCurrentGrid(f.B));
        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
    }

    [Fact]
    public void AiTurn_PreservesHostRequestAndDecisionPriority()
    {
        BattleFixture f = new();
        Guid requestId = Guid.Empty;
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);
        f.Queue.TurnStartEvent += ctx => { requestId = f.Submit(f.B).Id; return true; };
        f.Queue.DecideActionEvent += ctx =>
        {
            Assert.Equal(requestId, Assert.Single(ctx.ChainActions).Id);
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.ChainAction);
        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
    }

    [Fact]
    public void AiTurn_UsesLastPointForChainNormalAttack()
    {
        BattleFixture f = new();
        f.Points(f.B).CurrentDecisionPoints = 1;
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);

        f.Run(f.A);

        Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.B && a.ActionType == CharacterActionType.NormalAttack);
        Assert.Equal(0, f.Points(f.B).CurrentDecisionPoints);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AiTurn_SubmitsItemWithExactBudget(bool mapMode)
    {
        BattleFixture f = new(mapMode: mapMode);
        Skill skill = f.NewSkill(SkillType.Item, f.B);
        skill.IsNonDirectional = mapMode;
        skill.CastRange = 5;
        skill.MPCost = 5;
        Item item = new() { Name = "协同物品", RemainUseTimes = 1, IsReduceTimesAfterUse = true };
        item.Skills.Active = skill;
        f.B.Items.Add(item);
        f.Points(f.B).ActionTypes[CharacterActionType.NormalAttack] = f.Points(f.B)[CharacterActionType.NormalAttack];
        f.Points(f.B).CurrentDecisionPoints = f.Points(f.B).GetActionPointCost(CharacterActionType.UseItem);
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);
        double mp = f.B.MP;

        f.Run(f.A);

        ActionRecord execution = Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.B);
        Assert.Equal(CharacterActionType.UseItem, execution.ActionType);
        Assert.Same(item, execution.Item);
        Assert.Equal(0, f.Points(f.B).CurrentDecisionPoints);
        Assert.Equal(mp - 5, f.B.MP, 3);
        Assert.Equal(0, item.RemainUseTimes);
    }

    [Fact]
    public void AiTurn_WithControllerDisabledDoesNotSubmitRequests()
    {
        BattleFixture f = new();
        f.Queue.EnableAI = false;
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);
        f.Queue.DecideActionEvent += ctx =>
        {
            Assert.Empty(ctx.ChainActions);
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
    }
    [Fact]
    public void ManualTurn_DoesNotAutomaticallySubmitRequests()
    {
        BattleFixture f = new();
        f.Queue.DecideActionEvent += ctx =>
        {
            Assert.Empty(ctx.ChainActions);
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => a.Actor == f.B);
    }
}
