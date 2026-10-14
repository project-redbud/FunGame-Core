using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;
using Xunit.Abstractions;

namespace FunGame.Core.Tests.Queue;

[CollectionDefinition("AI decision baseline", DisableParallelization = true)]
public sealed class AIDecisionBaselineCollection;

[Collection("AI decision baseline")]
public class AIDecisionBaselineTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public void CastingBaseline_RecordsIsolatedInstantSkillLatency(bool mapMode, bool area, bool global)
    {
        BossBattleFixture fixture = new(mapMode: mapMode, usePolicy: false, mapSize: 48);
        Character boss = fixture.Boss;
        boss.ExMOV = 8;
        Skill skill = boss.Skills.Single(s => s.Id == 9004);
        skill.CastAnywhere = global;
        skill.IsNonDirectional = area;
        skill.CastTime = 0;
        GameMap? map = fixture.Queue.Map;
        Grid? origin = map?.GetCharacterCurrentGrid(boss);
        List<Grid> moves = map?.GetGridsByRange(origin!, boss.MOV, false) ?? [];
        List<Character> enemies = fixture.Queue.GetEnemies(boss);
        AIController ai = new(fixture.Queue, map) { SelectionMode = AISelectionMode.Deterministic };
        List<double> elapsed = [];
        for (int index = 0; index < 51; index++)
        {
            AIDecision decision = ai.DecideAIAction(boss, fixture.Queue.CharacterDecisionPoints[boss], origin, moves,
                [skill], [], enemies, [], enemies, [], 0, 1, 0);
            Assert.True(decision.HasCandidate);
            if (index > 0) elapsed.Add(ai.LastMetrics!.Elapsed.TotalMilliseconds);
        }
        elapsed.Sort();
        output.WriteLine($"isolated map={mapMode}, area={area}, global={global}, samples=50, medianMs={elapsed[24]:F3}, p95Ms={elapsed[47]:F3}, " +
            $"positions={ai.LastMetrics!.PositionsEvaluated}, plans={ai.LastMetrics.PlansEvaluated}, centers={ai.LastMetrics.CentersGenerated}, projections={ai.LastMetrics.CentersEvaluated}");
    }

    [Theory]
    [InlineData(false, 12, 0, false)]
    [InlineData(true, 12, 0, false)]
    [InlineData(true, 48, 8, false)]
    [InlineData(true, 48, 8, true)]
    public void Baseline_RecordsComparableDecisionLatencyWithoutTimingAssertions(bool mapMode, int mapSize, int extraMovement, bool castAnywhere)
    {
        BossBattleFixture fixture = new(mapMode: mapMode, usePolicy: false, mapSize: mapSize);
        Character boss = fixture.Boss;
        boss.ExMOV = extraMovement;
        if (castAnywhere)
        {
            boss.Skills.Single(s => s.Id == 9004).CastAnywhere = true;
            fixture.Mage.Skills.Single().CastAnywhere = true;
        }
        GameMap? map = fixture.Queue.Map;
        Grid? origin = map?.GetCharacterCurrentGrid(boss);
        List<Grid> moves = map?.GetGridsByRange(origin!, boss.MOV, false) ?? [];
        List<Character> enemies = fixture.Queue.GetEnemies(boss);
        foreach (AISelectionMode mode in new[] { AISelectionMode.Legacy, AISelectionMode.Deterministic })
        {
            AIController ai = new(fixture.Queue, map) { SelectionMode = mode };
            List<double> elapsed = [];
            for (int index = 0; index < 51; index++)
            {
                AIDecision decision = ai.DecideAIAction(boss, fixture.Queue.CharacterDecisionPoints[boss], origin, moves,
                    [.. boss.Skills], [], enemies, [], enemies, [], 0, .7, .3);
                Assert.True(decision.HasCandidate);
                if (index > 0) elapsed.Add(ai.LastMetrics!.Elapsed.TotalMilliseconds);
            }
            elapsed.Sort();
            output.WriteLine($"map={mapMode}, size={mapSize}, movement={boss.MOV}, anywhere={castAnywhere}, mode={mode}, samples=50, medianMs={elapsed[24]:F3}, p95Ms={elapsed[47]:F3}, " +
                $"positionPasses={ai.LastMetrics!.PositionsEvaluated}, plans={ai.LastMetrics.PlansEvaluated}");
        }
    }
}
