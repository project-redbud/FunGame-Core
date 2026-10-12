using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class DecisionPointsTests
{
    [Fact]
    public void Defaults_ComeFromEquilibriumConstant()
    {
        DecisionPoints points = new();
        EquilibriumConstant constants = points.GameplayEquilibriumConstant;

        Assert.Equal(constants.InitialDecisionPoints, points.CurrentDecisionPoints);
        Assert.Equal(constants.RecoverDecisionPointsPerRound, points.RecoverDecisionPointsPerRound);
        Assert.Empty(points.ActionTypes);
        Assert.Empty(points.ActionsHardnessTime);
        Assert.Equal(0, points.ActionsTaken);
        Assert.Equal(0, points.DecisionPointsCost);
        Assert.False(points.CourageCommandSkill);
    }

    [Fact]
    public void Indexer_ReturnsQuotaPerActionType()
    {
        DecisionPoints points = new();
        EquilibriumConstant constants = points.GameplayEquilibriumConstant;

        Assert.Equal(constants.ActionQuotaNormalAttack, points[CharacterActionType.NormalAttack]);
        Assert.Equal(constants.ActionQuotaSkill, points[CharacterActionType.CastSkill]);
        Assert.Equal(constants.ActionQuotaSuperSkill, points[CharacterActionType.CastSuperSkill]);
        Assert.Equal(constants.ActionQuotaMagic, points[CharacterActionType.PreCastSkill]);
        Assert.Equal(constants.ActionQuotaItem, points[CharacterActionType.UseItem]);
        Assert.Equal(constants.ActionQuotaOther, points[CharacterActionType.Move]);
        Assert.Equal(int.MaxValue, points[CharacterActionType.ChainAction]);
    }

    [Fact]
    public void AddActionType_AccumulatesTimesAndCost()
    {
        DecisionPoints points = new();
        EquilibriumConstant constants = points.GameplayEquilibriumConstant;

        points.AddActionType(CharacterActionType.NormalAttack);
        points.AddActionType(CharacterActionType.NormalAttack);
        points.AddActionType(CharacterActionType.Move, addActionTaken: false);

        Assert.Equal(2, points.ActionsTaken);
        Assert.Equal(2, points.ActionTypes[CharacterActionType.NormalAttack]);
        Assert.Equal(1, points.ActionTypes[CharacterActionType.Move]);
        Assert.Equal(constants.DecisionPointsCostNormalAttack * 2 + constants.DecisionPointsCostOther, points.DecisionPointsCost);
    }

    [Fact]
    public void CheckActionTypeQuota_StopsAtQuotaButChainActionIsUnlimited()
    {
        DecisionPoints points = new();
        int quota = points[CharacterActionType.NormalAttack];
        Assert.True(quota > 0);

        for (int i = 0; i < quota; i++)
        {
            Assert.True(points.CheckActionTypeQuota(CharacterActionType.NormalAttack));
            points.AddActionType(CharacterActionType.NormalAttack);
        }
        Assert.False(points.CheckActionTypeQuota(CharacterActionType.NormalAttack));

        for (int i = 0; i < 10; i++) points.AddActionType(CharacterActionType.ChainAction);
        Assert.True(points.CheckActionTypeQuota(CharacterActionType.ChainAction));
    }

    [Fact]
    public void GetActionPointCost_MatchesActionType()
    {
        DecisionPoints points = new();
        EquilibriumConstant constants = points.GameplayEquilibriumConstant;
        Skill skill = new TestSkill(SkillType.Skill);
        Skill super = new TestSkill(SkillType.SuperSkill);
        Skill magic = new TestSkill(SkillType.Magic);

        Assert.Equal(constants.DecisionPointsCostNormalAttack, points.GetActionPointCost(CharacterActionType.NormalAttack));
        Assert.Equal(constants.DecisionPointsCostSkill, points.GetActionPointCost(CharacterActionType.CastSkill, skill));
        Assert.Equal(constants.DecisionPointsCostSuperSkill, points.GetActionPointCost(CharacterActionType.CastSuperSkill, super));
        Assert.Equal(constants.DecisionPointsCostSuperSkillOutOfTurn, points.GetActionPointCost(CharacterActionType.CastSuperSkill));
        Assert.Equal(constants.DecisionPointsCostSkill, points.GetActionPointCost(CharacterActionType.PreCastSkill, skill));
        Assert.Equal(constants.DecisionPointsCostMagic, points.GetActionPointCost(CharacterActionType.PreCastSkill, magic));
        Assert.Equal(constants.DecisionPointsCostItem, points.GetActionPointCost(CharacterActionType.UseItem));
        Assert.Equal(constants.DecisionPointsCostChainAction, points.GetActionPointCost(CharacterActionType.ChainAction));
        Assert.Equal(constants.DecisionPointsCostOther, points.GetActionPointCost(CharacterActionType.Move));
    }

    [Fact]
    public void GetActionPointCost_ChainActionUsesNormalCost()
    {
        DecisionPoints points = new();
        EquilibriumConstant constants = points.GameplayEquilibriumConstant;

        Assert.Equal(constants.DecisionPointsCostSkill, points.GetActionPointCost(CharacterActionType.CastSkill, new TestSkill(SkillType.Skill), chainAction: true));
        Assert.Equal(constants.DecisionPointsCostSuperSkill, points.GetActionPointCost(CharacterActionType.CastSuperSkill, chainAction: true));
    }

    [Fact]
    public void ConsumeChainAction_KeepsTurnLocalCountersUntouched()
    {
        DecisionPoints points = new() { CurrentDecisionPoints = 5, ActionsTaken = 2, DecisionPointsCost = 4 };
        points.ActionsHardnessTime.Add(99);

        points.ConsumeChainAction(CharacterActionType.NormalAttack);

        Assert.Equal(4, points.CurrentDecisionPoints);
        Assert.Equal(1, points.ActionTypes[CharacterActionType.NormalAttack]);
        Assert.Equal(2, points.ActionsTaken);
        Assert.Equal(4, points.DecisionPointsCost);
        Assert.Equal(new[] { 99d }, points.ActionsHardnessTime);
    }

    [Fact]
    public void TempActionQuota_AddsToQuotaThenClears()
    {
        DecisionPoints points = new();
        Effect effect = new();
        int before = points[CharacterActionType.NormalAttack];

        points.AddTempActionQuota(effect, CharacterActionType.NormalAttack, 2);
        points.AddTempActionQuota(effect, CharacterActionType.NormalAttack);
        points.AddTempActionQuota(effect);

        Assert.Equal(3, points.TempActionQuotaNormalAttack);
        Assert.Equal(1, points.TempActionQuotaAllRound);
        Assert.Equal(before + 4, points[CharacterActionType.NormalAttack]);

        points.ClearTempActionQuota();

        Assert.Equal(0, points.TempActionQuotaNormalAttack);
        Assert.Equal(0, points.TempActionQuotaAllRound);
        Assert.Equal(before, points[CharacterActionType.NormalAttack]);
    }

    [Fact]
    public void GetDecisionPointsInfo_ReportsBudgetAndQuota()
    {
        DecisionPoints points = new() { CurrentDecisionPoints = 3, MaxDecisionPoints = 7 };

        string info = points.GetDecisionPointsInfo();

        Assert.Contains("3 / 7", info);
        Assert.Contains("决策点信息", info);
    }
}
