using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色需要使用 Factory.Get 的方式来构造，并赋值 <see cref="InitRequired"/> 标记的属性<para />
    /// 在使用时仅需要调用 <see cref="Copy"/> 方法即可获得相同对象<para />
    /// </summary>
    public class Character : BaseEntity
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public override Guid Guid { get; set; } = Guid.NewGuid();

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
                if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["S"])
                {
                    return RoleRating.X;
                }
                else if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["A+"] && Promotion <= GameplayEquilibriumConstant.PromotionsUpperLimit["S"])
                {
                    return RoleRating.S;
                }
                else if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["A"] && Promotion <= GameplayEquilibriumConstant.PromotionsUpperLimit["A+"])
                {
                    return RoleRating.APlus;
                }
                else if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["B"] && Promotion <= GameplayEquilibriumConstant.PromotionsUpperLimit["A"])
                {
                    return RoleRating.A;
                }
                else if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["C"] && Promotion <= GameplayEquilibriumConstant.PromotionsUpperLimit["B"])
                {
                    return RoleRating.B;
                }
                else if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["D"] && Promotion <= GameplayEquilibriumConstant.PromotionsUpperLimit["C"])
                {
                    return RoleRating.C;
                }
                else if (Promotion > GameplayEquilibriumConstant.PromotionsUpperLimit["E"] && Promotion <= GameplayEquilibriumConstant.PromotionsUpperLimit["D"])
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
                int past = _Level;
                _Level = Math.Min(Math.Max(1, value), GameplayEquilibriumConstant.MaxLevel);
                if (past != _Level)
                {
                    OnAttributeChanged();
                    Recovery();
                }
            }
        }

        /// <summary>
        /// 经验值
        /// </summary>
        public double EXP { get; set; } = 0;

        /// <summary>
        /// 等级突破进度 [ 对应 <see cref="Model.EquilibriumConstant.LevelBreakList"/> 中的索引 ]
        /// </summary>
        public int LevelBreak { get; set; } = -1;

        /// <summary>
        /// 角色目前所处的状态 [ 战斗相关 ]
        /// </summary>
        public CharacterState CharacterState { get; set; } = CharacterState.Actionable;

        /// <summary>
        /// 角色目前被特效施加的状态 [ 用于设置角色的状态，这些状态会影响角色的行动 ]
        /// </summary>
        public Dictionary<Effect, List<CharacterState>> CharacterEffectStates { get; } = [];

        /// <summary>
        /// 角色目前被特效施加的控制效果 [ 用于特效在不同的钩子中更改角色的行为 ]
        /// <para/>我们需要明确控制效果不一定能造成角色的状态改变（影响角色的行动），但是这些控制状态需要由特效在不同的地方改变角色的行为
        /// </summary>
        public Dictionary<Effect, List<EffectType>> CharacterEffectTypes { get; } = [];

        /// <summary>
        /// 角色目前被特效施加的免疫状态 [ 战斗相关 ]
        /// </summary>
        public Dictionary<Effect, List<ImmuneType>> CharacterImmuneTypes { get; } = [];

        /// <summary>
        /// 角色是否是中立的 [ 战斗相关 ]
        /// </summary>
        public bool IsNeutral { get; set; } = false;

        /// <summary>
        /// 角色是否是不可选中的 [ 战斗相关 ]
        /// </summary>
        public bool IsUnselectable { get; set; } = false;

        /// <summary>
        /// 角色是否具备免疫状态 [ 战斗相关 ]
        /// </summary>
        public ImmuneType ImmuneType { get; set; } = ImmuneType.None;

        /// <summary>
        /// 初始生命值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialHP { get; set; } = 0;

        /// <summary>
        /// 基础生命值 [ 与初始设定和等级相关 ] [ 与基础力量相关 ]
        /// </summary>
        public double BaseHP => InitialHP + (Level - 1) * (GameplayEquilibriumConstant.LevelToHPFactor + GameplayEquilibriumConstant.HPGrowthFactor * InitialHP) + BaseSTR * GameplayEquilibriumConstant.STRtoHPFactor;

        /// <summary>
        /// 额外生命值 [ 与额外力量相关 ]
        /// </summary>
        public double ExHP => ExSTR * GameplayEquilibriumConstant.STRtoHPFactor;

        /// <summary>
        /// 额外生命值2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExHP2 { get; set; } = 0;

        /// <summary>
        /// 额外生命值3 [ 额外生命值% ]
        /// </summary>
        public double ExHP3 => BaseHP * ExHPPercentage;

        /// <summary>
        /// 额外生命值% [ 与技能和物品相关 ]
        /// </summary>
        public double ExHPPercentage { get; set; } = 0;

        /// <summary>
        /// 最大生命值 = 基础生命值 + 额外生命值 + 额外生命值2 + 额外生命值3
        /// </summary>
        public double MaxHP => Math.Max(1, BaseHP + ExHP + ExHP2 + ExHP3);

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
        /// 是否有魔法值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public bool HasMP { get; set; } = true;
        
        /// <summary>
        /// 初始魔法值 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialMP { get; set; } = 0;

        /// <summary>
        /// 基础魔法值 [ 与初始设定和等级相关 ] [ 与基础智力相关 ]
        /// </summary>
        public double BaseMP => InitialMP + (Level - 1) * (GameplayEquilibriumConstant.LevelToMPFactor + GameplayEquilibriumConstant.MPGrowthFactor * InitialMP) + BaseINT * GameplayEquilibriumConstant.INTtoMPFactor;

        /// <summary>
        /// 额外魔法值 [ 与额外智力相关 ]
        /// </summary>
        public double ExMP => ExINT * GameplayEquilibriumConstant.INTtoMPFactor;

        /// <summary>
        /// 额外魔法值2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExMP2 { get; set; } = 0;

        /// <summary>
        /// 额外魔法值3 [ 额外魔法值% ]
        /// </summary>
        public double ExMP3 => BaseMP * ExMPPercentage;

        /// <summary>
        /// 额外魔法值% [ 与技能和物品相关 ]
        /// </summary>
        public double ExMPPercentage { get; set; } = 0;

        /// <summary>
        /// 最大魔法值 = 基础魔法值 + 额外魔法值 + 额外魔法值2 + 额外魔法值3
        /// </summary>
        public double MaxMP => Math.Max(1, BaseMP + ExMP + ExMP2 + ExMP3);

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
                return _EP < 0 ? 0 : (_EP > GameplayEquilibriumConstant.MaxEP ? GameplayEquilibriumConstant.MaxEP : _EP);
            }
            set
            {
                _EP = value;
                if (_EP > GameplayEquilibriumConstant.MaxEP) _EP = GameplayEquilibriumConstant.MaxEP;
                else if (_EP < 0) _EP = 0;
            }
        }

        /// <summary>
        /// 初始攻击力 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialATK { get; set; } = 0;

        /// <summary>
        /// 基础攻击力 [ 与初始设定和等级相关 ] [ 与核心属性相关 ]
        /// </summary>
        public double BaseATK
        {
            get
            {
                double atk = InitialATK + (Level - 1) * (GameplayEquilibriumConstant.LevelToATKFactor + GameplayEquilibriumConstant.ATKGrowthFactor * InitialATK);
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
        /// 额外攻击力3 [ 额外攻击力% ]
        /// </summary>
        public double ExATK3 => BaseATK * ExATKPercentage;

        /// <summary>
        /// 额外攻击力% [ 与技能和物品相关 ]
        /// </summary>
        public double ExATKPercentage { get; set; } = 0;

        /// <summary>
        /// 攻击力 = 基础攻击力 + 额外攻击力 + 额外攻击力2 + 额外攻击力3
        /// </summary>
        public double ATK => BaseATK + ExATK + ExATK2 + ExATK3;

        /// <summary>
        /// 初始物理护甲 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialDEF { get; set; } = 0;

        /// <summary>
        /// 基础物理护甲 [ 与初始设定相关 ] [ 与基础力量相关 ]
        /// </summary>
        public double BaseDEF => InitialDEF + BaseSTR * GameplayEquilibriumConstant.STRtoDEFFactor;

        /// <summary>
        /// 额外物理护甲 [ 与额外力量相关 ]
        /// </summary>
        public double ExDEF => ExSTR * GameplayEquilibriumConstant.STRtoDEFFactor;

        /// <summary>
        /// 额外物理护甲2 [ 与技能和物品相关 ]
        /// </summary>
        public double ExDEF2 { get; set; } = 0;

        /// <summary>
        /// 额外物理护甲3 [ 额外物理护甲% ]
        /// </summary>
        public double ExDEF3 => BaseDEF * ExDEFPercentage;

        /// <summary>
        /// 额外物理护甲% [ 与技能和物品相关 ]
        /// </summary>
        public double ExDEFPercentage { get; set; } = 0;

        /// <summary>
        /// 物理护甲 = 基础物理护甲 + 额外物理护甲 + 额外物理护甲2 + 额外物理护甲3
        /// </summary>
        public double DEF => BaseDEF + ExDEF + ExDEF2 + ExDEF3;

        /// <summary>
        /// 物理伤害减免(%) = [ 与物理护甲相关 ] + 额外物理伤害减免(%)
        /// </summary>
        public double PDR
        {
            get
            {
                double value = (DEF / (DEF + GameplayEquilibriumConstant.DEFReductionFactor)) + ExPDR;
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
        public double HR => InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor + ExHR;

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
        public double MR => InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor + ExMR;

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
        /// 额外力量2 [ 额外力量% ]
        /// </summary>
        public double ExSTR2 => BaseSTR * ExSTRPercentage;

        /// <summary>
        /// 额外敏捷2 [ 额外敏捷% ]
        /// </summary>
        public double ExAGI2 => BaseAGI * ExAGIPercentage;

        /// <summary>
        /// 额外智力2 [ 额外智力% ]
        /// </summary>
        public double ExINT2 => BaseINT * ExINTPercentage;

        /// <summary>
        /// 额外力量% [ 与技能和物品相关 ]
        /// </summary>
        public double ExSTRPercentage { get; set; } = 0;

        /// <summary>
        /// 额外敏捷% [ 与技能和物品相关 ]
        /// </summary>
        public double ExAGIPercentage { get; set; } = 0;

        /// <summary>
        /// 额外智力% [ 与技能和物品相关 ]
        /// </summary>
        public double ExINTPercentage { get; set; } = 0;

        /// <summary>
        /// 力量 = 基础力量 + 额外力量 + 额外力量2
        /// </summary>
        public double STR => BaseSTR + ExSTR + ExSTR2;

        /// <summary>
        /// 敏捷 = 基础敏捷 + 额外敏捷 + 额外敏捷2
        /// </summary>
        public double AGI => BaseAGI + ExAGI + ExAGI2;

        /// <summary>
        /// 智力 = 基础智力 + 额外智力 + 额外智力2
        /// </summary>
        public double INT => BaseINT + ExINT + ExINT2;

        /// <summary>
        /// 力量成长值(+BaseSTR/Lv)
        /// </summary>
        [InitOptional]
        public double STRGrowth { get; set; } = 0;

        /// <summary>
        /// 敏捷成长值(+BaseAGI/Lv)
        /// </summary>
        [InitOptional]
        public double AGIGrowth { get; set; } = 0;

        /// <summary>
        /// 智力成长值(+BaseINT/Lv)
        /// </summary>
        [InitOptional]
        public double INTGrowth { get; set; } = 0;

        /// <summary>
        /// 行动速度 [ 初始设定 ]
        /// </summary>
        [InitRequired]
        public double InitialSPD { get; set; } = 0;

        /// <summary>
        /// 行动速度 = [ 与初始设定相关 ][ 与敏捷相关 ] + 额外行动速度
        /// </summary>
        public double SPD => Math.Max(0, InitialSPD + AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD);

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
                double value = SPD / GameplayEquilibriumConstant.SPDUpperLimit + ExActionCoefficient;
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外行动系数(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExActionCoefficient { get; set; } = 0;

        /// <summary>
        /// 加速系数(%) = [ 与智力相关 ] + 额外加速系数(%)
        /// </summary>
        public double AccelerationCoefficient
        {
            get
            {
                double value = INT * GameplayEquilibriumConstant.INTtoAccelerationCoefficientMultiplier + ExAccelerationCoefficient;
                return Math.Min(1, value);
            }
        }

        /// <summary>
        /// 额外加速系数(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExAccelerationCoefficient { get; set; } = 0;

        /// <summary>
        /// 冷却缩减(%) = [ 与智力相关 ] + 额外冷却缩减(%)
        /// </summary>
        public double CDR
        {
            get
            {
                double value = INT * GameplayEquilibriumConstant.INTtoCDRMultiplier + ExCDR;
                return Math.Min(1, value);
            }
        }

        /// <summary>
        /// 额外冷却缩减(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExCDR { get; set; } = 0;

        /// <summary>
        /// 攻击距离 [ 与武器相关 ] [ 单位：格（半径） ]
        /// </summary>
        public int ATR
        {
            get
            {
                int baseATR = 1;
                if (EquipSlot.Weapon != null)
                {
                    baseATR = EquipSlot.Weapon.WeaponType switch
                    {
                        WeaponType.OneHandedSword => GameplayEquilibriumConstant.OneHandedSwordAttackRange,
                        WeaponType.TwoHandedSword => GameplayEquilibriumConstant.TwoHandedSwordAttackRange,
                        WeaponType.Bow => GameplayEquilibriumConstant.BowAttackRange,
                        WeaponType.Pistol => GameplayEquilibriumConstant.PistolAttackRange,
                        WeaponType.Rifle => GameplayEquilibriumConstant.RifleAttackRange,
                        WeaponType.DualDaggers => GameplayEquilibriumConstant.DualDaggersAttackRange,
                        WeaponType.Talisman => GameplayEquilibriumConstant.TalismanAttackRange,
                        WeaponType.Staff => GameplayEquilibriumConstant.StaffAttackRange,
                        WeaponType.Polearm => GameplayEquilibriumConstant.PolearmAttackRange,
                        WeaponType.Gauntlet => GameplayEquilibriumConstant.GauntletAttackRange,
                        WeaponType.HiddenWeapon => GameplayEquilibriumConstant.HiddenWeaponAttackRange,
                        _ => baseATR
                    };
                }
                return Math.Max(1, baseATR + ExATR);
            }
        }

        /// <summary>
        /// 额外攻击距离 [ 与技能和物品相关 ] [ 单位：格（半径） ]
        /// </summary>
        public int ExATR { get; set; } = 0;
        
        /// <summary>
        /// 行动力/可移动距离 [ 与第一定位相关 ] [ 单位：格（半径） ]
        /// </summary>
        public int MOV
        {
            get
            {
                int baseMOV = 3;
                if (EquipSlot.Weapon != null)
                {
                    baseMOV = FirstRoleType switch
                    {
                        RoleType.Core => GameplayEquilibriumConstant.RoleMOV_Core,
                        RoleType.Vanguard => GameplayEquilibriumConstant.RoleMOV_Vanguard,
                        RoleType.Guardian => GameplayEquilibriumConstant.RoleMOV_Guardian,
                        RoleType.Support => GameplayEquilibriumConstant.RoleMOV_Support,
                        RoleType.Medic => GameplayEquilibriumConstant.RoleMOV_Medic,
                        _ => baseMOV
                    };
                }
                return Math.Max(1, baseMOV + ExMOV);
            }
        }
        
        /// <summary>
        /// 行动力/可移动距离 [ 与技能和物品相关 ] [ 单位：格（半径） ]
        /// </summary>
        public int ExMOV { get; set; } = 0;
        
        /// <summary>
        /// 暴击率(%) = [ 与敏捷相关 ] + 额外暴击率(%)
        /// </summary>
        public double CritRate
        {
            get
            {
                double value = GameplayEquilibriumConstant.CritRate + AGI * GameplayEquilibriumConstant.AGItoCritRateMultiplier + ExCritRate;
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
                return GameplayEquilibriumConstant.CritDMG + STR * GameplayEquilibriumConstant.STRtoCritDMGMultiplier + ExCritDMG;
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
                double value = GameplayEquilibriumConstant.EvadeRate + AGI * GameplayEquilibriumConstant.AGItoEvadeRateMultiplier + ExEvadeRate;
                return Calculation.PercentageCheck(value);
            }
        }

        /// <summary>
        /// 额外闪避率(%) [ 与技能和物品相关 ]
        /// </summary>
        public double ExEvadeRate { get; set; } = 0;

        /// <summary>
        /// 生命偷取 [ 与技能和物品相关 ]
        /// </summary>
        public double Lifesteal { get; set; } = 0;

        /// <summary>
        /// 护盾值 [ 与技能和物品相关 ]
        /// </summary>
        public Shield Shield { get; set; }

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
            InitialHP = GameplayEquilibriumConstant.InitialHP;
            InitialMP = GameplayEquilibriumConstant.InitialMP;
            InitialATK = GameplayEquilibriumConstant.InitialATK;
            InitialDEF = GameplayEquilibriumConstant.InitialDEF;
            EquipSlot = new();
            MDF = new();
            Shield = new();
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
                HP += HR * time;
                MP += MR * time;
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
            if (pastHP > 0 && pastMaxHP > 0)
            {
                double pHP = pastHP / pastMaxHP;
                HP = MaxHP * pHP;
            }
            if (pastMP > 0 && pastMaxMP > 0)
            {
                double pMP = pastMP / pastMaxMP;
                MP = MaxMP * pMP;
            }
        }

        /// <summary>
        /// 为角色装备物品（必须使用此方法而不是自己去给EquipSlot里的物品赋值）<para/>
        /// 此方法装备到指定栏位，并返回被替换的装备（如果有的话）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <param name="previous"></param>
        public bool Equip(Item item, EquipSlotType slot, out Item? previous)
        {
            previous = null;
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
                        previous = UnEquip(EquipSlotType.MagicCardPack);
                        EquipSlot.MagicCardPack = item;
                        item.OnItemEquip(this, EquipSlotType.MagicCardPack);
                        result = true;
                    }
                    break;
                case EquipSlotType.Weapon:
                    if (item.ItemType == ItemType.Weapon)
                    {
                        previous = UnEquip(EquipSlotType.Weapon);
                        EquipSlot.Weapon = item;
                        item.OnItemEquip(this, EquipSlotType.Weapon);
                        result = true;
                    }
                    break;
                case EquipSlotType.Armor:
                    if (item.ItemType == ItemType.Armor)
                    {
                        previous = UnEquip(EquipSlotType.Armor);
                        EquipSlot.Armor = item;
                        item.OnItemEquip(this, EquipSlotType.Armor);
                        result = true;
                    }
                    break;
                case EquipSlotType.Shoes:
                    if (item.ItemType == ItemType.Shoes)
                    {
                        previous = UnEquip(EquipSlotType.Shoes);
                        EquipSlot.Shoes = item;
                        item.OnItemEquip(this, EquipSlotType.Shoes);
                        result = true;
                    }
                    break;
                case EquipSlotType.Accessory1:
                    if (item.ItemType == ItemType.Accessory)
                    {
                        previous = UnEquip(EquipSlotType.Accessory1);
                        EquipSlot.Accessory1 = item;
                        EquipSlot.LastEquipSlotType = EquipSlotType.Accessory1;
                        item.OnItemEquip(this, EquipSlotType.Accessory1);
                        result = true;
                    }
                    break;
                case EquipSlotType.Accessory2:
                    if (item.ItemType == ItemType.Accessory)
                    {
                        previous = UnEquip(EquipSlotType.Accessory2);
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
                    return Equip(item, EquipSlotType.MagicCardPack, out _);
                case ItemType.Weapon:
                    return Equip(item, EquipSlotType.Weapon, out _);
                case ItemType.Armor:
                    return Equip(item, EquipSlotType.Armor, out _);
                case ItemType.Shoes:
                    return Equip(item, EquipSlotType.Shoes, out _);
                case ItemType.Accessory:
                    if (EquipSlot.Accessory1 is null)
                    {
                        return Equip(item, EquipSlotType.Accessory1, out _);
                    }
                    else if (EquipSlot.Accessory1 != null && EquipSlot.Accessory2 is null)
                    {
                        return Equip(item, EquipSlotType.Accessory2, out _);
                    }
                    else if (EquipSlot.Accessory1 != null && EquipSlot.Accessory2 != null && EquipSlot.LastEquipSlotType == EquipSlotType.Accessory1)
                    {
                        return Equip(item, EquipSlotType.Accessory2, out _);
                    }
                    else
                    {
                        return Equip(item, EquipSlotType.Accessory1, out _);
                    }
            }
            return false;
        }

        /// <summary>
        /// 为角色装备物品（必须使用此方法而不是自己去给EquipSlot里的物品赋值）<para/>
        /// 此方法为根据物品类型，优先空位自动装备<para/>
        /// 此方法可返回被替换的装备（如果有的话）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previous"></param>
        public bool Equip(Item item, out Item? previous)
        {
            previous = null;
            switch (item.ItemType)
            {
                case ItemType.MagicCardPack:
                    return Equip(item, EquipSlotType.MagicCardPack, out previous);
                case ItemType.Weapon:
                    return Equip(item, EquipSlotType.Weapon, out previous);
                case ItemType.Armor:
                    return Equip(item, EquipSlotType.Armor, out previous);
                case ItemType.Shoes:
                    return Equip(item, EquipSlotType.Shoes, out previous);
                case ItemType.Accessory:
                    if (EquipSlot.Accessory1 is null)
                    {
                        return Equip(item, EquipSlotType.Accessory1, out previous);
                    }
                    else if (EquipSlot.Accessory1 != null && EquipSlot.Accessory2 is null)
                    {
                        return Equip(item, EquipSlotType.Accessory2, out previous);
                    }
                    else if (EquipSlot.Accessory1 != null && EquipSlot.Accessory2 != null && EquipSlot.LastEquipSlotType == EquipSlotType.Accessory1)
                    {
                        return Equip(item, EquipSlotType.Accessory2, out previous);
                    }
                    else
                    {
                        return Equip(item, EquipSlotType.Accessory1, out previous);
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
            double pastHP = HP;
            double pastMaxHP = MaxHP;
            double pastMP = MP;
            double pastMaxMP = MaxMP;
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
            if (result != null)
            {
                OnAttributeChanged();
                Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
            }
            return result;
        }

        /// <summary>
        /// 设置角色等级，并默认完全回复状态
        /// </summary>
        /// <param name="level">新的等级</param>
        /// <param name="recovery">false 为按百分比回复</param>
        public void SetLevel(int level, bool recovery = true)
        {
            if (!recovery)
            {
                double pastHP = HP;
                double pastMP = MP;
                double pastMaxHP = MaxHP;
                double pastMaxMP = MaxMP;
                int pastLevel = Level;
                Level = level;
                if (pastLevel != Level)
                {
                    Recovery(pastHP, pastMP, pastMaxHP, pastMaxMP);
                }
            }
            else
            {
                Level = level;
            }
        }

        /// <summary>
        /// 角色升级
        /// </summary>
        /// <param name="level"></param>
        /// <param name="checkLevelBreak"></param>
        public void OnLevelUp(int level = 0, bool checkLevelBreak = true)
        {
            bool isUp = false;
            int count = 0;
            while (true)
            {
                // 传入 level 表示最多升级多少次，0 为用完所有溢出的经验值
                if (level != 0 && count++ >= level)
                {
                    break;
                }
                if (GameplayEquilibriumConstant.UseLevelBreak && checkLevelBreak)
                {
                    // 检查角色突破进度
                    int[] breaks = [.. GameplayEquilibriumConstant.LevelBreakList];
                    int nextBreak = LevelBreak + 1;
                    if (nextBreak < breaks.Length && Level >= breaks[nextBreak])
                    {
                        // 需要突破才能继续升级
                        break;
                    }
                }
                if (Level > 0 && Level < GameplayEquilibriumConstant.MaxLevel && GameplayEquilibriumConstant.EXPUpperLimit.TryGetValue(Level, out double need) && EXP >= need)
                {
                    EXP -= need;
                    Level++;
                    isUp = true;
                    OnAttributeChanged();
                    Recovery();
                }
                else
                {
                    break;
                }
            }
            if (isUp)
            {
                Effect[] effects = [.. Effects.Where(e => e.IsInEffect)];
                foreach (Effect e in effects)
                {
                    e.OnOwnerLevelUp(this, Level);
                }
            }
        }

        /// <summary>
        /// 角色突破，允许继续升级
        /// </summary>
        public void OnLevelBreak()
        {
            if (GameplayEquilibriumConstant.UseLevelBreak)
            {
                // 检查角色突破进度
                int[] levels = [.. GameplayEquilibriumConstant.LevelBreakList];
                while (LevelBreak + 1 < levels.Length && Level >= levels[LevelBreak + 1])
                {
                    LevelBreak++;
                }
            }
        }

        /// <summary>
        /// 角色的属性发生变化，会影响特殊效果的计算
        /// </summary>
        public void OnAttributeChanged()
        {
            List<Effect> effects = [.. Effects.Where(e => e.IsInEffect)];
            foreach (Effect effect in effects)
            {
                effect.OnAttributeChanged(this);
            }
            NormalAttack.SetMagicType(null, null, null);
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
        /// 获取角色实例的昵称以及所属玩家，如果没有昵称，则用名字代替
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = NickName != "" ? NickName : GetName();
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
        /// 获取角色实例的名字、昵称以及所属玩家
        /// </summary>
        /// <returns></returns>
        public string ToStringWithUser()
        {
            string str = GetName();
            if (NickName != "")
            {
                if (str != "") str += ", ";
                str += NickName;
            }
            if (User != null && User.Username != "")
            {
                str += "（" + User.Username + "）";
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
            str += " - 等级 " + Level;
            if (User != null && User.Username != "")
            {
                str += "（" + User.Username + "）";
            }
            return str;
        }

        /// <summary>
        /// 获取角色实例的名字、昵称以及等级
        /// </summary>
        /// <returns></returns>
        public string ToStringWithLevelWithOutUser()
        {
            string str = GetName();
            if (NickName != "")
            {
                if (str != "") str += ", ";
                str += NickName;
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
        public string GetInfo(bool showUser = true, bool showGrowth = true, bool showEXP = false, bool showMapRelated = false)
        {
            StringBuilder builder = new();

            builder.AppendLine((HP == 0 ? "[ 死亡 ] " : "") + (showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser()));
            if (showEXP)
            {
                builder.AppendLine($"等级：{Level} / {GameplayEquilibriumConstant.MaxLevel}（突破进度：{LevelBreak + 1} / {GameplayEquilibriumConstant.LevelBreakList.Count}）");
                builder.AppendLine($"经验值：{EXP:0.##}{(Level != GameplayEquilibriumConstant.MaxLevel && GameplayEquilibriumConstant.EXPUpperLimit.TryGetValue(Level, out double need) ? " / " + need : "")}");
            }
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.TotalMix > 0) shield.Add($"混合：{Shield.TotalMix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {GameplayEquilibriumConstant.MaxEP:0.##}");
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            double exDEF = ExDEF + ExDEF2 + ExDEF3;
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (exDEF != 0 ? $" [{BaseDEF:0.##} {(exDEF >= 0 ? "+" : "-")} {Math.Abs(exDEF):0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            builder.AppendLine(GetMagicResistanceInfo().Trim());
            double exSPD = AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
            builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD != 0 ? $" [{InitialSPD:0.##} {(exSPD >= 0 ? "+" : "-")} {Math.Abs(exSPD):0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
            builder.AppendLine($"核心属性：{CharacterSet.GetPrimaryAttributeName(PrimaryAttribute)}");
            double exSTR = ExSTR + ExSTR2;
            builder.AppendLine($"力量：{STR:0.##}" + (exSTR != 0 ? $" [{BaseSTR:0.##} {(exSTR >= 0 ? "+" : "-")} {Math.Abs(exSTR):0.##}]" : "") + (showGrowth ? $"（{(STRGrowth >= 0 ? "+" : "-")}{Math.Abs(STRGrowth)}/Lv）" : ""));
            double exAGI = ExAGI + ExAGI2;
            builder.AppendLine($"敏捷：{AGI:0.##}" + (exAGI != 0 ? $" [{BaseAGI:0.##} {(exAGI >= 0 ? "+" : "-")} {Math.Abs(exAGI):0.##}]" : "") + (showGrowth ? $"（{(AGIGrowth >= 0 ? "+" : "-")}{Math.Abs(AGIGrowth)}/Lv）" : ""));
            double exINT = ExINT + ExINT2;
            builder.AppendLine($"智力：{INT:0.##}" + (exINT != 0 ? $" [{BaseINT:0.##} {(exINT >= 0 ? "+" : "-")} {Math.Abs(exINT):0.##}]" : "") + (showGrowth ? $"（{(INTGrowth >= 0 ? "+" : "-")}{Math.Abs(INTGrowth)}/Lv）" : ""));
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR != 0 ? $" [{InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor:0.##} {(ExHR >= 0 ? "+" : "-")} {Math.Abs(ExHR):0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR != 0 ? $" [{InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor:0.##} {(ExMR >= 0 ? "+" : "-")} {Math.Abs(ExMR):0.##}]" : ""));
            builder.AppendLine($"暴击率：{CritRate * 100:0.##}%");
            builder.AppendLine($"暴击伤害：{CritDMG * 100:0.##}%");
            builder.AppendLine($"闪避率：{EvadeRate * 100:0.##}%");
            builder.AppendLine($"生命偷取：{Lifesteal * 100:0.##}%");
            builder.AppendLine($"冷却缩减：{CDR * 100:0.##}%");
            builder.AppendLine($"加速系数：{AccelerationCoefficient * 100:0.##}%");
            builder.AppendLine($"物理穿透：{PhysicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法穿透：{MagicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法消耗减少：{INT * GameplayEquilibriumConstant.INTtoCastMPReduce * 100:0.##}%");
            builder.AppendLine($"能量消耗减少：{INT * GameplayEquilibriumConstant.INTtoCastEPReduce * 100:0.##}%");

            if (showMapRelated)
            {
                builder.AppendLine($"移动距离：{MOV}");
                builder.AppendLine($"攻击距离：{ATR}");
            }

            GetStatusInfo(builder);

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
                builder.AppendLine(GetEquipSlotInfo().Trim());
            }

            if (Items.Count > 0)
            {
                builder.AppendLine(GetBackpackItemsInfo().Trim());
            }

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in effects)
                {
                    builder.AppendLine(effect.ToString());
                }
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// 获取角色的简略信息
        /// </summary>
        /// <returns></returns>
        public string GetSimpleInfo(bool showUser = true, bool showGrowth = true, bool showEXP = false, bool showBasicOnly = false, bool showMapRelated = false)
        {
            StringBuilder builder = new();

            builder.AppendLine((HP == 0 ? "[ 死亡 ] " : "") + (showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser()));
            if (showEXP)
            {
                builder.AppendLine($"等级：{Level} / {GameplayEquilibriumConstant.MaxLevel}（突破进度：{LevelBreak + 1} / {GameplayEquilibriumConstant.LevelBreakList.Count}）");
                builder.AppendLine($"经验值：{EXP:0.##}{(Level != GameplayEquilibriumConstant.MaxLevel && GameplayEquilibriumConstant.EXPUpperLimit.TryGetValue(Level, out double need) ? " / " + need : "")}");
            }
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.TotalMix > 0) shield.Add($"混合：{Shield.TotalMix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {GameplayEquilibriumConstant.MaxEP:0.##}");
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            double exDEF = ExDEF + ExDEF2 + ExDEF3;
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (exDEF != 0 ? $" [{BaseDEF:0.##} {(exDEF >= 0 ? "+" : "-")} {Math.Abs(exDEF):0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            builder.AppendLine(GetMagicResistanceInfo().Trim());
            if (showBasicOnly)
            {
                builder.AppendLine($"核心属性：{PrimaryAttributeValue:0.##}（{CharacterSet.GetPrimaryAttributeName(PrimaryAttribute)}）");
            }
            else
            {
                double exSPD = AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
                builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD != 0 ? $" [{InitialSPD:0.##} {(exSPD >= 0 ? "+" : "-")} {Math.Abs(exSPD):0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
                builder.AppendLine($"核心属性：{CharacterSet.GetPrimaryAttributeName(PrimaryAttribute)}");
                double exSTR = ExSTR + ExSTR2;
                builder.AppendLine($"力量：{STR:0.##}" + (exSTR != 0 ? $" [{BaseSTR:0.##} {(exSTR >= 0 ? "+" : "-")} {Math.Abs(exSTR):0.##}]" : "") + (showGrowth ? $"（{(STRGrowth >= 0 ? "+" : "-")}{Math.Abs(STRGrowth)}/Lv）" : ""));
                double exAGI = ExAGI + ExAGI2;
                builder.AppendLine($"敏捷：{AGI:0.##}" + (exAGI != 0 ? $" [{BaseAGI:0.##} {(exAGI >= 0 ? "+" : "-")} {Math.Abs(exAGI):0.##}]" : "") + (showGrowth ? $"（{(AGIGrowth >= 0 ? "+" : "-")}{Math.Abs(AGIGrowth)}/Lv）" : ""));
                double exINT = ExINT + ExINT2;
                builder.AppendLine($"智力：{INT:0.##}" + (exINT != 0 ? $" [{BaseINT:0.##} {(exINT >= 0 ? "+" : "-")} {Math.Abs(exINT):0.##}]" : "") + (showGrowth ? $"（{(INTGrowth >= 0 ? "+" : "-")}{Math.Abs(INTGrowth)}/Lv）" : ""));
            }
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR != 0 ? $" [{InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor:0.##} {(ExHR >= 0 ? "+" : "-")} {Math.Abs(ExHR):0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR != 0 ? $" [{InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor:0.##} {(ExMR >= 0 ? "+" : "-")} {Math.Abs(ExMR):0.##}]" : ""));

            if (showMapRelated)
            {
                builder.AppendLine($"移动距离：{MOV}");
                builder.AppendLine($"攻击距离：{ATR}");
            }

            if (!showBasicOnly)
            {
                GetStatusInfo(builder);

                if (Skills.Count > 0)
                {
                    builder.AppendLine("== 角色技能 ==");
                    builder.AppendLine(string.Join("，", Skills.Select(s => s.Name)));
                }

                if (EquipSlot.Any())
                {
                    builder.AppendLine("== 已装备槽位 ==");
                    List<EquipSlotType> types = [];
                    if (EquipSlot.MagicCardPack != null)
                    {
                        types.Add(EquipSlotType.MagicCardPack);
                    }
                    if (EquipSlot.Weapon != null)
                    {
                        types.Add(EquipSlotType.Weapon);
                    }
                    if (EquipSlot.Armor != null)
                    {
                        types.Add(EquipSlotType.Armor);
                    }
                    if (EquipSlot.Shoes != null)
                    {
                        types.Add(EquipSlotType.Shoes);
                    }
                    if (EquipSlot.Accessory1 != null)
                    {
                        types.Add(EquipSlotType.Accessory1);
                    }
                    if (EquipSlot.Accessory2 != null)
                    {
                        types.Add(EquipSlotType.Accessory2);
                    }
                    builder.AppendLine(string.Join("，", types.Select(ItemSet.GetEquipSlotTypeName)));
                }

                Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
                if (effects.Length > 0)
                {
                    builder.AppendLine("== 状态栏 ==");
                    builder.Append(string.Join("，", effects.Select(e => e.Name)));
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

            builder.AppendLine((HP == 0 ? "[ 死亡 ] " : "") + ToStringWithLevel());
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.TotalMix > 0) shield.Add($"混合：{Shield.TotalMix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {GameplayEquilibriumConstant.MaxEP:0.##}");
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            builder.AppendLine($"核心属性：{PrimaryAttributeValue:0.##}" + (ExPrimaryAttributeValue != 0 ? $" [{BasePrimaryAttributeValue:0.##} {(ExPrimaryAttributeValue >= 0 ? "+" : "-")} {Math.Abs(ExPrimaryAttributeValue):0.##}]" : ""));

            GetStatusInfo(builder);

            builder.AppendLine($"硬直时间：{hardnessTimes:0.##}");

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in effects)
                {
                    builder.AppendLine(effect.ToString());
                }
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// 获取战斗状态的信息（简略版）
        /// </summary>
        /// <param name="hardnessTimes"></param>
        /// <returns></returns>
        public string GetSimpleInBattleInfo(double hardnessTimes)
        {
            StringBuilder builder = new();

            builder.AppendLine((HP == 0 ? "[ 死亡 ] " : "") + ToStringWithLevel());
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.TotalMix > 0) shield.Add($"混合：{Shield.TotalMix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {GameplayEquilibriumConstant.MaxEP:0.##}");
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            builder.AppendLine($"核心属性：{PrimaryAttributeValue:0.##}" + (ExPrimaryAttributeValue != 0 ? $" [{BasePrimaryAttributeValue:0.##} {(ExPrimaryAttributeValue >= 0 ? "+" : "-")} {Math.Abs(ExPrimaryAttributeValue):0.##}]" : ""));
            builder.AppendLine($"硬直时间：{hardnessTimes:0.##}");

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                builder.Append(string.Join("，", effects.Select(e => e.Name)));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取角色的技能信息
        /// </summary>
        /// <returns></returns>
        public string GetSkillInfo(bool showUser = true)
        {
            StringBuilder builder = new();

            builder.AppendLine((HP == 0 ? "[ 死亡 ] " : "") + (showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser()));

            GetStatusInfo(builder);

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

            Effect[] effects = [.. Effects.Where(e => e.ShowInStatusBar)];
            if (effects.Length > 0)
            {
                builder.AppendLine("== 状态栏 ==");
                foreach (Effect effect in effects)
                {
                    builder.AppendLine(effect.ToString());
                }
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// 获取角色的物品信息
        /// </summary>
        /// <returns></returns>
        public string GetItemInfo(bool showUser = true, bool showGrowth = true, bool showEXP = false, bool showMapRelated = false)
        {
            StringBuilder builder = new();

            builder.AppendLine((HP == 0 ? "[ 死亡 ] " : "") + (showUser ? ToStringWithLevel() : ToStringWithLevelWithOutUser()));
            if (showEXP)
            {
                builder.AppendLine($"等级：{Level} / {GameplayEquilibriumConstant.MaxLevel}（突破进度：{LevelBreak + 1} / {GameplayEquilibriumConstant.LevelBreakList.Count}）");
                builder.AppendLine($"经验值：{EXP:0.##}{(Level != GameplayEquilibriumConstant.MaxLevel && GameplayEquilibriumConstant.EXPUpperLimit.TryGetValue(Level, out double need) ? " / " + need : "")}");
            }
            double exHP = ExHP + ExHP2 + ExHP3;
            List<string> shield = [];
            if (Shield.TotalPhysical > 0) shield.Add($"物理：{Shield.TotalPhysical:0.##}");
            if (Shield.TotalMagical > 0) shield.Add($"魔法：{Shield.TotalMagical:0.##}");
            if (Shield.TotalMix > 0) shield.Add($"混合：{Shield.TotalMix:0.##}");
            builder.AppendLine($"生命值：{HP:0.##} / {MaxHP:0.##}" + (exHP != 0 ? $" [{BaseHP:0.##} {(exHP >= 0 ? "+" : "-")} {Math.Abs(exHP):0.##}]" : "") + (shield.Count > 0 ? $"（{string.Join("，", shield)}）" : ""));
            double exMP = ExMP + ExMP2 + ExMP3;
            builder.AppendLine($"魔法值：{MP:0.##} / {MaxMP:0.##}" + (exMP != 0 ? $" [{BaseMP:0.##} {(exMP >= 0 ? "+" : "-")} {Math.Abs(exMP):0.##}]" : ""));
            builder.AppendLine($"能量值：{EP:0.##} / {GameplayEquilibriumConstant.MaxEP:0.##}");
            double exATK = ExATK + ExATK2 + ExATK3;
            builder.AppendLine($"攻击力：{ATK:0.##}" + (exATK != 0 ? $" [{BaseATK:0.##} {(exATK >= 0 ? "+" : "-")} {Math.Abs(exATK):0.##}]" : ""));
            double exDEF = ExDEF + ExDEF2 + ExDEF3;
            builder.AppendLine($"物理护甲：{DEF:0.##}" + (exDEF != 0 ? $" [{BaseDEF:0.##} {(exDEF >= 0 ? "+" : "-")} {Math.Abs(exDEF):0.##}]" : "") + $" ({PDR * 100:0.##}%)");
            builder.AppendLine(GetMagicResistanceInfo().Trim());
            double exSPD = AGI * GameplayEquilibriumConstant.AGItoSPDMultiplier + ExSPD;
            builder.AppendLine($"行动速度：{SPD:0.##}" + (exSPD != 0 ? $" [{InitialSPD:0.##} {(exSPD >= 0 ? "+" : "-")} {Math.Abs(exSPD):0.##}]" : "") + $" ({ActionCoefficient * 100:0.##}%)");
            builder.AppendLine($"核心属性：{CharacterSet.GetPrimaryAttributeName(PrimaryAttribute)}");
            double exSTR = ExSTR + ExSTR2;
            builder.AppendLine($"力量：{STR:0.##}" + (exSTR != 0 ? $" [{BaseSTR:0.##} {(exSTR >= 0 ? "+" : "-")} {Math.Abs(exSTR):0.##}]" : "") + (showGrowth ? $"（{(STRGrowth >= 0 ? "+" : "-")}{Math.Abs(STRGrowth)}/Lv）" : ""));
            double exAGI = ExAGI + ExAGI2;
            builder.AppendLine($"敏捷：{AGI:0.##}" + (exAGI != 0 ? $" [{BaseAGI:0.##} {(exAGI >= 0 ? "+" : "-")} {Math.Abs(exAGI):0.##}]" : "") + (showGrowth ? $"（{(AGIGrowth >= 0 ? "+" : "-")}{Math.Abs(AGIGrowth)}/Lv）" : ""));
            double exINT = ExINT + ExINT2;
            builder.AppendLine($"智力：{INT:0.##}" + (exINT != 0 ? $" [{BaseINT:0.##} {(exINT >= 0 ? "+" : "-")} {Math.Abs(exINT):0.##}]" : "") + (showGrowth ? $"（{(INTGrowth >= 0 ? "+" : "-")}{Math.Abs(INTGrowth)}/Lv）" : ""));
            builder.AppendLine($"生命回复：{HR:0.##}" + (ExHR != 0 ? $" [{InitialHR + STR * GameplayEquilibriumConstant.STRtoHRFactor:0.##} {(ExHR >= 0 ? "+" : "-")} {Math.Abs(ExHR):0.##}]" : ""));
            builder.AppendLine($"魔法回复：{MR:0.##}" + (ExMR != 0 ? $" [{InitialMR + INT * GameplayEquilibriumConstant.INTtoMRFactor:0.##} {(ExMR >= 0 ? "+" : "-")} {Math.Abs(ExMR):0.##}]" : ""));
            builder.AppendLine($"暴击率：{CritRate * 100:0.##}%");
            builder.AppendLine($"暴击伤害：{CritDMG * 100:0.##}%");
            builder.AppendLine($"闪避率：{EvadeRate * 100:0.##}%");
            builder.AppendLine($"生命偷取：{Lifesteal * 100:0.##}%");
            builder.AppendLine($"冷却缩减：{CDR * 100:0.##}%");
            builder.AppendLine($"加速系数：{AccelerationCoefficient * 100:0.##}%");
            builder.AppendLine($"物理穿透：{PhysicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法穿透：{MagicalPenetration * 100:0.##}%");
            builder.AppendLine($"魔法消耗减少：{INT * GameplayEquilibriumConstant.INTtoCastMPReduce * 100:0.##}%");
            builder.AppendLine($"能量消耗减少：{INT * GameplayEquilibriumConstant.INTtoCastEPReduce * 100:0.##}%");

            if (showMapRelated)
            {
                builder.AppendLine($"移动距离：{MOV}");
                builder.AppendLine($"攻击距离：{ATR}");
            }

            if (EquipSlot.Any())
            {
                builder.AppendLine(GetEquipSlotInfo().Trim());
            }

            if (Items.Count > 0)
            {
                builder.AppendLine(GetBackpackItemsInfo().Trim());
            }

            return builder.ToString();
        }

        private void GetStatusInfo(StringBuilder builder)
        {
            if (CharacterState != CharacterState.Actionable)
            {
                builder.AppendLine(CharacterSet.GetCharacterState(CharacterState));
            }

            if ((ImmuneType & ImmuneType.Physical) != ImmuneType.None)
            {
                builder.AppendLine("角色现在物理免疫");
            }

            if ((ImmuneType & ImmuneType.Magical) != ImmuneType.None)
            {
                builder.AppendLine("角色现在魔法免疫");
            }

            if ((ImmuneType & ImmuneType.Skilled) != ImmuneType.None)
            {
                builder.AppendLine("角色现在技能免疫");
            }

            if ((ImmuneType & ImmuneType.All) != ImmuneType.None)
            {
                builder.AppendLine("角色现在完全免疫");
            }

            if (IsNeutral)
            {
                builder.AppendLine("角色是无敌的");
            }

            if (IsUnselectable)
            {
                builder.AppendLine("角色是不可选中的");
            }
        }

        /// <summary>
        /// 获取角色装备栏信息
        /// </summary>
        /// <returns></returns>
        public string GetEquipSlotInfo()
        {
            StringBuilder builder = new();

            builder.AppendLine("== 装备栏 ==");
            if (EquipSlot.MagicCardPack != null)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.MagicCardPack.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.MagicCardPack) + "：" + EquipSlot.MagicCardPack.Name);
                builder.AppendLine(EquipSlot.MagicCardPack.Description);
            }
            if (EquipSlot.Weapon != null)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Weapon.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Weapon) + "：" + EquipSlot.Weapon.Name);
                builder.AppendLine(EquipSlot.Weapon.Description);
            }
            if (EquipSlot.Armor != null)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Armor.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Armor) + "：" + EquipSlot.Armor.Name);
                builder.AppendLine(EquipSlot.Armor.Description);
            }
            if (EquipSlot.Shoes != null)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Shoes.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Shoes) + "：" + EquipSlot.Shoes.Name);
                builder.AppendLine(EquipSlot.Shoes.Description);
            }
            if (EquipSlot.Accessory1 != null)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Accessory1.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory1) + "：" + EquipSlot.Accessory1.Name);
                builder.AppendLine(EquipSlot.Accessory1.Description);
            }
            if (EquipSlot.Accessory2 != null)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(EquipSlot.Accessory2.QualityType)}]" + ItemSet.GetEquipSlotTypeName(EquipSlotType.Accessory2) + "：" + EquipSlot.Accessory2.Name);
                builder.AppendLine(EquipSlot.Accessory2.Description);
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// 获取角色背包信息
        /// </summary>
        /// <returns></returns>
        public string GetBackpackItemsInfo()
        {
            StringBuilder builder = new();

            builder.AppendLine("== 角色背包 ==");
            foreach (Item item in Items)
            {
                builder.AppendLine($"[{ItemSet.GetQualityTypeName(item.QualityType)}]" + ItemSet.GetItemTypeName(item.ItemType) + "：" + item.Name);
                builder.AppendLine(item.Description);
                if (item.Skills.Active != null)
                {
                    Skill skill = item.Skills.Active;
                    List<string> strings = [];
                    if (skill.RealMPCost > 0) strings.Add($"魔法消耗：{skill.RealMPCost}");
                    if (skill.RealEPCost > 0) strings.Add($"能量消耗：{skill.RealEPCost}");
                    if (skill.RealCD > 0) strings.Add($"冷却时间：{skill.RealCD}{(skill.CurrentCD > 0 ? $"（正在冷却：剩余 {skill.CurrentCD} {GameplayEquilibriumConstant.InGameTime}）" : "")}");
                    if (skill.RealHardnessTime > 0) strings.Add($"硬直时间：{skill.RealHardnessTime}");
                    builder.AppendLine($"技能【{skill.Name}】描述：{skill.Description.Trim()}{(strings.Count > 0 ? $"（{string.Join("；", strings)}）" : "")}");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取魔法抗性信息
        /// </summary>
        /// <returns></returns>
        public string GetMagicResistanceInfo()
        {
            StringBuilder builder = new();

            if (GameplayEquilibriumConstant.UseMagicType.Count > 0)
            {
                foreach (MagicType magicType in GameplayEquilibriumConstant.UseMagicType)
                {
                    builder.Append(CharacterSet.GetMagicResistanceName(magicType));
                    builder.AppendLine($"：{Calculation.Round4Digits(MDF[magicType] * 100):0.##}%");
                }
            }
            else builder.AppendLine($"魔法抗性：{Calculation.Round4Digits(MDF.Avg * 100):0.##}%（平均）");

            return builder.ToString();
        }

        /// <summary>
        /// 更新角色的状态，参见 <see cref="CharacterEffectStates"/>、<see cref="CharacterEffectTypes"/> 用法
        /// </summary>
        /// <returns></returns>
        public CharacterState UpdateCharacterState()
        {
            IEnumerable<CharacterState> states = CharacterEffectStates.Values.SelectMany(list => list);
            // 根据持有的特效判断角色所处的状态
            bool isNotActionable = states.Any(state => state == CharacterState.NotActionable);
            bool isActionRestricted = states.Any(state => state == CharacterState.ActionRestricted);
            bool isBattleRestricted = states.Any(state => state == CharacterState.BattleRestricted);
            bool isSkillRestricted = states.Any(state => state == CharacterState.SkillRestricted);
            bool isAttackRestricted = states.Any(state => state == CharacterState.AttackRestricted);

            IEnumerable<EffectType> types = CharacterEffectTypes.Values.SelectMany(list => list);
            // 判断角色的控制效果
            IsUnselectable = types.Any(type => type == EffectType.Unselectable);

            IEnumerable<ImmuneType> immunes = CharacterImmuneTypes.Values.SelectMany(list => list);
            // 判断角色的免疫状态，需要注意的是 All 不会覆盖任何其他类型，因为它是一种独立的类型
            bool isAllImmune = immunes.Any(type => type == ImmuneType.All);
            bool isPhysicalImmune = immunes.Any(type => type == ImmuneType.Physical);
            bool isMagicalImmune = immunes.Any(type => type == ImmuneType.Magical);
            bool isSkilledImmune = immunes.Any(type => type == ImmuneType.Skilled);
            ImmuneType = ImmuneType.None;
            if (isAllImmune)
            {
                ImmuneType |= ImmuneType.All;
            }
            if (isPhysicalImmune)
            {
                ImmuneType |= ImmuneType.Physical;
            }
            if (isMagicalImmune)
            {
                ImmuneType |= ImmuneType.Magical;
            }
            if (isSkilledImmune)
            {
                ImmuneType |= ImmuneType.Skilled;
            }

            bool isControl = isNotActionable || isActionRestricted || isBattleRestricted || isSkillRestricted || isAttackRestricted;
            bool isCasting = CharacterState == CharacterState.Casting;
            bool isPreCastSuperSkill = CharacterState == CharacterState.PreCastSuperSkill;

            // 预释放爆发技不可驱散，保持原状态
            if (!isPreCastSuperSkill)
            {
                if (isNotActionable)
                {
                    CharacterState = CharacterState.NotActionable;
                }
                else if (isActionRestricted)
                {
                    CharacterState = CharacterState.ActionRestricted;
                }
                else if (isBattleRestricted || (isSkillRestricted && isAttackRestricted))
                {
                    CharacterState = CharacterState.BattleRestricted;
                }
                else if (isSkillRestricted)
                {
                    CharacterState = CharacterState.SkillRestricted;
                }
                else if (isAttackRestricted)
                {
                    CharacterState = CharacterState.AttackRestricted;
                }

                if (!isControl && !isCasting)
                {
                    CharacterState = CharacterState.Actionable;
                }
            }

            return CharacterState;
        }

        /// <summary>
        /// 复制一个角色
        /// [ 推荐从模组中复制后使用对象 ]
        /// </summary>
        /// <returns></returns>
        public Character Copy(bool copyEx = false, bool copyMagic = false, bool copyItem = false)
        {
            Character c = new()
            {
                Id = Id,
                Name = Name,
                Guid = Guid,
                FirstName = FirstName,
                NickName = NickName,
                Profile = Profile.Copy(),
                FirstRoleType = FirstRoleType,
                SecondRoleType = SecondRoleType,
                ThirdRoleType = ThirdRoleType,
                Promotion = Promotion,
                PrimaryAttribute = PrimaryAttribute,
                Level = Level,
                LevelBreak = LevelBreak,
                EXP = EXP,
                InitialHP = InitialHP,
                InitialMP = InitialMP,
                EP = EP,
                InitialATK = InitialATK,
                InitialDEF = InitialDEF,
                InitialHR = InitialHR,
                InitialMR = InitialMR,
                ER = ER,
                InitialSTR = InitialSTR,
                InitialAGI = InitialAGI,
                InitialINT = InitialINT,
                STRGrowth = STRGrowth,
                AGIGrowth = AGIGrowth,
                INTGrowth = INTGrowth,
                InitialSPD = InitialSPD
            };
            if (copyEx)
            {
                c.ExHP2 = ExHP2;
                c.ExHPPercentage = ExHPPercentage;
                c.ExMP2 = ExMP2;
                c.ExMPPercentage = ExMPPercentage;
                c.ExATK2 = ExATK2;
                c.ExATKPercentage = ExATKPercentage;
                c.ExDEF2 = ExDEF2;
                c.ExDEFPercentage = ExDEFPercentage;
                c.ExHR = ExHR;
                c.ExMR = ExMR;
                c.ExSTRPercentage = ExSTRPercentage;
                c.ExAGIPercentage = ExAGIPercentage;
                c.ExINTPercentage = ExINTPercentage;
                c.ExSTR = ExSTR;
                c.ExAGI = ExAGI;
                c.ExINT = ExINT;
                c.ExSPD = ExSPD;
                c.ExActionCoefficient = ExActionCoefficient;
                c.ExAccelerationCoefficient = ExAccelerationCoefficient;
                c.ExCDR = ExCDR;
                c.ExCritRate = ExCritRate;
                c.ExCritDMG = ExCritDMG;
                c.ExEvadeRate = ExEvadeRate;
                c.PhysicalPenetration = PhysicalPenetration;
                c.MagicalPenetration = MagicalPenetration;
                c.MDF = MDF.Copy();
                c.Lifesteal = Lifesteal;
                c.Shield = Shield.Copy();
                c.ExATR = ExATR;
                c.ExMOV = ExMOV;
                c.MagicType = MagicType;
                c.ImmuneType = ImmuneType;
            }
            foreach (Skill skill in Skills)
            {
                if (skill.SkillType != SkillType.Magic || copyMagic)
                {
                    Skill newskill = skill.Copy();
                    newskill.Character = c;
                    c.Skills.Add(newskill);
                }
            }
            if (copyItem)
            {
                foreach (Item item in Items)
                {
                    Item newitem = item.Copy();
                    newitem.Character = c;
                    c.Items.Add(newitem);
                }
            }
            c.Recovery();
            return c;
        }

        /// <summary>
        /// 复活此角色，回复出厂状态
        /// <para>注意：此方法仅用于角色的复活，如果需要完全重构相同角色，请使用 <see cref="CharacterBuilder"/></para>
        /// </summary>
        /// <param name="original">需要一个原始的角色用于还原状态</param>
        /// <returns></returns>
        public void Respawn(Character original)
        {
            Item? mcp = UnEquip(EquipSlotType.MagicCardPack);
            Item? w = UnEquip(EquipSlotType.Weapon);
            Item? a = UnEquip(EquipSlotType.Armor);
            Item? s = UnEquip(EquipSlotType.Shoes);
            Item? ac1 = UnEquip(EquipSlotType.Accessory1);
            Item? ac2 = UnEquip(EquipSlotType.Accessory2);
            List<Skill> skills = [.. Skills];
            List<Item> items = [.. Items];
            Character c = original.Copy();
            List<Effect> effects = [.. Effects];
            foreach (Effect e in effects)
            {
                e.OnEffectLost(this);
            }
            Effects.Clear();
            Skills.Clear();
            Items.Clear();
            Id = c.Id;
            Guid = c.Guid;
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
            LevelBreak = c.LevelBreak;
            EXP = c.EXP;
            CharacterState = c.CharacterState;
            CharacterEffectStates.Clear();
            CharacterEffectTypes.Clear();
            IsUnselectable = false;
            UpdateCharacterState();
            InitialHP = c.InitialHP;
            ExHP2 = c.ExHP2;
            ExHPPercentage = c.ExHPPercentage;
            InitialMP = c.InitialMP;
            ExMP2 = c.ExMP2;
            ExMPPercentage = c.ExMPPercentage;
            EP = c.EP;
            InitialATK = c.InitialATK;
            ExATK2 = c.ExATK2;
            ExATKPercentage = c.ExATKPercentage;
            InitialDEF = c.InitialDEF;
            ExDEF2 = c.ExDEF2;
            ExDEFPercentage = c.ExDEFPercentage;
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
            ExSTRPercentage = c.ExSTRPercentage;
            ExAGI = c.ExAGI;
            ExAGIPercentage = c.ExAGIPercentage;
            ExINT = c.ExINT;
            ExINTPercentage = c.ExINTPercentage;
            STRGrowth = c.STRGrowth;
            AGIGrowth = c.AGIGrowth;
            INTGrowth = c.INTGrowth;
            InitialSPD = c.InitialSPD;
            ExSPD = c.ExSPD;
            ExActionCoefficient = c.ExActionCoefficient;
            ExAccelerationCoefficient = c.ExAccelerationCoefficient;
            ExCDR = c.ExCDR;
            ExATR = c.ExATR;
            ExMOV = c.ExMOV;
            ExCritRate = c.ExCritRate;
            ExCritDMG = c.ExCritDMG;
            ExEvadeRate = c.ExEvadeRate;
            foreach (Skill skill in skills)
            {
                Skill newskill = skill.Copy();
                newskill.Character = this;
                newskill.Level = skill.Level;
                newskill.CurrentCD = 0;
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
