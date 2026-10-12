using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Entities;

public class CharacterProgressionTests
{
    [Fact]
    public void Level_IsClampedToConfiguredRange()
    {
        Character character = EntitiesHelper.NewCharacter();

        character.Level = 0;
        Assert.Equal(1, character.Level);

        character.Level = 9999;
        Assert.Equal(character.GameplayEquilibriumConstant.MaxLevel, character.Level);
    }

    [Fact]
    public void LevelChange_RefillsHpAndMp()
    {
        Character character = EntitiesHelper.NewCharacter(hp: 100, mp: 100);
        character.HP -= 40;
        character.MP -= 30;

        character.Level = 3;

        Assert.Equal(character.MaxHP, character.HP, 3);
        Assert.Equal(character.MaxMP, character.MP, 3);
    }

    [Fact]
    public void Recovery_ByTime_AddsRecoveryRate()
    {
        Character character = EntitiesHelper.NewCharacter(hp: 100, mp: 100);
        Assert.Equal(1d, character.HR, 3);
        Assert.Equal(1d, character.MR, 3);
        character.HP -= 50;
        character.MP -= 50;

        character.Recovery(10);

        Assert.Equal(character.MaxHP - 40, character.HP, 3);
        Assert.Equal(character.MaxMP - 40, character.MP, 3);
    }

    [Fact]
    public void Recovery_ByPastValues_KeepsHpRatio()
    {
        Character character = EntitiesHelper.NewCharacter(hp: 100);
        character.HP = 40;
        double pastMax = character.MaxHP;

        character.Recovery(40, character.MaxMP, pastMax, character.MaxMP);

        Assert.Equal(character.MaxHP * .4, character.HP, 3);
    }

    [Fact]
    public void Copy_RefillsHpAndProducesIndependentCharacter()
    {
        Character character = EntitiesHelper.NewCharacter("原型");
        character.Level = 5;
        character.HP -= 10;

        Character copy = character.Copy();

        Assert.NotSame(character, copy);
        Assert.Equal(character.Name, copy.Name);
        Assert.Equal(character.Level, copy.Level);
        Assert.Equal(character.MaxHP, copy.MaxHP, 3);
        Assert.Equal(copy.MaxHP, copy.HP, 3);
        Assert.True(character.HP < copy.HP);

        copy.HP = 1;
        Assert.NotEqual(1d, character.HP, 3);
    }

    [Fact]
    public void Equip_PutsItemIntoMatchingSlotAndReturnsPrevious()
    {
        Character character = EntitiesHelper.NewCharacter();
        Item staff = EntitiesHelper.NewWeapon("法杖");
        Item sword = EntitiesHelper.NewWeapon("单手剑", WeaponType.OneHandedSword);

        Assert.True(character.Equip(staff, out Item? first));
        Assert.Null(first);
        Assert.Same(staff, character.EquipSlot.Weapon);
        Assert.True(character.EquipSlot.Any());

        Assert.True(character.Equip(sword, out Item? previous));
        Assert.Same(staff, previous);
        Assert.Same(sword, character.EquipSlot.Weapon);
    }

    [Fact]
    public void Equip_AutoSelectsSlotFromItemType()
    {
        Character character = EntitiesHelper.NewCharacter();
        Item armor = new(ItemType.Armor) { Name = "布甲", EquipSlotType = EquipSlotType.Armor };

        Assert.True(character.Equip(armor));
        Assert.Same(armor, character.EquipSlot.Armor);
    }

    [Fact]
    public void UnEquip_RemovesItemFromSlot()
    {
        Character character = EntitiesHelper.NewCharacter();
        Item weapon = EntitiesHelper.NewWeapon();
        character.Equip(weapon, out _);

        Item? removed = character.UnEquip(EquipSlotType.Weapon);

        Assert.Same(weapon, removed);
        Assert.Null(character.EquipSlot.Weapon);
        Assert.False(character.EquipSlot.Any());
    }

    [Fact]
    public void NormalAttack_PicksUpWeaponHardnessAndDamage()
    {
        Character character = EntitiesHelper.NewCharacter(atk: 100);
        double bareDamage = character.NormalAttack.Damage;
        double bareHardness = character.NormalAttack.HardnessTime;

        Item staff = EntitiesHelper.NewWeapon("法杖", WeaponType.Staff);
        character.Equip(staff, out _);

        Assert.NotEqual(bareDamage, character.NormalAttack.Damage, 3);
        Assert.NotEqual(bareHardness, character.NormalAttack.HardnessTime, 3);
        Assert.Equal(character.ATK * character.GameplayEquilibriumConstant.StaffBaseMultiplier, character.NormalAttack.Damage, 3);
        Assert.Equal(character.GameplayEquilibriumConstant.StaffHardness, character.NormalAttack.HardnessTime, 3);
    }
}
