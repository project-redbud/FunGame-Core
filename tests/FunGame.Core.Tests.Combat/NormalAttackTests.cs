using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Combat;

public class NormalAttackTests
{
    [Fact]
    public void Defaults_AreLevelOneWithoutWeapon()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;

        Assert.Equal(1, attack.Level);
        Assert.Equal(10d, attack.HardnessTime, 3);
        Assert.Equal(attack.HardnessTime, attack.RealHardnessTime, 3);
        Assert.Equal(duel.Actor.ATK, attack.Damage, 3);
        Assert.False(attack.IsMagic);
        Assert.Equal(MagicType.None, attack.MagicType);
        Assert.False(attack.IsInEffect);
        Assert.True(attack.Enable);
    }

    [Fact]
    public void Level_IsClampedToConfiguredMaxAndMin()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;

        attack.Level = 99;
        Assert.Equal(duel.Actor.GameplayEquilibriumConstant.MaxNormalAttackLevel, attack.Level);

        attack.Level = 0;
        Assert.Equal(1, attack.Level);
    }

    [Fact]
    public void HardnessTime_AddsExValueAfterPercentageBonus()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;

        attack.ExHardnessTime2 = .5;
        attack.ExHardnessTime = 5;

        Assert.Equal(20d, attack.HardnessTime, 3);
    }

    [Fact]
    public void RealHardnessTime_IsReducedByActionCoefficient()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;
        duel.Actor.ExActionCoefficient = .5;

        Assert.Equal(attack.HardnessTime * (1 - duel.Actor.ActionCoefficient), attack.RealHardnessTime, 3);
    }

    [Fact]
    public void Damage_ScalesWithLevelAndExtraBonuses()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;

        attack.Level = 3;
        attack.ExDamage2 = .5;
        attack.ExDamage = 10;

        Assert.Equal(duel.Actor.ATK * 1.1 * 1.5 + 10, attack.Damage, 3);
    }

    [Fact]
    public void Attack_ReducesEnemyHp()
    {
        Duel duel = new();
        double hp = duel.Enemy.HP;

        duel.Actor.NormalAttack.Attack(duel.Queue, duel.Actor, null, duel.Enemy);

        Assert.True(duel.Enemy.HP < hp);
        Assert.True(duel.Queue.LastRound.Damages.ContainsKey(duel.Enemy));
    }

    [Fact]
    public void SetMagicType_SwitchesToMagicalDamage()
    {
        Duel duel = new();
        duel.Enemy.InitialDEF = 200;
        duel.Enemy.MDF[MagicType.Bright] = 0;
        NormalAttack attack = duel.Actor.NormalAttack;
        double hp = duel.Enemy.HP;

        attack.SetMagicType(true, MagicType.Bright);
        attack.Attack(duel.Queue, duel.Actor, null, duel.Enemy);

        Assert.True(attack.IsMagic);
        Assert.Equal(MagicType.Bright, attack.MagicType);
        Assert.True(hp - duel.Enemy.HP > 0);
    }

    [Fact]
    public void SetMagicType_TogglesDamageOverride()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;

        attack.SetMagicType(true, MagicType.Bright);
        Assert.True(attack.IsMagic);
        Assert.Equal(MagicType.Bright, attack.MagicType);

        attack.SetMagicType(false);
        Assert.False(attack.IsMagic);
    }

    [Fact]
    public void NormalAttackOfEffects_AreRegisteredAndRemoved()
    {
        Duel duel = new();
        NormalAttack attack = duel.Actor.NormalAttack;
        Effect effect = new();
        NormalAttackOfEffect overrideEffect = new(effect, true, MagicType.Shadow, 0);

        attack.SetMagicType(overrideEffect);
        Assert.Contains(effect, attack.NormalAttackOfEffects.Keys);
        Assert.True(attack.IsMagic);

        attack.UnsetMagicType(effect);
        Assert.Empty(attack.NormalAttackOfEffects);
    }
}
