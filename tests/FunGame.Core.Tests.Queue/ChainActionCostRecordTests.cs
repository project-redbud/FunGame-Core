using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class ChainActionCostRecordTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConfirmChainAction_CostRoundTripKeepsExecutorAndResources(bool useItem)
    {
        BattleFixture battle = new();
        Skill skill = battle.NewSkill(useItem ? SkillType.Item : SkillType.Skill, battle.B);
        skill.EPCost = 10;
        Item? item = null;
        if (useItem)
        {
            item = new Item { Name = "协同物品", RemainUseTimes = 1, IsReduceTimesAfterUse = true };
            item.Skills.Active = skill;
            battle.B.Items.Add(item);
        }
        double ep = battle.B.EP;
        battle.DuringTurn(() =>
        {
            ChainActionRequest? request = battle.Queue.SubmitChainAction(battle.A, battle.B,
                useItem ? CharacterActionType.UseItem : CharacterActionType.CastSkill,
                [battle.D], out string reason, skill, item);
            Assert.NotNull(request);
            Assert.True(battle.Queue.ConfirmChainAction(battle.A, request.Id).IsSuccess, reason);
        });

        RoundRecord restored = JsonService.GetObject<RoundRecord>(JsonService.GetString(battle.Queue.LastRound))!;
        ActionRecord execution = Assert.Single(restored.Actions, a => a.Actor.Guid == battle.B.Guid);
        ActionRecord confirmation = Assert.Single(restored.Actions, a => a.ActionType == CharacterActionType.ChainAction);
        Assert.True(execution.IsSuccess);
        Assert.Equal(10d, execution.EPCost, 3);
        Assert.Equal(ep - 10, battle.B.EP, 3);
        Assert.Equal(battle.A.Guid, confirmation.Actor.Guid);
        Assert.Equal(battle.A.Guid, execution.TurnActor!.Guid);
        Assert.Equal(confirmation.ChainActionRequestId, execution.ChainActionRequestId);
        Assert.Equal(0d, confirmation.EPCost, 3);
        if (useItem)
        {
            KeyValuePair<Item, string> cost = Assert.Single(restored.ItemsCost);
            Assert.Equal("-10 EP", cost.Value);
            Assert.Same(execution.Item, cost.Key);
            Assert.Equal(item!.Guid, cost.Key.Guid);
            Assert.DoesNotContain(item, battle.B.Items);
        }
        else
        {
            KeyValuePair<Skill, string> cost = Assert.Single(restored.SkillsCost);
            Assert.Equal("-10 EP", cost.Value);
            Assert.Same(execution.Skill, cost.Key);
            Assert.Equal(skill.Guid, cost.Key.Guid);
        }
    }
}
