using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class CharacterBuilder(long id, string name, string firstName, string nickName, PrimaryAttribute primaryAttribute, double initialATK, double initialHP, double initialMP, double initialSTR, double strGrowth, double initialAGI, double agiGrowth, double initialINT, double intGrowth, double initialSPD, double initialHR, double initialMR)
    {
        public long Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string FirstName { get; set; } = firstName;
        public string NickName { get; set; } = nickName;
        public PrimaryAttribute PrimaryAttribute { get; set; } = primaryAttribute;
        public double InitialATK { get; set; } = initialATK;
        public double InitialHP { get; set; } = initialHP;
        public double InitialMP { get; set; } = initialMP;
        public double InitialSTR { get; set; } = initialSTR;
        public double STRGrowth { get; set; } = strGrowth;
        public double InitialAGI { get; set; } = initialAGI;
        public double AGIGrowth { get; set; } = agiGrowth;
        public double InitialINT { get; set; } = initialINT;
        public double INTGrowth { get; set; } = intGrowth;
        public double InitialSPD { get; set; } = initialSPD;
        public double InitialHR { get; set; } = initialHR;
        public double InitialMR { get; set; } = initialMR;

        public Character Build(int level, IEnumerable<Skill> skills, IEnumerable<Item> items, EquipSlot? equips = null)
        {
            Character character = Factory.GetCharacter();
            character.Id = Id;
            character.Name = Name;
            character.FirstName = FirstName;
            character.NickName = NickName;
            character.PrimaryAttribute = PrimaryAttribute;
            character.InitialATK = InitialATK;
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
            if (level > 1)
            {
                character.Level = level;
            }
            foreach (Skill skill in skills)
            {
                Skill newskill = skill.Copy();
                newskill.Character = character;
                newskill.Level = skill.Level;
                newskill.CurrentCD = 0;
                character.Skills.Add(newskill);
            }
            foreach (Item item in items)
            {
                Item newitem = item.Copy(true);
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
                if (mcp != null) character.Equip(mcp);
                if (w != null) character.Equip(w);
                if (a != null) character.Equip(a);
                if (s != null) character.Equip(s);
                if (ac1 != null) character.Equip(ac1);
                if (ac2 != null) character.Equip(ac2);
            }
            return character;
        }
    }
}
