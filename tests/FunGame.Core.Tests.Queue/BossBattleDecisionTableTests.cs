using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class BossBattleDecisionTableTests
{
    [Theory]
    [InlineData("emergency-recover", false)]
    [InlineData("pressure-cluster", true)]
    [InlineData("focus-wounded", false)]
    [InlineData("safe-channel", true)]
    [InlineData("basic-pressure", false)]
    public void BossPolicy_SelectsEachRequiredRule(string rule, bool mapMode)
    {
        BossBattleFixture fixture = new(mapMode: mapMode);
        foreach (DecisionRuleConfig entry in fixture.Config.Rules) entry.Enabled = entry.Id == rule;
        if (rule == "emergency-recover") fixture.Boss.HP = fixture.Boss.MaxHP * .2;
        if (rule == "focus-wounded") fixture.Mage.HP = fixture.Mage.MaxHP * .15;
        if (rule == "safe-channel")
        {
            fixture.Warrior.ExMOV = fixture.Mage.ExMOV = fixture.Healer.ExMOV = -100;
            foreach (Character actor in new[] { fixture.Warrior, fixture.Mage, fixture.Healer }) actor.Skills.Clear();
            fixture.Queue.Map!.SetCharacterCurrentGrid(fixture.Boss, fixture.Queue.Map[6, 2]!);
            fixture.Boss.Skills.Single(s => s.Id == 9004).CastRange = 6;
        }
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(fixture.Config).Evaluate(fixture.Context());
        Assert.Equal(rule, result.RuleId);
        Assert.True(result.Decision!.HasCandidate);
        if (rule == "emergency-recover") Assert.Same(fixture.Boss, Assert.Single(result.Decision.Targets));
        if (rule == "pressure-cluster") Assert.True(result.Decision.Targets.Count >= 2);
        if (rule == "focus-wounded") Assert.Same(fixture.Mage, Assert.Single(result.Decision.Targets));
        if (rule == "safe-channel") Assert.Equal(0, DecisionActionPlanner.EstimateRisk(fixture.Context(), result.Decision.TargetMoveGrid!));
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(fixture.Config));
        Assert.Empty(loader.Bind(fixture.Boss));
        Character turnActor = fixture.Queue.NextCharacter()!;
        Assert.Same(fixture.Boss, turnActor);
        fixture.Queue.ProcessTurn(turnActor);
        Assert.True(fixture.Hits.GetValueOrDefault(rule) > 0);
        Assert.Contains(fixture.Queue.LastRound.Actions, a => a.Actor == fixture.Boss && a.IsSuccess &&
            (rule == "basic-pressure" ? a.ActionType == FunGame.Core.Library.Constant.CharacterActionType.NormalAttack : a.Skill?.Id == result.Decision.SkillToUse?.Id));
    }

    [Fact]
    public void BossPolicy_SkipsUnavailableRecoveryAndUnsafeChannel()
    {
        BossBattleFixture fixture = new(mapMode: true);
        fixture.Boss.HP = fixture.Boss.MaxHP * .2;
        fixture.Boss.Skills.Single(s => s.Id == 9001).CurrentCD = 10;
        DecisionEvaluationResult result = new DecisionTableCompiler().Compile(fixture.Config).Evaluate(fixture.Context());
        Assert.NotEqual("emergency-recover", result.RuleId);
        foreach (DecisionRuleConfig rule in fixture.Config.Rules) rule.Enabled = rule.Id == "safe-channel";
        fixture.Boss.HP = fixture.Boss.MaxHP;
        fixture.Warrior.ExATR = fixture.Mage.ExATR = fixture.Healer.ExATR = 50;
        Assert.False(new DecisionTableCompiler().Compile(fixture.Config).Evaluate(fixture.Context()).Matched);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FullBattle_EndsAndRepeatsWithIdenticalSeed(bool mapMode)
    {
        BossBattleFixture first = new(mapMode: mapMode);
        BossBattleFixture second = new(mapMode: mapMode);
        Assert.Equal(first.Run(), second.Run());
        Assert.NotEmpty(first.Hits);
    }

    [Fact]
    public void FullBattle_FixedSeedSetTerminatesWithPolicyAndEmptyControl()
    {
        for (int seed = 1; seed <= 20; seed++)
        {
            BossBattleFixture policy = new(seed);
            BossBattleFixture control = new(seed, usePolicy: false);
            Assert.NotEmpty(policy.Run());
            Assert.NotEmpty(control.Run());
        }
    }

    [Fact]
    public void PositionBudget_PreservesClusterCoverageAgainstFullReference()
    {
        BossBattleFixture fixture = new(mapMode: true);
        foreach (DecisionRuleConfig rule in fixture.Config.Rules) rule.Enabled = rule.Id == "pressure-cluster";
        DecisionEvaluationContext full = fixture.Context();
        DecisionEvaluationContext bounded = new() { Queue = full.Queue, Actor = full.Actor, Points = full.Points, Skills = full.Skills,
            Enemies = full.Enemies, Allies = full.Allies, SelectableEnemies = full.SelectableEnemies, SelectableAllies = full.SelectableAllies,
            TurnOrigin = full.TurnOrigin, MoveGrids = full.MoveGrids, PositionBudget = 8 };
        FunGame.Core.Interface.IDecisionPolicy policy = new DecisionTableCompiler().Compile(fixture.Config);
        DecisionEvaluationResult expected = policy.Evaluate(full);
        DecisionEvaluationResult actual = policy.Evaluate(bounded);
        Assert.True(expected.Matched);
        Assert.True(actual.Matched);
        Assert.Equal(expected.Decision!.Targets.Count, actual.Decision!.Targets.Count);
        Assert.True(bounded.Metrics.PositionsEvaluated < full.Metrics.PositionsEvaluated);
    }
}
