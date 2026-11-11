using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Infrastructure;

public class CharacterAttributeSerializationTests
{
    [Theory]
    [InlineData(PrimaryAttribute.STR, 130)]
    [InlineData(PrimaryAttribute.AGI, 117)]
    [InlineData(PrimaryAttribute.INT, 110.5)]
    public void LegacyCoreAttributeFields_PreserveBonusesThroughRoundTrip(PrimaryAttribute primary, double expectedAttack)
    {
        const string json = """
            {
                "Level": 1,
                "InitialSTR": 20, "InitialAGI": 10, "InitialINT": 5,
                "InitialATK": 100, "InitialDEF": 10, "InitialHP": 100, "InitialMP": 80,
                "ExSTR": 4, "ExAGI": 4, "ExINT": 4,
                "ExSTRPercentage": 0.3, "ExAGIPercentage": 0.3, "ExINTPercentage": 0.3
            }
            """;
        Character? character = NetworkUtility.JsonDeserialize<Character>(json);
        Assert.NotNull(character);
        character.PrimaryAttribute = primary;

        Assert.Equal(expectedAttack, character.ATK, 3);
        Assert.Equal(490d, character.MaxHP, 3);
        Assert.Equal(32.5, character.DEF, 3);
        Assert.Equal(164d, character.MaxMP, 3);

        string encoded = NetworkUtility.JsonSerialize(character);
        using JsonDocument document = JsonDocument.Parse(encoded);
        Assert.Equal(4d, document.RootElement.GetProperty("ExSTR").GetDouble(), 3);
        Assert.Equal(.3, document.RootElement.GetProperty("ExSTRPercentage").GetDouble(), 3);
        Assert.False(document.RootElement.TryGetProperty("ExSTR2", out _));
        Character? decoded = NetworkUtility.JsonDeserialize<Character>(encoded);

        Assert.NotNull(decoded);
        Assert.Equal(character.STR, decoded.STR, 3);
        Assert.Equal(character.AGI, decoded.AGI, 3);
        Assert.Equal(character.INT, decoded.INT, 3);
        Assert.Equal(character.ATK, decoded.ATK, 3);
        Assert.Equal(character.DEF, decoded.DEF, 3);
        Assert.Equal(character.MaxHP, decoded.MaxHP, 3);
        Assert.Equal(character.MaxMP, decoded.MaxMP, 3);
        Assert.Equal(character.ExPrimaryAttributeValue, decoded.ExPrimaryAttributeValue, 3);
    }
}
