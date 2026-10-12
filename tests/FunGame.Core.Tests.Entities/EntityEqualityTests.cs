using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Entities;

public class EntityEqualityTests
{
    [Fact]
    public void CharacterEquality_IsBasedOnGuid()
    {
        Character first = EntitiesHelper.NewCharacter("甲");
        Character second = EntitiesHelper.NewCharacter("乙");
        Assert.NotEqual(first.Guid, second.Guid);

        second.Guid = first.Guid;

        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ItemEquality_IsBasedOnGuid()
    {
        Item first = EntitiesHelper.NewWeapon("甲");
        Item second = EntitiesHelper.NewWeapon("乙");

        Assert.False(first.Equals(second));

        second.Guid = first.Guid;

        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void UserEquality_IsBasedOnId()
    {
        User first = new(1001, "甲");
        User second = new(1002, "乙");

        Assert.False(first.Equals(second));

        second.Id = 1001;

        Assert.True(first.Equals(second));
    }

    [Fact]
    public void InventoryAndMarket_CompareByIdName()
    {
        Inventory first = new(new User(1001, "甲"));
        Inventory second = new(new User(1001, "甲"));
        second.Name = first.Name;

        Assert.True(first.Equals(second));

        second.Name = "别的库存";
        Assert.False(first.Equals(second));

        Market market = new("集市");
        Assert.False(market.Equals(new Market("别的集市")));
    }

    [Fact]
    public void GetIdName_JoinsIdAndName()
    {
        Item item = EntitiesHelper.NewWeapon("法杖");
        item.Id = 42;

        Assert.Equal("42.法杖", item.GetIdName());
    }

    [Fact]
    public void BaseEntity_DefaultsToEmptyGuidAndZeroId()
    {
        Item item = new();

        Assert.Equal(Guid.Empty, new BaseEntityStub().Guid);
        Assert.Equal(0, item.Id);
        Assert.NotEqual(Guid.Empty, item.Guid);
        Assert.Equal(EntityState.Unchanged, item.EntityState);
    }

    [Fact]
    public void OpenSkill_KeepsIdAndName()
    {
        OpenSkill skill = new(2001, "木杖", []);

        Assert.Equal(2001, skill.Id);
        Assert.Equal("木杖", skill.Name);
        Assert.Equal(SkillType.Passive, skill.SkillType);
    }

    private sealed class BaseEntityStub : BaseEntity
    {
        public override bool Equals(IBaseEntity? other) => other is BaseEntityStub;
    }
}
