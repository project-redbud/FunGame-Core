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
        public decimal EXP { get; set; } = 0;

        /// <summary>
        /// 基础生命值
        /// </summary>
        public decimal BaseHP { get; set; } = 0;

        /// <summary>
        /// 生命值
        /// </summary>
        public decimal HP { get; set; } = 0;

        /// <summary>
        /// 基础魔法值
        /// </summary>
        public decimal BaseMP { get; set; } = 0;

        /// <summary>
        /// 魔法值
        /// </summary>
        public decimal MP { get; set; } = 0;

        /// <summary>
        /// 能量
        /// </summary>
        public decimal EP { get; set; } = 0;

        /// <summary>
        /// 基础攻击力
        /// </summary>
        public decimal BaseATK { get; set; } = 0;

        /// <summary>
        /// 攻击力
        /// </summary>
        public decimal ATK { get; set; } = 0;

        /// <summary>
        /// 基础物理护甲
        /// </summary>
        public decimal BaseDEF { get; set; } = 0;

        /// <summary>
        /// 物理护甲
        /// </summary>
        public decimal DEF { get; set; } = 0;

        /// <summary>
        /// 物理伤害减免(%)
        /// </summary>
        public decimal PDR { get; set; } = 0;

        /// <summary>
        /// 魔法抗性(%)
        /// </summary>
        public decimal MDF { get; set; } = 0;

        /// <summary>
        /// 物理穿透(%)
        /// </summary>
        public decimal PhysicalPenetration { get; set; } = 0;

        /// <summary>
        /// 魔法穿透(%)
        /// </summary>
        public decimal MagicalPenetration { get; set; } = 0;

        /// <summary>
        /// 生命回复力
        /// </summary>
        public decimal HR { get; set; } = 0;

        /// <summary>
        /// 魔法回复力
        /// </summary>
        public decimal MR { get; set; } = 0;

        /// <summary>
        /// 能量回复力
        /// </summary>
        public decimal ER { get; set; } = 0;

        /// <summary>
        /// 基础力量
        /// </summary>
        public decimal BaseSTR { get; set; } = 0;

        /// <summary>
        /// 基础敏捷
        /// </summary>
        public decimal BaseAGI { get; set; } = 0;

        /// <summary>
        /// 基础智力
        /// </summary>
        public decimal BaseINT { get; set; } = 0;

        /// <summary>
        /// 力量
        /// </summary>
        public decimal STR { get; set; } = 0;

        /// <summary>
        /// 敏捷
        /// </summary>
        public decimal AGI { get; set; } = 0;

        /// <summary>
        /// 智力
        /// </summary>
        public decimal INT { get; set; } = 0;

        /// <summary>
        /// 力量成长值
        /// </summary>
        public decimal STRGrowth { get; set; } = 0;

        /// <summary>
        /// 敏捷成长值
        /// </summary>
        public decimal AGIGrowth { get; set; } = 0;

        /// <summary>
        /// 智力成长值
        /// </summary>
        public decimal INTGrowth { get; set; } = 0;

        /// <summary>
        /// 速度
        /// </summary>
        public decimal SPD { get; set; } = 0;

        /// <summary>
        /// 行动系数(%)
        /// </summary>
        public decimal ActionCoefficient { get; set; } = 0;

        /// <summary>
        /// 加速系数(%)
        /// </summary>
        public decimal AccelerationCoefficient { get; set; } = 0;

        /// <summary>
        /// 攻击距离
        /// </summary>
        public decimal ATR { get; set; } = 0;

        /// <summary>
        /// 暴击率(%)
        /// </summary>
        public decimal CritRate { get; set; } = 0.05M;

        /// <summary>
        /// 暴击伤害
        /// </summary>
        public decimal CritDMG { get; set; } = 1.25M;

        /// <summary>
        /// 闪避率(%)
        /// </summary>
        public decimal EvadeRate { get; set; } = 0.05M;

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

        public void SetDefaultBase()
        {
            HP = BaseHP;
            MP = BaseMP;
            ATK = BaseATK;
            DEF = BaseDEF;
            STR = BaseSTR;
            AGI = BaseAGI;
            INT = BaseINT;
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
                PDR = PDR,
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
                ActionCoefficient = ActionCoefficient,
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
