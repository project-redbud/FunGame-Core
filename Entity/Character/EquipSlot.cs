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
        public Item? MagicCardPack { get; set; } = null;
        /// <summary>
        /// 武器
        /// </summary>
        public Item? Weapon { get; set; } = null;
        /// <summary>
        /// 防具
        /// </summary>
        public Item? Armor { get; set; } = null;
        /// <summary>
        /// 鞋子
        /// </summary>
        public Item? Shoes { get; set; } = null;
        /// <summary>
        /// 饰品1
        /// </summary>
        public Item? Accessory1 { get; set; } = null;
        /// <summary>
        /// 饰品2
        /// </summary>
        public Item? Accessory2 { get; set; } = null;
    }
}
