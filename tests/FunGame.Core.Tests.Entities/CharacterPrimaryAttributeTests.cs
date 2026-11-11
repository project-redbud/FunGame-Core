using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Entities;

public class CharacterPrimaryAttributeTests
{
    [Theory]
    [InlineData(PrimaryAttribute.STR, 0, .3)]
    [InlineData(PrimaryAttribute.AGI, 0, .3)]
    [InlineData(PrimaryAttribute.INT, 0, .3)]
    [InlineData(PrimaryAttribute.STR, 4, 0)]
    [InlineData(PrimaryAttribute.AGI, 4, 0)]
    [InlineData(PrimaryAttribute.INT, 4, 0)]
    [InlineData(PrimaryAttribute.STR, 4, .3)]
    [InlineData(PrimaryAttribute.AGI, 4, .3)]
    [InlineData(PrimaryAttribute.INT, 4, .3)]
    [InlineData(PrimaryAttribute.STR, 4, -.3)]
    [InlineData(PrimaryAttribute.AGI, 4, -.3)]
    [InlineData(PrimaryAttribute.INT, 4, -.3)]
    [InlineData(PrimaryAttribute.None, 4, .3)]
    public void CoreAttributeBonuses_PropagateToCombatAttributes(PrimaryAttribute primary, double flat, double percentage)
    {
        Character character = NewCharacter(primary);
        character.ExSTR = character.ExAGI = character.ExINT = flat;
        character.ExSTRPercentage = character.ExAGIPercentage = character.ExINTPercentage = percentage;
        double primaryBase = primary switch
        {
            PrimaryAttribute.AGI => 10,
            PrimaryAttribute.INT => 5,
            _ => 20
        };
        double extraPrimary = flat + primaryBase * percentage;
        double extraStrength = flat + 20 * percentage;
        double extraIntelligence = flat + 5 * percentage;

        Assert.Equal(extraPrimary, character.ExPrimaryAttributeValue, 3);
        Assert.Equal(character.BasePrimaryAttributeValue + extraPrimary, character.PrimaryAttributeValue, 3);
        Assert.Equal(extraPrimary, character.ExATK, 3);
        Assert.Equal(100 + primaryBase + extraPrimary, character.ATK, 3);
        Assert.Equal(character.ATK, character.NormalAttack.Damage, 3);
        Assert.Equal(extraStrength * character.GameplayEquilibriumConstant.STRtoHPFactor, character.ExHP, 3);
        Assert.Equal(extraStrength * character.GameplayEquilibriumConstant.STRtoDEFFactor, character.ExDEF, 3);
        Assert.Equal(extraIntelligence * character.GameplayEquilibriumConstant.INTtoMPFactor, character.ExMP, 3);
        Assert.Equal(100 + (20 + extraStrength) * character.GameplayEquilibriumConstant.STRtoHPFactor, character.MaxHP, 3);
        Assert.Equal(10 + (20 + extraStrength) * character.GameplayEquilibriumConstant.STRtoDEFFactor, character.DEF, 3);
        Assert.Equal(80 + (5 + extraIntelligence) * character.GameplayEquilibriumConstant.INTtoMPFactor, character.MaxMP, 3);
    }

    [Fact]
    public void NonPrimaryPercentageBonuses_OnlyChangeTheirDerivedAttributes()
    {
        Character character = NewCharacter(PrimaryAttribute.AGI);
        double attack = character.ATK;
        double maxHp = character.MaxHP;
        double maxMp = character.MaxMP;
        double defense = character.DEF;

        character.ExSTRPercentage = .3;

        Assert.Equal(attack, character.ATK, 3);
        Assert.Equal(maxHp + 6 * character.GameplayEquilibriumConstant.STRtoHPFactor, character.MaxHP, 3);
        Assert.Equal(defense + 6 * character.GameplayEquilibriumConstant.STRtoDEFFactor, character.DEF, 3);
        Assert.Equal(maxMp, character.MaxMP, 3);

        character.ExINTPercentage = .3;

        Assert.Equal(attack, character.ATK, 3);
        Assert.Equal(maxMp + 1.5 * character.GameplayEquilibriumConstant.INTtoMPFactor, character.MaxMP, 3);
    }

    [Theory]
    [InlineData(PrimaryAttribute.STR, 24)]
    [InlineData(PrimaryAttribute.AGI, 16)]
    [InlineData(PrimaryAttribute.INT, 13)]
    public void PercentageBonuses_FollowLevelGrowth(PrimaryAttribute primary, double primaryBase)
    {
        Character character = NewCharacter(primary);
        character.ExSTRPercentage = character.ExAGIPercentage = character.ExINTPercentage = .3;

        character.Level = 3;

        Assert.Equal(primaryBase * .3, character.ExPrimaryAttributeValue, 3);
        Assert.Equal(primaryBase * .3, character.ExATK, 3);
        Assert.Equal(24 * .3 * character.GameplayEquilibriumConstant.STRtoHPFactor, character.ExHP, 3);
        Assert.Equal(24 * .3 * character.GameplayEquilibriumConstant.STRtoDEFFactor, character.ExDEF, 3);
        Assert.Equal(13 * .3 * character.GameplayEquilibriumConstant.INTtoMPFactor, character.ExMP, 3);
    }

    [Fact]
    public void PercentageBonuses_StackWithDirectCombatBonuses()
    {
        Character character = NewCharacter(PrimaryAttribute.STR);
        character.ExSTRPercentage = character.ExINTPercentage = .3;
        character.ExATK2 = character.ExDEF2 = character.ExHP2 = character.ExMP2 = 7;
        character.ExATKPercentage = character.ExDEFPercentage = character.ExHPPercentage = character.ExMPPercentage = .2;

        Assert.Equal(character.BaseATK * 1.2 + 6 + 7, character.ATK, 3);
        Assert.Equal(character.BaseDEF * 1.2 + 6 * character.GameplayEquilibriumConstant.STRtoDEFFactor + 7, character.DEF, 3);
        Assert.Equal(character.BaseHP * 1.2 + 6 * character.GameplayEquilibriumConstant.STRtoHPFactor + 7, character.MaxHP, 3);
        Assert.Equal(character.BaseMP * 1.2 + 1.5 * character.GameplayEquilibriumConstant.INTtoMPFactor + 7, character.MaxMP, 3);
    }

    [Fact]
    public void PercentageBonuses_DoNotRefillCurrentResources()
    {
        Character character = NewCharacter(PrimaryAttribute.STR);
        character.HP -= 40;
        character.MP -= 25;
        double hp = character.HP;
        double mp = character.MP;
        double maxHp = character.MaxHP;
        double maxMp = character.MaxMP;

        character.ExSTRPercentage = character.ExINTPercentage = .3;

        Assert.True(character.MaxHP > maxHp);
        Assert.True(character.MaxMP > maxMp);
        Assert.Equal(hp, character.HP, 3);
        Assert.Equal(mp, character.MP, 3);
    }

    [Fact]
    public void NegativePercentageBonuses_KeepResourceMinimums()
    {
        Character character = NewCharacter(PrimaryAttribute.STR);
        character.ExSTRPercentage = character.ExINTPercentage = -10;

        Assert.Equal(1d, character.MaxHP, 3);
        Assert.Equal(1d, character.MaxMP, 3);
    }

    [Theory]
    [InlineData(PrimaryAttribute.STR)]
    [InlineData(PrimaryAttribute.AGI)]
    [InlineData(PrimaryAttribute.INT)]
    public void CopyWithExtras_PreservesCoreAttributeBonuses(PrimaryAttribute primary)
    {
        Character character = NewCharacter(primary);
        character.ExSTR = character.ExAGI = character.ExINT = 4;
        character.ExSTRPercentage = character.ExAGIPercentage = character.ExINTPercentage = .3;

        Character copy = character.Copy(copyEx: true);

        Assert.Equal(character.PrimaryAttributeValue, copy.PrimaryAttributeValue, 3);
        Assert.Equal(copy.BasePrimaryAttributeValue + copy.ExPrimaryAttributeValue, copy.PrimaryAttributeValue, 3);
        Assert.Equal(character.ATK, copy.ATK, 3);
        Assert.Equal(character.DEF, copy.DEF, 3);
        Assert.Equal(character.MaxHP, copy.MaxHP, 3);
        Assert.Equal(character.MaxMP, copy.MaxMP, 3);
        Assert.Equal(4d, copy.ExSTR, 3);
        Assert.Equal(6d, copy.ExSTR2, 3);
    }

    private static Character NewCharacter(PrimaryAttribute primary)
    {
        Character character = Factory.GetCharacter();
        character.Name = "属性回归角色";
        character.PrimaryAttribute = primary;
        character.InitialSTR = 20;
        character.InitialAGI = 10;
        character.InitialINT = 5;
        character.STRGrowth = 2;
        character.AGIGrowth = 3;
        character.INTGrowth = 4;
        character.InitialATK = 100;
        character.InitialDEF = 10;
        character.InitialHP = 100;
        character.InitialMP = 80;
        character.Level = 1;
        character.Recovery();
        return character;
    }
}
