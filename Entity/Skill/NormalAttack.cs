using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class NormalAttack(Character character, bool isMagic = false, MagicType magicType = MagicType.None) : BaseEntity, ISkill
    {
        /// <summary>
        /// 普通攻击名称
        /// </summary>
        public override string Name => "普通攻击";

        /// <summary>
        /// 普通攻击说明
        /// </summary>
        public string Description => $"对目标敌人造成 {BaseDamageMultiplier * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

        /// <summary>
        /// 普通攻击的通用说明
        /// </summary>
        public string GeneralDescription => $"对目标敌人造成基于总攻击力的{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

        /// <summary>
        /// 所属的角色
        /// </summary>
        public Character Character { get; } = character;

        /// <summary>
        /// 基础普通攻击伤害倍率 [ 武器类型相关 ]
        /// </summary>
        private double BaseDamageMultiplier
        {
            get
            {
                double baseMultiplier = 1.0 + 0.05 * (Level - 1);
                if (Character.EquipSlot.Weapon != null)
                {
                    baseMultiplier = Character.EquipSlot.Weapon.WeaponType switch
                    {
                        WeaponType.OneHandedSword => 1.0,
                        WeaponType.TwoHandedSword => 1.2,
                        WeaponType.Bow => 0.9,
                        WeaponType.Pistol => 0.8,
                        WeaponType.Rifle => 1.1,
                        WeaponType.DualDaggers => 0.85,
                        WeaponType.Talisman => 1.0,
                        WeaponType.Staff => 1.15,
                        WeaponType.Polearm => 0.95,
                        WeaponType.Gauntlet => 1.05,
                        WeaponType.HiddenWeapon => 0.9,
                        _ => 1.0
                    };
                    double levelBonus = Character.EquipSlot.Weapon.WeaponType switch
                    {
                        WeaponType.TwoHandedSword => 0.06,
                        WeaponType.Staff => 0.056,
                        WeaponType.Bow => 0.045,
                        WeaponType.Pistol => 0.0375,
                        WeaponType.DualDaggers => 0.0375,
                        WeaponType.Polearm => 0.044,
                        WeaponType.HiddenWeapon => 0.044,
                        _ => 0.05
                    };
                    baseMultiplier += levelBonus * (Level - 1);
                }
                return baseMultiplier;
            }
        }

        /// <summary>
        /// 普通攻击的伤害
        /// </summary>
        public double Damage => Character.ATK * BaseDamageMultiplier * (1 + ExDamage2) + ExDamage;

        /// <summary>
        /// 额外普通攻击伤害 [ 技能和物品相关 ]
        /// </summary>
        public double ExDamage { get; set; } = 0;

        /// <summary>
        /// 额外普通攻击伤害% [ 技能和物品相关 ]
        /// </summary>
        public double ExDamage2 { get; set; } = 0;

        /// <summary>
        /// 普通攻击等级
        /// </summary>
        public int Level
        {
            get
            {
                return Math.Max(1, _Level);
            }
            set
            {
                _Level = Math.Min(Math.Max(1, value), GameplayEquilibriumConstant.MaxNormalAttackLevel);
            }
        }

        /// <summary>
        /// 是否是魔法伤害
        /// </summary>
        public bool IsMagic => _IsMagic;

        /// <summary>
        /// 魔法伤害需要指定魔法类型
        /// </summary>
        public MagicType MagicType => _MagicType;

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 是否在持续生效，为 true 时不允许再次使用。普通攻击始终为 false
        /// </summary>
        public bool IsInEffect => false;

        /// <summary>
        /// 无视免疫类型
        /// </summary>
        public ImmuneType IgnoreImmune { get; set; } = ImmuneType.None;

        /// <summary>
        /// 硬直时间 [ 武器类型相关 ]
        /// </summary>
        public double HardnessTime
        {
            get
            {
                double ht = 10;
                if (Character.EquipSlot.Weapon != null)
                {
                    ht = Character.EquipSlot.Weapon.WeaponType switch
                    {
                        WeaponType.OneHandedSword => 8,
                        WeaponType.TwoHandedSword => 12,
                        WeaponType.Bow => 9,
                        WeaponType.Pistol => 6,
                        WeaponType.Rifle => 11,
                        WeaponType.DualDaggers => 7,
                        WeaponType.Talisman => 10,
                        WeaponType.Staff => 12,
                        WeaponType.Polearm => 10,
                        WeaponType.Gauntlet => 8,
                        WeaponType.HiddenWeapon => 7,
                        _ => 10,
                    };
                }
                return ht * (1 + ExHardnessTime2) + ExHardnessTime;
            }
        }

        /// <summary>
        /// 额外硬直时间 [ 技能和物品相关 ]
        /// </summary>
        public double ExHardnessTime { get; set; } = 0;
        
        /// <summary>
        /// 额外硬直时间% [ 技能和物品相关 ]
        /// </summary>
        public double ExHardnessTime2 { get; set; } = 0;

        /// <summary>
        /// 实际硬直时间
        /// </summary>
        public double RealHardnessTime => Math.Max(0, HardnessTime * (1 - Calculation.PercentageCheck(Character?.ActionCoefficient ?? 0)));

        /// <summary>
        /// 可选取自身
        /// </summary>
        public bool CanSelectSelf { get; set; } = false;

        /// <summary>
        /// 可选取敌对角色
        /// </summary>
        public bool CanSelectEnemy { get; set; } = true;

        /// <summary>
        /// 可选取友方角色
        /// </summary>
        public bool CanSelectTeammate { get; set; } = false;

        /// <summary>
        /// 可选取的作用目标数量
        /// </summary>
        public int CanSelectTargetCount { get; set; } = 1;

        /// <summary>
        /// 可选取的作用范围
        /// </summary>
        public double CanSelectTargetRange { get; set; } = 0;

        /// <summary>
        /// 普通攻击没有魔法消耗
        /// </summary>
        public double RealMPCost => 0;

        /// <summary>
        /// 普通攻击没有吟唱时间
        /// </summary>
        public double RealCastTime => 0;

        /// <summary>
        /// 普通攻击没有能量消耗
        /// </summary>
        public double RealEPCost => 0;

        /// <summary>
        /// 普通攻击没有冷却时间
        /// </summary>
        public double RealCD => 0;

        /// <summary>
        /// 普通攻击没有冷却时间
        /// </summary>
        public double CurrentCD => 0;

        /// <summary>
        /// 绑定到特效的普通攻击扩展。键为特效，值为对应的普攻扩展对象。
        /// </summary>
        public Dictionary<Effect, NormalAttackOfEffect> NormalAttackOfEffects { get; } = [];

        /// <summary>
        /// 获取可选择的目标列表
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public List<Character> GetSelectableTargets(Character caster, List<Character> enemys, List<Character> teammates)
        {
            List<Character> selectable = [];

            if (CanSelectSelf)
            {
                selectable.Add(caster);
            }

            if (CanSelectEnemy)
            {
                selectable.AddRange(enemys);
            }
            if (CanSelectTeammate)
            {
                selectable.AddRange(teammates);
            }

            return selectable;
        }

        /// <summary>
        /// 对目标（或多个目标）发起普通攻击
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="attacker"></param>
        /// <param name="enemys"></param>
        public void Attack(IGamingQueue queue, Character attacker, params IEnumerable<Character> enemys)
        {
            if (!Enable)
            {
                return;
            }
            foreach (Character enemy in enemys)
            {
                if (enemy.HP > 0)
                {
                    queue.WriteLine($"[ {Character} ] 对 [ {enemy} ] 发起了普通攻击！");
                    double expected = Damage;
                    int changeCount = 0;
                    DamageResult result = IsMagic ? queue.CalculateMagicalDamage(attacker, enemy, true, MagicType, expected, out double damage, ref changeCount) : queue.CalculatePhysicalDamage(attacker, enemy, true, expected, out damage, ref changeCount);
                    queue.DamageToEnemyAsync(attacker, enemy, damage, true, IsMagic ? DamageType.Magical : DamageType.Physical, MagicType, result);
                }
            }
        }

        /// <summary>
        /// 修改基础伤害类型。不一定转换成功，要看是否有特效覆盖
        /// </summary>
        /// <param name="isMagic"></param>
        /// <param name="magicType"></param>
        /// <param name="queue"></param>
        public void SetMagicType(bool? isMagic, MagicType? magicType = null, IGamingQueue? queue = null)
        {
            _ExIsMagic = isMagic;
            if (isMagic.HasValue && isMagic.Value)
            {
                magicType ??= MagicType.None;
            }
            _ExMagicType = magicType;
            ResolveMagicType(queue);
        }

        /// <summary>
        /// 修改伤害类型。不一定转换成功，要看是否有其他特效覆盖
        /// </summary>
        /// <param name="naoe"></param>
        /// <param name="queue"></param>
        public void SetMagicType(NormalAttackOfEffect naoe, IGamingQueue? queue = null)
        {
            NormalAttackOfEffects[naoe.Effect] = naoe;
            ResolveMagicType(queue);
        }

        /// <summary>
        /// 移除特效对伤害类型的更改
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="queue"></param>
        public void UnsetMagicType(Effect effect, IGamingQueue? queue = null)
        {
            NormalAttackOfEffects.Remove(effect);
            ResolveMagicType(queue);
        }

        /// <summary>
        /// 计算是否是魔法伤害和当前的魔法类型
        /// </summary>
        internal void ResolveMagicType(IGamingQueue? queue = null)
        {
            bool past = _IsMagic;
            MagicType pastType = _MagicType;
            if (NormalAttackOfEffects.Count > 0)
            {
                if (NormalAttackOfEffects.Values.OrderByDescending(n => n.Priority).FirstOrDefault() is NormalAttackOfEffect naoe)
                {
                    _IsMagic = naoe.IsMagic;
                    _MagicType = naoe.MagicType;
                }
            }
            else if (_ExIsMagic.HasValue && _ExMagicType.HasValue)
            {
                _IsMagic = _ExIsMagic.Value;
                _MagicType = _ExMagicType.Value;
            }
            else
            {
                _IsMagic = false;
                _MagicType = MagicType.None;
                if (Character.EquipSlot.Weapon != null)
                {
                    WeaponType type = Character.EquipSlot.Weapon.WeaponType;
                    if (type == WeaponType.Talisman || type == WeaponType.Staff)
                    {
                        _IsMagic = true;
                    }
                }
            }
            if (queue != null && (past != _IsMagic || pastType != _MagicType))
            {
                queue.WriteLine($"[ {Character} ] 的普通攻击类型已转变为：{(_IsMagic ? CharacterSet.GetMagicDamageName(_MagicType) : "物理伤害")}！");
            }
        }

        /// <summary>
        /// 比较两个普攻对象
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is NormalAttack c && c.Name == Name;
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="showOriginal"></param>
        /// <returns></returns>
        public string GetInfo(bool showOriginal = false)
        {
            StringBuilder builder = new();

            builder.AppendLine($"{Name} - 等级 {Level}");
            builder.AppendLine($"描述：{Description}");
            builder.AppendLine($"硬直时间：{RealHardnessTime:0.##}{(showOriginal && RealHardnessTime != HardnessTime ? $"（原始值：{HardnessTime}）" : "")}");

            return builder.ToString();
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <returns></returns>
        public override string ToString() => GetInfo(true);

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 0;

        /// <summary>
        /// 是否是魔法伤害 [ 生效型 ]
        /// </summary>
        private bool _IsMagic = isMagic;

        /// <summary>
        /// 魔法类型 [ 生效型 ]
        /// </summary>
        private MagicType _MagicType = magicType;

        /// <summary>
        /// 是否是魔法伤害 [ 修改型 ]
        /// </summary>
        private bool? _ExIsMagic = null;

        /// <summary>
        /// 魔法类型 [ 修改型 ]
        /// </summary>
        private MagicType? _ExMagicType = null;
    }

    /// <summary>
    /// 绑定到特效的普通攻击扩展。这个类没有 JSON 转换器支持。
    /// </summary>
    public class NormalAttackOfEffect(Effect effect, bool isMagic, MagicType type, int priority)
    {
        public Effect Effect { get; set; } = effect;
        public bool IsMagic { get; set; } = isMagic;
        public MagicType MagicType { get; set; } = type;
        public int Priority { get; set; } = priority;
    }
}
