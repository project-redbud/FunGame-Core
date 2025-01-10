using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class CharacterBuilder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string NickName { get; set; }
        public PrimaryAttribute PrimaryAttribute { get; set; }
        public double InitialATK { get; set; }
        public double InitialDEF { get; set; }
        public double InitialHP { get; set; }
        public double InitialMP { get; set; }
        public double InitialSTR { get; set; }
        public double STRGrowth { get; set; }
        public double InitialAGI { get; set; }
        public double AGIGrowth { get; set; }
        public double InitialINT { get; set; }
        public double INTGrowth { get; set; }
        public double InitialSPD { get; set; }
        public double InitialHR { get; set; }
        public double InitialMR { get; set; }

        /// <summary>
        /// 基于初始属性创建角色构建器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="firstName"></param>
        /// <param name="nickName"></param>
        /// <param name="primaryAttribute"></param>
        /// <param name="initialATK"></param>
        /// <param name="initialDEF"></param>
        /// <param name="initialHP"></param>
        /// <param name="initialMP"></param>
        /// <param name="initialSTR"></param>
        /// <param name="strGrowth"></param>
        /// <param name="initialAGI"></param>
        /// <param name="agiGrowth"></param>
        /// <param name="initialINT"></param>
        /// <param name="intGrowth"></param>
        /// <param name="initialSPD"></param>
        /// <param name="initialHR"></param>
        /// <param name="initialMR"></param>
        public CharacterBuilder(long id, string name, string firstName, string nickName, PrimaryAttribute primaryAttribute, double initialATK, double initialDEF, double initialHP, double initialMP, double initialSTR, double strGrowth, double initialAGI, double agiGrowth, double initialINT, double intGrowth, double initialSPD, double initialHR, double initialMR)
        {
            Id = id;
            Name = name;
            FirstName = firstName;
            NickName = nickName;
            PrimaryAttribute = primaryAttribute;
            InitialATK = initialATK;
            InitialDEF = initialDEF;
            InitialHP = initialHP;
            InitialMP = initialMP;
            InitialSTR = initialSTR;
            STRGrowth = strGrowth;
            InitialAGI = initialAGI;
            AGIGrowth = agiGrowth;
            InitialINT = initialINT;
            INTGrowth = intGrowth;
            InitialSPD = initialSPD;
            InitialHR = initialHR;
            InitialMR = initialMR;
        }
        
        /// <summary>
        /// 基于模板角色创建角色构建器
        /// </summary>
        /// <param name="character"></param>
        public CharacterBuilder(Character character)
        {
            Id = character.Id;
            Name = character.Name;
            FirstName = character.FirstName;
            NickName = character.NickName;
            PrimaryAttribute = character.PrimaryAttribute;
            InitialATK = character.InitialATK;
            InitialDEF = character.InitialDEF;
            InitialHP = character.InitialHP;
            InitialMP = character.InitialMP;
            InitialSTR = character.InitialSTR;
            STRGrowth = character.STRGrowth;
            InitialAGI = character.InitialAGI;
            AGIGrowth = character.AGIGrowth;
            InitialINT = character.InitialINT;
            INTGrowth = character.INTGrowth;
            InitialSPD = character.InitialSPD;
            InitialHR = character.InitialHR;
            InitialMR = character.InitialMR;
        }

        /// <summary>
        /// 基于初始条件构建新的角色
        /// <para>需要传入技能、物品，可选构建装备</para>
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <param name="newItemGuid"></param>
        /// <param name="equips"></param>
        /// <param name="inventory"></param>
        /// <param name="itemsDefined"></param>
        /// <param name="skillsDefined"></param>
        /// <returns>构建的新角色</returns>
        public Character Build(IEnumerable<Skill> skills, IEnumerable<Item> items, bool newItemGuid = true, EquipSlot? equips = null, Inventory? inventory = null, IEnumerable<Item>? itemsDefined = null, IEnumerable<Skill>? skillsDefined = null)
        {
            Character character = Factory.GetCharacter();
            character.Id = Id;
            character.Name = Name;
            character.FirstName = FirstName;
            character.NickName = NickName;
            character.PrimaryAttribute = PrimaryAttribute;
            character.InitialATK = InitialATK;
            character.InitialDEF = InitialDEF;
            character.InitialHP = InitialHP;
            character.InitialMP = InitialMP;
            character.InitialSTR = InitialSTR;
            character.STRGrowth = STRGrowth;
            character.InitialAGI = InitialAGI;
            character.AGIGrowth = AGIGrowth;
            character.InitialINT = InitialINT;
            character.INTGrowth = INTGrowth;
            character.InitialSPD = InitialSPD;
            character.InitialHR = InitialHR;
            character.InitialMR = InitialMR;
            foreach (Skill skill in skills)
            {
                // 主动技能的Guid表示与其关联的物品
                if (skill.Guid == Guid.Empty)
                {
                    Skill newskill = skill.Copy(true, skillsDefined);
                    newskill.Character = character;
                    newskill.Level = skill.Level;
                    if (skill.CurrentCD > 0 && !skill.Enable)
                    {
                        newskill.Enable = true;
                        newskill.CurrentCD = 0;
                    }
                    character.Skills.Add(newskill);
                }
            }
            foreach (Item item in items)
            {
                Item newitem;
                if (itemsDefined != null && itemsDefined.FirstOrDefault(i => i.GetIdName() == item.GetIdName()) is Item itemDefined)
                {
                    newitem = itemDefined.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                    item.SetPropertyToItemModuleNew(newitem);
                }
                else
                {
                    newitem = item.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                }
                newitem.Character = character;
                character.Items.Add(newitem);
            }
            if (equips != null)
            {
                Item? mcp = equips.MagicCardPack;
                Item? w = equips.Weapon;
                Item? a = equips.Armor;
                Item? s = equips.Shoes;
                Item? ac1 = equips.Accessory1;
                Item? ac2 = equips.Accessory2;
                if (inventory != null)
                {
                    if (inventory.Items.FirstOrDefault(i => i.Guid == mcp?.Guid) is Item newmcp)
                    {
                        character.Equip(newmcp);
                    }
                    if (inventory.Items.FirstOrDefault(i => i.Guid == w?.Guid) is Item neww)
                    {
                        character.Equip(neww);
                    }
                    if (inventory.Items.FirstOrDefault(i => i.Guid == a?.Guid) is Item newa)
                    {
                        character.Equip(newa);
                    }
                    if (inventory.Items.FirstOrDefault(i => i.Guid == s?.Guid) is Item news)
                    {
                        character.Equip(news);
                    }
                    if (inventory.Items.FirstOrDefault(i => i.Guid == ac1?.Guid) is Item newac1)
                    {
                        character.Equip(newac1);
                    }
                    if (inventory.Items.FirstOrDefault(i => i.Guid == ac2?.Guid) is Item newac2)
                    {
                        character.Equip(newac2);
                    }
                }
                else
                {
                    if (mcp != null)
                    {
                        mcp = mcp.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                        character.Equip(mcp);
                    }
                    if (w != null)
                    {
                        w = w.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                        character.Equip(w);
                    }
                    if (a != null)
                    {
                        a = a.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                        character.Equip(a);
                    }
                    if (s != null)
                    {
                        s = s.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                        character.Equip(s);
                    }
                    if (ac1 != null)
                    {
                        ac1 = ac1.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                        character.Equip(ac1);
                    }
                    if (ac2 != null)
                    {
                        ac2 = ac2.Copy(true, !newItemGuid, true, itemsDefined, skillsDefined);
                        character.Equip(ac2);
                    }
                }
            }
            character.Recovery();
            return character;
        }

        /// <summary>
        /// 使用 <paramref name="reference"/> 角色构建
        /// <para>新角色将获得参考角色的等级、技能、装备和身上的物品</para>
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="newItemGuid"></param>
        /// <param name="copyLevel"></param>
        /// <param name="inventory"></param>
        /// <param name="itemsDefined">对于动态扩展的物品而言，传入已定义的物品表，不使用被复制物品的数据</param>
        /// <param name="skillsDefined">对于动态扩展的技能而言，传入已定义的技能表，不使用被复制技能的数据</param>
        /// <param name="recovery"></param>
        /// <returns>构建的新角色</returns>
        public static Character Build(Character reference, bool newItemGuid = true, bool copyLevel = true, Inventory? inventory = null, IEnumerable<Item>? itemsDefined = null, IEnumerable<Skill>? skillsDefined = null, bool recovery = true)
        {
            Character character = new CharacterBuilder(reference).Build(reference.Skills, reference.Items, newItemGuid, reference.EquipSlot, inventory, itemsDefined, skillsDefined);
            if (copyLevel)
            {
                character.Level = reference.Level;
                character.LevelBreak = reference.LevelBreak;
                character.EXP = reference.EXP;
            }
            character.NormalAttack.Level = reference.NormalAttack.Level;
            character.NormalAttack.HardnessTime = reference.NormalAttack.HardnessTime;
            character.NormalAttack.SetMagicType(reference.NormalAttack.IsMagic, reference.NormalAttack.MagicType);
            if (!recovery)
            {
                character.HP = reference.HP;
                character.MP = reference.MP;
            }
            return character;
        }
    }
}
