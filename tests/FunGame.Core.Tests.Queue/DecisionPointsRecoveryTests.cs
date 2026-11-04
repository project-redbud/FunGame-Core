using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

public class DecisionPointsRecoveryTests
{
    [Fact]
    public void Defaults_HaveNoRecoveryBonus()
    {
        Assert.Equal(0, new EquilibriumConstant().RecoverDecisionPointsPerRound);
    }

    [Theory]
    [InlineData(CharacterState.Actionable, 7, 0, 3)]
    [InlineData(CharacterState.Actionable, 7, 2, 5)]
    [InlineData(CharacterState.NotActionable, 7, 0, 1)]
    [InlineData(CharacterState.NotActionable, 7, 2, 3)]
    [InlineData(CharacterState.ActionRestricted, 7, 2, 3)]
    [InlineData(CharacterState.Actionable, 1, 2, 3)]
    [InlineData(CharacterState.NotActionable, 1, 2, 3)]
    [InlineData(CharacterState.Actionable, 7, -5, 1)]
    public void Recovery_AddsBonusAfterStateBasedRecovery(CharacterState state, int maximum, int bonus, int expected)
    {
        BattleFixture fixture = new();
        DecisionPoints points = fixture.Points(fixture.A);
        points.GameplayEquilibriumConstant = new EquilibriumConstant { MaxDecisionPoints = maximum };
        points.MaxDecisionPoints = maximum;
        points.CurrentDecisionPoints = 0;
        points.RecoverDecisionPointsPerRound = bonus;
        fixture.A.CharacterState = state;

        Assert.Same(points, fixture.Queue.DecisionPointsRecovery(fixture.A));

        Assert.Equal(expected, points.DecisionPointsRecovery);
        Assert.Equal(Math.Min(expected, maximum), points.CurrentDecisionPoints);
    }

    [Fact]
    public void Recovery_GrowsMaximumBeforeCalculatingBaseAndClampsStock()
    {
        BattleFixture fixture = new();
        DecisionPoints points = fixture.Points(fixture.A);
        points.GameplayEquilibriumConstant = new EquilibriumConstant { MaxDecisionPoints = 7 };
        points.MaxDecisionPoints = 3;
        points.CurrentDecisionPoints = 3;
        points.RecoverDecisionPointsPerRound = 2;

        fixture.Queue.DecisionPointsRecovery(fixture.A);

        Assert.Equal(4, points.MaxDecisionPoints);
        Assert.Equal(4, points.DecisionPointsRecovery);
        Assert.Equal(4, points.CurrentDecisionPoints);
    }

    [Theory]
    [InlineData(CharacterState.Casting)]
    [InlineData(CharacterState.PreCastSuperSkill)]
    public void Recovery_SkipsBonusAndMaximumGrowthWhileCasting(CharacterState state)
    {
        BattleFixture fixture = new();
        DecisionPoints points = fixture.Points(fixture.A);
        points.MaxDecisionPoints = 3;
        points.CurrentDecisionPoints = 1;
        points.RecoverDecisionPointsPerRound = 2;
        fixture.A.CharacterState = state;

        fixture.Queue.DecisionPointsRecovery(fixture.A);

        Assert.Equal(3, points.MaxDecisionPoints);
        Assert.Equal(1, points.CurrentDecisionPoints);
    }

    [Fact]
    public void InitialBudget_UsesQueueRecoveryBonusAndKeepsCharacterOverride()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        MixGamingQueue queue = QueueTestHelper.NewMix(actor);
        queue.GameplayEquilibriumConstant = new EquilibriumConstant { RecoverDecisionPointsPerRound = 2 };
        queue.AddCharacter(actor, 0, false);
        DecisionPoints points = queue.CharacterDecisionPoints[actor];
        Assert.Equal(2, points.RecoverDecisionPointsPerRound);
        points.CurrentDecisionPoints = 0;

        queue.DecisionPointsRecovery(actor);

        Assert.Equal(3, points.DecisionPointsRecovery);
        Assert.Equal(2, points.CurrentDecisionPoints);
        points.CurrentDecisionPoints = 0;
        points.RecoverDecisionPointsPerRound = 1;
        queue.DecisionPointsRecovery(actor);
        Assert.Equal(2, points.DecisionPointsRecovery);
        Assert.Equal(2, points.CurrentDecisionPoints);
    }
}
