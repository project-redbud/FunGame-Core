using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Entities;

public class CharacterAttributeTests
{
    [Fact]
    public void MaxHp_EqualsInitialHpAtLevelOne()
    {
        Character character = EntitiesHelper.NewCharacter(hp: 100);

        Assert.Equal(1, character.Level);
        Assert.Equal(100d, character.MaxHP, 3);
        Assert.Equal(character.MaxHP, character.HP, 3);
    }

    [Fact]
    public void MaxHp_GrowsWithLevel()
    {
        Character character = EntitiesHelper.NewCharacter(hp: 100);
        EquilibriumConstant constants = character.GameplayEquilibriumConstant;

        character.Level = 2;

        double expected = 100 + (constants.LevelToHPFactor + constants.HPGrowthFactor * 100);
        Assert.Equal(expected, character.MaxHP, 3);
    }

    [Fact]
    public void Attack_IncludesExtraBonuses()
    {
        Character character = EntitiesHelper.NewCharacter(atk: 100);

        Assert.Equal(100d, character.ATK, 3);

        character.ExATK2 = 30;
        character.ExATKPercentage = .5;

        Assert.Equal(180d, character.ATK, 3);
        Assert.Equal(50d, character.BaseATK * character.ExATKPercentage, 3);
    }

    [Fact]
    public void Defense_ProducesPhysicalDamageReduction()
    {
        Character character = EntitiesHelper.NewCharacter(def: 240);
        EquilibriumConstant constants = character.GameplayEquilibriumConstant;

        Assert.Equal(.5, character.PDR, 3);
        Assert.Equal(240d / (240d + constants.DEFReductionFactor), character.PDR, 3);

        character.ExDEF2 = -240;
        Assert.Equal(0d, character.PDR, 3);
    }

    [Fact]
    public void CritEvadeAndAcceleration_UseConfiguredBaselines()
    {
        Character character = EntitiesHelper.NewCharacter(spd: 100);
        EquilibriumConstant constants = character.GameplayEquilibriumConstant;

        Assert.Equal(constants.CritRate, character.CritRate, 3);
        Assert.Equal(constants.CritDMG, character.CritDMG, 3);
        Assert.Equal(constants.EvadeRate, character.EvadeRate, 3);
        Assert.Equal(0d, character.AccelerationCoefficient, 3);
        Assert.Equal(0d, character.CDR, 3);
    }

    [Fact]
    public void SpeedDerivedCoefficients_AreClamped()
    {
        Character character = EntitiesHelper.NewCharacter(spd: 300);
        EquilibriumConstant constants = character.GameplayEquilibriumConstant;

        Assert.Equal(0d, character.ActionCoefficient, 3);

        character.ExActionCoefficient = .1;
        Assert.Equal(300d / constants.SPDUpperLimit + .1, character.ActionCoefficient, 3);

        character.InitialSPD = constants.SPDUpperLimit;
        character.ExActionCoefficient = 1;
        Assert.Equal(.9, character.ActionCoefficient, 3);
    }

    [Fact]
    public void PenetrationRate_IsClampedIntoPercentageRange()
    {
        Character character = EntitiesHelper.NewCharacter();

        character.PhysicalPenetration = 5;
        Assert.Equal(1d, character.PhysicalPenetration, 3);

        character.MagicalPenetration = -1;
        Assert.Equal(0d, character.MagicalPenetration, 3);
    }

    [Fact]
    public void HpMpAndEp_AreClampedIntoRange()
    {
        Character character = EntitiesHelper.NewCharacter(hp: 100, mp: 80);
        double maxMp = character.MaxMP;
        double maxEp = character.GameplayEquilibriumConstant.MaxEP;

        character.HP = 9999;
        character.MP = 9999;
        character.EP = 9999;
        Assert.Equal(character.MaxHP, character.HP, 3);
        Assert.Equal(maxMp, character.MP, 3);
        Assert.Equal(maxEp, character.EP, 3);

        character.HP = -50;
        character.MP = -50;
        character.EP = -50;
        Assert.Equal(0d, character.HP, 3);
        Assert.Equal(0d, character.MP, 3);
        Assert.Equal(0d, character.EP, 3);
    }

    [Fact]
    public void MagicResistance_ClampsAndAverages()
    {
        Character character = EntitiesHelper.NewCharacter();
        MagicResistance resistance = character.MDF;

        resistance[MagicType.Bright] = 2;
        Assert.Equal(1d, resistance[MagicType.Bright], 3);

        resistance[MagicType.Bright] = .9;
        Assert.Equal(.9 / 9, resistance.Avg, 4);
        Assert.Equal(resistance.Avg, resistance.Copy().Avg, 4);
    }

    [Fact]
    public void Shield_TotalsGroupPhysicalMagicalAndMix()
    {
        Shield shield = new()
        {
            Physical = 10,
            Mix = 20,
            Bright = 30
        };

        Assert.Equal(10d, shield.TotalPhysical, 3);
        Assert.Equal(20d, shield.TotalMix, 3);
        Assert.Equal(30d, shield.TotalMagical, 3);
        Assert.Equal(30d, shield[true, MagicType.Bright], 3);
        Assert.Equal(10d, shield[false], 3);
    }

    [Fact]
    public void ShieldOfEffect_IsConsumedAndRemovedWhenBroken()
    {
        Shield shield = new();
        Effect effect = new();
        shield.AddShieldOfEffect(new ShieldOfEffect(effect, 100, ShieldType.Physical));

        Assert.Equal(100d, shield.TotalPhysical, 3);
        Assert.Equal(60d, shield.CalculateShieldOfEffect(effect, 40), 3);
        Assert.Equal(60d, shield.TotalPhysical, 3);

        Assert.Equal(0d, shield.CalculateShieldOfEffect(effect, 100), 3);
        Assert.Empty(shield.ShieldOfEffects);
        Assert.Equal(0d, shield.TotalPhysical, 3);
    }
}
