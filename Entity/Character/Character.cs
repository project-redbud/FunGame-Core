using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色需要使用 Factory.Get 的方式来构造，并赋值 <see cref="InitRequired"/> 标记的属性<para />
    /// 在使用时仅需要调用 <see cref="Copy"/> 方法即可获得相同对象<para />
    /// 不建议继承
    /// </summary>
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
        /// 角色的详细资料
        /// </summary>
        public CharacterProfile Profile { get; set; }

        /// <summary>
        /// 角色的装备
        /// </summary>
        public EquipSlot EquipSlot { get; set; }

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
                if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdSUpperLimit)
                {
                    return RoleRating.X;
                }
                else if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdAPlusUpperLimit && Promotion <= General.GameplayEquilibriumConstant.PromotionThresholdSUpperLimit)
                {
                    return RoleRating.S;
                }
                else if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdAUpperLimit && Promotion <= General.GameplayEquilibriumConstant.PromotionThresholdAPlusUpperLimit)
                {
                    return RoleRating.APlus;
                }
                else if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdBUpperLimit && Promotion <= General.GameplayEquilibriumConstant.PromotionThresholdAUpperLimit)
                {
                    return RoleRating.A;
                }
                else if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdCUpperLimit && Promotion <= General.GameplayEquilibriumConstant.PromotionThresholdBUpperLimit)
                {
                    return RoleRating.B;
                }
                else if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdDUpperLimit && Promotion <= General.GameplayEquilibriumConstant.PromotionThresholdCUpperLimit)
                {
                    return RoleRating.C;
                }
                else if (Promotion > General.GameplayEquilibriumConstant.PromotionThresholdEUpperLimit && Promotion <= General.GameplayEquilibriumConstant.PromotionThresholdDUpperLimit)
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
                return _Level >= 1 ? _Level : 1;
            }
            set
            {
                _Level = Math.Min(Math.Max(1, value), General.GameplayEquilibriumConstant.MaxLevel);
                OnAttributeChanged();
                Recovery();
            }
        }

        /// <summary>
        /// 经验值
        /// </summary>
        public double EXP { get; set; } = 0;

        /// <summary>
        /// 角色目前所处的状态 [ 战斗相关 ]
        /// </summary>
        public CharacterState CharacterState { get; set; } = CharacterState.Actionable;

        /// <summary>
        /// 角色目前被特效施加的状态 [ 用于设置角色是否被控制的状态 ]
        /// </summary>
        public Dictionary<Effect, List<CharacterState>> CharacterEffectStates { get; } = [];

        /// <summary>
        /// 角色目前被特效施加的控制效果 [ 用于特效判断是否需要在移除特效时更改角色状态 ]
        /// </summary>
        public Dictionary<Effect, List<EffectType>> CharacterEffectTypes { get; } = [];

        /// <summary>
        /// 角色是否是中立的 [ 战斗相关 ]
        /// </summary>
        public bool IsNeutral { get; set; } = false;

        /// <summary>
        /// 角色是否是不可选中的 [ 战斗相关 ]
        /// </summary>
        public bool IsUnselectable { get; set; } = false;

        /// <summary>
        /// 初始生命值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialHP { get; set; } = General.GameplayEquilibriumConstant.InitialHP;

        /// <summary>
        /// 基础生命值 [ 与初始设定和等级相关 ] [ 与基础力量相关 ]
        /// </summary>
        public double BaseHP => InitialHP + (Level - 1) * (General.GameplayEquilibriumConstant.LevelToHPFactor + General.GameplayEquilibriumConstant.HPGrowthFactor * InitialHP) + BaseSTR * General.GameplayEquilibriumConstant.STRtoHPFactor;

        /// <summary>
        /// 额外生命值 [ 与额外力量相关 ]
        /// </summary>
        public double ExHP => ExSTR * General.GameplayEquilibriumConstant.STRtoHPFactor;

        /// <summary>
        /// 额外生命值2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExHP2 { get; set; } = 0;

        /// <summary>
        /// 最大生命值 = 基础生命值 + 额外生命值 + 额外生命值2
        /// </summary>
        public double MaxHP => BaseHP + ExHP + ExHP2;

        /// <summary>
        /// 当前生命值 [ 战斗相关 ]
        /// </summary>
        public double HP
        {
            get
            {
                return _HP < 0 ? 0 : (_HP > MaxHP ? MaxHP : _HP);
            }
            set
            {
                _HP = value;
                if (_HP > MaxHP) _HP = MaxHP;
                else if (_HP < 0) _HP = 0;
            }
        }

        /// <summary>
        /// 初始魔法值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialMP { get; set; } = General.GameplayEquilibriumConstant.InitialMP;

        /// <summary>
        /// 基础魔法值 [ 与初始设定和等级相关 ] [ 与基础智力相关 ]
        /// </summary>
        public double BaseMP => InitialMP + (Level - 1) * (General.GameplayEquilibriumConstant.LevelToMPFactor + General.GameplayEquilibriumConstant.MPGrowthFactor * InitialMP) + BaseINT * General.GameplayEquilibriumConstant.INTtoMPFactor;

        /// <summary>
        /// 额外魔法值 [ 与额外智力相关 ]
        /// </summary>
        public double ExMP => ExINT * General.GameplayEquilibriumConstant.INTtoMPFactor;

        /// <summary>
        /// 额外魔法值2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExMP2 { get; set; } = 0;

        /// <summary>
        /// 最大魔法值 = 基础魔法值 + 额外魔法值 + 额外魔法值2
        /// </summary>
        public double MaxMP => BaseMP + ExMP + ExMP2;

        /// <summary>
        /// 当前魔法值 [ 战斗相关 ]
        /// </summary>
        public double MP
        {
            get
            {
                return _MP < 0 ? 0 : (_MP > MaxMP ? MaxMP : _MP);
            }
            set
            {
                _MP = value;
                if (_MP > MaxMP) _MP = MaxMP;
                else if (_MP < 0) _MP = 0;
            }
        }

        /// <summary>
        /// 当前爆发能量 [ 战斗相关 ]
        /// </summary>
        public double EP
        {
            get
            {
                return _EP < 0 ? 0 : (_EP > General.GameplayEquilibriumConstant.MaxEP ? General.GameplayEquilibriumConstant.MaxEP : _EP);
            }
            set
            {
                _EP = value;
                if (_EP > General.GameplayEquilibriumConstant.MaxEP) _EP = General.GameplayEquilibriumConstant.MaxEP;
                else if (_EP < 0) _EP = 0;
            }
        }

        /// <summary>
        /// 初始攻击力 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialATK { get; set; } = General.GameplayEquilibriumConstant.InitialATK;

        /// <summary>
        /// 基础攻击力 [ 与初始设定和等级相关 ] [ 与核心属性相关 ]
        /// </summary>
        public double BaseATK
        {
            get
            {
                double atk = InitialATK + (Level - 1) * (General.GameplayEquilibriumConstant.LevelToATKFactor + General.GameplayEquilibriumConstant.ATKGrowthFactor * InitialATK);
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
        public double InitialDEF { get; set; } = General.GameplayEquilibriumConstant.InitialDEF;

        /// <summary>
        /// 基础物理护甲 [ 与初始设定相关 ] [ 与基础力量相关 ]
        /// </summary>
        public double BaseDEF => InitialDEF + BaseSTR * General.GameplayEquilibriumConstant.STRtoDEFFactor;

        /// <summary>
        /// 额外物理护甲 [ 与额外力量相关 ]
        /// </summary>
        public double ExDEF => ExSTR * General.GameplayEquilibriumConstant.STRtoDEFFactor;

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
                double value = (DEF / (DEF + General.GameplayEquilibriumConstant.DEFReductionFactor)) + ExPDR;
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
        public MagicResistance MDF { get; set; }

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
                _PhysicalPenetration = Calculation.PercentageCheck(value);
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
                _MagicalPenetration = Calculation.PercentageCheck(value);
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
        public double HR => InitialHR + STR * General.GameplayEquilibriumConstant.STRtoHRFactor + ExHR;

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
        public double MR => InitialMR + INT * General.GameplayEquilibriumConstant.INTtoMRFactor + ExMR;

        /// <summary>
        /// 额外魔法回复力 [ 与技能和物品相关 ]
        /// </summary>
        public double ExMR { get; set; } = 0;

        /// <summary>
        /// 能量回复力 [ 与技能和物品相关 ]
        /// </summary>
        public double ER { get; set; } = 0;

        /// <summary>
        /// 核心属性的值 [ 核心属性相关 ]
        /// </summary>
        public double PrimaryAttributeValue
        {
            get
            {
                if (PrimaryAttribute == PrimaryAttribute.AGI)
                {
                    return AGI;
                }
                else if (PrimaryAttribute == PrimaryAttribute.INT)
                {
                    return INT;
                }
                else
                {
                    return STR;
                }
            }
        }

        /// <summary>
        /// 基础核心属性的值 [ 核心属性相关 ]
        /// </summary>
        public double BasePrimaryAttributeValue
        {
            get
            {
                if (PrimaryAttribute == PrimaryAttribute.AGI)
                {
                    return BaseAGI;
                }
                else if (PrimaryAttribute == PrimaryAttribute.INT)
                {
                    return BaseINT;
                }
                else
                {
                    return BaseSTR;
                }
            }
        }

        /// <summary>
        /// 额外核心属性的值 [ 核心属性相关 ]
        /// </summary>
        public double ExPrimaryAttributeValue
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
                else
                {
                    return ExSTR;
                }
            }
        }

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
        public double BaseSTR => InitialSTR + STRGrowth * (Level - 1);

        /// <summary>
        /// 基础敏捷 [ 与初始设定和等级相关 ]
        /// </summary>
        public double BaseAGI => InitialAGI + AGIGrowth * (Level - 1);

        /// <summary>
        /// 基础智力 [ 与初始设定和等级相关 ]
        /// </summary>
        public double BaseINT => InitialINT + INTGrowth * (Level - 1);

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
        public double SPD => InitialSPD + AGI * General.GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;

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
                double value = SPD / General.GameplayEquilibriumConstant.SPDUpperLimit + ExActionCoefficient;
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
                double value = INT * General.GameplayEquilibriumConstant.INTtoCDRMultiplier + ExCDR;
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
                double value = General.GameplayEquilibriumConstant.CritRate + AGI * General.GameplayEquilibriumConstant.AGItoCritRateMultiplier + ExCritRate;
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
                return General.GameplayEquilibriumConstant.CritDMG + STR * General.GameplayEquilibriumConstant.STRtoCritDMGMultiplier + ExCritDMG;
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
                double value = General.GameplayEquilibriumConstant.EvadeRate + AGI * General.GameplayEquilibriumConstant.AGItoEvadeRateMultiplier + ExEvadeRate;
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外闪避率(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExEvadeRate { get; set; } = 0;

        /// <summary>
        /// 普通攻击对象
        /// </summary>
        public NormalAttack NormalAttack { get; }

        /// <summary>
        /// 角色的技能列表
        /// </summary>
        public HashSet<Skill> Skills { get; } = [];

        /// <summary>
        /// 角色的持续性特效列表
        /// </summary>
        public HashSet<Effect> Effects { get; } = [];

        /// <summary>
        /// 角色携带的物品
        /// </summary>
        public HashSet<Item> Items { get; } = [];

        /**
         * ===== 私有变量 =====
         */

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 1;

        /// <summary>
        /// 生命值
        /// </summary>
        private double _HP = 0;

        /// <summary>
        /// 魔法值
        /// </summary>
        private double _MP = 0;

        /// <summary>
        /// 能量值
        /// </summary>
        private double _EP = 0;

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
            Profile = new(Name, FirstName, NickName);
            EquipSlot = new();
            MDF = new();
            NormalAttack = new(this);
        }

        internal static Character GetInstance()
        {
            return new();
        }

        /// <summary>
        /// 回复状态至满
        /// </summary>
        /// <param name="EP"></param>
        public void Recovery(double EP = -1)
        {
            HP = MaxHP;
            MP = MaxMP;
            if (EP != -1) this.EP = EP;
        }

        /// <summary>
        /// 按时间回复状态
        /// </summary>
        /// <param name="time"></param>
        /// <param name="EP"></param>
        public void Recovery(int time, double EP = -1)
        {
            if (time > 0)
            {
                HP = Math.Min(MaxHP, HP + HR * time);
                MP = Math.Min(MaxMP, MP + MR * time);
                if (EP != -1) this.EP = EP;
            }
        }

        /// <summary>
        /// 按当前百分比回复状态（一般在属性变化时调用）
        /// </summary>
        /// <param name="pastHP"></param>
        /// <param name="pastMP"></param>
        /// <param name="pastMaxHP"></param>
        /// <param name="pastMaxMP"></param>
        public void Recovery(double pastHP, double pastMP, double pastMaxHP, double pastMaxMP)
        {
            double pHP = pastHP / pastMaxHP;
            double pMP = pastMP / pastMaxMP;
            HP = MaxHP * pHP;
            MP = MaxMP * pMP;
        }

        /// <summary>
        /// 为角色装备物品（必须使用此方法而不是自己去给EquipSlot里的物品赋值）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        public bool Equip(Item item, EquipSlotType slot)
        {
            bool result = false;
            double pastHP = HP;
            double pastMaxHP = MaxHP;
            double pastMP = MP;
            double pastMaxMP = MaxMP;
            switch (slot)
            {
                case EquipSlotType.MagicCardPack:
                    if (item.ItemType == ItemType.MagicCardPack)
                    {
                        UnEquip(EquipSlotType.MagicCardPack);
                        EquipSlot.MagicCardPack = item;
                        item.OnItemEquip(this, EquipSlotType.MagicCardPack);
                        result = true;
                    }
                    break;
                case EquipSlotType.Weapon:
                    if (item.ItemType == ItemType.Weapon)
                    {
                        UnEquip(EquipSlotType.Weapon);
                        EquipSlot.Weapon = item;
                        item.OnItemEquip(this, EquipSlotType.Weapon);
                        result = true;
                    }
                    break;
                case EquipSlotType.Armor:
                    if (item.ItemType == ItemType.Armor)
                    {
                        UnEquip(EquipSlotType.Armor);
                        EquipSlot.Armor = item;
                        item.OnItemEquip(this, EquipSlotType.Armor);
                        result = true;
                    }
                    break;
                case EquipSlotType.Shoes:
                    if (item.ItemType == ItemType.Shoes)
                    {
                        UnEquip(EquipSlotType.Shoes);
                        EquipSlot.Shoes = item;
                        item.OnItemEquip(this, EquipSlotType.Shoes);
                        result = true;
                    }
                    break;
                case EquipSlotType.Accessory1:
                    if (item.ItemType == ItemType.Accessory)
                    {
                        UnEquip(EquipSlotType.Accessory1);
                        EquipSlot.Accessory1 = item;
                        EquipSlot.LastEquipSlotType = EquipSlotType.Accessory1;
                        item.OnItemEquip(this, EquipSlotType.Accessory1);
                        result = true;
                    }
                    break;
                case EquipSlotType.Accessory2:
                    if (item.ItemType == ItemType.Accessory)
                    {
                        UnEquip(EquipSlotType.Accessory2);
                        EquipSlot.Accessory2 = item;
                        EquipSlot.LastEquipSlotType = EquipSlotType.Accessory2;
                        item.OnItemEquip(this, EquipSlotType.Accessory2);
                        result = true;
                    }
                    break;
            }
            if (result)
            {
                OnAttributeChanged();
                Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
            }
            return result;
        }

        /// <summary>
        /// 为角色装备物品（必须使用此方法而不是自己去给EquipSlot里的物品赋值）<para/>
        /// 此方法为根据物品类型，优先空位自动装备
        /// </summary>
        /// <param name="item"></param>
        public bool Equip(Item item)
        {
            switch (item.ItemType)
            {
                case ItemType.MagicCardPack:
                    return Equip(item, EquipSlotType.MagicCardPack);
                case ItemType.Weapon:
                    return Equip(item, EquipSlotType.Weapon);
                case ItemType.Armor:
                    return Equip(item, EquipSlotType.Armor);
                case ItemType.Shoes:
                    return Equip(item, EquipSlotType.Shoes);
                case ItemType.Accessory:
                    if (EquipSlot.Accessory1 is null)
                    {
                        return Equip(item, EquipSlotType.Accessory1);
                    }
                    else if (EquipSlot.Accessory1 != null && EquipSlot.Accessory2 is null)
                    {
                        return Equip(item, EquipSlotType.Accessory2);
                    }
                    else if (EquipSlot.Accessory1 != null && EquipSlot.Accessory2 != null && EquipSlot.LastEquipSlotType == EquipSlotType.Accessory1)
                    {
                        return Equip(item, EquipSlotType.Accessory2);
                    }
                    else
                    {
                        return Equip(item, EquipSlotType.Accessory1);
                    }
            }
            return false;
        }

        /// <summary>
        /// 取消装备，返回被取消的物品对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Item? UnEquip(EquipSlotType type)
        {
            Item? result = null;
            switch (type)
            {
                case EquipSlotType.MagicCardPack:
                    if (EquipSlot.MagicCardPack != null)
                    {
                        result = EquipSlot.MagicCardPack;
                        EquipSlot.MagicCardPack.OnItemUnEquip(EquipSlotType.MagicCardPack);
                    }
                    break;
                case EquipSlotType.Weapon:
                    if (EquipSlot.Weapon != null)
                    {
                        result = EquipSlot.Weapon;
                        EquipSlot.Weapon.OnItemUnEquip(EquipSlotType.Weapon);
                    }
                    break;
                case EquipSlotType.Armor:
                    if (EquipSlot.Armor != null)
                    {
                        result = EquipSlot.Armor;
                        EquipSlot.Armor.OnItemUnEquip(EquipSlotType.Armor);
                    }
                    break;
                case EquipSlotType.Shoes:
                    if (EquipSlot.Shoes != null)
                    {
                        result = EquipSlot.Shoes;
                        EquipSlot.Shoes.OnItemUnEquip(EquipSlotType.Shoes);
                    }
                    break;
                case EquipSlotType.Accessory1:
                    if (EquipSlot.Accessory1 != null)
                    {
                        result = EquipSlot.Accessory1;
                        EquipSlot.Accessory1.OnItemUnEquip(EquipSlotType.Accessory1);
                    }
                    break;
                case EquipSlotType.Accessory2:
                    if (EquipSlot.Accessory2 != null)
                    {
                        result = EquipSlot.Accessory2;
                        EquipSlot.Accessory2.OnItemUnEquip(EquipSlotType.Accessory2);
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 角色的属性发生变化，会影响特殊效果的计算
        /// </summary>
        public void OnAttributeChanged()
        {
            foreach (Effect effect in Effects.Where(e => e.Level > 0).ToList())
            {
                effect.OnAttributeChanged(this);
            }
        }

        /// <summary>
        /// 比较一个角色（只比较 <see cref="ToString"/>）
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is Character c && c.ToString() == ToString();
        }

        /// <summary>
        /// 获取角色实例的名字、昵称以及所属玩家
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = GetName();
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
        /// 获取角色实例的名字、昵称
        /// </summary>
        /// <returns></returns>
        public string ToStringWithOutUser()
        {
            string str = GetName();
            if (NickName != "")
            {
                if (str != "") str += ", ";
                str += NickName;
            }
            return str;
        }

        /// <summary>
        /// 获取角色实例的名字、昵称以及所属玩家，包含等级
        /// </summary>
        /// <returns></returns>
        public string ToStringWithLevel()
        {
            string str = GetName();
            if (NickName != "")
            {
                if (str != "") str += ", ";
                str += NickName;
            }
            if (User != null && User.Username != "")
            {
                str += "(" + User.Username + ")";
            }
            str += " - 等级 " + Level;
            return str;
        }

        /// <summary>
        /// 获取角色的名字
        /// </summary>
        /// <param name="full"></param>
        /// <returns>如果 <paramref name="full"/> = false，返回 <see cref="FirstName"/>；反之，返回 <see cref="Name"/> + <see cref="FirstName"/>。</returns>
        public string GetName(bool full = true)
        {
            if (full)
            {
                bool isChineseName = NetworkUtility.IsChineseName(Name + FirstName);
                string str = isChineseName ? (Name + FirstName).Trim() : (Name + " " + FirstName).Trim();
                return str;
            }
            else
            {
                return FirstName;
            }
        }

        /// <summary>
        /// 获取角色的详细信息
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            StringBuilder builder = new();

            builder.AppendLine(ToStringWithLevel());
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (ExHP + ExHP2 > 0 ? $" [{BaseHP:0.##} + {(ExHP + ExHP2):0.##}]" : ""));
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (ExMP + ExMP2 > 0 ? $" [{BaseMP:0.##} + {(ExMP + ExMP2):0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {General.GameplayEquilibriumConstant.MaxEP:0.##}");
            builder.AppendLine($"攻击力：{ATK:0.##}" + (ExATK + ExATK2 > 0 ? $" [{BaseATK:0.##} + {ExATK + ExATK2:0.##}]" : ""));
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (ExDEF + ExDEF2 > 0 ? $" [{BaseDEF:0.##} + {ExDEF + ExDEF2:0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            double mdf = (MDF.None + MDF.Starmark + MDF.PurityNatural + MDF.PurityContemporary +
                MDF.Bright + MDF.Shadow + MDF.Element + MDF.Fleabane + MDF.Particle) / 9;
            builder.AppendLine($"魔法抗性：{mdf * 100:0.##}%（平均）");
            double exSPD = AGI * General.GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
            builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD > 0 ? $" [{InitialSPD:0.##} + {exSPD:0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
            builder.AppendLine($"核心属性：{CharacterSet.GetPrimaryAttributeName(PrimaryAttribute)}");
            builder.AppendLine($"力量：{STR:0.##}" + (ExSTR > 0 ? $" [{BaseSTR:0.##} + {ExSTR:0.##}]" : ""));
            builder.AppendLine($"敏捷：{AGI:0.##}" + (ExAGI > 0 ? $" [{BaseAGI:0.##} + {ExAGI:0.##}]" : ""));
            builder.AppendLine($"智力：{INT:0.##}" + (ExINT > 0 ? $" [{BaseINT:0.##} + {ExINT:0.##}]" : ""));
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR > 0 ? $" [{InitialHR + STR * 0.25:0.##} + {ExHR:0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR > 0 ? $" [{InitialMR + INT * 0.1:0.##} + {ExMR:0.##}]" : ""));
            builder.AppendLine($"暴击率：{CritRate * 100:0.##}%");
            builder.AppendLine($"暴击伤害：{CritDMG * 100:0.##}%");
            builder.AppendLine($"闪避率：{EvadeRate * 100:0.##}%");
            builder.AppendLine($"冷却缩减：{CDR * 100:0.##}%");
            builder.AppendLine($"加速系数：{AccelerationCoefficient * 100:0.##}%");
            builder.AppendLine($"物理穿透：{PhysicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法穿透：{MagicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法消耗减少：{INT * 0.00125 * 100:0.##}%");
            builder.AppendLine($"能量消耗减少：{INT * 0.00075 * 100:0.##}%");

            if (CharacterState != CharacterState.Actionable)
            {
                builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
            }

            if (IsNeutral)
            {
                builder.AppendLine("角色是无敌的");
            }

            if (IsUnselectable)
            {
                builder.AppendLine("角色是不可选中的");
            }

            builder.AppendLine("== 普通攻击 ==");
            builder.Append(NormalAttack.ToString());

            if (Skills.Count > 0)
            {
                builder.AppendLine("== 角色技能 ==");
                foreach (Skill skill in Skills)
                {
                    builder.Append(skill.ToString());
                }
            }

            if (EquipSlot.Any())
            {
                builder.AppendLine("== 装备栏 ==");
                if (EquipSlot.MagicCardPack != null)
                {
                    builder.AppendLine(ItemSet.GetEquipSlotTypeName(EquipSlotType.MagicCardPack) + "：" + EquipSlot.MagicCardPack.Name);
                    builder.AppendLine(EquipSlot.MagicCardPack.Description);
                }
                if (EquipSlot.Weapon != null)
                {
                    builder.AppendLine(ItemSet.GetEquipSlotTypeName(EquipSlotType.Weapon) + "：" + EquipSlot.Weapon.Name);
                    builder.AppendLine(EquipSlot.Weapon.Description);
                }
                if (EquipSlot.Armor != null)
                {
                    builder.AppendLine(ItemSet.GetEquipSlotTypeName(EquipSlotType.Armor) + "：" + EquipSlot.Armor.Name);
                    builder.AppendLine(EquipSlot.Armor.Description);
                }
                if (EquipSlot.Shoes != null)
                {
                    builder.AppendLine(ItemSet.GetEquipSlotTypeName(EquipSlotType.Shoes) + "：" + EquipSlot.Shoes.Name);
                    builder.AppendLine(EquipSlot.Shoes.Description);
                }
                if (EquipSlot.Accessory1 != null)
                {
                    builder.AppendLine(ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory1) + "：" + EquipSlot.Accessory1.Name);
                    builder.AppendLine(EquipSlot.Accessory1.Description);
                }
                if (EquipSlot.Accessory2 != null)
                {
                    builder.AppendLine(ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory2) + "：" + EquipSlot.Accessory2.Name);
                    builder.AppendLine(EquipSlot.Accessory2.Description);
                }
            }

            if (Items.Count > 0)
            {
                builder.AppendLine("== 角色背包 ==");
                foreach (Item item in Items)
                {
                    builder.Append(item.ToString());
                }
            }

            if (Effects.Where(e => e.EffectType != EffectType.Item).Any())
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in Effects.Where(e => e.EffectType != EffectType.Item))
                {
                    builder.Append(effect.ToString());
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取战斗状态的信息
        /// </summary>
        /// <param name="hardnessTimes"></param>
        /// <returns></returns>
        public string GetInBattleInfo(double hardnessTimes)
        {
            StringBuilder builder = new();

            builder.AppendLine(ToStringWithLevel());
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (ExHP + ExHP2 > 0 ? $" [{BaseHP:0.##} + {ExHP + ExHP2:0.##}]" : ""));
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (ExMP + ExMP2 > 0 ? $" [{BaseMP:0.##} + {ExMP + ExMP2:0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {General.GameplayEquilibriumConstant.MaxEP:0.##}");
            builder.AppendLine($"攻击力：{ATK:0.##}" + (ExATK + ExATK2 > 0 ? $" [{BaseATK:0.##} + {ExATK + ExATK2:0.##}]" : ""));
            builder.AppendLine($"核心属性：{PrimaryAttributeValue:0.##}" + (ExPrimaryAttributeValue > 0 ? $" [{BasePrimaryAttributeValue:0.##} + {ExPrimaryAttributeValue:0.##}]" : ""));

            if (CharacterState != CharacterState.Actionable)
            {
                builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
            }

            if (IsNeutral)
            {
                builder.AppendLine("角色是中立单位，处于无敌状态");
            }

            if (IsUnselectable)
            {
                builder.AppendLine("角色是不可选中的");
            }

            builder.AppendLine($"硬直时间：{hardnessTimes:0.##}");

            if (Effects.Count > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in Effects.Where(e => e.EffectType != EffectType.Item))
                {
                    builder.Append(effect.ToString());
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 更新角色的状态
        /// </summary>
        /// <returns></returns>
        public CharacterState UpdateCharacterState()
        {
            bool isNotActionable = false;
            bool isActionRestricted = false;
            bool isBattleRestricted = false;
            bool isSkillRestricted = false;

            IEnumerable<CharacterState> states = CharacterEffectStates.Values.SelectMany(list => list);
            // 根据持有的特效判断角色所处的状态
            isNotActionable = states.Any(state => state == CharacterState.NotActionable);
            isActionRestricted = states.Any(state => state == CharacterState.ActionRestricted);
            isBattleRestricted = states.Any(state => state == CharacterState.BattleRestricted);
            isSkillRestricted = states.Any(state => state == CharacterState.SkillRestricted);

            IEnumerable<EffectType> types = CharacterEffectTypes.Values.SelectMany(list => list);
            // 判断角色的控制效果
            IsUnselectable = types.Any(type => type == EffectType.Unselectable);

            bool isControl = isNotActionable || isActionRestricted || isBattleRestricted || isSkillRestricted;
            bool isCasting = CharacterState == CharacterState.Casting;
            bool isPreCastSuperSkill = CharacterState == CharacterState.PreCastSuperSkill;

            if (isNotActionable)
            {
                CharacterState = CharacterState.NotActionable;
            }
            else if (isActionRestricted)
            {
                CharacterState = CharacterState.ActionRestricted;
            }
            else if (isBattleRestricted)
            {
                CharacterState = CharacterState.BattleRestricted;
            }
            else if (isSkillRestricted)
            {
                CharacterState = CharacterState.SkillRestricted;
            }

            if (!isControl && !isCasting && !isPreCastSuperSkill)
            {
                CharacterState = CharacterState.Actionable;
            }

            return CharacterState;
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
                Id = Id,
                Name = Name,
                FirstName = FirstName,
                NickName = NickName,
                Profile = Profile.Copy(),
                MagicType = MagicType,
                FirstRoleType = FirstRoleType,
                SecondRoleType = SecondRoleType,
                ThirdRoleType = ThirdRoleType,
                Promotion = Promotion,
                PrimaryAttribute = PrimaryAttribute,
                Level = Level,
                EXP = EXP,
                CharacterState = CharacterState,
                InitialHP = InitialHP,
                ExHP2 = ExHP2,
                InitialMP = InitialMP,
                ExMP2 = ExMP2,
                EP = EP,
                InitialATK = InitialATK,
                ExATK2 = ExATK2,
                InitialDEF = InitialDEF,
                ExDEF2 = ExDEF2,
                MDF = MDF.Copy(),
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
                ExEvadeRate = ExEvadeRate
            };
            foreach (Skill skill in Skills)
            {
                Skill newskill = skill.Copy();
                newskill.Character = c;
                c.Skills.Add(newskill);
            }
            foreach (Item item in Items)
            {
                Item newitem = item.Copy();
                newitem.Character = c;
                c.Items.Add(newitem);
            }
            c.Recovery();
            return c;
        }

        /// <summary>
        /// 复活此角色，回复出厂状态
        /// </summary>
        /// <returns></returns>
        public void Respawn()
        {
            Item? mcp = UnEquip(EquipSlotType.MagicCardPack);
            Item? w = UnEquip(EquipSlotType.Weapon);
            Item? a = UnEquip(EquipSlotType.Armor);
            Item? s = UnEquip(EquipSlotType.Shoes);
            Item? ac1 = UnEquip(EquipSlotType.Accessory1);
            Item? ac2 = UnEquip(EquipSlotType.Accessory2);
            List<Skill> skills = new(Skills);
            List<Item> items = new(Items);
            Character c = Copy();
            Effects.Clear();
            Skills.Clear();
            Items.Clear();
            Id = c.Id;
            Name = c.Name;
            FirstName = c.FirstName;
            NickName = c.NickName;
            Profile = c.Profile.Copy();
            MagicType = c.MagicType;
            FirstRoleType = c.FirstRoleType;
            SecondRoleType = c.SecondRoleType;
            ThirdRoleType = c.ThirdRoleType;
            Promotion = c.Promotion;
            PrimaryAttribute = c.PrimaryAttribute;
            Level = c.Level;
            EXP = c.EXP;
            CharacterState = c.CharacterState;
            InitialHP = c.InitialHP;
            ExHP2 = c.ExHP2;
            InitialMP = c.InitialMP;
            ExMP2 = c.ExMP2;
            EP = c.EP;
            InitialATK = c.InitialATK;
            ExATK2 = c.ExATK2;
            InitialDEF = c.InitialDEF;
            ExDEF2 = c.ExDEF2;
            MDF = c.MDF.Copy();
            PhysicalPenetration = c.PhysicalPenetration;
            MagicalPenetration = c.MagicalPenetration;
            InitialHR = c.InitialHR;
            ExHR = c.ExHR;
            InitialMR = c.InitialMR;
            ExMR = c.ExMR;
            ER = c.ER;
            InitialSTR = c.InitialSTR;
            InitialAGI = c.InitialAGI;
            InitialINT = c.InitialINT;
            ExSTR = c.ExSTR;
            ExAGI = c.ExAGI;
            ExINT = c.ExINT;
            STRGrowth = c.STRGrowth;
            AGIGrowth = c.AGIGrowth;
            INTGrowth = c.INTGrowth;
            InitialSPD = c.InitialSPD;
            ExSPD = c.ExSPD;
            ExActionCoefficient = c.ExActionCoefficient;
            AccelerationCoefficient = c.AccelerationCoefficient;
            ExCDR = c.ExCDR;
            ATR = c.ATR;
            ExCritRate = c.ExCritRate;
            ExCritDMG = c.ExCritDMG;
            ExEvadeRate = c.ExEvadeRate;
            foreach (Skill skill in skills)
            {
                Skill newskill = skill.Copy();
                newskill.Character = this;
                newskill.Level = skill.Level;
                Skills.Add(newskill);
            }
            foreach (Item item in items)
            {
                Item newitem = item.Copy(true);
                newitem.Character = this;
                Items.Add(newitem);
            }
            if (mcp != null) Equip(mcp);
            if (w != null) Equip(w);
            if (a != null) Equip(a);
            if (s != null) Equip(s);
            if (ac1 != null) Equip(ac1);
            if (ac2 != null) Equip(ac2);
            Recovery(0D);
        }
    }
}