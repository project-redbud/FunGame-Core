using System.Text;
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
        public virtual string Description { get; } = "";

        /// <summary>
        /// 物品的通用描述
        /// </summary>
        public virtual string GeneralDescription { get; } = "";

        /// <summary>
        /// 物品类型
        /// </summary>
        public virtual ItemType ItemType { get; set; } = ItemType.Others;

        /// <summary>
        /// 物品槽位
        /// </summary>
        public virtual EquipSlotType EquipSlotType { get; set; } = EquipSlotType.None;

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
        /// 是否允许装备
        /// </summary>
        public bool Equipable { get; set; } = true;
        
        /// <summary>
        /// 是否允许取消装备
        /// </summary>
        public bool Unequipable { get; set; } = true;

        /// <summary>
        /// 是否是局内使用的物品（局内是指对角色生效的物品）
        /// </summary>
        public bool IsInGameItem { get; set; } = true;

        /// <summary>
        /// 是否允许购买
        /// </summary>
        public bool IsPurchasable { get; set; } = true;

        /// <summary>
        /// 物品的价格
        /// </summary>
        public double Price { get; set; } = 0;

        /// <summary>
        /// 是否允许出售
        /// </summary>
        public bool IsSellable { get; set; } = true;

        /// <summary>
        /// 下次可出售的时间
        /// </summary>
        public DateTime NextSellableTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 是否允许交易
        /// </summary>
        public bool IsTradable { get; set; } = true;
        
        /// <summary>
        /// 下次可交易的时间
        /// </summary>
        public DateTime NextTradableTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 剩余使用次数（消耗品才会用到）
        /// </summary>
        public int RemainUseTimes { get; set; } = 0;

        /// <summary>
        /// 物品所属的角色（只有装备物品，才需要设置）
        /// </summary>
        public Character? Character
        {
            get => _character;
            set
            {
                _character = value;
                if (Skills.Active != null) Skills.Active.Character = _character;
                foreach (Skill skill in Skills.Passives)
                {
                    skill.Character = _character;
                    foreach (Effect e in skill.Effects)
                    {
                        e.Source = _character;
                    }
                }
            }
        }

        /// <summary>
        /// 所属的玩家
        /// </summary>
        public User? User { get; set; } = null;

        /// <summary>
        /// 物品拥有的技能
        /// </summary>
        public SkillGroup Skills { get; set; } = new();

        /// <summary>
        /// 当装备物品时
        /// </summary>
        public void OnItemEquip(Character character)
        {
            Character = character;
            Character.Items.Add(this);
            foreach (Skill skill in Skills.Passives)
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
        /// 当取消装备物品时
        /// </summary>
        public void OnItemUnequip()
        {
            if (Character != null)
            {
                if (Skills.Active != null)
                {
                    foreach (Effect e in Character.Effects.Where(e => e.Skill == Skills.Active && e.Level > 0).ToList())
                    {
                        Character.Effects.Remove(e);
                        e.OnEffectLost(Character);
                    }
                }
                foreach (Skill skill in Skills.Passives)
                {
                    foreach (Effect e in Character.Effects.Where(e => e.Skill == skill && e.Level > 0).ToList())
                    {
                        Character.Effects.Remove(e);
                        e.OnEffectLost(Character);
                    }
                }
                Character.Items.Remove(this);
            }
            Character = null;
        }

        /// <summary>
        /// 局内使用物品触发 对某个角色使用
        /// </summary>
        public void UseItem(ActionQueue queue, Character character, List<Character> enemys, List<Character> teammates)
        {
            if (Skills.Active != null)
            {
                Skills.Active.OnSkillCasted(queue, character, enemys, teammates);
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

        protected Item(ItemType type, bool isInGame = true, EquipSlotType slot = EquipSlotType.None)
        {
            ItemType = type;
            IsInGameItem = isInGame;
            EquipSlotType = slot;
        }

        internal Item() { }

        /// <summary>
        /// 显示物品的详细信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"【{Name}】");
            builder.AppendLine($"{ItemSet.GetItemTypeName(ItemType)}" + (IsPurchasable && Price > 0 ? $"    售价：{Price}" : ""));

            if (RemainUseTimes > 0)
            {
                builder.AppendLine($"剩余可用次数：{RemainUseTimes}");
            }

            List<string> sellandtrade = [""];
            if (IsSellable)
            {
                sellandtrade.Add("可出售");
            }
            if (IsTradable)
            {
                sellandtrade.Add("可交易");
            }
            builder.AppendLine(string.Join(" ", sellandtrade).Trim());

            if (!IsSellable && NextSellableTime != DateTime.MinValue)
            {
                builder.AppendLine($"此物品将在 {NextSellableTime.ToString(General.GeneralDateTimeFormatChinese)} 后可出售");
            }
            
            if (!IsTradable && NextTradableTime != DateTime.MinValue)
            {
                builder.AppendLine($"此物品将在 {NextTradableTime.ToString(General.GeneralDateTimeFormatChinese)} 后可交易");
            }

            if (Skills.Active != null) builder.AppendLine($"{Skills.Active.ToString()}");
            foreach (Skill skill in Skills.Passives)
            {
                builder.AppendLine($"{skill.ToString()}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 判断两个物品是否相同 检查Id.Name
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is Item c && c.Id + "." + c.Name == Id + "." + Name;
        }

        /// <summary>
        /// 所属的角色
        /// </summary>
        private Character? _character = null;
    }
}
