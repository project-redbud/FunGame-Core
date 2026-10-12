using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Combat;

public class DamageCalculationTests
{
    [Fact]
    public void Physical_DefenseReducesDamageAfterPenetration()
    {
        Duel duel = new();
        duel.Enemy.InitialDEF = 100;

        (DamageResult result, double damage) = duel.Physical(1000);

        Assert.Equal(DamageResult.Normal, result);
        Assert.Equal(1000d * (1 - Reduction(duel, duel.Enemy.DEF)), damage, 3);
        Assert.True(damage < 1000);
    }

    [Fact]
    public void Physical_FullPenetrationRemovesDefenseReduction()
    {
        Duel duel = new();
        duel.Enemy.InitialDEF = 100;
        duel.Actor.PhysicalPenetration = 1;

        (DamageResult result, double damage) = duel.Physical(1000);

        Assert.Equal(DamageResult.Normal, result);
        Assert.Equal(1000d, damage, 3);
    }

    [Fact]
    public void Physical_CriticalMultipliesPostReductionDamage()
    {
        Duel duel = new();
        duel.Enemy.InitialDEF = 100;
        duel.Actor.ExCritRate = 10;

        (DamageResult result, double damage) = duel.Physical(1000, critical: true);

        Assert.Equal(DamageResult.Critical, result);
        double baseDamage = 1000d * (1 - Reduction(duel, duel.Enemy.DEF));
        Assert.Equal(baseDamage * duel.Actor.CritDMG, damage, 3);
    }

    [Fact]
    public void Physical_CriticalIsSkippedWhenCritRateIsZero()
    {
        Duel duel = new();
        duel.Actor.ExCritRate = -1;
        Assert.Equal(0d, duel.Actor.CritRate, 3);

        (DamageResult result, double _) = duel.Physical(100, critical: true);

        Assert.Equal(DamageResult.Normal, result);
    }

    [Fact]
    public void NormalAttack_CanBeEvaded()
    {
        Duel duel = new();
        duel.Enemy.ExEvadeRate = 10;

        (DamageResult result, double damage) = duel.Physical(100, isNormalAttack: true, evade: true);

        Assert.Equal(DamageResult.Evaded, result);
        Assert.Equal(0d, damage, 3);
    }

    [Fact]
    public void EvadeCheck_OnlyAppliesToNormalAttacks()
    {
        Duel duel = new();
        duel.Enemy.ExEvadeRate = 10;

        (DamageResult result, double damage) = duel.Physical(100, isNormalAttack: false, evade: true);

        Assert.Equal(DamageResult.Normal, result);
        Assert.True(damage > 0);
    }

    [Fact]
    public void EvadeCheck_CanBeIgnoredByEffect()
    {
        Duel duel = new();
        duel.Enemy.ExEvadeRate = 10;
        Skill skill = new TestSkill(SkillType.Passive).Grant(duel.Actor, duel.Queue);
        new EvadeIgnoreEffect(skill).AddToCharacter(duel.Actor);

        (DamageResult result, double damage) = duel.Physical(100, isNormalAttack: true, evade: true, triggerEffects: true);

        Assert.Equal(DamageResult.Normal, result);
        Assert.True(damage > 0);
    }

    [Fact]
    public void Magical_UsesResistanceOfTheGivenMagicType()
    {
        Duel duel = new();
        duel.Enemy.MDF[MagicType.Bright] = .5;

        (DamageResult result, double damage) = duel.Magical(1000, MagicType.Bright);

        Assert.Equal(DamageResult.Normal, result);
        Assert.Equal(500d, damage, 3);
    }

    [Fact]
    public void Magical_OtherMagicTypesAreUnaffected()
    {
        Duel duel = new();
        duel.Enemy.MDF[MagicType.Bright] = .5;

        (DamageResult _, double damage) = duel.Magical(1000, MagicType.Shadow);

        Assert.Equal(1000d, damage, 3);
    }

    [Fact]
    public void Magical_PenetrationReducesResistance()
    {
        Duel duel = new();
        duel.Enemy.MDF[MagicType.Bright] = .5;
        duel.Actor.MagicalPenetration = .5;

        (DamageResult _, double damage) = duel.Magical(1000, MagicType.Bright);

        Assert.Equal(750d, damage, 3);
    }

    [Fact]
    public void EffectHooks_AddBonusBeforeReduction()
    {
        Duel duel = new();
        duel.Enemy.InitialDEF = 100;
        Skill skill = new TestSkill(SkillType.Passive).Grant(duel.Actor, duel.Queue);
        new FlatDamageBonusEffect(skill, 200).AddToCharacter(duel.Actor);

        (DamageResult _, double withBonus) = duel.Physical(1000, triggerEffects: true);
        (DamageResult _, double withoutBonus) = duel.Physical(1000);

        double reduction = Reduction(duel, duel.Enemy.DEF);
        Assert.Equal(200d * (1 - reduction), withBonus - withoutBonus, 3);
    }

    [Fact]
    public void EffectHooks_CanRedirectPhysicalDamageIntoMagical()
    {
        Duel duel = new();
        duel.Enemy.InitialDEF = 100;
        duel.Enemy.MDF[MagicType.Bright] = .25;
        Skill skill = new TestSkill(SkillType.Passive).Grant(duel.Actor, duel.Queue);
        new DamageTypeOverrideEffect(skill, DamageType.Magical, MagicType.Bright).AddToCharacter(duel.Actor);

        (DamageResult _, double damage) = duel.Physical(1000, triggerEffects: true);

        Assert.Equal(750d, damage, 3);
    }

    [Fact]
    public void EffectHooks_AreIgnoredWhenTriggerEffectsIsOff()
    {
        Duel duel = new();
        Skill skill = new TestSkill(SkillType.Passive).Grant(duel.Actor, duel.Queue);
        new FlatDamageBonusEffect(skill, 200).AddToCharacter(duel.Actor);

        (DamageResult _, double damage) = duel.Physical(1000);

        Assert.Equal(1000d, damage, 3);
    }

    private static double Reduction(Duel duel, double defense) => defense / (defense + duel.DefenseReductionFactor);
}
