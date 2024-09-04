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
        public User User { get; set; }

        /// <summary>
        /// 角色统计数据
        /// </summary>
        public CharacterStatistics Statistics { get; set; }

        /// <summary>
        /// 魔法属性
        /// </summary>
        public MagicType MagicType { get; set; } = MagicType.None;

        /// <summary>
        /// 角色定位1
        /// </summary>
        public RoleType FirstRoleType { get; set; } = RoleType.None;

        /// <summary>
        /// 角色定位2
        /// </summary>
        public RoleType SecondRoleType { get; set; } = RoleType.None;

        /// <summary>
        /// 角色定位3
        /// </summary>
        public RoleType ThirdRoleType { get; set; } = RoleType.None;

        /// <summary>
        /// 角色评级
        /// </summary>
        public RoleRating RoleRating
        {
            get
            {
                if (Promotion > 998)
                {
                    return RoleRating.X;
                }
                else if (Promotion > 850 && Promotion <= 998)
                {
                    return RoleRating.S;
                }
                else if (Promotion > 700 && Promotion <= 850)
                {
                    return RoleRating.APlus;
                }
                else if (Promotion > 550 && Promotion <= 700)
                {
                    return RoleRating.A;
                }
                else if (Promotion > 400 && Promotion <= 550)
                {
                    return RoleRating.B;
                }
                else if (Promotion > 300 && Promotion <= 400)
                {
                    return RoleRating.C;
                }
                else if (Promotion > 200 && Promotion <= 300)
                {
                    return RoleRating.D;
                }
                else
                {
                    return RoleRating.E;
                }
            }
        }

        /// <summary>
        /// 晋升点数
        /// </summary>
        public int Promotion { get; set; } = 100;

        /// <summary>
        /// 核心属性
        /// </summary>
        public PrimaryAttribute PrimaryAttribute { get; set; } = PrimaryAttribute.None;

        /// <summary>
        /// 等级
        /// </summary>
        public int Level
        {
            get
            {
                return _Level > 0 ? _Level : 1;
            }
            set
            {
                if (_Level > 0) _Level = value;
            }
        }

        /// <summary>
        /// 经验值
        /// </summary>
        public double EXP { get; set; } = 0;

        /// <summary>
        /// 初始生命值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialHP { get; set; } = 60;

        /// <summary>
        /// 基础生命值 [ 与初始设定和等级相关 ] [ 与基础力量相关 ]
        /// </summary>
        public double BaseHP => Calculation.Round2Digits(InitialHP + (Level - 1) * (17 + 0.68 * InitialHP) + BaseSTR * 17);

        /// <summary>
        /// 额外生命值 [ 与额外力量相关 ]
        /// </summary>
        public double ExHP => Calculation.Round2Digits(ExSTR * 17);

        /// <summary>
        /// 额外生命值2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExHP2 { get; set; } = 0;

        /// <summary>
        /// 生命值 = 基础生命值 + 额外生命值 + 额外生命值2
        /// </summary>
        public double HP => BaseHP + ExHP + ExHP2;

        /// <summary>
        /// 初始魔法值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialMP { get; set; } = 10;

        /// <summary>
        /// 基础魔法值 [ 与初始设定和等级相关 ] [ 与基础智力相关 ]
        /// </summary>
        public double BaseMP => Calculation.Round2Digits(InitialMP + (Level - 1) * (1.5 + 0.14 * InitialMP) + BaseINT * 8);

        /// <summary>
        /// 额外魔法值 [ 与额外智力相关 ]
        /// </summary>
        public double ExMP => Calculation.Round2Digits(ExINT * 8);

        /// <summary>
        /// 额外魔法值2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExMP2 { get; set; } = 0;

        /// <summary>
        /// 魔法值 = 基础魔法值 + 额外魔法值 + 额外魔法值2
        /// </summary>
        public double MP => BaseMP + ExMP + ExMP2;

        /// <summary>
        /// 爆发能量 [ 战斗相关 ]
        /// </summary>
        public double EP { get; set; } = 0;

        /// <summary>
        /// 初始攻击力 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialATK { get; set; } = 15;

        /// <summary>
        /// 基础攻击力 [ 与初始设定和等级相关 ] [ 与核心属性相关 ]
        /// </summary>
        public double BaseATK
        {
            get
            {
                double atk = Calculation.Round2Digits(InitialATK + (Level - 1) * (0.95 + 0.045 * InitialATK));
                if (PrimaryAttribute == PrimaryAttribute.AGI)
                {
                    return atk + BaseAGI;
                }
                else if (PrimaryAttribute == PrimaryAttribute.INT)
                {
                    return atk + BaseINT;
                }
                else // 默认STR
                {
                    return atk + BaseSTR;
                }
            }
        }

        /// <summary>
        /// 额外攻击力 [ 与额外核心属性相关 ]
        /// </summary>
        public double ExATK
        {
            get
            {
                if (PrimaryAttribute == PrimaryAttribute.AGI)
                {
                    return ExAGI;
                }
                else if (PrimaryAttribute == PrimaryAttribute.INT)
                {
                    return ExINT;
                }
                else // 默认STR
                {
                    return ExSTR;
                }
            }
        }

        /// <summary>
        /// 额外攻击力2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExATK2 { get; set; } = 0;

        /// <summary>
        /// 攻击力 = 基础攻击力 + 额外攻击力 + 额外攻击力2
        /// </summary>
        public double ATK => BaseATK + ExATK + ExATK2;

        /// <summary>
        /// 初始物理护甲 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialDEF { get; set; } = 5;

        /// <summary>
        /// 基础物理护甲 [ 与初始设定相关 ] [ 与基础力量相关 ]
        /// </summary>
        public double BaseDEF => Calculation.Round2Digits(InitialDEF + BaseSTR * 0.75);

        /// <summary>
        /// 额外物理护甲 [ 与额外力量相关 ]
        /// </summary>
        public double ExDEF => Calculation.Round2Digits(ExSTR * 0.75);

        /// <summary>
        /// 额外物理护甲2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExDEF2 { get; set; } = 0;

        /// <summary>
        /// 物理护甲 = 基础物理护甲 + 额外物理护甲 + 额外物理护甲2
        /// </summary>
        public double DEF => BaseDEF + ExDEF + ExDEF2;

        /// <summary>
        /// 物理伤害减免(%) = [ 与物理护甲相关 ] + 额外物理伤害减免(%)
        /// </summary>
        public double PDR
        {
            get
            {
                double value = Calculation.Round4Digits((DEF / (DEF + 120)) + ExPDR);
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外物理伤害减免(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExPDR { get; set; } = 0;

        /// <summary>
        /// 魔法抗性(%) [ 与技能和物品相关 ]
        /// </summary>
        public double MDF { get; set; } = 0;

        /// <summary>
        /// 物理穿透(%) [ 与技能和物品相关 ]
        /// </summary>
        public double PhysicalPenetration
        {
            get
            {
                return Calculation.PercentageCheck(_PhysicalPenetration);
            }
            set
            {
                _PhysicalPenetration = Calculation.PercentageCheck(Calculation.Round4Digits(value));
            }
        }

        /// <summary>
        /// 魔法穿透(%) [ 与技能和物品相关 ]
        /// </summary>
        public double MagicalPenetration
        {
            get
            {
                return Calculation.PercentageCheck(_MagicalPenetration);
            }
            set
            {
                _MagicalPenetration = Calculation.PercentageCheck(Calculation.Round4Digits(value));
            }
        }

        /// <summary>
        /// 初始生命回复力 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialHR { get; set; } = 0;

        /// <summary>
        /// 生命回复力 = [ 与初始设定相关 ] [ 与力量相关 ] + 额外生命回复力
        /// </summary>
        public double HR => Calculation.Round2Digits(InitialHR + STR * 0.25 + ExHR);

        /// <summary>
        /// 额外生命回复力 [ 与技能和物品相关 ]
        /// </summary>
        public double ExHR { get; set; } = 0;

        /// <summary>
        /// 初始魔法回复力 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialMR { get; set; } = 0;

        /// <summary>
        /// 魔法回复力 = [ 与初始设定相关 ] [ 与智力相关 ] + 额外魔法回复力
        /// </summary>
        public double MR => Calculation.Round2Digits(InitialMR + INT * 0.1 + ExMR);

        /// <summary>
        /// 额外魔法回复力 [ 与技能和物品相关 ]
        /// </summary>
        public double ExMR { get; set; } = 0;

        /// <summary>
        /// 能量回复力 [ 与技能和物品相关 ]
        /// </summary>
        public double ER { get; set; } = 0;

        /// <summary>
        /// 初始力量 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialSTR { get; set; } = 0;

        /// <summary>
        /// 初始敏捷 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialAGI { get; set; } = 0;

        /// <summary>
        /// 初始智力 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialINT { get; set; } = 0;

        /// <summary>
        /// 基础力量 [ 与初始设定和等级相关 ]
        /// </summary>
        public double BaseSTR => Calculation.Round2Digits(InitialSTR + STRGrowth * (Level - 1));

        /// <summary>
        /// 基础敏捷 [ 与初始设定和等级相关 ]
        /// </summary>
        public double BaseAGI => Calculation.Round2Digits(InitialAGI + AGIGrowth * (Level - 1));

        /// <summary>
        /// 基础智力 [ 与初始设定和等级相关 ]
        /// </summary>
        public double BaseINT => Calculation.Round2Digits(InitialINT + INTGrowth * (Level - 1));

        /// <summary>
        /// 额外力量 [ 与技能和物品相关 ]
        /// </summary>
        public double ExSTR { get; set; } = 0;

        /// <summary>
        /// 额外敏捷 [ 与技能和物品相关 ]
        /// </summary>
        public double ExAGI { get; set; } = 0;

        /// <summary>
        /// 额外智力 [ 与技能和物品相关 ]
        /// </summary>
        public double ExINT { get; set; } = 0;

        /// <summary>
        /// 力量 = 基础力量 + 额外力量
        /// </summary>
        public double STR => BaseSTR + ExSTR;

        /// <summary>
        /// 敏捷 = 基础敏捷 + 额外敏捷
        /// </summary>
        public double AGI => BaseAGI + ExAGI;

        /// <summary>
        /// 智力 = 基础智力 + 额外智力
        /// </summary>
        public double INT => BaseINT + ExINT;

        /// <summary>
        /// 力量成长值(+BaseSTR/Lv)
        /// </summary>
        public double STRGrowth { get; set; } = 0;

        /// <summary>
        /// 敏捷成长值(+BaseAGI/Lv)
        /// </summary>
        public double AGIGrowth { get; set; } = 0;

        /// <summary>
        /// 智力成长值(+BaseINT/Lv)
        /// </summary>
        public double INTGrowth { get; set; } = 0;

        /// <summary>
        /// 行动速度 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialSPD { get; set; } = 0;

        /// <summary>
        /// 行动速度 = [ 与初始设定相关 ][ 与敏捷相关 ] + 额外行动速度
        /// </summary>
        public double SPD => Calculation.Round2Digits(InitialSPD + AGI * 0.65 + ExSPD);

        /// <summary>
        /// 额外行动速度 [ 与技能和物品相关 ]
        /// </summary>
        public double ExSPD { get; set; } = 0;

        /// <summary>
        /// 行动系数(%) = [ 与速度相关 ] + 额外行动系数(%)
        /// </summary>
        public double ActionCoefficient
        {
            get
            {
                double value = Calculation.Round4Digits(SPD / 1500.00 + ExActionCoefficient);
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外行动系数(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExActionCoefficient { get; set; } = 0;

        /// <summary>
        /// 加速系数(%) [ 与技能和物品相关 ]
        /// </summary>
        public double AccelerationCoefficient { get; set; } = 0;

        /// <summary>
        /// 冷却缩减(%) = [ 与智力相关 ] + 额外冷却缩减(%)
        /// </summary>
        public double CDR
        {
            get
            {
                double value = Calculation.Round4Digits(INT * 0.0025 + ExCDR);
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外冷却缩减(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExCDR { get; set; } = 0;

        /// <summary>
        /// 攻击距离 [ 与技能和物品相关 ] [ 单位：格 ]
        /// </summary>
        [InitOptional]
        public double ATR { get; set; } = 1;

        /// <summary>
        /// 暴击率(%) = [ 与敏捷相关 ] + 额外暴击率(%)
        /// </summary>
        public double CritRate
        {
            get
            {
                double value = Calculation.Round4Digits(0.05 + INT * 0.0025 + ExCDR);
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外暴击率(%)  [ 与技能和物品相关 ]
        /// </summary>
        public double ExCritRate { get; set; } = 0;

        /// <summary>
        /// 暴击伤害(%) = [ 与力量相关 ] + 额外暴击伤害(%)
        /// </summary>
        public double CritDMG
        {
            get
            {
                return Calculation.Round4Digits(1.25 + STR * 0.00575 + ExCritDMG);
            }
        }

        /// <summary>
        /// 额外暴击伤害(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExCritDMG { get; set; } = 0;

        /// <summary>
        /// 闪避率(%) = [ 与敏捷相关 ] + 额外闪避率(%)
        /// </summary>
        public double EvadeRate
        {
            get
            {
                double value = Calculation.Round4Digits(0.05 + AGI * 0.0025 + ExEvadeRate);
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外闪避率(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExEvadeRate { get; set; } = 0;

        /// <summary>
        /// 角色的技能组
        /// </summary>
        public Dictionary<string, Skill> Skills { get; set; } = [];

        /// <summary>
        /// 角色携带的物品
        /// </summary>
        public Dictionary<string, Item> Items { get; set; } = [];

        /**
         * ===== 私有变量 =====
         */

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 1;

        /// <summary>
        /// 物理穿透
        /// </summary>
        private double _PhysicalPenetration = 0;

        /// <summary>
        /// 魔法穿透
        /// </summary>
        private double _MagicalPenetration = 0;

        protected Character()
        {
            User = General.UnknownUserInstance;
            Statistics = new();
        }

        internal static Character GetInstance()
        {
            return new();
        }

        /// <summary>
        /// 比较一个角色（只比较 <see cref="Name"/>）
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is Character c && c.Name == Name;
        }

        /// <summary>
        /// 获取角色实例的名字、昵称以及所属玩家
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 复制一个角色
        /// [ 推荐从模组中复制后使用对象 ]
        /// </summary>
        /// <returns></returns>
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
                Promotion = Promotion,
                PrimaryAttribute = PrimaryAttribute,
                Level = Level,
                EXP = EXP,
                InitialHP = InitialHP,
                ExHP2 = ExHP2,
                InitialMP = InitialMP,
                ExMP2 = ExMP2,
                EP = EP,
                InitialATK = InitialATK,
                ExATK2 = ExATK2,
                InitialDEF = InitialDEF,
                ExDEF2 = ExDEF2,
                MDF = MDF,
                PhysicalPenetration = PhysicalPenetration,
                MagicalPenetration = MagicalPenetration,
                InitialHR = InitialHR,
                ExHR = ExHR,
                InitialMR = InitialMR,
                ExMR = ExMR,
                ER = ER,
                InitialSTR = InitialSTR,
                InitialAGI = InitialAGI,
                InitialINT = InitialINT,
                ExSTR = ExSTR,
                ExAGI = ExAGI,
                ExINT = ExINT,
                STRGrowth = STRGrowth,
                AGIGrowth = AGIGrowth,
                INTGrowth = INTGrowth,
                InitialSPD = InitialSPD,
                ExSPD = ExSPD,
                ExActionCoefficient = ExActionCoefficient,
                AccelerationCoefficient = AccelerationCoefficient,
                ExCDR = ExCDR,
                ATR = ATR,
                ExCritRate = ExCritRate,
                ExCritDMG = ExCritDMG,
                ExEvadeRate = ExEvadeRate,
                Skills = new(Skills),
                Items = new(Items),
            };
            return c;
        }
    }
}
