using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace FunGame.Core.Tests.Entities;

internal static class EntitiesHelper
{
    public static Character NewCharacter(string name = "角色", double hp = 100, double mp = 100, double atk = 100, double def = 0, double spd = 100)
    {
        Character character = new()
        {
            Name = name,
            InitialHP = hp,
            InitialMP = mp,
            InitialATK = atk,
            InitialDEF = def,
            InitialSPD = spd,
            InitialHR = 1,
            InitialMR = 1,
            ExActionCoefficient = -1
        };
        character.Level = 1;
        character.Recovery();
        return character;
    }

    public static Item NewWeapon(string name = "法杖", WeaponType type = WeaponType.Staff)
    {
        return new Item(ItemType.Weapon)
        {
            Name = name,
            WeaponType = type,
            EquipSlotType = EquipSlotType.Weapon
        };
    }
}
