using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public partial class DecisionRiskBudgetTests
{
    private static DecisionEvaluationContext Context(BattleFixture fixture, int budget = 4096, params Skill[] skills) => new()
    {
        Queue = fixture.Queue, Actor = fixture.A, Points = fixture.Points(fixture.A), Skills = skills,
        Enemies = [fixture.D], SelectableEnemies = [fixture.D], Allies = [fixture.B, fixture.C],
        SelectableAllies = [fixture.B, fixture.C], PlanBudget = budget,
        TurnOrigin = fixture.Queue.Map?.GetCharacterCurrentGrid(fixture.A),
        MoveGrids = fixture.Queue.Map?.Grids.Values.ToArray() ?? []
    };

    [Theory]
    [InlineData("current")]
    [InlineData("coverage")]
    [InlineData("safe")]
    public void Risk_NestedConditionUsesActualThreatWithoutMoving(string preference)
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.D.ExATR = 10;
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.GamingQueue = fixture.Queue;
        skill.CastAnywhere = true;
        ActionConfig action = Actions.CastSkill(skill.Id, movement: "never");
        action.PositionPreference = preference;
        action.BindPlan = "p";
        DecisionTableConfig config = DecisionTableBuilder.Create("test:risk")
            .AddRule("risk", r => r.When(Conditions.All(
                Conditions.Any(Conditions.Compare("plan.risk", "greaterThan", 0, "p")),
                Conditions.Not(Conditions.Compare("plan.risk", "atMost", 0, "p")))).Then(action)).Build();
        DecisionEvaluationContext context = Context(fixture, 4096, skill);
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(config).Evaluate(context);
        Assert.True(result.Matched);
        Assert.Equal(1, context.Metrics.PositionsEvaluated);
        Assert.Same(context.TurnOrigin, result.Decision!.TargetMoveGrid);
    }

    [Fact]
    public void Budget_LaterRuleReceivesShareAfterUnmatchedPlans()
    {
        BattleFixture fixture = new();
        fixture.A.NormalAttack.CanSelectSelf = fixture.A.NormalAttack.CanSelectTeammate = true;
        ActionConfig first = Actions.NormalAttack(movement: "never");
        first.BindPlan = "p";
        DecisionTableConfig config = DecisionTableBuilder.Create("test:share")
            .AddRule("miss", r => r.When(Conditions.Compare("plan.targetCount", "greaterThan", 100, "p")).Then(first))
            .AddRule("hit", r => r.Target(Targets.Enemies()).Then(Actions.NormalAttack())).Build();
        DecisionEvaluationContext context = Context(fixture, 2);
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(config).Evaluate(context);
        Assert.Equal("hit", result.RuleId);
        Assert.Equal(2, context.Metrics.PlansEvaluated);
    }

    [Fact]
    public void Budget_FalseSelfConditionDoesNotConsumeOnlyPlan()
    {
        BattleFixture fixture = new();
        DecisionTableConfig config = DecisionTableBuilder.Create("test:precheck")
            .AddRule("miss", r => r.When(Conditions.Not(Conditions.Any(Conditions.SelfHpRatio("atLeast", 0)))).Then(Actions.NormalAttack()))
            .AddRule("hit", r => r.Then(Actions.NormalAttack())).Build();
        DecisionEvaluationContext context = Context(fixture, 1);
        Assert.Equal("hit", new DecisionTableCompiler().Compile(config).Evaluate(context).RuleId);
        Assert.Equal(1, context.Metrics.PlansEvaluated);
    }

    [Theory]
    [InlineData(AISelectionMode.Legacy)]
    [InlineData(AISelectionMode.Deterministic)]
    [InlineData(AISelectionMode.WeightedNearBest)]
    public void Fallback_UsesIndependentBudgetAndStablePolicyMetrics(AISelectionMode mode)
    {
        BattleFixture fixture = new();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register("test:drain", new DrainPolicy()));
        fixture.A.DecisionTableId = "test:drain";
        Assert.Empty(loader.Bind(fixture.A));
        AIController ai = new(fixture.Queue, null) { SelectionMode = mode };
        DecisionMetrics? snapshot = null;
        int policyPlans = 0;
        ai.PolicyEvaluatedEvent += (_, _, metrics) => { snapshot = metrics; policyPlans = metrics.PlansEvaluated; };
        AIDecision decision = ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), null, [], [], [],
            [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], 0, 0, 1);
        Assert.True(decision.HasCandidate);
        Assert.Equal(CharacterActionType.NormalAttack, decision.ActionType);
        Assert.NotNull(snapshot);
        Assert.Equal(4096, policyPlans);
        Assert.Equal(policyPlans, snapshot.PlansEvaluated);
        Assert.Equal(policyPlans + 1, ai.LastMetrics!.PlansEvaluated);
    }

    private sealed class DrainPolicy : IDecisionPolicy
    {
        public DecisionEvaluationResult Evaluate(DecisionEvaluationContext context)
        {
            while (DecisionActionPlanner.Plan(context, Actions.NormalAttack(), null).Count > 0) { }
            return new(null, null, []);
        }
    }
}
