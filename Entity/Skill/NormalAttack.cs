using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

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
        public string Description => $"{(_isMagicByWeapon ? "已由武器附魔。" : "")}对目标敌人造成 {BaseDamageMultiplier * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

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
                        WeaponType.OneHandedSword => GameplayEquilibriumConstant.OneHandedSwordBaseMultiplier,
                        WeaponType.TwoHandedSword => GameplayEquilibriumConstant.TwoHandedSwordBaseMultiplier,
                        WeaponType.Bow => GameplayEquilibriumConstant.BowBaseMultiplier,
                        WeaponType.Pistol => GameplayEquilibriumConstant.PistolBaseMultiplier,
                        WeaponType.Rifle => GameplayEquilibriumConstant.RifleBaseMultiplier,
                        WeaponType.DualDaggers => GameplayEquilibriumConstant.DualDaggersBaseMultiplier,
                        WeaponType.Talisman => GameplayEquilibriumConstant.TalismanBaseMultiplier,
                        WeaponType.Staff => GameplayEquilibriumConstant.StaffBaseMultiplier,
                        WeaponType.Polearm => GameplayEquilibriumConstant.PolearmBaseMultiplier,
                        WeaponType.Gauntlet => GameplayEquilibriumConstant.GauntletBaseMultiplier,
                        WeaponType.HiddenWeapon => GameplayEquilibriumConstant.HiddenWeaponBaseMultiplier,
                        _ => 1.0
                    };
                    double levelBonus = Character.EquipSlot.Weapon.WeaponType switch
                    {
                        WeaponType.OneHandedSword => GameplayEquilibriumConstant.OneHandedSwordLevelBonus,
                        WeaponType.TwoHandedSword => GameplayEquilibriumConstant.TwoHandedSwordLevelBonus,
                        WeaponType.Bow => GameplayEquilibriumConstant.BowLevelBonus,
                        WeaponType.Pistol => GameplayEquilibriumConstant.PistolLevelBonus,
                        WeaponType.Rifle => GameplayEquilibriumConstant.RifleLevelBonus,
                        WeaponType.DualDaggers => GameplayEquilibriumConstant.DualDaggersLevelBonus,
                        WeaponType.Staff => GameplayEquilibriumConstant.StaffLevelBonus,
                        WeaponType.Polearm => GameplayEquilibriumConstant.PolearmLevelBonus,
                        WeaponType.Gauntlet => GameplayEquilibriumConstant.GauntletLevelBonus,
                        WeaponType.HiddenWeapon => GameplayEquilibriumConstant.HiddenWeaponLevelBonus,
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
                return Math.Max(1, _level);
            }
            set
            {
                _level = Math.Min(Math.Max(1, value), GameplayEquilibriumConstant.MaxNormalAttackLevel);
            }
        }

        /// <summary>
        /// 是否是魔法伤害
        /// </summary>
        public bool IsMagic => _isMagic;

        /// <summary>
        /// 魔法伤害需要指定魔法类型
        /// </summary>
        public MagicType MagicType => _magicType;

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
                        WeaponType.OneHandedSword => GameplayEquilibriumConstant.OneHandedSwordHardness,
                        WeaponType.TwoHandedSword => GameplayEquilibriumConstant.TwoHandedSwordHardness,
                        WeaponType.Bow => GameplayEquilibriumConstant.BowHardness,
                        WeaponType.Pistol => GameplayEquilibriumConstant.PistolHardness,
                        WeaponType.Rifle => GameplayEquilibriumConstant.RifleHardness,
                        WeaponType.DualDaggers => GameplayEquilibriumConstant.DualDaggersHardness,
                        WeaponType.Talisman => GameplayEquilibriumConstant.TalismanHardness,
                        WeaponType.Staff => GameplayEquilibriumConstant.StaffHardness,
                        WeaponType.Polearm => GameplayEquilibriumConstant.PolearmHardness,
                        WeaponType.Gauntlet => GameplayEquilibriumConstant.GauntletHardness,
                        WeaponType.HiddenWeapon => GameplayEquilibriumConstant.HiddenWeaponHardness,
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
        /// 选取所有敌对角色，优先级大于 <see cref="CanSelectTargetCount"/>
        /// </summary>
        public bool SelectAllEnemies { get; set; } = false;

        /// <summary>
        /// 选取所有友方角色，优先级大于 <see cref="CanSelectTargetCount"/>，默认包含自身
        /// </summary>
        public bool SelectAllTeammates { get; set; } = false;

        /// <summary>
        /// 可选取的作用目标数量
        /// </summary>
        public int CanSelectTargetCount { get; set; } = 1;

        /// <summary>
        /// 可选取的作用范围 [ 单位：格 ]
        /// </summary>
        public int CanSelectTargetRange { get; set; } = 0;

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
        /// 游戏中的行动顺序表实例，使用时需要判断其是否存在
        /// </summary>
        public IGamingQueue? GamingQueue { get; set; } = null;

        /// <summary>
        /// 绑定到特效的普通攻击扩展。键为特效，值为对应的普攻扩展对象。
        /// </summary>
        public Dictionary<Effect, NormalAttackOfEffect> NormalAttackOfEffects { get; } = [];

        /// <summary>
        /// 获取可选择的目标列表
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public List<Character> GetSelectableTargets(Character attacker, List<Character> enemys, List<Character> teammates)
        {
            List<Character> selectable = [];

            if (CanSelectSelf)
            {
                selectable.Add(attacker);
            }

            foreach (Character character in enemys)
            {
                if (CanSelectEnemy && ((character.ImmuneType & ImmuneType.All) != ImmuneType.All || IgnoreImmune == ImmuneType.All))
                {
                    selectable.Add(character);
                }
            }

            foreach (Character character in teammates)
            {
                if (CanSelectTeammate)
                {
                    selectable.Add(character);
                }
            }

            return selectable;
        }

        /// <summary>
        /// 实际可选取的目标数量
        /// </summary>
        public int RealCanSelectTargetCount(List<Character> enemys, List<Character> teammates)
        {
            int count = CanSelectTargetCount;
            if (SelectAllTeammates)
            {
                return teammates.Count + 1;
            }
            if (SelectAllEnemies)
            {
                return enemys.Count;
            }
            return count;
        }

        /// <summary>
        /// 选取普攻目标
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public List<Character> SelectTargets(Character attacker, List<Character> enemys, List<Character> teammates)
        {
            List<Character> tobeSelected = GetSelectableTargets(attacker, enemys, teammates);

            List<Character> targets = [];

            if (SelectAllTeammates || SelectAllEnemies)
            {
                if (SelectAllTeammates)
                {
                    targets.AddRange(tobeSelected.Where(c => c == attacker || teammates.Contains(c)));
                }
                if (SelectAllEnemies)
                {
                    targets.AddRange(tobeSelected.Where(enemys.Contains));
                }
            }
            else if (tobeSelected.Count <= CanSelectTargetCount)
            {
                targets.AddRange(tobeSelected);
            }
            else
            {
                targets.AddRange(tobeSelected.OrderBy(x => Random.Shared.Next()).Take(CanSelectTargetCount));
            }

            return [.. targets.Distinct()];
        }

        /// <summary>
        /// 对目标（或多个目标）发起普通攻击
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="attacker"></param>
        /// <param name="options"></param>
        /// <param name="enemys"></param>
        public void Attack(IGamingQueue queue, Character attacker, DamageCalculationOptions? options, params IEnumerable<Character> enemys)
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
                    DamageResult result = IsMagic ? queue.CalculateMagicalDamage(attacker, enemy, true, MagicType, expected, out double damage, ref changeCount, ref options) : queue.CalculatePhysicalDamage(attacker, enemy, true, expected, out damage, ref changeCount, ref options);
                    queue.DamageToEnemy(attacker, enemy, damage, true, IsMagic ? DamageType.Magical : DamageType.Physical, MagicType, result, options);
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
            _exIsMagic = isMagic;
            if (isMagic.HasValue && isMagic.Value)
            {
                magicType ??= MagicType.None;
            }
            _exMagicType = magicType;
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
            bool past = _isMagic;
            MagicType pastType = _magicType;
            if (NormalAttackOfEffects.Count > 0)
            {
                if (NormalAttackOfEffects.Values.OrderByDescending(n => n.Priority).FirstOrDefault() is NormalAttackOfEffect naoe)
                {
                    _isMagic = naoe.IsMagic;
                    _magicType = naoe.MagicType;
                }
            }
            else if (_exIsMagic.HasValue && _exMagicType.HasValue)
            {
                _isMagic = _exIsMagic.Value;
                _magicType = _exMagicType.Value;
            }
            else
            {
                _isMagic = false;
                _magicType = MagicType.None;
                if (Character.EquipSlot.Weapon != null)
                {
                    WeaponType type = Character.EquipSlot.Weapon.WeaponType;
                    if (type == WeaponType.Talisman || type == WeaponType.Staff)
                    {
                        _isMagic = true;
                        _isMagicByWeapon = true;
                    }
                    else
                    {
                        _isMagicByWeapon = false;
                    }
                }
                else
                {
                    _isMagicByWeapon = false;
                }
            }
            if (queue != null && (past != _isMagic || pastType != _magicType))
            {
                queue.WriteLine($"[ {Character} ] 的普通攻击类型已转变为：{(_isMagic ? CharacterSet.GetMagicDamageName(_magicType) : "物理伤害")}！");
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
            if (GamingQueue?.Map != null) builder.AppendLine($"攻击距离：{Character.ATR}");
            builder.AppendLine($"硬直时间：{RealHardnessTime:0.##}{(showOriginal && RealHardnessTime != HardnessTime ? $"（原始值：{HardnessTime}）" : "")}");

            return builder.ToString();
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <returns></returns>
        public override string ToString() => GetInfo(true);

        /// <summary>
        /// 在选取目标前向角色（玩家）发起询问的事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="normalAttack"></param>
        /// <returns></returns>
        public delegate InquiryOptions? NormalAttackInquiryOptionsDelegate(Character character, NormalAttack normalAttack);
        public event NormalAttackInquiryOptionsDelegate? InquiryBeforeTargetSelectionEvent;
        /// <summary>
        /// 触发选择目标前的询问事件
        /// </summary>
        /// <param name="character"></param>
        /// <param name="normalAttack"></param>
        public InquiryOptions? OnInquiryBeforeTargetSelection(Character character, NormalAttack normalAttack)
        {
            return InquiryBeforeTargetSelectionEvent?.Invoke(character, normalAttack);
        }

        /// <summary>
        /// 等级
        /// </summary>
        private int _level = 0;

        /// <summary>
        /// 是否是魔法伤害 [ 生效型 ]
        /// </summary>
        private bool _isMagic = isMagic;

        /// <summary>
        /// 指示普通攻击是否由武器附魔
        /// </summary>
        private bool _isMagicByWeapon = false;

        /// <summary>
        /// 魔法类型 [ 生效型 ]
        /// </summary>
        private MagicType _magicType = magicType;

        /// <summary>
        /// 是否是魔法伤害 [ 修改型 ]
        /// </summary>
        private bool? _exIsMagic = null;

        /// <summary>
        /// 魔法类型 [ 修改型 ]
        /// </summary>
        private MagicType? _exMagicType = null;
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
