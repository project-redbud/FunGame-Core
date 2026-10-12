using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Combat;

public class SkillCastTests
{
    [Fact]
    public void InstantSkill_PaysEPAndStartsCooldownInTheSameTurn()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, castTime: 3, epCost: 30, cd: 20);
        double ep = battle.Caster.EP;

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.Equal(ep - 30, battle.Caster.EP, 3);
        Assert.Equal(skill.RealCD, skill.CurrentCD, 3);
        Assert.True(skill.CurrentCD > 0);
        Assert.Equal(CharacterState.Actionable, battle.Caster.CharacterState);
        ActionRecord action = Assert.Single(battle.Queue.LastRound.Actions);
        Assert.Equal(CharacterActionType.CastSkill, action.ActionType);
        Assert.True(action.IsSuccess);
    }

    [Fact]
    public void InstantSkill_IsNotCastWhenEPIsInsufficient()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 30, cd: 20);
        battle.Caster.EP = 0;

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.Equal(0d, battle.Caster.EP, 3);
        Assert.Equal(0d, skill.CurrentCD, 3);
        Assert.Equal(0, battle.Points(battle.Caster).ActionTypes.GetValueOrDefault(CharacterActionType.CastSkill));
    }

    [Fact]
    public void SkillWithZeroCooldown_CanBeCastOnConsecutiveTurns()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 10, cd: 0);
        double ep = battle.Caster.EP;

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.True(skill.Enable);
        Assert.Equal(0d, skill.CurrentCD, 3);
        Assert.Contains(skill, battle.ObserveOfferedSkills());

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.Equal(ep - 20, battle.Caster.EP, 3);
    }

    [Fact]
    public void SkillWithCooldown_StaysDisabledUntilCooldownExpires()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 10, cd: 50);

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.False(skill.Enable);
        Assert.DoesNotContain(skill, battle.ObserveOfferedSkills());
    }

    [Fact]
    public void InstantResetSkill_IgnoresItsCooldownAndStaysCastable()
    {
        TurnBattle battle = new();
        TestSkill skill = new TestSkill(SkillType.Skill)
        {
            Name = "InstantReset", HardnessTime = 5, CastTime = 3, EPCost = 10, CD = 100, InstantReset = true
        }.Grant(battle.Caster, battle.Queue);

        Assert.Equal(0d, skill.RealCD, 3);

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.True(skill.Enable);
        Assert.Contains(skill, battle.ObserveOfferedSkills());
    }

    [Fact]
    public void CoolingDownSkill_IsFilteredOutOfTheOfferedSkillList()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 10, cd: 1000);

        Assert.Contains(skill, battle.ObserveOfferedSkills());

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);
        Assert.True(skill.CurrentCD > 0);

        Assert.DoesNotContain(skill, battle.ObserveOfferedSkills());
    }

    [Fact]
    public void UnaffordableSkill_IsFilteredOutOfTheOfferedSkillList()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 10);

        Assert.Contains(skill, battle.ObserveOfferedSkills());

        battle.Caster.EP = 0;

        Assert.DoesNotContain(skill, battle.ObserveOfferedSkills());
    }

    [Fact]
    public void SuperSkill_IsReleasedImmediatelyAndPaysEP()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.SuperSkill, castTime: 3, epCost: 100, cd: 60);
        double ep = battle.Caster.EP;
        int cost = battle.Points(battle.Caster).GameplayEquilibriumConstant.DecisionPointsCostSuperSkill;

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.Equal(ep - 100, battle.Caster.EP, 3);
        Assert.Equal(battle.DecisionPointsAtTurnStart - cost, battle.Points(battle.Caster).CurrentDecisionPoints);
        Assert.Contains(CharacterActionType.CastSuperSkill, battle.Queue.LastRound.ActionTypes);
    }

    [Fact]
    public void Magic_EntersCastingWithoutPayingMP()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Magic, castTime: 3, mpCost: 80);
        double mp = battle.Caster.MP;
        int cost = battle.Points(battle.Caster).GameplayEquilibriumConstant.DecisionPointsCostMagic;

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        Assert.Equal(mp, battle.Caster.MP, 3);
        Assert.Equal(3d, battle.Queue.HardnessTime[battle.Caster], 3);
        Assert.Equal(CharacterState.Casting, battle.Caster.CharacterState);
        Assert.Equal(battle.DecisionPointsAtTurnStart - cost, battle.Points(battle.Caster).CurrentDecisionPoints);
    }

    [Fact]
    public void Magic_PaysMPWhenReleasedOnTheNextTurn()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Magic, castTime: 3, mpCost: 80);
        double mp = battle.Caster.MP;

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);
        battle.MakeNext(battle.Caster);
        battle.ResolveTurn(battle.Caster);

        Assert.Equal(mp - 80, battle.Caster.MP, 3);
        Assert.Equal(CharacterState.Actionable, battle.Caster.CharacterState);
        Assert.True(battle.Queue.LastRound.Actions.Single().IsSuccess);
    }

    [Fact]
    public void Rounds_AreArchivedAndStampedWithSeed()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 5);

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);
        battle.MakeNext(battle.Caster);
        battle.ResolveTurn(battle.Caster);

        Assert.NotEmpty(battle.Queue.Rounds);
        Assert.All(battle.Queue.Rounds, round => Assert.Equal(battle.Queue.Seed, round.Seed));
        Assert.Equal(1, battle.Queue.Rounds[0].Round);
        Assert.Same(battle.Caster, battle.Queue.Rounds[0].Actor);
    }

    [Fact]
    public void SkillStats_TrackActionTurnAndDecisionPoints()
    {
        TurnBattle battle = new();
        TestSkill skill = battle.NewSkill(SkillType.Skill, epCost: 5);

        battle.TakeTurn(skill, CharacterActionType.PreCastSkill);

        CharacterStatistics statistics = battle.Queue.CharacterStatistics[battle.Caster];
        Assert.Equal(1, statistics.ActionTurn);
        Assert.True(statistics.UseDecisionPoints > 0);
        Assert.True(statistics.TurnDecisions > 0);
    }
}
