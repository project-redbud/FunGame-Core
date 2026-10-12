using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 装备栏状态快照（槽位 + 物品 ID + 物品名字，用于回放/展示时显示装备名）
    /// </summary>
    public class EquipmentStateSnapshot
    {
        /// <summary>
        /// 装备槽位
        /// </summary>
        public EquipSlotType Slot { get; set; }

        /// <summary>
        /// 物品 ID
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// 物品名字
        /// </summary>
        public string ItemName { get; set; } = "";

        /// <summary>
        /// 物品描述
        /// </summary>
        public string Description { get; set; } = "";
    }
}
