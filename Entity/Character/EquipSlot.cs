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
        /// 上一次装备的饰品槽
        /// </summary>
        public EquipSlotType LastEquipSlotType { get; internal set; } = EquipSlotType.Accessory1;

        /// <summary>
        /// 是否有任意装备
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return MagicCardPack != null || Weapon != null || Armor != null || Shoes != null || Accessory1 != null || Accessory2 != null;
        }
    }
}
