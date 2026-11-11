using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;

namespace FunGame.Core.Tests.Infrastructure;

public class UnitAttributeSerializationTests
{
    private const string AttributeJson = """
        {
            "HP": 500, "MP": 180, "EP": 60,
            "InitialHP": 100, "InitialMP": 80, "InitialATK": 100, "InitialDEF": 10,
            "InitialSTR": 20, "InitialAGI": 10, "InitialINT": 5, "Level": 1,
            "ExSTR2": 4, "ExAGI2": 4, "ExINT2": 4,
            "ExSTRPercentage": 0.3, "ExAGIPercentage": 0.3, "ExINTPercentage": 0.3,
            "ExHP2": 7, "ExMP2": 7, "ExHPPercentage": 0.2, "ExMPPercentage": 0.2
        }
        """;

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Unit_RoundTripsWritableAttributesAndDefersResources(int route)
    {
        JsonTool tool = new();
        Unit unit = route switch
        {
            1 => NetworkUtility.JsonDeserialize<Unit>(AttributeJson)!,
            2 => tool.GetObject<Unit>(AttributeJson)!,
            _ => JsonService.GetObject<Unit>(AttributeJson)!
        };

        Assert.Equal(30d, unit.STR, 3);
        Assert.Equal(569d, unit.MaxHP, 3);
        Assert.Equal(195d, unit.MaxMP, 3);
        Assert.Equal(500d, unit.HP, 3);
        Assert.Equal(180d, unit.MP, 3);
        string json = route switch
        {
            1 => NetworkUtility.JsonSerialize(unit),
            2 => tool.GetString(unit),
            _ => JsonService.GetString(unit)
        };
        using JsonDocument document = JsonDocument.Parse(json);
        Assert.Equal(4d, document.RootElement.GetProperty("ExSTR2").GetDouble(), 3);
        string[] attributes = ["HP", "MP", "STR", "AGI", "INT", "ATK", "DEF"];
        foreach (string attribute in attributes)
        {
            Assert.False(document.RootElement.TryGetProperty($"Base{attribute}", out _));
            Assert.False(document.RootElement.TryGetProperty($"Ex{attribute}", out _));
            Assert.False(document.RootElement.TryGetProperty($"Ex{attribute}3", out _));
        }
        string[] derivedProperties = ["MaxHP", "MaxMP", "ExHPFromExSTR", "ExMPFromExINT", "ExATKFromExPrimaryAttribute", "ExDEFFromExSTR"];
        foreach (string property in derivedProperties)
        {
            Assert.False(document.RootElement.TryGetProperty(property, out _), property);
        }

        Unit decoded = JsonService.GetObject<Unit>(json)!;
        Assert.True(decoded.IsUnit);
        Assert.Equal(unit.STR, decoded.STR, 3);
        Assert.Equal(unit.HP, decoded.HP, 3);
        Assert.Equal(unit.MP, decoded.MP, 3);
        Assert.Equal(JsonService.GetString<Character>(unit), JsonService.GetString(unit));
    }

    [Fact]
    public void Character_WritesPointSlotsAndExcludesDerivedAttributes()
    {
        Character character = JsonService.GetObject<Character>(AttributeJson)!;
        using JsonDocument document = JsonDocument.Parse(JsonService.GetString(character));

        Assert.Equal(4d, document.RootElement.GetProperty("ExSTR2").GetDouble(), 3);
        Assert.False(document.RootElement.TryGetProperty("ExSTR", out _));
        Assert.False(document.RootElement.TryGetProperty("ExSTR3", out _));
        Assert.Equal(500d, character.HP, 3);
    }

    [Fact]
    public void UserInventory_RoundTripsCorePointSlots()
    {
        Character character = JsonService.GetObject<Character>(AttributeJson)!;
        character.Id = 123;
        User user = new();
        user.Inventory.Characters.Add(character);

        User decoded = JsonService.GetObject<User>(JsonService.GetString(user))!;
        Character restored = Assert.Single(decoded.Inventory.Characters);

        Assert.Equal(4d, restored.ExSTR2, 3);
        Assert.Equal(6d, restored.ExSTR3, 3);
        Assert.Equal(character.STR, restored.STR, 3);
        Assert.Equal(character.HP, restored.HP, 3);
        Assert.Equal(character.MP, restored.MP, 3);
    }
}
