using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 和 <see cref="Skill"/> 一样，需要继承构造
    /// </summary>
    public class Item : BaseEntity, IItem
    {
        /// <summary>
        /// 物品的描述
        /// </summary>
        public string Description { get; } = "";

        /// <summary>
        /// 物品类型
        /// </summary>
        public virtual ItemType ItemType { get; set; } = ItemType.Others;

        /// <summary>
        /// 物品槽位
        /// </summary>
        public virtual EquipSlotType EquipSlotType { get; set; } = EquipSlotType.None;

        /// <summary>
        /// 物品的价格
        /// </summary>
        public double Price { get; set; } = 0;

        /// <summary>
        /// 快捷键
        /// </summary>
        public char Key { get; set; } = '/';

        /// <summary>
        /// 是否是主动物品
        /// </summary>
        public bool IsActive => Skills.Active != null;

        /// <summary>
        /// 是否可用（涉及冷却和禁用等）
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 物品所属的角色
        /// </summary>
        public Character? Character { get; set; } = null;

        /// <summary>
        /// 物品拥有的技能
        /// </summary>
        public SkillGroup Skills { get; set; } = new();

        /// <summary>
        /// 当获得物品时
        /// </summary>
        public void OnItemGained()
        {
            foreach (Skill skill in Skills.Passive)
            {
                if (!skill.IsActive && skill.Level > 0)
                {
                    foreach (Effect e in skill.AddInactiveEffectToCharacter())
                    {
                        e.ActionQueue = skill.ActionQueue;
                        if (Character != null && !Character.Effects.Contains(e))
                        {
                            Character.Effects.Add(e);
                            e.OnEffectGained(Character);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 局内使用物品触发
        /// </summary>
        public void UseItem(ActionQueue queue, List<Character> enemys, List<Character> teammates)
        {
            if (Skills.Active != null && Character != null)
            {
                Skills.Active.OnSkillCasted(queue, Character, enemys, teammates);
            }
            OnItemUsed();
        }

        /// <summary>
        /// 局外（库存）使用物品触发
        /// </summary>
        public void UseItem(/*Inventory inventory*/)
        {

            OnItemUsed();
        }

        /// <summary>
        /// 当物品被使用时
        /// </summary>
        public virtual void OnItemUsed()
        {

        }

        protected Item(ItemType type, EquipSlotType slot = EquipSlotType.None)
        {
            ItemType = type;
            EquipSlotType = slot;
        }

        internal Item() { }

        /// <summary>
        /// 判断两个物品是否相同 检查Id.Name
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is Item c && c.Id + "." + c.Name == Id + "." + Name;
        }
    }
}
