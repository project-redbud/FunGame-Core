﻿using System.Text;
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
        public string Description => $"对目标敌人造成 {(1.0 + 0.05 * (Level - 1)) * 100:0.##}% 攻击力 [ {Damage:0.##} ] 点{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

        /// <summary>
        /// 普通攻击的通用说明
        /// </summary>
        public string GeneralDescription => $"对目标敌人造成基于 100（+5/Lv）% 攻击力的{(IsMagic ? CharacterSet.GetMagicDamageName(MagicType) : "物理伤害")}。";

        /// <summary>
        /// 所属的角色
        /// </summary>
        public Character Character { get; } = character;

        /// <summary>
        /// 普通攻击的伤害
        /// </summary>
        public double Damage => Character.ATK * (1.0 + 0.05 * (Level - 1));

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
        /// 硬直时间
        /// </summary>
        public double HardnessTime { get; set; } = 10;

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
                    queue.WriteLine("[ " + Character + $" ] 对 [ {enemy} ] 发起了普通攻击！");
                    double expected = Damage;
                    int changeCount = 0;
                    DamageResult result = IsMagic ? queue.CalculateMagicalDamage(attacker, enemy, true, MagicType, expected, out double damage, ref changeCount) : queue.CalculatePhysicalDamage(attacker, enemy, true, expected, out damage, ref changeCount);
                    queue.DamageToEnemyAsync(attacker, enemy, damage, true, IsMagic, MagicType, result);
                }
            }
        }

        /// <summary>
        /// 修改伤害类型
        /// </summary>
        /// <param name="isMagic"></param>
        /// <param name="magicType"></param>
        public void SetMagicType(bool isMagic, MagicType magicType)
        {
            _IsMagic = isMagic;
            _MagicType = magicType;
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
        /// 是否是魔法伤害
        /// </summary>
        private bool _IsMagic = isMagic;

        /// <summary>
        /// 魔法类型
        /// </summary>
        private MagicType _MagicType = magicType;
    }
}
