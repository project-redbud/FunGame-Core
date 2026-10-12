using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Library.Module.Example;

namespace FunGame.Core.Tests.Combat;

public class DamagePipelineTests
{
    [Fact]
    public void Shield_PartiallyAbsorbsDamage()
    {
        Duel duel = new();
        duel.Enemy.Shield.Physical = 40;

        double taken = duel.Deal(100);

        Assert.Equal(60d, taken, 3);
        Assert.Equal(0d, duel.Enemy.Shield.Physical, 3);
    }

    [Fact]
    public void Shield_FullyAbsorbsDamageBelowItsValue()
    {
        Duel duel = new();
        duel.Enemy.Shield.Physical = 500;
        double hp = duel.Enemy.HP;

        double taken = duel.Deal(100);

        Assert.Equal(0d, taken, 3);
        Assert.Equal(hp, duel.Enemy.HP, 3);
        Assert.Equal(400d, duel.Enemy.Shield.Physical, 3);
    }

    [Fact]
    public void Shield_IsSeparatedByDamageAndMagicType()
    {
        Duel duel = new();
        duel.Enemy.Shield.Physical = 500;
        duel.Enemy.Shield.Bright = 500;

        double physical = duel.Deal(100);
        double magical = duel.Deal(100, damageType: DamageType.Magical, magicType: MagicType.Shadow);

        Assert.Equal(0d, physical, 3);
        Assert.Equal(100d, magical, 3);
        Assert.Equal(400d, duel.Enemy.Shield.Physical, 3);
        Assert.Equal(500d, duel.Enemy.Shield.Bright, 3);
    }

    [Fact]
    public void ShieldCanBeSkippedByOption()
    {
        Duel duel = new();
        duel.Enemy.Shield.Physical = 500;

        double taken = duel.Deal(100, calculateShield: false);

        Assert.Equal(100d, taken, 3);
        Assert.Equal(500d, duel.Enemy.Shield.Physical, 3);
    }

    [Fact]
    public void PhysicalImmunity_BlocksPhysicalDamage()
    {
        Duel duel = new();
        duel.Enemy.ImmuneType = ImmuneType.Physical;
        double hp = duel.Enemy.HP;

        double taken = duel.Deal(100);

        Assert.Equal(0d, taken, 3);
        Assert.Equal(hp, duel.Enemy.HP, 3);
    }

    [Fact]
    public void SkilledImmunity_DoesNotBlockNormalAttack()
    {
        Duel duel = new();
        duel.Enemy.ImmuneType = ImmuneType.Skilled;

        Assert.Equal(100d, duel.Deal(100, isNormalAttack: true), 3);
    }

    [Fact]
    public void SkilledImmunity_BlocksSkillDamage()
    {
        Duel duel = new();
        duel.Enemy.ImmuneType = ImmuneType.Skilled;

        Assert.Equal(0d, duel.Deal(100, isNormalAttack: false), 3);
    }

    [Fact]
    public void TrueDamage_IgnoresImmunity()
    {
        Duel duel = new();
        duel.Enemy.ImmuneType = ImmuneType.All;
        duel.Enemy.Shield.Physical = 500;

        double taken = duel.Deal(100, damageType: DamageType.True);

        Assert.Equal(100d, taken, 3);
        Assert.Equal(500d, duel.Enemy.Shield.Physical, 3);
    }

    [Fact]
    public void IgnoreImmuneOption_BypassesImmunity()
    {
        Duel duel = new();
        duel.Enemy.ImmuneType = ImmuneType.Physical;

        double taken = duel.Deal(100, ignoreImmune: true);

        Assert.Equal(100d, taken, 3);
        Assert.Equal(ImmuneType.Physical, duel.Enemy.ImmuneType);
    }

    [Fact]
    public void Lifesteal_RestoresHpProportionalToDamage()
    {
        Duel duel = new();
        duel.Actor.HP -= 300;
        duel.Actor.Lifesteal = .5;
        double hp = duel.Actor.HP;

        double taken = duel.Deal(100);

        Assert.Equal(100d, taken, 3);
        Assert.Equal(hp + 50d, duel.Actor.HP, 3);
    }

    [Fact]
    public void DealingDamage_GrantsEnergyWithinConfiguredCap()
    {
        Duel duel = new();
        duel.Actor.EP = 0;

        duel.Deal(100);

        Assert.InRange(duel.Actor.EP, 0.01, duel.Queue.GameplayEquilibriumConstant.DamageGetEPMax);
    }

    [Fact]
    public void LethalDamage_KillsTargetAndCreditsActor()
    {
        Duel duel = new();
        double maxHp = duel.Enemy.MaxHP;

        double taken = duel.Deal(maxHp + 1);

        Assert.Equal(maxHp, taken, 3);
        Assert.Equal(0d, duel.Enemy.HP, 3);
        Assert.Equal(1, duel.Queue.CharacterStatistics[duel.Actor].Kills);
        Assert.Equal(1, duel.Queue.CharacterStatistics[duel.Enemy].Deaths);
        Assert.DoesNotContain(duel.Enemy, duel.Queue.Queue);
    }

    [Fact]
    public void DamageAgainstDeadTarget_IsIgnored()
    {
        Duel duel = new();
        duel.Enemy.HP = 0;

        Assert.Equal(0d, duel.Deal(100), 3);
    }

    [Fact]
    public void HealToTarget_ClampsAtMaxHp()
    {
        Duel duel = new();
        duel.Actor.HP -= 100;

        duel.Queue.HealToTarget(duel.Actor, duel.Actor, 500);

        Assert.Equal(duel.Actor.MaxHP, duel.Actor.HP, 3);
        Assert.True(duel.Queue.CharacterStatistics[duel.Actor].TotalHeal > 0);
    }

    [Fact]
    public void ActualDamageBonusEffect_AdjustsDamageAfterCalculation()
    {
        Duel duel = new();
        Skill skill = new TestSkill(SkillType.Passive).Grant(duel.Actor, duel.Queue);
        new ActualDamageDeltaEffect(skill, -50).AddToCharacter(duel.Actor);

        double taken = duel.Deal(100, triggerEffects: true);

        Assert.Equal(50d, taken, 3);
    }

    [Fact]
    public void ExampleDamageEffect_DealsAtkScaledTrueDamage()
    {
        Duel duel = new();
        Skill skill = new TestSkill(SkillType.Skill).Grant(duel.Actor, duel.Queue);
        ExampleDamageBasedOnATKWithBasicDamage effect = new(skill, 100, 50, .2, .2, DamageType.True);
        double hp = duel.Enemy.HP;

        effect.Activate(duel.Actor, [duel.Enemy]);

        double dealt = hp - duel.Enemy.HP;
        double expected = 100d + .2 * duel.Actor.ATK;
        Assert.InRange(dealt, expected, expected * duel.Actor.CritDMG);
    }
}
