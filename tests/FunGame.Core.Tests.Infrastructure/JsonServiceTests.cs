using System.Collections;
using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;

namespace FunGame.Core.Tests.Infrastructure;

public class JsonServiceTests
{
    [Fact]
    public void GeneralOptions_AreCaseInsensitiveIndentedAndChineseFriendly()
    {
        Assert.True(JsonService.GeneralOptions.PropertyNameCaseInsensitive);
        Assert.True(JsonService.GeneralOptions.WriteIndented);
        Assert.NotEmpty(JsonService.GeneralOptions.Converters);
    }

    [Fact]
    public void GetString_KeepsChineseCharactersUnescaped()
    {
        string json = JsonService.GetString(new Sample { Name = "中文" });

        Assert.Contains("中文", json);
        Assert.Contains("\n", json);
    }

    [Fact]
    public void GetObject_IsCaseInsensitiveForPlainTypes()
    {
        Sample? sample = JsonService.GetObject<Sample>("{\"name\":\"甲\",\"VALUE\":7}");

        Assert.Equal("甲", sample!.Name);
        Assert.Equal(7, sample.Value);
    }

    [Fact]
    public void GetStringAndGetObject_RoundTrip()
    {
        Sample sample = new() { Name = "甲", Value = 3 };

        Sample? decoded = JsonService.GetObject<Sample>(JsonService.GetString(sample));

        Assert.Equal(sample.Name, decoded!.Name);
        Assert.Equal(sample.Value, decoded.Value);
    }

    [Fact]
    public void GetObjects_AcceptsConcatenatedAndArrayInput()
    {
        List<Sample> concatenated = JsonService.GetObjects<Sample>("{\"Name\":\"甲\"}{\"Name\":\"乙\"}");
        List<Sample> separated = JsonService.GetObjects<Sample>("{\"Name\":\"甲\"}\n{\"Name\":\"乙\"}");
        List<Sample> array = JsonService.GetObjects<Sample>("[{\"Name\":\"甲\"},{\"Name\":\"乙\"}]");
        List<Sample> empty = JsonService.GetObjects<Sample>("");

        Assert.Equal(new[] { "甲", "乙" }, concatenated.Select(s => s.Name));
        Assert.Equal(new[] { "甲", "乙" }, separated.Select(s => s.Name));
        Assert.Equal(new[] { "甲", "乙" }, array.Select(s => s.Name));
        Assert.Empty(empty);
    }

    [Fact]
    public void GetObjectFromHashtable_ReadsJsonElementAndFallsBackToDefault()
    {
        using JsonDocument document = JsonDocument.Parse("{\"greeting\":\"你好\"}");
        Hashtable table = new() { { "greeting", document.RootElement.GetProperty("greeting") } };

        Assert.Equal("你好", JsonService.GetObject<string>(table, "greeting"));
        Assert.Null(JsonService.GetObject<string>(table, "missing"));
    }

    [Fact]
    public void GetObjectFromDictionaryAndEnumerable_ReadElements()
    {
        using JsonDocument document = JsonDocument.Parse("{\"value\":12}");
        Dictionary<string, object> dictionary = new() { { "value", document.RootElement.GetProperty("value") } };

        Assert.Equal(12, JsonService.GetObject<int>(dictionary, "value"));
        Assert.Equal(12, JsonService.GetObject<int>(new object[] { document.RootElement.GetProperty("value") }, 0));
        Assert.Equal(0, JsonService.GetObject<int>(new object[] { document.RootElement.GetProperty("value") }, 5));
    }

    [Fact]
    public void IsCompleteJson_ReportsFailureForBrokenJson()
    {
        Assert.True(JsonService.IsCompleteJson<Sample>("{\"Name\":\"甲\"}"));
        Assert.False(JsonService.IsCompleteJson<Sample>("{\"Name\":"));
    }

    [Fact]
    public void JsonTool_UsesIndependentCaseSensitiveOptions()
    {
        JsonTool tool = new();

        Assert.False(tool.Options.PropertyNameCaseInsensitive);

        string json = tool.GetString(new Sample { Name = "甲" });
        Assert.NotNull(tool.GetObject<Sample>(json));
    }

    [Fact]
    public void JsonTool_AddConverterIsIdempotent()
    {
        JsonTool tool = new();
        int before = tool.Options.Converters.Count;
        var converter = new FunGame.Core.Library.Common.JsonConverter.DateTimeConverter();

        tool.AddConverter(converter);
        tool.AddConverter(converter);

        Assert.Equal(before + 1, tool.Options.Converters.Count);
    }

    [Fact]
    public void CharacterJson_RoundTripKeepsResources()
    {
        Character character = new()
        {
            Name = "甲",
            InitialHP = 200,
            InitialMP = 100,
            InitialATK = 30,
            InitialDEF = 10
        };
        character.Level = 1;
        character.HP -= 40;
        character.MP -= 25;
        character.EP = 60;

        Character? decoded = JsonService.GetObject<Character>(JsonService.GetString(character));

        Assert.NotNull(decoded);
        Assert.Equal(character.Name, decoded.Name);
        Assert.Equal(character.MaxHP, decoded.MaxHP, 3);
        Assert.Equal(character.HP, decoded.HP, 3);
        Assert.Equal(character.MP, decoded.MP, 3);
        Assert.Equal(character.EP, decoded.EP, 3);
    }

    internal sealed class Sample
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
    }
}
