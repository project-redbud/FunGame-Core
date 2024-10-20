using System.Text;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

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
        public virtual string Description { get; set; } = "";

        /// <summary>
        /// 物品的通用描述
        /// </summary>
        public virtual string GeneralDescription { get; set; } = "";

        /// <summary>
        /// 物品的背景故事
        /// </summary>
        public virtual string BackgroundStory { get; set; } = "";

        /// <summary>
        /// 物品类型
        /// </summary>
        public virtual ItemType ItemType { get; set; } = ItemType.Others;

        /// <summary>
        /// 是否允许装备
        /// </summary>
        public bool Equipable { get; set; } = true;

        /// <summary>
        /// 是否允许取消装备
        /// </summary>
        public bool Unequipable { get; set; } = true;

        /// <summary>
        /// 装备槽位
        /// </summary>
        public virtual EquipSlotType EquipSlotType { get; set; } = EquipSlotType.None;

        /// <summary>
        /// 武器类型（如果是武器）
        /// </summary>
        public virtual WeaponType WeaponType { get; set; } = WeaponType.None;

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
        public void OnItemEquip(Character character, EquipItemToSlot type)
        {
            Character = character;
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
            if (Character != null) OnItemEquipped(Character, this, type);
        }

        /// <summary>
        /// 当取消装备物品时
        /// </summary>
        public void OnItemUnEquip(EquipItemToSlot type)
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
                switch (type)
                {
                    case EquipItemToSlot.MagicCardPack:
                        Character.EquipSlot.MagicCardPack = null;
                        break;
                    case EquipItemToSlot.Weapon:
                        Character.EquipSlot.Weapon = null;
                        break;
                    case EquipItemToSlot.Armor:
                        Character.EquipSlot.Armor = null;
                        break;
                    case EquipItemToSlot.Shoes:
                        Character.EquipSlot.Shoes = null;
                        break;
                    case EquipItemToSlot.Accessory1:
                        Character.EquipSlot.Accessory1 = null;
                        break;
                    case EquipItemToSlot.Accessory2:
                        Character.EquipSlot.Accessory2 = null;
                        break;
                }
                OnItemUnEquipped(Character, this, type);
            }
            Character = null;
        }

        /// <summary>
        /// 局内使用物品触发 对某个角色使用
        /// </summary>
        public void UseItem(ActionQueue queue, Character character, List<Character> enemys, List<Character> teammates)
        {
            OnItemUsed(character, this);
            Skills.Active?.OnSkillCasted(queue, character, enemys, teammates);
        }

        /// <summary>
        /// 局外（库存）使用物品触发
        /// </summary>
        public void UseItem(/*Inventory inventory*/)
        {
            if (User != null) OnItemUsed(User, this);
        }

        /// <summary>
        /// 当物品被角色使用时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        public virtual void OnItemUsed(Character character, Item item)
        {

        }

        /// <summary>
        /// 当物品被玩家使用时
        /// </summary>
        /// <param name="user"></param>
        /// <param name="item"></param>
        public virtual void OnItemUsed(User user, Item item)
        {

        }

        /// <summary>
        /// 当物品被装备时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public virtual void OnItemEquipped(Character character, Item item, EquipItemToSlot type)
        {

        }

        /// <summary>
        /// 当物品被取消装备时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public virtual void OnItemUnEquipped(Character character, Item item, EquipItemToSlot type)
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
        /// Id.Name
        /// </summary>
        /// <returns></returns>
        public string GetIdName()
        {
            return Id + "." + Name;
        }

        /// <summary>
        /// 显示物品的详细信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// 显示物品的详细信息
        /// </summary>
        /// <param name="isShowGeneralDescription">是否显示通用描述，而不是描述</param>
        /// <returns></returns>
        public string ToString(bool isShowGeneralDescription)
        {
            StringBuilder builder = new();

            builder.AppendLine($"【{Name}】");
            builder.AppendLine($"{ItemSet.GetItemTypeName(ItemType)}" + (IsPurchasable && Price > 0 ? $"    售价：{Price:0.##}" : ""));

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

            if (isShowGeneralDescription && GeneralDescription != "")
            {
                builder.AppendLine("物品描述：" + GeneralDescription);
            }
            else if (Description != "")
            {
                builder.AppendLine("物品描述：" + Description);
            }

            if (Skills.Active != null) builder.AppendLine($"{Skills.Active.ToString()}");
            foreach (Skill skill in Skills.Passives)
            {
                builder.AppendLine($"{skill.ToString()}");
            }

            if (BackgroundStory != "")
            {
                builder.AppendLine("\r\n" + BackgroundStory);
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
        /// 设置一些属性给从 <see cref="ItemModule"/> 新建来的 <paramref name="item"/><para/>
        /// 通常，在使用 JSON 反序列化 Item，且从 <see cref="ItemModule.GetItem(long, string, ItemType)"/> 中获取了实例后，需要使用此方法复制给新实例
        /// </summary>
        /// <param name="item"></param>
        public void SetPropertyToItemModuleNew(Item item)
        {
            item.WeaponType = WeaponType;
            item.EquipSlotType = EquipSlotType;
            item.Equipable = Equipable;
            item.IsPurchasable = IsPurchasable;
            item.Price = Price;
            item.IsSellable = IsSellable;
            item.NextSellableTime = NextSellableTime;
            item.IsTradable = IsTradable;
            item.NextTradableTime = NextTradableTime;
        }

        /// <summary>
        /// 复制一个物品
        /// </summary>
        /// <returns></returns>
        public Item Copy()
        {
            Item item = new()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                GeneralDescription = GeneralDescription,
                ItemType = ItemType,
                Equipable = Equipable,
                Unequipable = Unequipable,
                EquipSlotType = EquipSlotType,
                WeaponType = WeaponType,
                Key = Key,
                Enable = Enable,
                IsInGameItem = IsInGameItem,
                IsPurchasable = IsPurchasable,
                Price = Price,
                IsSellable = IsSellable,
                NextSellableTime = NextSellableTime,
                IsTradable = IsTradable,
                NextTradableTime = NextTradableTime,
                RemainUseTimes = RemainUseTimes,
            };
            item.Skills.Active = Skills.Active?.Copy();
            foreach (Skill skill in Skills.Passives)
            {
                Skill newskill = skill.Copy();
                newskill.Item = item;
                item.Skills.Passives.Add(newskill);
            }
            return item;
        }

        /// <summary>
        /// 所属的角色
        /// </summary>
        private Character? _character = null;
    }
}
