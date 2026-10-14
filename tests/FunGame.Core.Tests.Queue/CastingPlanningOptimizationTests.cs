using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

public class CastingPlanningOptimizationTests
{
    [Theory]
    [InlineData("current", false)]
    [InlineData("safe", true)]
    public void GlobalDirectional_SelectsTargetsOnceAcrossPositions(string preference, bool multiplePositions)
    {
        BossBattleFixture fixture = new(mapMode: true, usePolicy: false, mapSize: 48);
        CountingSkill skill = new() { Id = 9901, Character = fixture.Boss, GamingQueue = fixture.Queue, Level = 1,
            CastAnywhere = true, CanSelectEnemy = true, CanSelectSelf = false };
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, PositionPreference = preference }, null);
        Assert.NotEmpty(plans);
        Assert.Equal(1, skill.TargetSelections);
        Assert.Equal(multiplePositions, context.Metrics.PositionsEvaluated > 1);
        Assert.All(plans, plan => Assert.Contains(plan.Target!, context.Enemies));
    }

    [Theory]
    [InlineData(SkillRangeType.Diamond, false)]
    [InlineData(SkillRangeType.Circle, false)]
    [InlineData(SkillRangeType.Square, false)]
    [InlineData(SkillRangeType.Diamond, true)]
    [InlineData(SkillRangeType.Circle, true)]
    [InlineData(SkillRangeType.Square, true)]
    public void AnchoredArea_MatchesExhaustiveCoverageIncludingEmptyMidpoint(SkillRangeType shape, bool global)
    {
        BossBattleFixture fixture = new(mapMode: true);
        GameMap map = fixture.Queue.Map!;
        Grid origin = map[2, 4]!;
        map.SetCharacterCurrentGrid(fixture.Boss, origin);
        map.SetCharacterCurrentGrid(fixture.Warrior, map[4, 4]!);
        map.SetCharacterCurrentGrid(fixture.Mage, map[6, 4]!);
        map.SetCharacterCurrentGrid(fixture.Healer, map[10, 10]!);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9002);
        skill.SkillRangeType = shape;
        skill.CastAnywhere = global;
        skill.CastRange = 3;
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "never", PositionPreference = "coverage" }, null);
        int expected = map.GetGridsByRange(origin, skill.CastRange, true).Max(center =>
            skill.SelectTargetsByRangeFrom(fixture.Boss, origin, [.. context.Enemies], [],
                skill.SelectNonDirectionalTargets(fixture.Boss, center, true)).Count);
        Assert.Equal(2, expected);
        Assert.Equal(expected, plans.Max(p => p.TargetCount));
        Assert.Contains(plans, p => p.Center == map[5, 4] && p.TargetCount == 2);
        Assert.All(plans, p => Assert.InRange(GameMap.CalculateManhattanDistance(origin, p.Center!), 0, skill.CastRange));
        Assert.True(context.Metrics.CentersGenerated < map.GetGridsByRange(origin, skill.CastRange, true).Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FiniteRange_AllCandidatesReachableKeepsCurrentPosition(bool area)
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9002);
        skill.IsNonDirectional = area;
        skill.CastRange = 8;
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context, Actions.CastSkill(skill.Id), null);
        Assert.NotEmpty(plans);
        Assert.Equal(1, context.Metrics.PositionsEvaluated);
        Assert.All(plans, p => Assert.Same(context.TurnOrigin, p.Decision.TargetMoveGrid));
    }

    [Fact]
    public void FiniteDirectional_FiltersSharedPoolWithoutRepeatingPredicates()
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9003);
        skill.CastRange = 2;
        int checks = 0;
        skill.SelectTargetPredicates.Add(target => { checks++; return target != fixture.Mage; });
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context, Actions.CastSkill(skill.Id), null);
        Assert.Equal(3, checks);
        Assert.True(context.Metrics.PositionsEvaluated > 1);
        Assert.Contains(plans, p => p.Target == fixture.Warrior);
        Assert.DoesNotContain(plans, p => p.Decision.Targets.Contains(fixture.Mage));
        Assert.All(plans, p => Assert.InRange(GameMap.CalculateManhattanDistance(p.Decision.TargetMoveGrid!,
            fixture.Queue.Map!.GetCharacterCurrentGrid(p.Target!)!), 0, skill.CastRange));
    }

    [Fact]
    public void GlobalRule_DistanceConditionRetainsMovementCandidates()
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9003);
        skill.CastAnywhere = true;
        DecisionTableConfig config = DecisionTableBuilder.Create("test:close")
            .AddRule("close", r => r.Target(Targets.Enemies("victim"))
                .When(Conditions.Compare("distance", "atMost", 1, "victim"))
                .Then(Actions.CastSkill(skill.Id, "victim"))).Build();
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(config).Evaluate(Context(fixture, skill));
        Assert.True(result.Matched);
        Assert.NotSame(fixture.Queue.Map!.GetCharacterCurrentGrid(fixture.Boss), result.Decision!.TargetMoveGrid);
        Grid target = fixture.Queue.Map.GetCharacterCurrentGrid(result.Decision.Targets[0])!;
        Assert.Equal(1, GameMap.CalculateManhattanDistance(result.Decision.TargetMoveGrid!, target));
    }

    [Fact]
    public void AllyArea_UsesFriendlyAnchorsAndEmptyMidpoint()
    {
        BattleFixture fixture = new(mapMode: true);
        GameMap map = fixture.Queue.Map!;
        map.SetCharacterCurrentGrid(fixture.B, map[2, 0]!);
        map.SetCharacterCurrentGrid(fixture.C, map[4, 0]!);
        map.SetCharacterCurrentGrid(fixture.D, map[5, 0]!);
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.Id = 9902;
        skill.GamingQueue = fixture.Queue;
        skill.IsNonDirectional = true;
        skill.CanSelectTargetRange = 1;
        skill.CanSelectEnemy = false;
        skill.CanSelectTeammate = true;
        DecisionEvaluationContext context = new() { Queue = fixture.Queue, Actor = fixture.A, Points = fixture.Points(fixture.A),
            Skills = [skill], Allies = [fixture.B, fixture.C], SelectableAllies = [fixture.B, fixture.C], Enemies = [fixture.D] };
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "never" }, Targets.Allies());
        Assert.Contains(plans, p => p.Center == map[3, 0] && p.TargetCount == 2);
        Assert.DoesNotContain(plans, p => p.Decision.Targets.Contains(fixture.D));
    }

    [Fact]
    public void SelfArea_IncludesCasterAtHypotheticalOrigin()
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9001);
        skill.IsNonDirectional = true;
        skill.CanSelectTargetRange = 1;
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "required" }, Targets.Self());
        Assert.NotEmpty(plans);
        Assert.All(plans, p =>
        {
            Assert.Contains(fixture.Boss, p.Decision.Targets);
            Assert.Contains(p.Decision.TargetMoveGrid!, p.Decision.TargetGrids);
            Assert.NotSame(context.TurnOrigin, p.Decision.TargetMoveGrid);
        });
    }

    [Fact]
    public void CustomFootprint_RetainsCompleteCenterSearch()
    {
        BossBattleFixture fixture = new(mapMode: true);
        ShiftedSkill skill = new() { Id = 9903, Character = fixture.Boss, GamingQueue = fixture.Queue, Level = 1,
            CastAnywhere = true, IsNonDirectional = true, CanSelectTargetRange = 1 };
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "never" }, null);
        Assert.Contains(plans, p => p.Center == fixture.Queue.Map![0, 2] && p.Decision.Targets.Contains(fixture.Warrior));
        Assert.Equal(fixture.Queue.Map!.Grids.Count, context.Metrics.CentersGenerated);
    }

    [Theory]
    [InlineData(SkillRangeType.Line)]
    [InlineData(SkillRangeType.LinePass)]
    [InlineData(SkillRangeType.Sector)]
    public void DirectionalFootprint_RetainsOriginDependentProjection(SkillRangeType shape)
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9002);
        skill.SkillRangeType = shape;
        skill.CanSelectTargetRange = 3;
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "never" }, null);
        Assert.NotEmpty(plans);
        Assert.All(plans, p => Assert.Equal(skill.SelectNonDirectionalTargets(fixture.Boss, p.Center!, true), p.Decision.TargetGrids));
        Assert.Equal(fixture.Queue.Map!.GetGridsByRange(context.TurnOrigin!, skill.CastRange, true).Count, context.Metrics.CentersGenerated);
    }

    [Fact]
    public void EmptyGroundSkill_RetainsCentersWithoutCharacters()
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9002);
        skill.CastAnywhere = true;
        skill.AllowSelectNoCharacterGrid = true;
        DecisionEvaluationContext context = Context(fixture, skill);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "never" }, null);
        Assert.Contains(plans, p => p.TargetCount == 0 && p.Center == fixture.Queue.Map![11, 11]);
        Assert.Equal(fixture.Queue.Map!.Grids.Count, context.Metrics.CentersGenerated);
    }

    [Fact]
    public void InstantGlobalMagic_KeepsCurrentTileWithoutSafetyEvaluation()
    {
        BossBattleFixture fixture = new(mapMode: true, usePolicy: false, mapSize: 48);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9004);
        skill.IsNonDirectional = false;
        skill.CastAnywhere = true;
        skill.CastTime = 0;
        AIController ai = new(fixture.Queue, fixture.Queue.Map) { SelectionMode = AISelectionMode.Deterministic };
        List<Character> enemies = fixture.Queue.GetEnemies(fixture.Boss);
        Grid origin = fixture.Queue.Map!.GetCharacterCurrentGrid(fixture.Boss)!;
        AIDecision decision = ai.DecideAIAction(fixture.Boss, fixture.Queue.CharacterDecisionPoints[fixture.Boss], origin,
            [.. fixture.Queue.Map.Grids.Values], [skill], [], enemies, [], enemies, [], 0, 1, 0);
        Assert.Same(origin, decision.TargetMoveGrid);
        Assert.Equal(1, ai.LastMetrics!.PositionsEvaluated);
    }

    [Fact]
    public void GlobalThreat_DoesNotExpandMovementPaths()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        Character enemy = QueueTestHelper.NewCharacter("enemy");
        TeamGamingQueue queue = new([actor, enemy], _ => { }, new QueryMap());
        queue.AddCharacter(actor, 0, false);
        queue.AddCharacter(enemy, 0, false);
        QueryMap map = (QueryMap)queue.Map!;
        map.SetCharacterCurrentGrid(actor, map[4, 4]!);
        map.SetCharacterCurrentGrid(enemy, map[0, 0]!);
        enemy.Skills.Add(new TestSkill(SkillType.Magic) { GamingQueue = queue, CastAnywhere = true });
        DecisionEvaluationContext context = new() { Queue = queue, Actor = actor, Points = queue.CharacterDecisionPoints[actor], Enemies = [enemy] };
        Assert.Equal(1d, DecisionActionPlanner.EstimateRisk(context, map[4, 4]!));
        Assert.Equal(1d, DecisionActionPlanner.EstimateRisk(context, map[7, 7]!));
        Assert.Equal(0, map.PathQueries);
        Assert.Equal(1, map.RangeQueries);
    }

    [Fact]
    public void CustomCastRange_StillControlsDirectionalTargets()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        Character enemy = QueueTestHelper.NewCharacter("enemy");
        TeamGamingQueue queue = new([actor, enemy], _ => { }, new QueryMap(restricted: true));
        queue.AddCharacter(actor, 0, false);
        queue.AddCharacter(enemy, 0, false);
        GameMap map = queue.Map!;
        queue.CharacterDecisionPoints[actor].CurrentDecisionPoints = 5;
        map.SetCharacterCurrentGrid(actor, map[4, 4]!);
        map.SetCharacterCurrentGrid(enemy, map[5, 4]!);
        Skill skill = new TestSkill(SkillType.Skill) { Id = 9904, GamingQueue = queue, CastAnywhere = true, Level = 1 };
        DecisionEvaluationContext context = new() { Queue = queue, Actor = actor, Points = queue.CharacterDecisionPoints[actor],
            Skills = [skill], Enemies = [enemy], SelectableEnemies = [enemy] };
        Assert.True(DecisionActionPlanner.CanUse(context, "castSkill", skill));
        Assert.Empty(DecisionActionPlanner.Plan(context, new() { Type = "castSkill", SkillId = skill.Id, Movement = "never" }, null));
        map.SetCharacterCurrentGrid(enemy, map[3, 4]!);
        DecisionEvaluationContext refreshed = new() { Queue = queue, Actor = actor, Points = context.Points, Skills = [skill],
            Enemies = [enemy], SelectableEnemies = [enemy] };
        Assert.NotEmpty(DecisionActionPlanner.Plan(refreshed, new() { Type = "castSkill", SkillId = skill.Id, Movement = "never" }, null));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GlobalDirectional_RequiredMovementHonorsChainRestriction(bool chain)
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9003);
        skill.CastAnywhere = true;
        DecisionEvaluationContext context = Context(fixture, skill, chain: chain);
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context,
            new() { Type = "castSkill", SkillId = skill.Id, Movement = "required" }, null);
        if (chain) Assert.Empty(plans);
        else
        {
            Assert.NotEmpty(plans);
            Assert.All(plans, p => Assert.NotSame(context.TurnOrigin, p.Decision.TargetMoveGrid));
        }
    }

    private static DecisionEvaluationContext Context(BossBattleFixture fixture, Skill skill, int centerBudget = 1000, bool chain = false)
    {
        DecisionEvaluationContext source = fixture.Context();
        return new() { Queue = source.Queue, Actor = source.Actor, Points = source.Points, TurnOrigin = source.TurnOrigin,
            MoveGrids = source.MoveGrids, Enemies = source.Enemies, Allies = source.Allies,
            SelectableEnemies = source.SelectableEnemies, SelectableAllies = source.SelectableAllies,
            Skills = [skill], CenterBudget = centerBudget, IsChainAction = chain };
    }

    private sealed class CountingSkill() : Skill(SkillType.Skill)
    {
        internal int TargetSelections { get; private set; }
        public override List<Character> GetSelectableTargets(Character caster, List<Character> allEnemys, List<Character> allTeammates,
            List<Character> enemys, List<Character> teammates)
        {
            TargetSelections++;
            return base.GetSelectableTargets(caster, allEnemys, allTeammates, enemys, teammates);
        }
    }

    private sealed class ShiftedSkill() : Skill(SkillType.Skill)
    {
        public override List<Grid> SelectNonDirectionalTargetsFrom(Character caster, Grid origin, Grid targetGrid, bool includeCharacter = false) =>
            GamingQueue?.Map?[targetGrid.X + 2, targetGrid.Y] is Grid affected ? [affected] : [];
    }

    private sealed class QueryMap(bool restricted = false) : GameMap
    {
        public override string Name => "query";
        public override string Description => "";
        public override string Version => "1";
        public override string Author => "Tests";
        public override int Length => 8;
        public override int Width => 8;
        public override int Height => 1;
        public override float Size => 1;
        internal int PathQueries { get; private set; }
        internal int RangeQueries { get; private set; }
        public override GameMap InitGamingQueue(IGamingQueue queue) { QueryMap map = new(restricted); map.Load(); return map; }
        public override List<Grid> GetGridsByRange(Grid grid, int range, bool includeCharacter = false)
        {
            RangeQueries++;
            return [.. base.GetGridsByRange(grid, range, includeCharacter).Where(g => !restricted || g.X <= grid.X)];
        }
        public override IReadOnlyDictionary<Grid, int> GetMovementCosts(Character character, Grid origin)
        {
            PathQueries++;
            return base.GetMovementCosts(character, origin);
        }
    }
}
