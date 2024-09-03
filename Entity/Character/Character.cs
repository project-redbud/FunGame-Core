using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Character : BaseEntity, ICopyable<Character>
    {
        /// <summary>
        /// 角色的姓
        /// </summary>
        public override string Name { get; set; } = "";

        /// <summary>
        /// 角色的名字
        /// </summary>
        public string FirstName { get; set; } = "";

        /// <summary>
        /// 角色的昵称
        /// </summary>
        public string NickName { get; set; } = "";

        /// <summary>
        /// 角色所属的玩家
        /// </summary>
        public User? User { get; set; } = null;

        /// <summary>
        /// 角色统计数据
        /// </summary>
        public CharacterStatistics? Statistics { get; set; } = null;

        /// <summary>
        /// 魔法属性
        /// </summary>
        public MagicType MagicType { get; set; } = MagicType.Particle;

        /// <summary>
        /// 角色定位1
        /// </summary>
        public RoleType FirstRoleType { get; set; } = RoleType.Core;

        /// <summary>
        /// 角色定位2
        /// </summary>
        public RoleType SecondRoleType { get; set; } = RoleType.Guardian;

        /// <summary>
        /// 角色定位3
        /// </summary>
        public RoleType ThirdRoleType { get; set; } = RoleType.Vanguard;

        /// <summary>
        /// 角色评级
        /// </summary>
        public RoleRating RoleRating { get; set; } = RoleRating.E;

        /// <summary>
        /// 晋升点数
        /// </summary>
        public int Promotion { get; set; } = 0;

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 经验值
        /// </summary>
        public double EXP { get; set; } = 0;

        /// <summary>
        /// 基础生命值
        /// </summary>
        public double BaseHP { get; set; } = 1;

        /// <summary>
        /// 生命值
        /// </summary>
        public double HP { get; set; } = 0;

        /// <summary>
        /// 基础魔法值
        /// </summary>
        public double BaseMP { get; set; } = 0;

        /// <summary>
        /// 魔法值
        /// </summary>
        public double MP { get; set; } = 0;

        /// <summary>
        /// 爆发能量
        /// </summary>
        public double EP { get; set; } = 0;

        /// <summary>
        /// 基础攻击力
        /// </summary>
        public double BaseATK { get; set; } = 1;

        /// <summary>
        /// 攻击力
        /// </summary>
        public double ATK { get; set; } = 0;

        /// <summary>
        /// 基础物理护甲
        /// </summary>
        public double BaseDEF { get; set; } = 5;

        /// <summary>
        /// 物理护甲
        /// </summary>
        public double DEF { get; set; } = 0;

        /// <summary>
        /// 物理伤害减免(%)
        /// </summary>
        public double PDR
        {
            get
            {
                double value = Math.Round((BaseDEF + DEF) / BaseDEF + DEF + 300, 4, MidpointRounding.AwayFromZero) + ExPDR;
                return value > 1 ? 1 : value;
            }
        }

        /// <summary>
        /// 额外物理伤害减免(%)
        /// </summary>
        public double ExPDR { get; set; } = 0;

        /// <summary>
        /// 魔法抗性(%)
        /// </summary>
        public double MDF { get; set; } = 0;

        /// <summary>
        /// 物理穿透(%)
        /// </summary>
        public double PhysicalPenetration { get; set; } = 0;

        /// <summary>
        /// 魔法穿透(%)
        /// </summary>
        public double MagicalPenetration { get; set; } = 0;

        /// <summary>
        /// 生命回复力
        /// </summary>
        public double HR { get; set; } = 0;

        /// <summary>
        /// 魔法回复力
        /// </summary>
        public double MR { get; set; } = 0;

        /// <summary>
        /// 能量回复力
        /// </summary>
        public double ER { get; set; } = 0;

        /// <summary>
        /// 基础力量
        /// </summary>
        public double BaseSTR { get; set; } = 0;

        /// <summary>
        /// 基础敏捷
        /// </summary>
        public double BaseAGI { get; set; } = 0;

        /// <summary>
        /// 基础智力
        /// </summary>
        public double BaseINT { get; set; } = 0;

        /// <summary>
        /// 力量
        /// </summary>
        public double STR { get; set; } = 0;

        /// <summary>
        /// 敏捷
        /// </summary>
        public double AGI { get; set; } = 0;

        /// <summary>
        /// 智力
        /// </summary>
        public double INT { get; set; } = 0;

        /// <summary>
        /// 力量成长值
        /// </summary>
        public double STRGrowth { get; set; } = 0;

        /// <summary>
        /// 敏捷成长值
        /// </summary>
        public double AGIGrowth { get; set; } = 0;

        /// <summary>
        /// 智力成长值
        /// </summary>
        public double INTGrowth { get; set; } = 0;

        /// <summary>
        /// 行动速度
        /// </summary>
        public double SPD { get; set; } = 0;

        /// <summary>
        /// 行动系数(%)
        /// </summary>
        public double ActionCoefficient
        {
            get
            {
                double value = Math.Round(SPD / 1500.00, 4, MidpointRounding.AwayFromZero) + ExActionCoefficient;
                return value > 1 ? 1 : value;
            }
        }

        /// <summary>
        /// 额外行动系数(%)
        /// </summary>
        public double ExActionCoefficient { get; set; } = 0;

        /// <summary>
        /// 加速系数(%)
        /// </summary>
        public double AccelerationCoefficient { get; set; } = 0;
        
        /// <summary>
        /// 冷却缩减(%)
        /// </summary>
        public double CDR { get; set; } = 0;

        /// <summary>
        /// 攻击距离
        /// </summary>
        public double ATR { get; set; } = 0;

        /// <summary>
        /// 暴击率(%)
        /// </summary>
        public double CritRate { get; set; } = 0.05;

        /// <summary>
        /// 暴击伤害(%)
        /// </summary>
        public double CritDMG { get; set; } = 1.25;

        /// <summary>
        /// 闪避率(%)
        /// </summary>
        public double EvadeRate { get; set; } = 0.05;

        /// <summary>
        /// 角色的技能组
        /// </summary>
        public Dictionary<string, Skill> Skills { get; set; } = [];

        /// <summary>
        /// 角色携带的物品
        /// </summary>
        public Dictionary<string, Item> Items { get; set; } = [];

        protected Character()
        {

        }

        internal static Character GetInstance()
        {
            return new();
        }

        public void Init()
        {
            STR = BaseSTR;
            AGI = BaseAGI;
            INT = BaseINT;
            HP = BaseHP;
            MP = BaseMP;
            ATK = BaseATK;
            DEF = BaseDEF;
            // STR
            HP = Math.Round(HP + STR * 17, 2, MidpointRounding.AwayFromZero);
            HR = Math.Round(HR + STR * 0.7, 2, MidpointRounding.AwayFromZero);
            DEF = Math.Round(DEF + STR * 0.75, 2, MidpointRounding.AwayFromZero);
            CritDMG = Math.Round(CritDMG + STR * 0.00575, 4, MidpointRounding.AwayFromZero);
            // AGI
            SPD = Math.Round(SPD + AGI * 0.65, 2, MidpointRounding.AwayFromZero);
            EvadeRate = Math.Round(EvadeRate + AGI * 0.0025, 4, MidpointRounding.AwayFromZero);
            CritRate = Math.Round(CritRate + AGI * 0.00235, 4, MidpointRounding.AwayFromZero);
            // INT
            MP = Math.Round(MP + INT * 8, 2, MidpointRounding.AwayFromZero);
            MR = Math.Round(MR + INT * 0.4, 2, MidpointRounding.AwayFromZero);
            CDR = Math.Round(CDR + INT * 0.0025, 4, MidpointRounding.AwayFromZero);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Character c && c.Name == Name;
        }

        public override string ToString()
        {
            bool isChineseName = NetworkUtility.IsChineseName(Name + FirstName);
            string str = isChineseName ? (Name + FirstName).Trim() : (Name + " " + FirstName).Trim();
            if (NickName != "")
            {
                if (str != "") str += ", ";
                str += NickName;
            }
            if (User != null && User.Username != "")
            {
                str += "(" + User.Username + ")";
            }
            return str;
        }

        public Character Copy()
        {
            Character c = new()
            {
                Name = Name,
                FirstName = FirstName,
                NickName = NickName,
                Statistics = Statistics,
                MagicType = MagicType,
                FirstRoleType = FirstRoleType,
                SecondRoleType = SecondRoleType,
                ThirdRoleType = ThirdRoleType,
                RoleRating = RoleRating,
                Promotion = Promotion,
                Level = Level,
                EXP = EXP,
                BaseHP = BaseHP,
                HP = HP,
                BaseMP = BaseMP,
                MP = MP,
                EP = EP,
                BaseATK = BaseATK,
                ATK = ATK,
                BaseDEF = BaseDEF,
                DEF = DEF,
                MDF = MDF,
                PhysicalPenetration = PhysicalPenetration,
                MagicalPenetration = MagicalPenetration,
                HR = HR,
                MR = MR,
                ER = ER,
                BaseSTR = BaseSTR,
                BaseAGI = BaseAGI,
                BaseINT = BaseINT,
                STR = STR,
                AGI = AGI,
                INT = INT,
                STRGrowth = STRGrowth,
                AGIGrowth = AGIGrowth,
                INTGrowth = INTGrowth,
                SPD = SPD,
                AccelerationCoefficient = AccelerationCoefficient,
                ATR = ATR,
                CritRate = CritRate,
                CritDMG = CritDMG,
                EvadeRate = EvadeRate,
                Skills = Skills,
                Items = Items,
            };
            return c;
        }
    }
}
