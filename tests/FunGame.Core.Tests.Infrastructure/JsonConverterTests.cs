using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Common.JsonConverter;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Infrastructure;

public class JsonConverterTests
{
    [Fact]
    public void DateTimeConverter_RoundTripsConfiguredFormat()
    {
        DateTime value = new(2026, 10, 12, 20, 30, 40, 123);

        DateTime decoded = NetworkUtility.JsonDeserialize<DateTime>(NetworkUtility.JsonSerialize(value));

        Assert.Equal(value, decoded);
    }

    [Fact]
    public void DateTimeConverter_RejectsInvalidPayload()
    {
        Assert.Throws<JsonException>(() => NetworkUtility.JsonDeserialize<DateTime>("\"不是时间\""));
    }

    [Fact]
    public void EffectConverter_StoresUnknownPropertiesInValues()
    {
        Effect effect = new() { Id = 1001, Name = "自定义特效" };
        effect.Values["exatk"] = 20d;

        Effect? decoded = JsonService.GetObject<Effect>(JsonService.GetString(effect));

        Assert.NotNull(decoded);
        Assert.Equal(1001, decoded.Id);
        Assert.Equal("自定义特效", decoded.Name);
        Assert.Equal(20d, Convert.ToDouble(decoded.Values["exatk"]), 3);
    }

    [Fact]
    public void ItemConverter_RoundTripsSkillsAndFlags()
    {
        Item item = new(ItemType.Weapon)
        {
            Id = 10001,
            Name = "木杖",
            Price = 250,
            WeaponType = WeaponType.Staff,
            IsTradable = false
        };
        item.Skills.Passives.Add(new OpenSkill(2001, "木杖被动词", []));

        Item? decoded = JsonService.GetObject<Item>(JsonService.GetString(item));

        Assert.NotNull(decoded);
        Assert.Equal(10001, decoded.Id);
        Assert.Equal("木杖", decoded.Name);
        Assert.Equal(250d, decoded.Price, 3);
        Assert.Equal(WeaponType.Staff, decoded.WeaponType);
        Assert.False(decoded.IsTradable);
        Assert.Single(decoded.Skills.Passives);
    }

    [Fact]
    public void ItemConverter_ReadsDynamicItemFromJson()
    {
        string json = """
            {
                "Id": 10001,
                "Name": "木杖",
                "Description": "增加角色 20 点攻击力。",
                "ItemType": 1,
                "WeaponType": 8,
                "Skills": {
                    "Active": null,
                    "Passives": [
                        { "Id": 2001, "Name": "木杖", "SkillType": 3, "Effects": [ { "Id": 1001, "exatk": 20 } ] }
                    ]
                }
            }
            """;

        Item? item = NetworkUtility.JsonDeserialize<Item>(json);

        Assert.NotNull(item);
        Assert.Equal("木杖", item.Name);
        Assert.Single(item.Skills.Passives);
        Assert.Equal(2001, item.Skills.Passives.First().Id);
        Assert.Equal(20d, Convert.ToDouble(item.Skills.Passives.First().Effects.First().Values["exatk"]), 3);
    }
}
