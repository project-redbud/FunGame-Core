using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class DecisionTableTests
{
    private static DecisionEvaluationContext Context(BattleFixture fixture, Character? actor = null, bool chain = false)
    {
        Character character = actor ?? fixture.A;
        List<Character> allies = fixture.Queue.GetTeammates(character);
        List<Character> enemies = fixture.Queue.GetEnemies(character);
        (List<Character> selectableAllies, List<Character> selectableEnemies, List<Skill> skills, List<Item> items) = fixture.Queue.GetTurnStartNeedyList(character, allies, enemies);
        foreach (Skill skill in skills) skill.GamingQueue = fixture.Queue;
        return new() { Queue = fixture.Queue, Actor = character, Points = fixture.Points(character), Allies = allies, Enemies = enemies,
            Skills = skills, Items = items, SelectableAllies = selectableAllies, SelectableEnemies = selectableEnemies, IsChainAction = chain,
            TurnOrigin = fixture.Queue.Map?.GetCharacterCurrentGrid(character), MoveGrids = fixture.Queue.Map?.Grids.Values.ToArray() ?? [] };
    }

    private static DecisionTableConfig AttackTable() => DecisionTableBuilder.Create("test:attack")
        .AddRule("attack", r => r.Target(Targets.Enemies("victim")).Then(Actions.NormalAttack("victim"))).Build();

    [Fact]
    public void EmptyPolicy_DoesNotConsumeDefaultRandomSequence()
    {
        BattleFixture fixture = new();
        AIController first = new(fixture.Queue, null);
        AIController second = new(fixture.Queue, null);
        AIDecision without = first.DecideAIAction(fixture.A, fixture.Points(fixture.A), null, [], [], [], [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], 0, 0, 1);
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(DecisionTableBuilder.Create("test:empty").Build()));
        fixture.A.DecisionTableId = "test:empty";
        Assert.Empty(loader.Bind(fixture.A));
        AIDecision with = second.DecideAIAction(fixture.A, fixture.Points(fixture.A), null, [], [], [], [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], 0, 0, 1);
        Assert.False(second.LastPolicyResult!.Matched);
        Assert.Equal(without.ActionType, with.ActionType);
        Assert.Equal(without.Targets, with.Targets);
        Assert.Equal(without.Score, with.Score);
    }

    [Fact]
    public void Policy_UsesBoundPatientAndSkipsUnavailableRule()
    {
        BattleFixture fixture = new();
        fixture.B.HP = fixture.B.MaxHP * .2;
        fixture.C.HP = fixture.C.MaxHP * .1;
        Skill heal = fixture.NewSkill(SkillType.Skill, fixture.A);
        heal.Id = 1001;
        heal.CanSelectEnemy = false;
        heal.CanSelectTeammate = true;
        DecisionTableConfig config = DecisionTableBuilder.Create("test:heal")
            .AddRule("missing", r => r.Priority(100).Then(Actions.CastSkill(999)))
            .AddRule("heal", r => r.Target(Targets.Allies("patient"))
                .When(Conditions.All(Conditions.TargetHpRatio("patient", "lessThan", .3), Conditions.SkillAvailable(1001)))
                .Then(Actions.CastSkill(1001, "patient"))).Build();
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(config).Evaluate(Context(fixture));
        Assert.Equal("heal", result.RuleId);
        Assert.Same(heal, result.Decision!.SkillToUse);
        Assert.Same(fixture.C, Assert.Single(result.Decision.Targets));
        Assert.Single(result.Diagnostics);
        Assert.Equal(0, heal.CurrentCD);
    }

    [Fact]
    public void EndTurn_IsAnEffectiveDecisionAndKeepsEventPriority()
    {
        BattleFixture fixture = new();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(DecisionTableBuilder.Create("test:stop").AddRule("stop", r => r.Then(Actions.EndTurn())).Build()));
        fixture.A.DecisionTableId = "test:stop";
        Assert.Empty(loader.Bind(fixture.A));
        fixture.Queue.SetCharactersToAIControl(false, false, [fixture.A]);
        fixture.Points(fixture.B).CurrentDecisionPoints = fixture.Points(fixture.C).CurrentDecisionPoints = 0;
        fixture.Run(fixture.A);
        Assert.Empty(fixture.Queue.LastRound.Actions);
        Assert.True(fixture.Queue.AI!.LastPolicyResult!.Matched);
        Assert.Equal("stop", fixture.Queue.AI.LastPolicyResult.RuleId);
    }

    [Theory]
    [InlineData(CharacterState.AttackRestricted)]
    [InlineData(CharacterState.BattleRestricted)]
    [InlineData(CharacterState.NotActionable)]
    public void Policy_DoesNotOverrideHardRestrictions(CharacterState state)
    {
        BattleFixture fixture = new();
        fixture.A.CharacterState = state;
        Assert.False(new DecisionTableCompiler().Compile(AttackTable()).Evaluate(Context(fixture)).Matched);
    }

    [Fact]
    public void Policy_PreservesPriorityAndConfigurationSnapshot()
    {
        BattleFixture fixture = new();
        DecisionTableBuilder builder = DecisionTableBuilder.Create("test:order")
            .AddRule("first", r => r.Priority(10).Then(Actions.EndTurn()))
            .AddRule("second", r => r.Priority(10).Then(Actions.NormalAttack()));
        DecisionTableConfig config = builder.Build();
        FunGame.Core.Interface.Base.IDecisionPolicy policy = new DecisionTableCompiler().Compile(config);
        config.Rules.Clear();
        builder.AddRule("later", r => r.Priority(20).Then(Actions.NormalAttack()));
        Assert.Equal("first", policy.Evaluate(Context(fixture)).RuleId);
        Assert.Empty(config.Rules);
        Assert.Equal(3, builder.Build().Rules.Count);
    }

    [Fact]
    public void Loader_DiagnosesInvalidReferencesAndDuplicateIds()
    {
        DecisionTableCompiler compiler = new(skills: [1001]);
        DecisionTableConfig config = DecisionTableBuilder.Create("test:invalid").AddRule("cast", r => r.Then(Actions.CastSkill(1002))).Build();
        Assert.Contains(compiler.Validate(config), d => d.Path == "rules[0].action.skillId");
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(AttackTable()));
        Assert.NotEmpty(loader.Register(AttackTable()));
        Assert.False(DecisionTableParser.TryParse("{\"unknown\":1}", out _, out IReadOnlyList<DecisionDiagnostic> errors));
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void JsonRoundTrip_PreservesRuleExecutionAndCharacterBinding()
    {
        BattleFixture fixture = new();
        DecisionTableConfig config = AttackTable();
        DecisionTableConfig parsed = DecisionTableParser.Parse(DecisionTableGenerator.GetJson(config));
        Assert.Equal("attack", new DecisionTableCompiler().Compile(parsed).Evaluate(Context(fixture)).RuleId);
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(config));
        fixture.A.DecisionTableId = config.Id;
        Assert.Empty(loader.Bind(fixture.A));
        Assert.Same(fixture.A.DecisionPolicy, fixture.A.Copy().DecisionPolicy);
        Assert.Same(fixture.A.DecisionPolicy, CharacterBuilder.Build(fixture.A).DecisionPolicy);
        string json = JsonService.GetString(fixture.A);
        Assert.DoesNotContain("DecisionPolicy", json);
        Character restored = JsonService.GetObject<Character>(json)!;
        Assert.Equal(config.Id, restored.DecisionTableId);
        Assert.Null(restored.DecisionPolicy);
        Assert.Empty(loader.Bind(restored));
    }

    [Fact]
    public void ChainPolicy_DoesNotMoveToReachTarget()
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.A.ExATR = -100;
        fixture.A.ExMOV = 5;
        Assert.False(new DecisionTableCompiler().Compile(AttackTable()).Evaluate(Context(fixture, chain: true)).Matched);
        Assert.True(new DecisionTableCompiler().Compile(AttackTable()).Evaluate(Context(fixture)).Matched);
    }

    [Fact]
    public void ActionStart_InvalidatesSkillWithoutSpendingResources()
    {
        BattleFixture fixture = new();
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.Id = 1001;
        skill.EPCost = 10;
        fixture.A.EP = 50;
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(DecisionTableBuilder.Create("test:cast").AddRule("cast", r => r.Then(Actions.CastSkill(1001))).AddRule("stop", r => r.Then(Actions.EndTurn())).Build()));
        fixture.A.DecisionTableId = "test:cast";
        Assert.Empty(loader.Bind(fixture.A));
        fixture.Queue.SetCharactersToAIControl(false, false, [fixture.A]);
        fixture.A.Effects.Add(new InvalidateSkillEffect(skill));
        fixture.Run(fixture.A);
        Assert.Equal(50, fixture.A.EP);
        Assert.DoesNotContain(fixture.Queue.LastRound.Actions, a => a.Skill == skill);
    }

    [Fact]
    public void Extensions_RequireRegistrationAndShareJsonProtocol()
    {
        DecisionTableRegistry registry = new();
        registry.RegisterCondition("test:yes", (_, _, _) => true);
        DecisionTableConfig config = new() { Id = "test:custom", Rules = [new() { Id = "yes", When = new() { Type = "test:yes" }, Action = Actions.EndTurn() }] };
        Assert.NotEmpty(new DecisionTableCompiler().Validate(config));
        Assert.True(new DecisionTableCompiler(registry).Compile(DecisionTableParser.Parse(DecisionTableGenerator.GetJson(config))).Evaluate(Context(new())).Matched);
    }

    [Fact]
    public void NoMap_SpatialConditionsDoNotMatch()
    {
        DecisionTableConfig config = new() { Id = "test:space", Rules = [new() { Id = "space", Target = Targets.Enemies("victim"),
            When = Conditions.Compare("distance", "lessThan", 5, "victim"), Action = Actions.NormalAttack("victim") }] };
        Assert.False(new DecisionTableCompiler().Compile(config).Evaluate(Context(new())).Matched);
    }

    private sealed class InvalidateSkillEffect(Skill skill) : Effect(skill)
    {
        public override void OnCharacterActionStart(FunGame.Core.Model.EffectContext.ActionContext ctx) => Skill.CurrentCD = 100;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Queue_PreservesModDecisionAndEnableAISwitch(bool enableAI)
    {
        BattleFixture fixture = new();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(DecisionTableBuilder.Create("test:stop").AddRule("stop", r => r.Then(Actions.EndTurn())).Build()));
        fixture.A.DecisionTableId = "test:stop";
        Assert.Empty(loader.Bind(fixture.A));
        fixture.Queue.SetCharactersToAIControl(false, false, [fixture.A]);
        int evaluated = 0;
        fixture.Queue.AI!.PolicyEvaluatedEvent += (_, _, _) => evaluated++;
        fixture.Queue.EnableAI = enableAI;
        fixture.Queue.DecideActionEvent += ctx => CharacterActionType.NormalAttack;
        fixture.Run(fixture.A);
        Assert.Equal(0, evaluated);
        Assert.Contains(fixture.Queue.LastRound.Actions, a => a.Actor == fixture.A && a.ActionType == CharacterActionType.NormalAttack);
    }

    [Fact]
    public void Policy_RespectsExplicitCategoryDisableAndRejectedRules()
    {
        BattleFixture fixture = new();
        DecisionEvaluationContext context = Context(fixture);
        DecisionEvaluationContext disabled = new() { Queue = context.Queue, Actor = context.Actor, Points = context.Points,
            Enemies = context.Enemies, SelectableEnemies = context.SelectableEnemies, AllowAttack = false };
        Assert.False(new DecisionTableCompiler().Compile(AttackTable()).Evaluate(disabled).Matched);
        DecisionEvaluationContext rejected = new() { Queue = context.Queue, Actor = context.Actor, Points = context.Points,
            Enemies = context.Enemies, SelectableEnemies = context.SelectableEnemies, RejectedRules = new HashSet<string> { "attack" } };
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(AttackTable()).Evaluate(rejected);
        Assert.False(result.Matched);
        Assert.Contains(result.Diagnostics, d => d.Message.Contains("重验"));
    }

    [Fact]
    public void PolicyIdChange_ClearsStaleRuntimeBinding()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(AttackTable()));
        actor.DecisionTableId = "test:attack";
        Assert.Empty(loader.Bind(actor));
        CharacterBuilder builder = new(actor) { DecisionTableId = "test:missing" };
        Assert.Null(builder.Build([], []).DecisionPolicy);
        actor.DecisionTableId = "test:missing";
        Assert.Null(actor.DecisionPolicy);
        Assert.NotEmpty(loader.Bind(actor));
    }

    [Fact]
    public void CodePolicy_BindsThroughLoaderAndProducesEffectiveEndTurn()
    {
        BattleFixture fixture = new();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register("test:code", new EndPolicy()));
        Assert.NotEmpty(loader.Register("test:code", new EndPolicy()));
        fixture.A.DecisionTableId = "test:code";
        Assert.Empty(loader.Bind(fixture.A));
        AIController ai = new(fixture.Queue, null);
        AIDecision decision = ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), null, [], [], [], [fixture.D], [], [fixture.D], [], 0, 0, 1);
        Assert.Equal(CharacterActionType.EndTurn, decision.ActionType);
        Assert.True(decision.HasCandidate);
    }

    private sealed class EndPolicy : FunGame.Core.Interface.Base.IDecisionPolicy
    {
        public DecisionEvaluationResult Evaluate(DecisionEvaluationContext context) => new(new() { ActionType = CharacterActionType.EndTurn }, "code", []);
    }
}
