using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class AIPlanningTests
{
    private static AIDecision Decide(AIController ai, BattleFixture fixture, List<Skill> skills, double skillWeight = .5, double attackWeight = .5) =>
        ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), fixture.Queue.Map?.GetCharacterCurrentGrid(fixture.A),
            fixture.Queue.Map?.Grids.Values.ToList() ?? [], skills, [], [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], 0, skillWeight, attackWeight);

    [Fact]
    public void WeightedSelection_PreservesCategoryDistributionDespiteSkillCount()
    {
        BattleFixture fixture = new();
        fixture.Points(fixture.A).CurrentDecisionPoints = 10;
        List<Skill> skills = [];
        for (int id = 1; id <= 10; id++) { Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A); skill.Id = id; skills.Add(skill); }
        AIController ai = new(fixture.Queue, null) { SelectionMode = AISelectionMode.WeightedNearBest };
        ai.EvaluatePlanEvent += (_, _) => 100;
        int attacks = 0;
        for (int i = 0; i < 1000; i++) if (Decide(ai, fixture, skills).ActionType == CharacterActionType.NormalAttack) attacks++;
        Assert.InRange(attacks, 510, 690);
    }

    [Fact]
    public void WeightedSelection_RepeatsWithSameSeedAndHandlesNegativeValues()
    {
        BattleFixture fixture = new();
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        AIController first = new(fixture.Queue, null) { SelectionMode = AISelectionMode.WeightedNearBest };
        AIController second = new(fixture.Queue, null) { SelectionMode = AISelectionMode.WeightedNearBest };
        first.EvaluatePlanEvent += (_, _) => -100;
        second.EvaluatePlanEvent += (_, _) => -100;
        for (int i = 0; i < 50; i++) Assert.Equal(Decide(first, fixture, [skill]).ActionType, Decide(second, fixture, [skill]).ActionType);
        Assert.Equal(CharacterActionType.NormalAttack, Decide(first, fixture, [skill], double.NaN, 1).ActionType);
    }

    [Fact]
    public void DeterministicSelection_PreservesCurrentTileAndRejectsInsignificantMovement()
    {
        BattleFixture fixture = new(mapMode: true);
        fixture.A.ExATR = 5;
        AIController ai = new(fixture.Queue, fixture.Queue.Map) { SelectionMode = AISelectionMode.Deterministic };
        AIDecision decision = Decide(ai, fixture, []);
        Assert.Equal(fixture.Queue.Map!.GetCharacterCurrentGrid(fixture.A), decision.TargetMoveGrid);
        Assert.Equal(CharacterActionType.NormalAttack, decision.ActionType);
    }

    [Theory]
    [InlineData(SkillRangeType.Line)]
    [InlineData(SkillRangeType.Sector)]
    public void HypotheticalOrigin_MatchesRangeAfterRealMovement(SkillRangeType type)
    {
        BossBattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.Boss.Skills.Single(s => s.Id == 9004);
        skill.SkillRangeType = type;
        skill.CanSelectTargetRange = 3;
        GameMap map = fixture.Queue.Map!;
        Grid original = map.GetCharacterCurrentGrid(fixture.Boss)!;
        Grid hypothetical = map[4, 5]!;
        Grid center = map[2, 4]!;
        List<Grid> planned = skill.SelectNonDirectionalTargetsFrom(fixture.Boss, hypothetical, center, true);
        Assert.Same(original, map.GetCharacterCurrentGrid(fixture.Boss));
        map.SetCharacterCurrentGrid(fixture.Boss, hypothetical);
        Assert.Equal(planned, skill.SelectNonDirectionalTargets(fixture.Boss, center, true));
    }

    [Fact]
    public void PathQuery_IsReadOnlyAndRejectsDisconnectedTiles()
    {
        BattleFixture fixture = new(mapMode: true);
        GameMap map = fixture.Queue.Map!;
        Grid origin = map.GetCharacterCurrentGrid(fixture.A)!;
        Grid destination = map[5, 0]!;
        Assert.Equal(5, map.GetMovementSteps(fixture.A, origin, destination));
        Grid gap = map[3, 0]!;
        map.Grids.Remove(gap.Id);
        map.GridsByCoordinate.Remove((3, 0, 0));
        Assert.Equal(-1, map.GetMovementSteps(fixture.A, origin, destination));
        Assert.Same(origin, map.GetCharacterCurrentGrid(fixture.A));
    }

    [Fact]
    public void FileGeneration_LoadsEquivalentPolicyAndRejectsOverwrite()
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "boss.json");
        try
        {
            DecisionTableGenerator.Save(BossBattleFixture.CreateBossConfig(), path);
            Assert.Throws<IOException>(() => DecisionTableGenerator.Save(BossBattleFixture.CreateBossConfig(), path));
            DecisionTableLoader loader = new();
            Assert.Empty(loader.Load(path));
            Assert.NotNull(loader.Get("example:boss"));
        }
        finally { File.Delete(path); Directory.Delete(directory); }
    }

    [Fact]
    public void FixtureFiles_MatchBuilderAndLoadCompleteBossTable()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "boss.json");
        DecisionTableConfig parsed = DecisionTableParser.Parse(File.ReadAllText(path));
        Assert.Equal(DecisionTableGenerator.GetJson(BossBattleFixture.CreateBossConfig()), DecisionTableGenerator.GetJson(parsed));
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Load(path));
        Assert.Empty(loader.Load(Path.Combine(AppContext.BaseDirectory, "Fixtures", "empty.json")));
    }

    [Fact]
    public void MovementExecution_UsesCustomReadOnlyCosts()
    {
        BlockedMovementMap map = new();
        map.Load();
        Character actor = QueueTestHelper.NewCharacter("actor");
        Grid origin = map[0, 0]!;
        Grid target = map[1, 0]!;
        map.SetCharacterCurrentGrid(actor, origin);
        Assert.Equal(-1, map.GetMovementSteps(actor, origin, target));
        Assert.Equal(-1, map.CharacterMove(actor, origin, target));
        Assert.Same(origin, map.GetCharacterCurrentGrid(actor));
    }

    [Fact]
    public void GlobalRange_PreservesPlaneDistanceOrderAndOccupancyFiltering()
    {
        BossMap map = new(48);
        map.Load();
        Character actor = QueueTestHelper.NewCharacter("actor");
        Grid origin = map[0, 0]!;
        map.SetCharacterCurrentGrid(actor, origin);
        Assert.Equal(2304, map.GetGridsByRange(origin, map.Grids.Count, true).Count);
        Assert.Equal(2303, map.GetGridsByRange(origin, map.Grids.Count, false).Count);
        List<Grid> diamond = map.GetGridsByRange(origin, 48, true);
        Assert.All(diamond, grid => Assert.InRange(GameMap.CalculateManhattanDistance(origin, grid), 0, 48));
        Assert.DoesNotContain(map[47, 47]!, diamond);
        Assert.Equal(diamond.OrderBy(g => g.X).ThenBy(g => g.Y), diamond);
    }

    private sealed class BlockedMovementMap : GameMap
    {
        public override string Name => "blocked";
        public override string Description => "";
        public override string Version => "1";
        public override string Author => "Tests";
        public override int Length => 2;
        public override int Width => 1;
        public override int Height => 1;
        public override float Size => 1;
        public override GameMap InitGamingQueue(FunGame.Core.Interface.Base.IGamingQueue queue) { BlockedMovementMap map = new(); map.Load(); return map; }
        public override IReadOnlyDictionary<Grid, int> GetMovementCosts(Character character, Grid origin) => new Dictionary<Grid, int> { [origin] = 0 };
    }
}
