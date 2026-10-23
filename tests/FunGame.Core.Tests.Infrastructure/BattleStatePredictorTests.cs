using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Infrastructure;

public class BattleStatePredictorTests
{
    [Fact]
    public void Predict_CopiesInventoryAndEquipmentDetailsWithoutMutatingCheckpoint()
    {
        Character actor = new() { Name = "甲" };
        CharacterStateSnapshot checkpoint = new() { Character = actor, HP = 100, MaxHP = 100, MP = 50, MaxMP = 50 };
        checkpoint.Items.Add(new() { ItemId = 7001, ItemName = "药剂", Description = "背包物品" });
        checkpoint.Equipments[EquipSlotType.Weapon] = 7002;
        checkpoint.EquipmentsDetail.Add(new() { Slot = EquipSlotType.Weapon, ItemId = 7002, ItemName = "长剑", Description = "武器" });
        RoundRecord baseline = new(1) { Actor = actor, Checkpoint = [checkpoint] };

        CharacterStateSnapshot predicted = Assert.IsType<CharacterStateSnapshot>(
            BattleStatePredictor.Predict([baseline, new RoundRecord(2)], 2, actor.Guid));

        ItemStateSnapshot item = Assert.Single(predicted.Items);
        EquipmentStateSnapshot equipment = Assert.Single(predicted.EquipmentsDetail);
        Assert.Equal(7001L, item.ItemId);
        Assert.Equal("药剂", item.ItemName);
        Assert.Equal("背包物品", item.Description);
        Assert.Equal(EquipSlotType.Weapon, equipment.Slot);
        Assert.Equal(7002L, equipment.ItemId);
        Assert.Equal("长剑", equipment.ItemName);
        Assert.Equal("武器", equipment.Description);
        Assert.Equal(7002L, predicted.Equipments[EquipSlotType.Weapon]);
        Assert.NotSame(checkpoint.Items[0], item);
        Assert.NotSame(checkpoint.EquipmentsDetail[0], equipment);

        item.ItemName = "预测中的名字";
        item.Description = "预测中的描述";
        equipment.ItemName = "预测中的装备";
        equipment.Description = "预测中的装备描述";
        predicted.Items.Clear();
        predicted.EquipmentsDetail.Clear();

        Assert.Equal("药剂", Assert.Single(checkpoint.Items).ItemName);
        Assert.Equal("背包物品", checkpoint.Items[0].Description);
        Assert.Equal("长剑", Assert.Single(checkpoint.EquipmentsDetail).ItemName);
        Assert.Equal("武器", checkpoint.EquipmentsDetail[0].Description);
    }
}
