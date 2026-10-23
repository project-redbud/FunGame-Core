using System.Text.Json;
using System.Text.Json.Nodes;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Infrastructure;

public class RoundRecordCostTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeserializeCosts_RestoresLegacyReferencesRegardlessOfPropertyOrder(bool costsFirst)
    {
        Skill skill = new OpenSkill(6007, "技能", []);
        Item item = new OpenItem(6008, "物品", []);
        RoundRecord round = new(1);
        round.Skills[CharacterActionType.CastSkill] = skill;
        round.SkillsCost[skill] = "-30 EP";
        round.Items[CharacterActionType.UseItem] = item;
        round.ItemsCost[item] = "-10 MP";

        RoundRecord restored = Read(round, costsFirst);

        KeyValuePair<Skill, string> skillCost = Assert.Single(restored.SkillsCost);
        KeyValuePair<Item, string> itemCost = Assert.Single(restored.ItemsCost);
        Assert.Same(restored.Skills[CharacterActionType.CastSkill], skillCost.Key);
        Assert.Same(restored.Items[CharacterActionType.UseItem], itemCost.Key);
        Assert.Equal("-30 EP", skillCost.Value);
        Assert.Equal("-10 MP", itemCost.Value);
        Assert.Empty(restored.Actions);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeserializeCosts_UsesLatestMatchingActionWhenRoundReferenceIsMissing(bool costsFirst)
    {
        Skill firstSkill = new OpenSkill(6007, "技能", []);
        Skill lastSkill = new OpenSkill(6007, "技能", []);
        Item firstItem = new OpenItem(6008, "物品", []);
        Item lastItem = new OpenItem(6008, "物品", []);
        RoundRecord round = new(1);
        round.Actions.Add(new(1) { Skill = firstSkill, Item = firstItem, EPCost = 30, MPCost = 10 });
        round.Actions.Add(new(1) { Skill = lastSkill, Item = lastItem, EPCost = 20, MPCost = 5 });
        round.SkillsCost[lastSkill] = "-20 EP";
        round.ItemsCost[lastItem] = "-5 MP";

        RoundRecord restored = Read(round, costsFirst);

        KeyValuePair<Skill, string> skillCost = Assert.Single(restored.SkillsCost);
        KeyValuePair<Item, string> itemCost = Assert.Single(restored.ItemsCost);
        Assert.Same(restored.Actions[1].Skill, skillCost.Key);
        Assert.Same(restored.Actions[1].Item, itemCost.Key);
        Assert.Equal("-20 EP", skillCost.Value);
        Assert.Equal("-5 MP", itemCost.Value);
        Assert.Equal(30d, restored.Actions[0].EPCost, 3);
        Assert.Equal(20d, restored.Actions[1].EPCost, 3);
        RoundRecord savedAgain = Read(restored, !costsFirst);
        Assert.Equal("-20 EP", Assert.Single(savedAgain.SkillsCost).Value);
        Assert.Equal("-5 MP", Assert.Single(savedAgain.ItemsCost).Value);
    }

    [Fact]
    public void DeserializeCosts_PrefersRoundReferencesAndSkipsUnmatchedEntries()
    {
        Skill roundSkill = new OpenSkill(6007, "技能", []);
        Item roundItem = new OpenItem(6008, "物品", []);
        RoundRecord round = new(1);
        round.Skills[CharacterActionType.CastSkill] = roundSkill;
        round.Items[CharacterActionType.UseItem] = roundItem;
        round.SkillsCost[roundSkill] = "-30 EP";
        round.ItemsCost[roundItem] = "-10 MP";
        round.SkillsCost[new OpenSkill(9991, "无引用技能", [])] = "-50 EP";
        round.ItemsCost[new OpenItem(9992, "无引用物品", [])] = "-50 MP";
        round.Actions.Add(new(1)
        {
            Skill = new OpenSkill(6007, "技能", []),
            Item = new OpenItem(6008, "物品", [])
        });

        RoundRecord restored = Read(round, true);

        Assert.Same(restored.Skills[CharacterActionType.CastSkill], Assert.Single(restored.SkillsCost).Key);
        Assert.Same(restored.Items[CharacterActionType.UseItem], Assert.Single(restored.ItemsCost).Key);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SerializeCosts_MergesConflictingValuesWithoutChangingActionDetails(bool reverseInsertion)
    {
        Skill firstSkill = new OpenSkill(6007, "技能", []);
        Skill lastSkill = new OpenSkill(6007, "技能", []);
        Item firstItem = new OpenItem(6008, "物品", []);
        Item lastItem = new OpenItem(6008, "物品", []);
        RoundRecord round = new(1);
        if (reverseInsertion)
        {
            round.SkillsCost[lastSkill] = "-20 EP";
            round.ItemsCost[lastItem] = "-20 MP / -5 EP";
        }
        round.SkillsCost[firstSkill] = "-30 EP";
        round.ItemsCost[firstItem] = "-30 MP";
        if (!reverseInsertion)
        {
            round.SkillsCost[lastSkill] = "-20 EP";
            round.ItemsCost[lastItem] = "-20 MP / -5 EP";
        }
        round.SkillsCost[firstSkill] = "-10 EP";
        round.ItemsCost[firstItem] = "";
        round.SkillsCost[new OpenSkill(6010, "另一技能", [])] = "-1 EP";
        round.ItemsCost[new OpenItem(6011, "另一物品", [])] = "-2 MP";
        round.Actions.Add(new(1) { Skill = firstSkill, Item = firstItem, MPCost = 30, EPCost = 30 });
        round.Actions.Add(new(1) { Skill = lastSkill, Item = lastItem, MPCost = 20, EPCost = 20 });
        round.Actions.Add(new(1) { Skill = firstSkill, Item = firstItem, MPCost = 0, EPCost = 10 });

        string json = JsonService.GetString(round);
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement skillCosts = document.RootElement.GetProperty(nameof(RoundRecord.SkillsCost));
        JsonElement itemCosts = document.RootElement.GetProperty(nameof(RoundRecord.ItemsCost));

        Assert.Equal(2, skillCosts.EnumerateObject().Count());
        Assert.Equal(2, itemCosts.EnumerateObject().Count());
        Assert.Equal(reverseInsertion ? "-10 EP" : "-20 EP", skillCosts.GetProperty("6007.技能").GetString());
        Assert.Equal(reverseInsertion ? "" : "-20 MP / -5 EP", itemCosts.GetProperty("6008.物品").GetString());
        Assert.Equal("-1 EP", skillCosts.GetProperty("6010.另一技能").GetString());
        Assert.Equal("-2 MP", itemCosts.GetProperty("6011.另一物品").GetString());
        RoundRecord restored = JsonService.GetObject<RoundRecord>(json)!;
        double[] expectedEP = [30, 20, 10];
        double[] expectedMP = [30, 20, 0];
        Assert.Equal(expectedEP, restored.Actions.Select(a => a.EPCost));
        Assert.Equal(expectedMP, restored.Actions.Select(a => a.MPCost));
        Assert.Equal(firstSkill.Guid, restored.Actions[2].Skill!.Guid);
        Assert.Equal(lastItem.Guid, restored.Actions[1].Item!.Guid);
    }

    [Fact]
    public void SerializeCosts_EmptyMapsRemainJsonObjects()
    {
        using JsonDocument document = JsonDocument.Parse(JsonService.GetString(new RoundRecord(1)));

        Assert.Equal(JsonValueKind.Object, document.RootElement.GetProperty(nameof(RoundRecord.SkillsCost)).ValueKind);
        Assert.Equal(JsonValueKind.Object, document.RootElement.GetProperty(nameof(RoundRecord.ItemsCost)).ValueKind);
        Assert.Empty(document.RootElement.GetProperty(nameof(RoundRecord.SkillsCost)).EnumerateObject());
        Assert.Empty(document.RootElement.GetProperty(nameof(RoundRecord.ItemsCost)).EnumerateObject());
    }

    private static RoundRecord Read(RoundRecord round, bool costsFirst)
    {
        JsonObject source = JsonNode.Parse(JsonService.GetString(round))!.AsObject();
        JsonObject ordered = new();
        if (costsFirst)
        {
            string[] names = [nameof(RoundRecord.SkillsCost), nameof(RoundRecord.ItemsCost)];
            foreach (string name in names)
            {
                JsonNode? costs = source[name];
                source.Remove(name);
                ordered[name] = costs;
            }
        }
        foreach (KeyValuePair<string, JsonNode?> property in source)
        {
            ordered[property.Key] = property.Value?.DeepClone();
        }
        return JsonService.GetObject<RoundRecord>(ordered.ToJsonString())!;
    }
}
