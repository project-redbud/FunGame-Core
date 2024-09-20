using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的装备槽位
    /// </summary>
    public class EquipSlot()
    {
        /// <summary>
        /// 魔法卡包
        /// </summary>
        public Item? MagicCardPack { get; internal set; } = null;
        /// <summary>
        /// 武器
        /// </summary>
        public Item? Weapon { get; internal set; } = null;
        /// <summary>
        /// 防具
        /// </summary>
        public Item? Armor { get; internal set; } = null;
        /// <summary>
        /// 鞋子
        /// </summary>
        public Item? Shoes { get; internal set; } = null;
        /// <summary>
        /// 饰品1
        /// </summary>
        public Item? Accessory1 { get; internal set; } = null;
        /// <summary>
        /// 饰品2
        /// </summary>
        public Item? Accessory2 { get; internal set; } = null;

        /// <summary>
        /// 是否有任意装备
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return MagicCardPack != null || Weapon != null || Armor != null || Shoes != null || Accessory1 != null || Accessory2 != null;
        }

        /// <summary>
        /// 获取物品所装备的栏位
        /// </summary>
        /// <returns></returns>
        public EquipItemToSlot GetEquipItemToSlot(Item item)
        {
            if (MagicCardPack == item)
            {
                return EquipItemToSlot.MagicCardPack;
            }
            else if (Weapon == item)
            {
                return EquipItemToSlot.Weapon;
            }
            else if (Armor == item)
            {
                return EquipItemToSlot.Armor;
            }
            else if (Shoes == item)
            {
                return EquipItemToSlot.Shoes;
            }
            else if (Accessory1 == item)
            {
                return EquipItemToSlot.Accessory1;
            }
            else if (Accessory2 == item)
            {
                return EquipItemToSlot.Accessory2;
            }
            return EquipItemToSlot.None;
        }
    }
}
