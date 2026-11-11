using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.PrefabricatedEntity;

namespace FunGame.Core.Tests.Entities;

public class CharacterAggregationTests
{
    private const string AttributeJson = """
        {
            "InitialHP": 100, "InitialMP": 80, "InitialATK": 100, "InitialDEF": 10,
            "InitialSTR": 20, "InitialAGI": 10, "InitialINT": 5, "Level": 1,
            "ExSTR2": 4, "ExAGI2": 4, "ExINT2": 4,
            "ExSTRPercentage": 0.3, "ExAGIPercentage": 0.3, "ExINTPercentage": 0.3,
            "ExATK2": 7, "ExDEF2": 7, "ExHP2": 7, "ExMP2": 7,
            "ExATKPercentage": 0.2, "ExDEFPercentage": 0.2, "ExHPPercentage": 0.2, "ExMPPercentage": 0.2
        }
        """;

    [Fact]
    public void ExtraAttributes_AggregateAllContributions()
    {
        Character character = JsonService.GetObject<Character>(AttributeJson)!;

        Assert.Equal(10d, character.ExSTR, 3);
        Assert.Equal(7d, character.ExAGI, 3);
        Assert.Equal(5.5, character.ExINT, 3);
        Assert.Equal(41d, character.ExATK, 3);
        Assert.Equal(19.5, character.ExDEF, 3);
        Assert.Equal(209d, character.ExHP, 3);
        Assert.Equal(75d, character.ExMP, 3);
        Assert.Equal(161d, character.ATK, 3);
        Assert.Equal(44.5, character.DEF, 3);
        Assert.Equal(569d, character.MaxHP, 3);
        Assert.Equal(195d, character.MaxMP, 3);
        Assert.Equal(10d, character.ExATKFromExPrimaryAttribute, 3);
        Assert.Equal(7.5, character.ExDEFFromExSTR, 3);
        Assert.Equal(130d, character.ExHPFromExSTR, 3);
        Assert.Equal(44d, character.ExMPFromExINT, 3);
        Assert.Equal(4d, character.ExSTR2, 3);
        Assert.Equal(6d, character.ExSTR3, 3);
    }

    [Theory]
    [InlineData(PrimaryAttribute.None, 10)]
    [InlineData(PrimaryAttribute.STR, 10)]
    [InlineData(PrimaryAttribute.AGI, 7)]
    [InlineData(PrimaryAttribute.INT, 5.5)]
    public void PrimaryAttributeSource_OnlyIncludesCoreAttributeBonuses(PrimaryAttribute primary, double expected)
    {
        Character character = JsonService.GetObject<Character>(AttributeJson)!;
        character.PrimaryAttribute = primary;

        Assert.Equal(expected, character.ExPrimaryAttributeValue, 3);
        Assert.Equal(expected, character.ExATKFromExPrimaryAttribute, 3);
        Assert.Equal(character.BasePrimaryAttributeValue + expected, character.PrimaryAttributeValue, 3);
        Assert.Equal(character.BaseATK + expected + character.ExATK2 + character.ExATK3, character.ATK, 3);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AttributePanels_DisplayEachContributionOnce(bool asUnit)
    {
        Character character = asUnit ? JsonService.GetObject<Unit>(AttributeJson)! : JsonService.GetObject<Character>(AttributeJson)!;
        string[] panels = character is Unit unit
            ? [unit.GetInfo(), unit.GetSimpleInfo(), unit.GetInBattleInfo(0), unit.GetItemInfo()]
            : [character.GetInfo(), character.GetSimpleInfo(), character.GetInBattleInfo(0), character.GetItemInfo(), character.GetAttributeInfo(), character.GetSimpleAttributeInfo()];

        foreach (string panel in panels)
        {
            Assert.Contains("[360 + 209]", panel);
            Assert.Contains("[120 + 75]", panel);
            Assert.Contains("[120 + 41]", panel);
        }
        Dictionary<string, string> values = character.GetAttributeValues();
        Assert.Contains("[25 + 19.5]", values["物理护甲"]);
        Assert.Contains("[20 + 10]", values["力量"]);
        Assert.Contains("[10 + 7]", values["敏捷"]);
        Assert.Contains("[5 + 5.5]", values["智力"]);
    }

    [Fact]
    public void CopyAndRespawn_PreserveConfiguredBonusLifecycle()
    {
        Character character = JsonService.GetObject<Character>(AttributeJson)!;
        Character copy = character.Copy(copyEx: true);
        Character withoutExtras = character.Copy();

        Assert.Equal(character.ExSTR2, copy.ExSTR2, 3);
        Assert.Equal(character.ExSTR3, copy.ExSTR3, 3);
        Assert.Equal(character.ATK, copy.ATK, 3);
        Assert.Equal(character.MaxHP, copy.MaxHP, 3);
        Assert.Equal(0d, withoutExtras.ExSTR, 3);
        Assert.Equal(0d, withoutExtras.ExATK, 3);

        character.Respawn(copy);

        Assert.Equal(0d, character.ExSTR2, 3);
        Assert.Equal(0d, character.ExSTRPercentage, 3);
        Assert.Equal(0d, character.ExATK, 3);
        Assert.Equal(character.MaxHP, character.HP, 3);
    }

    [Fact]
    public void MagicCardPack_UpdatesPointSlotsAndRestoresResonance()
    {
        Character character = JsonService.GetObject<Character>(AttributeJson)!;
        character.PrimaryAttribute = PrimaryAttribute.STR;
        MagicCardPack pack = new(new Dictionary<string, object>
        {
            ["exstr"] = 15d,
            ["exagi"] = 10d,
            ["exint"] = 5d,
            ["resonance"] = "AGI"
        });

        Assert.True(character.Equip(pack));
        Assert.Equal(19d, character.ExSTR2, 3);
        Assert.Equal(14d, character.ExAGI2, 3);
        Assert.Equal(9d, character.ExINT2, 3);
        Assert.Equal(6d, character.ExSTR3, 3);
        Assert.Equal(PrimaryAttribute.AGI, character.PrimaryAttribute);
        Assert.Equal(17d, character.ExATKFromExPrimaryAttribute, 3);

        Assert.Same(pack, character.UnEquip(EquipSlotType.MagicCardPack));
        Assert.Equal(4d, character.ExSTR2, 3);
        Assert.Equal(4d, character.ExAGI2, 3);
        Assert.Equal(4d, character.ExINT2, 3);
        Assert.Equal(PrimaryAttribute.STR, character.PrimaryAttribute);
    }
}
