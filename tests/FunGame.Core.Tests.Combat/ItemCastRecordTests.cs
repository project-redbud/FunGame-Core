using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Combat;

public class ItemCastRecordTests
{
    [Theory]
    [InlineData(30, 0, 0, 10, "-10 EP")]
    [InlineData(30, 10, 0, 10, "-10 EP")]
    [InlineData(30, 10, 0, 0, "")]
    [InlineData(0, 0, 20, 5, "-20 MP / -5 EP")]
    public void RepeatedItemUse_RecordsOnlyLatestCostAndPreservesEachAction(
        double firstMP, double firstEP, double secondMP, double secondEP, string expectedCost)
    {
        TurnBattle battle = new();
        Skill skill = new TestSkill(SkillType.Item)
        {
            Name = "复用物品技能", Level = 1, MPCost = firstMP, EPCost = firstEP, CD = 0, HardnessTime = 5
        };
        Item item = new() { Name = "复用物品", IsReduceTimesAfterUse = false, IsRemoveAfterUse = false };
        item.Skills.Active = skill;
        item.Character = battle.Caster;
        battle.Caster.Items.Add(item);
        double mp = battle.Caster.MP, ep = battle.Caster.EP;
        int decisions = 0;
        battle.Queue.TurnStartEvent += ctx =>
        {
            DecisionPoints points = battle.Points(battle.Caster);
            points.MaxDecisionPoints = 20;
            points.CurrentDecisionPoints = 20;
            points.AddTempActionQuota(new Effect(), CharacterActionType.UseItem, 2);
            return true;
        };
        battle.Queue.DecideActionEvent += ctx => ++decisions <= 2 ? CharacterActionType.UseItem : CharacterActionType.EndTurn;
        battle.Queue.SelectItemEvent += ctx => item;
        battle.Queue.SelectSkillTargetsEvent += ctx => [battle.Enemy];
        battle.Queue.CharacterCastItemSkillEvent += ctx => { skill.MPCost = secondMP; skill.EPCost = secondEP; };

        battle.Run(battle.Caster);

        List<ActionRecord> actions = [.. battle.Queue.LastRound.Actions.Where(a => a.ActionType == CharacterActionType.UseItem)];
        Assert.Equal(2, actions.Count);
        Assert.All(actions, action => Assert.True(action.IsSuccess));
        Assert.Equal(firstMP, actions[0].MPCost, 3);
        Assert.Equal(firstEP, actions[0].EPCost, 3);
        Assert.Equal(secondMP, actions[1].MPCost, 3);
        Assert.Equal(secondEP, actions[1].EPCost, 3);
        Assert.Equal(expectedCost, actions[1].Cost);
        Assert.Equal(expectedCost, battle.Queue.LastRound.ItemsCost[item]);
        Assert.Equal(mp - firstMP - secondMP, battle.Caster.MP, 3);
        Assert.Equal(ep - firstEP - secondEP, battle.Caster.EP, 3);
    }
}
