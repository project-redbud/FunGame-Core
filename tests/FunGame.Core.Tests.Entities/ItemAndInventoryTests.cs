using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Entities;

public class ItemAndInventoryTests
{
    [Fact]
    public void NewItem_HasExpectedDefaults()
    {
        Item item = new();

        Assert.Equal(ItemType.Others, item.ItemType);
        Assert.True(item.Equipable);
        Assert.True(item.Unequipable);
        Assert.True(item.IsInGameItem);
        Assert.True(item.IsPurchasable);
        Assert.True(item.IsSellable);
        Assert.True(item.IsTradable);
        Assert.True(item.IsReduceTimesAfterUse);
        Assert.True(item.IsRemoveAfterUse);
        Assert.False(item.Stackable);
        Assert.Equal(1, item.StackCount);
        Assert.Equal(0, item.RemainUseTimes);
        Assert.Equal(0d, item.Price, 3);
        Assert.False(item.IsActive);
        Assert.Null(item.Skills.Active);
        Assert.Empty(item.Skills.Passives);
        Assert.Empty(item.Skills.Magics);
    }

    [Fact]
    public void IsActive_FollowsActiveSkill()
    {
        Item item = new();

        item.Skills.Active = new OpenSkill(1, "主动", []);

        Assert.True(item.IsActive);
    }

    [Fact]
    public void CharacterAssignment_PropagatesToActiveSkill()
    {
        Character character = EntitiesHelper.NewCharacter("持有者");
        Item item = new();
        item.Skills.Active = new OpenSkill(1, "主动", []);

        item.Character = character;

        Assert.Same(character, item.Skills.Active.Character);
    }

    [Fact]
    public void ReduceTimesAndRemove_MarksItemDeletedWhenExhausted()
    {
        Character character = EntitiesHelper.NewCharacter();
        Item item = new() { Name = "药剂", RemainUseTimes = 2 };
        item.Character = character;
        character.Items.Add(item);

        item.ReduceTimesAndRemove();

        Assert.Equal(1, item.RemainUseTimes);
        Assert.Equal(EntityState.Unchanged, item.EntityState);

        item.ReduceTimesAndRemove();

        Assert.Equal(0, item.RemainUseTimes);
        Assert.Equal(EntityState.Deleted, item.EntityState);
    }

    [Fact]
    public void ReduceTimesAndRemove_KeepsItemWhenNotRemovable()
    {
        Item item = new() { Name = "药剂", RemainUseTimes = 1, IsRemoveAfterUse = false };

        item.ReduceTimesAndRemove();

        Assert.Equal(0, item.RemainUseTimes);
        Assert.Equal(EntityState.Unchanged, item.EntityState);
    }

    [Fact]
    public void Copy_ProducesIndependentItem()
    {
        Item item = EntitiesHelper.NewWeapon("法杖");
        item.Price = 100;
        Item copy = item.Copy();

        Assert.NotSame(item, copy);
        Assert.Equal(item.Name, copy.Name);
        Assert.Equal(100d, copy.Price, 3);

        copy.Price = 5;
        Assert.Equal(100d, item.Price, 3);
    }

    [Fact]
    public void OpenItem_CarriesIdAndName()
    {
        OpenItem item = new(10001, "木杖", new Dictionary<string, object> { { "exatk", 20 } });

        Assert.Equal(10001, item.Id);
        Assert.Equal("木杖", item.Name);
    }

    [Fact]
    public void Inventory_StartsWithNoCreditsAndFallsBackToEmptyCharacter()
    {
        User user = new(1001, "甲");
        Inventory inventory = user.Inventory;

        Assert.Equal(0d, inventory.Credits, 3);
        Assert.Equal(0d, inventory.Materials, 3);
        Assert.Empty(inventory.Characters);
        Assert.NotNull(inventory.MainCharacter);
        Assert.Equal(1001, inventory.Id);
    }

    [Fact]
    public void Inventory_MainCharacterDefaultsToFirstCharacter()
    {
        User user = new(1001, "甲");
        Character character = EntitiesHelper.NewCharacter("一号");
        user.Inventory.Characters.Add(character);

        Assert.Same(character, user.Inventory.MainCharacter);
    }

    [Fact]
    public void Inventory_TracksSquadAndTraining()
    {
        User user = new(1001, "甲");
        DateTime start = new(2026, 1, 1);

        user.Inventory.Squad.Add(11);
        user.Inventory.Training[11] = start;

        Assert.Contains(11L, user.Inventory.Squad);
        Assert.Equal(start, user.Inventory.Training[11]);
    }

    [Fact]
    public void User_CarriesProfileStatisticsAndEqualityById()
    {
        User user = new(1001, "甲", new DateTime(2026, 1, 1));

        Assert.Equal("甲", user.Name);
        Assert.Equal("甲", user.Username);
        Assert.True(user.IsEnable);
        Assert.False(user.IsAdmin);
        Assert.NotNull(user.Profile);
        Assert.NotNull(user.Statistics);
        Assert.Equal(1001, user.Inventory.Id);
    }

    [Fact]
    public void User_GuestAndLocalFactories_UseReservedIds()
    {
        User guest = new(UserType.Guest);
        User local = new(UserType.LocalUser);

        Assert.Equal(FunGame.Core.Library.Constant.UserSet.GuestUserId, guest.Id);
        Assert.Equal(FunGame.Core.Library.Constant.UserSet.LocalUserId, local.Id);
    }
}
