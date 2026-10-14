using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class ChainActionAITests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ManualTurn_ReceivesAIRequestWithoutExecuting(bool mapMode)
    {
        BattleFixture f = new(mapMode: mapMode);
        if (f.Queue.Map != null)
        {
            f.Queue.Map.SetCharacterCurrentGrid(f.C, f.Queue.Map[5, 0]!);
            f.Queue.Map.SetCharacterCurrentGrid(f.D, f.Queue.Map[2, 0]!);
        }
        f.Queue.SetCharactersToAIControl(false, false, [f.B]);
        f.Queue.DecideActionEvent += ctx =>
        {
            ChainActionRequest request = Assert.Single(ctx.ChainActions);
            Assert.Same(f.B, request.Actor);
            Assert.Same(f.A, request.TurnActor);
            Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
        Assert.DoesNotContain(f.Queue.LastRound.Actions, action => action.Actor == f.B);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ManualTurn_ConfirmsAIRequest(bool mapMode)
    {
        BattleFixture f = new(mapMode: mapMode);
        if (f.Queue.Map != null)
        {
            f.Queue.Map.SetCharacterCurrentGrid(f.C, f.Queue.Map[5, 0]!);
            f.Queue.Map.SetCharacterCurrentGrid(f.D, f.Queue.Map[2, 0]!);
        }
        f.Queue.SetCharactersToAIControl(false, false, [f.B]);
        int decisions = 0;
        f.Queue.DecideActionEvent += ctx => ++decisions == 1
            ? CharacterActionType.ChainAction : CharacterActionType.EndTurn;
        f.Queue.SelectChainActionEvent += (_, pending) => Assert.Single(pending).Id;

        f.Run(f.A);

        ActionRecord execution = Assert.Single(f.Queue.LastRound.Actions, action => action.Actor == f.B);
        Assert.Same(f.A, execution.TurnActor);
        Assert.Equal(1, execution.DecisionPointsCost);
        Assert.Equal(4, f.Points(f.B).CurrentDecisionPoints);
    }

    [Fact]
    public void AiTurn_DoesNotSubmitForManualTeammate()
    {
        BattleFixture f = new();
        f.Queue.SetCharactersToAIControl(false, false, [f.A]);
        f.Queue.DecideActionEvent += ctx =>
        {
            Assert.Empty(ctx.ChainActions);
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
        Assert.DoesNotContain(f.Queue.LastRound.Actions, action => action.Actor == f.B);
    }

    [Fact]
    public void ManualTurn_UsesApplicantPolicyToTargetTurnActor()
    {
        BattleFixture f = new(mapMode: true);
        Skill skill = f.NewSkill(SkillType.Skill, f.B);
        skill.Id = 1001;
        skill.CanSelectEnemy = false;
        skill.CanSelectSelf = false;
        skill.CanSelectTeammate = true;
        skill.CastRange = 3;
        f.A.HP = f.A.MaxHP * 0.2;
        DecisionTableConfig config = DecisionTableBuilder.Create("test:chain-healer")
            .AddRule("heal", rule => rule.Target(Targets.Allies("patient"))
                .When(Conditions.TargetHpRatio("patient", "lessThan", 0.3))
                .Then(Actions.CastSkill(skill.Id, "patient", "never"))).Build();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(config));
        f.B.DecisionTableId = config.Id;
        Assert.Empty(loader.Bind(f.B));
        f.Queue.SetCharactersToAIControl(false, false, [f.B]);
        f.Queue.DecideActionEvent += ctx =>
        {
            ChainActionRequest request = Assert.Single(ctx.ChainActions);
            Assert.Same(skill, request.Skill);
            Assert.Same(f.A, Assert.Single(request.Targets));
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
    }

    [Fact]
    public void AiTurn_SubmitsAndConfirmsForConsecutiveTeammates()
    {
        BattleFixture f = new(continuous: true);
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B, f.C]);

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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);

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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);

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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);

        f.Run(f.A);

        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.ChainAction);
        Assert.Equal(5, f.Points(f.C).CurrentDecisionPoints);
    }

    [Fact]
    public void AiTurn_DoesNotMoveChainActorToReachTarget()
    {
        BattleFixture f = new(mapMode: true);

        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);

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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);
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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);

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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);
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
        f.Queue.SetCharactersToAIControl(false, false, [f.A, f.B]);
        f.Queue.DecideActionEvent += ctx =>
        {
            Assert.Empty(ctx.ChainActions);
            return CharacterActionType.EndTurn;
        };

        f.Run(f.A);

        Assert.Equal(5, f.Points(f.B).CurrentDecisionPoints);
    }
    [Fact]
    public void ManualTurn_DoesNotSubmitForManualTeammate()
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
