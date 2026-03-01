using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 记录所有伤害的组成部分
    /// </summary>
    public readonly struct DamageRecord
    {
        /// <summary>
        /// 记录所有伤害的组成部分
        /// </summary>
        public DamageRecord()
        {
            ExpectedDamage = 0;
            BeforeDamageBonus = [];
            CriticalDamage = 0;
            DefenseReduction = 0;
            AfterDamageBonus = [];
            MagicEfficacyDamage = 0;
            FinalDamage = 0;
            ShieldReduction = 0;
            ActualDamage = 0;
        }

        /// <summary>
        /// 记录所有伤害的组成部分
        /// </summary>
        public DamageRecord(DamageType type, DamageResult result, double expectedDamage, Dictionary<Effect, double> beforeDamageBonus, double criticalDamage, double defenseReduction, Dictionary<Effect, double> afterDamageBonus, double magicEfficacyDamage, double finalDamage, double shieldReduction, double actualDamage)
        {
            DamageType = type;
            DamageResult = result;
            ExpectedDamage = expectedDamage;
            BeforeDamageBonus = beforeDamageBonus;
            CriticalDamage = criticalDamage;
            DefenseReduction = defenseReduction;
            AfterDamageBonus = afterDamageBonus;
            MagicEfficacyDamage = magicEfficacyDamage;
            FinalDamage = finalDamage;
            ShieldReduction = shieldReduction;
            ActualDamage = actualDamage;
        }

        /// <summary>
        /// 伤害类型
        /// </summary>
        public DamageType DamageType { get; } = DamageType.Physical;

        /// <summary>
        /// 伤害结果
        /// </summary>
        public DamageResult DamageResult { get; } = DamageResult.Normal;

        /// <summary>
        /// 伤害基底（期望）
        /// </summary>
        public double ExpectedDamage { get; }

        /// <summary>
        /// 特效伤害加成记录（乘区1：暴击和减伤计算前）
        /// </summary>
        public Dictionary<Effect, double> BeforeDamageBonus { get; }

        /// <summary>
        /// 暴击伤害
        /// </summary>
        public double CriticalDamage { get; }

        /// <summary>
        /// 伤害减免
        /// </summary>
        public double DefenseReduction { get; }

        /// <summary>
        /// 特效伤害加成记录（乘区2：暴击和减伤计算后）
        /// </summary>
        public Dictionary<Effect, double> AfterDamageBonus { get; }

        /// <summary>
        /// 魔法效能修正
        /// </summary>
        public double MagicEfficacyDamage { get; }

        /// <summary>
        /// 最终伤害
        /// </summary>
        public double FinalDamage { get; }

        /// <summary>
        /// 护盾减免
        /// </summary>
        public double ShieldReduction { get; }

        /// <summary>
        /// 实际造成伤害
        /// </summary>
        public double ActualDamage { get; }
    }
}
