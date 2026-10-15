using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public partial class DecisionRiskBudgetTests
{
    [Theory]
    [InlineData(AISelectionMode.Deterministic)]
    [InlineData(AISelectionMode.WeightedNearBest)]
    public void DefaultAI_LateItemReceivesBudgetAfterManyAttackAndSkillPlans(AISelectionMode mode)
    {
        BattleFixture fixture = new();
        List<Character> enemies = [.. Enumerable.Range(0, 4100).Select(index => QueueTestHelper.NewCharacter($"enemy-{index}"))];
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.Id = 1001;
        Skill active = new TestSkill(SkillType.Item) { Level = 1 };
        Item item = new(ItemType.Consumable) { Id = 1002, RemainUseTimes = 1 };
        item.Skills.Active = active;
        AIController ai = new(fixture.Queue, null) { SelectionMode = mode };
        HashSet<CharacterActionType> evaluated = [];
        ai.EvaluatePlanEvent += (_, plan) =>
        {
            evaluated.Add(plan.Decision.ActionType);
            return plan.Decision.ItemToUse == item ? 10000 : 0;
        };
        AIDecision decision = ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), null,
            [], [skill], [item], enemies, [], enemies, [], 1, 1, 1);
        Assert.Same(item, decision.ItemToUse);
        Assert.Contains(CharacterActionType.NormalAttack, evaluated);
        Assert.Contains(CharacterActionType.PreCastSkill, evaluated);
        Assert.Contains(CharacterActionType.UseItem, evaluated);
        Assert.Equal(4096, ai.LastMetrics!.PlansEvaluated);
        Assert.Equal(4096, ai.LastMetrics.DefaultAIPlansEvaluated);
        Assert.Contains(ai.LastPlanningDiagnostics, d => d.Path == "defaultAI.useItem[1002]" && d.Code == "budget.truncated");
    }

    [Fact]
    public void DefaultAI_ReservesMovementWhenAllScoredPlansAreRejected()
    {
        BossBattleFixture fixture = new(mapMode: true, usePolicy: false);
        fixture.Boss.ExMOV = 8;
        List<Skill> skills = [.. Enumerable.Range(0, 2).Select(index => new TestSkill(SkillType.Skill)
        {
            Id = 9901 + index, Character = fixture.Boss, GamingQueue = fixture.Queue, Level = 1,
            IsNonDirectional = true, AllowSelectNoCharacterGrid = true, CastAnywhere = true
        })];
        AIController ai = new(fixture.Queue, fixture.Queue.Map) { SelectionMode = AISelectionMode.Deterministic };
        ai.EvaluatePlanEvent += (_, _) => double.NaN;
        List<Character> enemies = fixture.Queue.GetEnemies(fixture.Boss);
        AIDecision decision = ai.DecideAIAction(fixture.Boss,
            fixture.Queue.CharacterDecisionPoints[fixture.Boss], fixture.Queue.Map!.GetCharacterCurrentGrid(fixture.Boss),
            [.. fixture.Queue.Map.Grids.Values], skills, [], enemies, [], enemies, [], 0, 1, 0);
        Assert.True(decision.HasCandidate);
        Assert.Equal(CharacterActionType.Move, decision.ActionType);
        Assert.Equal(4096, ai.LastMetrics!.DefaultAIPlansEvaluated);
        Assert.Contains(ai.LastPlanningDiagnostics, d => d.Code == "budget.truncated");
    }

    private static DecisionPlan StopPlan() => new() { Decision = new() { ActionType = CharacterActionType.EndTurn, HasCandidate = true } };

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CustomAction_ClipsResultsAndDoesNotDoubleChargeNestedPlanner(bool nested)
    {
        BattleFixture fixture = new();
        fixture.A.NormalAttack.CanSelectSelf = fixture.A.NormalAttack.CanSelectTeammate = true;
        DecisionTableRegistry registry = new();
        registry.RegisterAction("test:many", (ctx, _, _) => nested
            ? DecisionActionPlanner.Plan(ctx, Actions.NormalAttack(), null)
            : [StopPlan(), StopPlan(), StopPlan(), StopPlan(), StopPlan()]);
        DecisionTableCompiler compiler = new(registry);
        DecisionTableConfig config = DecisionTableBuilder.Create("test:custom-budget")
            .AddRule("miss", r => r.When(Conditions.Compare("plan.targetCount", "greaterThan", 100, "p"))
                .Then(new() { Type = "test:many", BindPlan = "p" }))
            .AddRule("hit", r => r.Target(Targets.Enemies()).Then(Actions.NormalAttack())).Build(compiler);
        DecisionEvaluationContext context = Context(fixture, 3);
        DecisionEvaluationResult result = compiler.Compile(config).Evaluate(context);
        Assert.Equal("hit", result.RuleId);
        Assert.Equal(3, context.Metrics.PlansEvaluated);
        Assert.Equal(3, context.Metrics.PolicyPlansEvaluated);
        Assert.Equal(1, context.Metrics.BudgetTruncations);
        Assert.Contains(result.Diagnostics, d => d.Path == "miss" && d.Code == "budget.truncated");
    }

    [Fact]
    public void Budget_UnusedQuotaPassesToLaterRules()
    {
        BattleFixture fixture = new();
        List<int> quotas = [];
        DecisionTableRegistry registry = new();
        registry.RegisterAction("test:one", (ctx, _, _) => { quotas.Add(ctx.PlanBudget); return [StopPlan()]; });
        registry.RegisterAction("test:empty", (ctx, _, _) => { quotas.Add(ctx.PlanBudget); return []; });
        DecisionTableCompiler compiler = new(registry);
        DecisionTableConfig config = DecisionTableBuilder.Create("test:unused")
            .AddRule("one", r => r.When(Conditions.Compare("plan.targetCount", "greaterThan", 0, "p")).Then(new() { Type = "test:one", BindPlan = "p" }))
            .AddRule("empty", r => r.Then(new() { Type = "test:empty" }))
            .AddRule("hit", r => r.Then(new() { Type = "test:one" })).Build(compiler);
        DecisionEvaluationContext context = Context(fixture, 6);
        Assert.Equal("hit", compiler.Compile(config).Evaluate(context).RuleId);
        Assert.Equal([2, 3, 5], quotas);
        Assert.Equal(2, context.Metrics.PlansEvaluated);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Budget_ExhaustionIsDistinctAndEndTurnRemainsAvailable(int budget)
    {
        BattleFixture fixture = new();
        ActionConfig miss = Actions.NormalAttack();
        miss.BindPlan = "p";
        DecisionTableConfig config = DecisionTableBuilder.Create("test:exhausted")
            .AddRule("miss", r => r.When(Conditions.Compare("plan.targetCount", "greaterThan", 100, "p")).Then(miss))
            .AddRule("blocked", r => r.Then(Actions.NormalAttack()))
            .AddRule("stop", r => r.Then(Actions.EndTurn())).Build();
        DecisionEvaluationContext context = Context(fixture, budget);
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(config).Evaluate(context);
        Assert.Equal("stop", result.RuleId);
        Assert.Equal(budget, context.Metrics.PlansEvaluated);
        Assert.Contains(result.Diagnostics, d => d.Path == "blocked" && d.Code == "budget.exhausted");
        Assert.Equal(budget == 0 ? 2 : 1, context.Metrics.BudgetExhaustions);
    }

    [Fact]
    public void Precheck_PreservesUnknownAcrossAnyAndNotWithoutCallingExtensions()
    {
        BattleFixture fixture = new();
        int calls = 0;
        DecisionTableRegistry registry = new();
        registry.RegisterCondition("test:unknown", (_, _, _) => { calls++; return true; });
        DecisionTableCompiler compiler = new(registry);
        DecisionTableConfig config = DecisionTableBuilder.Create("test:unknown")
            .AddRule("false", r => r.When(Conditions.All(new() { Type = "test:unknown" }, Conditions.SelfHpRatio("lessThan", 0))).Then(Actions.NormalAttack()))
            .AddRule("true", r => r.When(Conditions.Not(Conditions.Any(Conditions.SelfHpRatio("lessThan", 0),
                Conditions.Not(new() { Type = "test:unknown" })))).Then(Actions.NormalAttack())).Build(compiler);
        DecisionEvaluationContext context = Context(fixture, 1);
        Assert.Equal("true", compiler.Compile(config).Evaluate(context).RuleId);
        Assert.Equal(1, calls);
        Assert.Equal(1, context.Metrics.PlansEvaluated);
    }

    [Fact]
    public void Budget_DisabledRejectedAndFalseRulesDoNotTakeShares()
    {
        BattleFixture fixture = new();
        DecisionTableConfig config = DecisionTableBuilder.Create("test:excluded")
            .AddRule("disabled", r => r.Enabled(false).Then(Actions.NormalAttack()))
            .AddRule("rejected", r => r.Then(Actions.NormalAttack()))
            .AddRule("false", r => r.When(Conditions.Any()).Then(Actions.NormalAttack()))
            .AddRule("hit", r => r.Then(Actions.NormalAttack())).Build();
        DecisionEvaluationContext context = new()
        {
            Queue = fixture.Queue, Actor = fixture.A, Points = fixture.Points(fixture.A), Enemies = [fixture.D],
            SelectableEnemies = [fixture.D], RejectedRules = new HashSet<string> { "rejected" }, PlanBudget = 1
        };
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(config).Evaluate(context);
        Assert.Equal("hit", result.RuleId);
        Assert.DoesNotContain(result.Diagnostics, d => d.Path == "disabled");
        Assert.Contains(result.Diagnostics, d => d.Path == "rejected" && d.Code == "execution.rejected");
        Assert.Equal(1, context.Metrics.PlansEvaluated);
    }

    [Fact]
    public void EndTurn_ConditionStillRejectsAtZeroBudget()
    {
        BattleFixture fixture = new();
        DecisionTableConfig config = DecisionTableBuilder.Create("test:stop-condition")
            .AddRule("no", r => r.When(Conditions.Any()).Then(Actions.EndTurn()))
            .AddRule("yes", r => r.When(Conditions.All()).Then(Actions.EndTurn())).Build();
        Assert.Equal("yes", new DecisionTableCompiler().Compile(config).Evaluate(Context(fixture, 0)).RuleId);
    }

    [Fact]
    public void Scopes_ShareRiskCacheAcrossUnmatchedRules()
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.D.ExATR = 10;
        fixture.A.NormalAttack.AttackAnywhere = true;
        ActionConfig action = Actions.NormalAttack(movement: "never");
        action.BindPlan = "p";
        DecisionTableConfig config = DecisionTableBuilder.Create("test:shared-risk")
            .AddRule("miss", r => r.When(Conditions.Compare("plan.targetCount", "greaterThan", 2, "p")).Then(action))
            .AddRule("hit", r => r.Then(action)).Build();
        DecisionEvaluationContext context = Context(fixture);
        Assert.Equal("hit", new DecisionTableCompiler().Compile(config).Evaluate(context).RuleId);
        Assert.Equal(1, context.Metrics.RiskEvaluations);
        Assert.Equal(1, context.Metrics.RiskCacheHits);
    }
}
