using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public partial class DecisionRiskBudgetTests
{
    [Theory]
    [InlineData("attack", AISelectionMode.Deterministic)]
    [InlineData("skill", AISelectionMode.Deterministic)]
    [InlineData("instant", AISelectionMode.Deterministic)]
    [InlineData("chant", AISelectionMode.Deterministic)]
    [InlineData("item", AISelectionMode.Deterministic)]
    [InlineData("attack", AISelectionMode.WeightedNearBest)]
    [InlineData("item", AISelectionMode.WeightedNearBest)]
    public void Scoring_EveryActionPaysActualRisk(string type, AISelectionMode mode)
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.D.ExATR = 10;
        fixture.A.NormalAttack.AttackAnywhere = true;
        Skill skill = fixture.NewSkill(type is "instant" or "chant" ? SkillType.Magic : SkillType.Skill, fixture.A);
        skill.GamingQueue = fixture.Queue;
        skill.CastAnywhere = true;
        skill.CastTime = type == "chant" ? 3 : 0;
        List<Item> items = [];
        if (type == "item")
        {
            skill.SkillType = SkillType.Item;
            Item item = new(ItemType.Consumable) { Id = 123, RemainUseTimes = 1 };
            item.Skills.Active = skill;
            items.Add(item);
        }
        AIController ai = new(fixture.Queue, fixture.Queue.Map) { SelectionMode = mode };
        ai.EvaluatePlanEvent += (_, _) => 100;
        AIDecision decision = ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), fixture.Queue.Map![0, 0],
            [.. fixture.Queue.Map.Grids.Values], type is "attack" or "item" ? [] : [skill], items,
            [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C],
            type == "item" ? 1 : 0, type is "attack" or "item" ? 0 : 1, type == "attack" ? 1 : 0, isChainAction: true);
        Assert.True(decision.HasCandidate);
        Assert.Equal(99d, decision.Score, 3);
        Assert.Equal(1, ai.LastMetrics!.PositionsEvaluated);
        Assert.Equal(1, ai.LastMetrics.RiskEvaluations);
        Assert.Equal(0, ai.LastMetrics.CentersGenerated);
    }

    [Fact]
    public void Risk_CacheIsSharedAcrossActionsButNotContexts()
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.D.ExATR = 10;
        fixture.A.NormalAttack.AttackAnywhere = true;
        DecisionEvaluationContext context = Context(fixture);
        IReadOnlyList<DecisionPlan> first = DecisionActionPlanner.Plan(context, Actions.NormalAttack(), null);
        IReadOnlyList<DecisionPlan> second = DecisionActionPlanner.Plan(context, Actions.NormalAttack(), null);
        Assert.All(first.Concat(second), p => Assert.Equal(1d, p.Risk));
        Assert.Equal(1, context.Metrics.RiskEvaluations);
        Assert.True(context.Metrics.RiskCacheHits > 0);
        fixture.D.CharacterState = CharacterState.NotActionable;
        DecisionEvaluationContext next = Context(fixture);
        Assert.All(DecisionActionPlanner.Plan(next, Actions.NormalAttack(), null), p => Assert.Equal(0d, p.Risk));
        Assert.Equal(1, next.Metrics.RiskEvaluations);
    }

    [Fact]
    public void Risk_GlobalCurrentDoesNotStartPositionSearch()
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.D.ExATR = 10;
        fixture.A.NormalAttack.AttackAnywhere = true;
        DecisionEvaluationContext context = Context(fixture);
        Assert.NotEmpty(DecisionActionPlanner.Plan(context, Actions.NormalAttack(), null));
        Assert.Equal(1, context.Metrics.PositionsGenerated);
        Assert.Equal(1, context.Metrics.PositionsEvaluated);
        Assert.Equal(1, context.Metrics.RiskEvaluations);
    }

    [Fact]
    public void Risk_NoMapLeafIsFalseAndNotUsesBooleanSemantics()
    {
        BattleFixture fixture = new();
        ActionConfig action = Actions.NormalAttack();
        action.BindPlan = "p";
        DecisionTableConfig config = DecisionTableBuilder.Create("test:no-map-risk")
            .AddRule("leaf", r => r.When(Conditions.Compare("plan.risk", "atLeast", 0, "p")).Then(action))
            .AddRule("negated", r => r.When(Conditions.Not(Conditions.Compare("plan.risk", "atLeast", 0, "p"))).Then(action)).Build();
        DecisionEvaluationContext context = Context(fixture);
        Assert.Equal("negated", new DecisionTableCompiler().Compile(config).Evaluate(context).RuleId);
        Assert.Equal(0, context.Metrics.RiskEvaluations);
    }

    [Fact]
    public void Risk_CustomConditionReadsActualValue()
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.A.NormalAttack.AttackAnywhere = true;
        fixture.D.ExATR = 10;
        DecisionTableRegistry registry = new();
        registry.RegisterCondition("test:risk", (_, _, plan) => plan.Risk == 1);
        DecisionTableCompiler compiler = new(registry);
        DecisionTableConfig config = DecisionTableBuilder.Create("test:custom-risk")
            .AddRule("risk", r => r.When(new() { Type = "test:risk" }).Then(Actions.NormalAttack(movement: "never"))).Build(compiler);
        Assert.True(compiler.Compile(config).Evaluate(Context(fixture)).Matched);
    }
}
