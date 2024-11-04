using System.Net.NetworkInformation;
using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
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
        /// 当前装备的槽位
        /// </summary>
        public virtual EquipSlotType EquipSlotType { get; set; } = EquipSlotType.None;

        /// <summary>
        /// 武器类型（如果是武器）
        /// </summary>
        public virtual WeaponType WeaponType { get; set; } = WeaponType.None;

        /// <summary>
        /// 品质类型
        /// </summary>
        public virtual QualityType QualityType { get; set; } = QualityType.White;

        /// <summary>
        /// 稀有度类型
        /// </summary>
        public virtual RarityType RarityType { get; set; } = RarityType.OneStar;

        /// <summary>
        /// 物品评级
        /// </summary>
        public virtual ItemRankType RankType { get; set; } = ItemRankType.D;

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
        public void OnItemEquip(Character character, EquipSlotType type)
        {
            Character = character;
            EquipSlotType = type;
            foreach (Skill skill in Skills.Passives)
            {
                if (!skill.IsActive && skill.Level > 0)
                {
                    foreach (Effect e in skill.AddInactiveEffectToCharacter())
                    {
                        e.GamingQueue = skill.GamingQueue;
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
        public void OnItemUnEquip(EquipSlotType type)
        {
            if (Character != null)
            {
                if (Skills.Active != null)
                {
                    List<Effect> effects = Character.Effects.Where(e => e.Skill == Skills.Active && e.Level > 0).ToList();
                    foreach (Effect e in effects)
                    {
                        Character.Effects.Remove(e);
                        e.OnEffectLost(Character);
                    }
                }
                foreach (Skill skill in Skills.Passives)
                {
                    List<Effect> effects = Character.Effects.Where(e => e.Skill == skill && e.Level > 0).ToList();
                    foreach (Effect e in effects)
                    {
                        Character.Effects.Remove(e);
                        e.OnEffectLost(Character);
                    }
                }
                switch (type)
                {
                    case EquipSlotType.MagicCardPack:
                        Character.EquipSlot.MagicCardPack = null;
                        break;
                    case EquipSlotType.Weapon:
                        Character.EquipSlot.Weapon = null;
                        break;
                    case EquipSlotType.Armor:
                        Character.EquipSlot.Armor = null;
                        break;
                    case EquipSlotType.Shoes:
                        Character.EquipSlot.Shoes = null;
                        break;
                    case EquipSlotType.Accessory1:
                        Character.EquipSlot.Accessory1 = null;
                        break;
                    case EquipSlotType.Accessory2:
                        Character.EquipSlot.Accessory2 = null;
                        break;
                }
                OnItemUnEquipped(Character, this, type);
            }
            Character = null;
            EquipSlotType = EquipSlotType.None;
        }

        /// <summary>
        /// 设置游戏内的行动顺序表实例
        /// </summary>
        /// <param name="queue"></param>
        public void SetGamingQueue(IGamingQueue queue)
        {
            if (Skills.Active != null) Skills.Active.GamingQueue = queue;
            foreach (Skill skill in Skills.Passives)
            {
                skill.GamingQueue = queue;
            }
        }

        /// <summary>
        /// 局内使用物品触发 对某个角色使用
        /// </summary>
        public void UseItem(IGamingQueue queue, Character character, List<Character> enemys, List<Character> teammates)
        {
            bool cancel = false;
            bool used = false;
            if (Skills.Active != null)
            {
                Skill skill = Skills.Active;
                List<Character> targets = queue.SelectTargets(character, skill, enemys, teammates, out cancel);
                if (!cancel)
                {
                    skill.OnSkillCasted(queue, character, targets);
                    used = true;
                }
            }
            OnItemUsed(character, this, cancel, used);
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
        /// <param name="cancel"></param>
        /// <param name="used"></param>
        public virtual void OnItemUsed(Character character, Item item, bool cancel, bool used)
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
        public virtual void OnItemEquipped(Character character, Item item, EquipSlotType type)
        {

        }

        /// <summary>
        /// 当物品被取消装备时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public virtual void OnItemUnEquipped(Character character, Item item, EquipSlotType type)
        {

        }


        protected Item(ItemType type, bool isInGame = true)
        {
            ItemType = type;
            IsInGameItem = isInGame;
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
            builder.AppendLine($"{ItemSet.GetItemTypeName(ItemType) + (ItemType == ItemType.Weapon && WeaponType != WeaponType.None ? "-" + ItemSet.GetWeaponTypeName(WeaponType) : "")}" + (IsPurchasable && Price > 0 ? $"  售价：{Price}" : ""));

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
                builder.Append($"{skill.ToString()}");
            }

            if (BackgroundStory != "")
            {
                builder.AppendLine($"\"{BackgroundStory}\"");
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
        /// 设置一些属性给从工厂构造出来的 <paramref name="newbyFactory"/> 对象
        /// </summary>
        /// <param name="newbyFactory"></param>
        public void SetPropertyToItemModuleNew(Item newbyFactory)
        {
            newbyFactory.WeaponType = WeaponType;
            newbyFactory.EquipSlotType = EquipSlotType;
            newbyFactory.Equipable = Equipable;
            newbyFactory.IsPurchasable = IsPurchasable;
            newbyFactory.Price = Price;
            newbyFactory.IsSellable = IsSellable;
            newbyFactory.NextSellableTime = NextSellableTime;
            newbyFactory.IsTradable = IsTradable;
            newbyFactory.NextTradableTime = NextTradableTime;
        }

        /// <summary>
        /// 复制一个物品
        /// </summary>
        /// <returns></returns>
        public Item Copy(bool copyLevel = false)
        {
            Item item = Factory.OpenFactory.GetInstance<Item>(Id, Name, []);
            SetPropertyToItemModuleNew(item);
            item.Id = Id;
            item.Name = Name;
            item.Description = Description;
            item.GeneralDescription = GeneralDescription;
            item.BackgroundStory = BackgroundStory;
            item.ItemType = ItemType;
            item.Equipable = Equipable;
            item.Unequipable = Unequipable;
            item.WeaponType = WeaponType;
            item.QualityType = QualityType;
            item.RarityType = RarityType;
            item.RankType = RankType;
            item.Key = Key;
            item.Enable = Enable;
            item.IsInGameItem = IsInGameItem;
            item.IsPurchasable = IsPurchasable;
            item.Price = Price;
            item.IsSellable = IsSellable;
            item.NextSellableTime = NextSellableTime;
            item.IsTradable = IsTradable;
            item.NextTradableTime = NextTradableTime;
            item.RemainUseTimes = RemainUseTimes;
            item.Skills.Active = Skills.Active?.Copy();
            if (item.Skills.Active != null && copyLevel)
            {
                item.Skills.Active.Level = Skills.Active?.Level ?? 0;
            }
            foreach (Skill skill in Skills.Passives)
            {
                Skill newskill = skill.Copy();
                newskill.Item = item;
                newskill.Level = copyLevel ? skill.Level : 0;
                item.Skills.Passives.Add(newskill);
            }
            return item;
        }

        /// <summary>
        /// 设置所有技能的等级
        /// </summary>
        /// <param name="level"></param>
        public void SetLevel(int level)
        {
            if (Skills.Active != null)
            {
                Skills.Active.Level = level;
            }
            foreach (Skill skill in Skills.Passives)
            {
                skill.Level = level;
            }
        }

        /// <summary>
        /// 所属的角色
        /// </summary>
        private Character? _character = null;
    }
}
