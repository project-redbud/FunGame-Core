using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class AIControllerLifecycleTests
{
    private static AIDecision Decide(BattleFixture fixture, AIController ai)
    {
        fixture.A.NormalAttack.AttackAnywhere = true;
        Grid? origin = fixture.Queue.Map?.GetCharacterCurrentGrid(fixture.A);
        return ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), origin, origin == null ? [] : [origin],
            [], [], [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], 0, 0, 1);
    }

    private static void Place(BattleFixture fixture)
    {
        GameMap map = fixture.Queue.Map!;
        map.SetCharacterCurrentGrid(fixture.A, map[0, 0]!);
        map.SetCharacterCurrentGrid(fixture.D, map[4, 0]!);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LoadMap_PreservesHeldControllerSettingsAndSubscriptions(bool initiallyMapped)
    {
        BattleFixture fixture = new(mapMode: initiallyMapped);
        AIController ai = fixture.Queue.AI!;
        ai.SelectionMode = AISelectionMode.Deterministic;
        ai.ScoreTolerance = 17;
        ai.MovementBenefitThreshold = 4;
        ai.RepeatedPositionPenalty = 8;
        int evaluations = 0;
        ai.EvaluatePlanEvent += (_, _) => { evaluations++; return 100; };
        fixture.Queue.LoadGameMap(new TestMap());
        Assert.Same(fixture.Queue.Map, ai.Map);
        Assert.Same(ai, fixture.Queue.AI);
        Assert.True(ai.IsMapMode);
        Assert.Equal(AISelectionMode.Deterministic, ai.SelectionMode);
        Assert.Equal(17, ai.ScoreTolerance);
        Assert.Equal(4, ai.MovementBenefitThreshold);
        Assert.Equal(8, ai.RepeatedPositionPenalty);
        Place(fixture);
        Assert.True(Decide(fixture, fixture.Queue.AI!).HasCandidate);
        Assert.Equal(1, evaluations);
    }

    [Fact]
    public void DisableAndReload_PreservesInstanceAndInvokesEachSubscriptionOnce()
    {
        BattleFixture fixture = new();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(DecisionTableBuilder.Create("test:lifecycle").Build()));
        fixture.A.DecisionTableId = "test:lifecycle";
        Assert.Empty(loader.Bind(fixture.A));
        AIController ai = fixture.Queue.AI!;
        ai.SelectionMode = AISelectionMode.Deterministic;
        int policies = 0;
        int plans = 0;
        ai.PolicyEvaluatedEvent += (_, _, _) => policies++;
        ai.EvaluatePlanEvent += (_, _) => { plans++; return 100; };
        for (int i = 0; i < 3; i++)
        {
            fixture.Queue.EnableAI = false;
            Assert.Null(fixture.Queue.AI);
            fixture.Queue.LoadGameMap(new TestMap());
            Assert.Same(fixture.Queue.Map, ai.Map);
            fixture.Queue.EnableAI = true;
            Assert.Same(ai, fixture.Queue.AI);
            Assert.Equal(AISelectionMode.Deterministic, ai.SelectionMode);
            Place(fixture);
            Assert.True(Decide(fixture, fixture.Queue.AI!).HasCandidate);
        }
        Assert.Equal(3, policies);
        Assert.Equal(3, plans);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LifecycleChange_ClearsResultsAndHistory(bool disable)
    {
        BattleFixture fixture = new(mapMode: true);
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(DecisionTableBuilder.Create("test:missing")
            .AddRule("missing", rule => rule.When(Conditions.SelfHpRatio("lessThan", 0)).Then(Actions.NormalAttack())).Build()));
        fixture.A.DecisionTableId = "test:missing";
        Assert.Empty(loader.Bind(fixture.A));
        AIController ai = fixture.Queue.AI!;
        ai.SelectionMode = AISelectionMode.Deterministic;
        IReadOnlyList<int> recent = [];
        ai.EvaluatePlanEvent += (context, _) => { recent = context.RecentPositions; return 100; };
        Decide(fixture, ai);
        fixture.Queue.Map!.SetCharacterCurrentGrid(fixture.A, fixture.Queue.Map[3, 0]!);
        Decide(fixture, ai);
        Assert.NotEmpty(recent);
        Assert.NotNull(ai.LastPolicyResult);
        Assert.NotNull(ai.LastMetrics);
        Assert.NotEmpty(ai.LastPlanningDiagnostics);
        if (disable) fixture.Queue.EnableAI = false;
        else fixture.Queue.LoadGameMap(new TestMap());
        Assert.Null(ai.LastPolicyResult);
        Assert.Null(ai.LastMetrics);
        Assert.Empty(ai.LastPlanningDiagnostics);
        fixture.Queue.EnableAI = true;
        Place(fixture);
        Decide(fixture, fixture.Queue.AI!);
        Assert.Empty(recent);
    }

    [Fact]
    public void SameMapAndUnchangedEnableFlag_PreserveTransientState()
    {
        BattleFixture fixture = new();
        RetainedMap map = new();
        map.Load();
        fixture.Queue.LoadGameMap(map);
        Place(fixture);
        AIController ai = fixture.Queue.AI!;
        ai.SelectionMode = AISelectionMode.Deterministic;
        IReadOnlyList<int> recent = [];
        ai.EvaluatePlanEvent += (context, _) => { recent = context.RecentPositions; return 100; };
        Decide(fixture, ai);
        map.SetCharacterCurrentGrid(fixture.A, map[3, 0]!);
        Decide(fixture, ai);
        int[] expected = recent.ToArray();
        Assert.NotEmpty(expected);
        DecisionMetrics metrics = ai.LastMetrics!;
        fixture.Queue.LoadGameMap(map);
        fixture.Queue.EnableAI = true;
        Assert.Same(ai, fixture.Queue.AI);
        Assert.Same(metrics, ai.LastMetrics);
        Decide(fixture, ai);
        Assert.Equal(expected, recent);
    }

    [Fact]
    public void ExplicitController_KeepsItsOwnMapBinding()
    {
        BattleFixture fixture = new(mapMode: true);
        GameMap oldMap = fixture.Queue.Map!;
        AIController independent = new(fixture.Queue, oldMap);
        fixture.Queue.LoadGameMap(new TestMap());
        Assert.Same(oldMap, independent.Map);
        Assert.NotSame(fixture.Queue.AI, independent);
    }

    [Fact]
    public void SeparateQueues_DoNotShareSettingsOrTransientState()
    {
        BattleFixture first = new(mapMode: true);
        BattleFixture second = new(mapMode: true);
        AIController ai = first.Queue.AI!;
        AIController other = second.Queue.AI!;
        ai.SelectionMode = AISelectionMode.Deterministic;
        Decide(first, ai);
        Decide(second, other);
        DecisionMetrics metrics = other.LastMetrics!;
        first.Queue.EnableAI = false;
        first.Queue.LoadGameMap(new TestMap());
        Assert.NotSame(ai, other);
        Assert.Equal(AISelectionMode.Legacy, other.SelectionMode);
        Assert.Same(metrics, other.LastMetrics);
        Assert.Same(second.Queue.Map, other.Map);
    }

    [Fact]
    public void WeightedSequence_ContinuesAcrossTogglesAndEquivalentMapsAndReplays()
    {
        string[] baseline = Sequence(false);
        string[] changed = Sequence(true);
        Assert.True(baseline.Distinct().Count() > 1);
        Assert.Equal(baseline, changed);
        Assert.Equal(changed, Sequence(true));
    }

    private static string[] Sequence(bool changeLifecycle)
    {
        BossBattleFixture fixture = new(mapMode: true, usePolicy: false);
        AIController ai = fixture.Queue.AI!;
        ai.SelectionMode = AISelectionMode.WeightedNearBest;
        ai.ScoreTolerance = 1000;
        Character actor = fixture.Boss;
        actor.NormalAttack.AttackAnywhere = true;
        List<Character> enemies = [fixture.Warrior, fixture.Mage, fixture.Healer];
        List<string> result = [];
        for (int i = 0; i < 32; i++)
        {
            if (changeLifecycle && i > 0 && i % 4 == 0)
            {
                fixture.Queue.EnableAI = false;
                fixture.Queue.LoadGameMap(new BossMap());
                GameMap replacement = fixture.Queue.Map!;
                replacement.SetCharacterCurrentGrid(actor, replacement[5, 2]!);
                replacement.SetCharacterCurrentGrid(fixture.Warrior, replacement[2, 2]!);
                replacement.SetCharacterCurrentGrid(fixture.Mage, replacement[2, 3]!);
                replacement.SetCharacterCurrentGrid(fixture.Healer, replacement[3, 2]!);
                fixture.Queue.EnableAI = true;
                Assert.Same(ai, fixture.Queue.AI);
            }
            Grid origin = fixture.Queue.Map!.GetCharacterCurrentGrid(actor)!;
            AIDecision decision = fixture.Queue.AI!.DecideAIAction(actor, fixture.Queue.CharacterDecisionPoints[actor],
                origin, [origin], [], [], enemies, [], enemies, [], 0, 0, 1);
            Assert.True(decision.HasCandidate);
            result.Add(string.Join(",", decision.Targets.Select(target => target.Guid)));
        }
        return result.ToArray();
    }

    private sealed class RetainedMap : GameMap
    {
        public override string Name => "Retained";
        public override string Description => "";
        public override string Version => "1";
        public override string Author => "Tests";
        public override int Length => 6;
        public override int Width => 1;
        public override int Height => 1;
        public override float Size => 1;
        public override GameMap InitGamingQueue(IGamingQueue queue) => this;
    }
}
