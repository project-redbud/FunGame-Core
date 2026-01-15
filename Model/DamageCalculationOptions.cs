using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 精准的分步控制伤害计算
    /// </summary>
    public class DamageCalculationOptions
    {
        /// <summary>
        /// 完整计算伤害
        /// </summary>
        public bool NeedCalculate { get; set; } = true;

        /// <summary>
        /// 计算减伤
        /// </summary>
        public bool CalculateReduction { get; set; } = true;

        /// <summary>
        /// 计算暴击
        /// </summary>
        public bool CalculateCritical { get; set; } = true;

        /// <summary>
        /// 计算闪避
        /// </summary>
        public bool CalculateEvade { get; set; } = true;

        /// <summary>
        /// 计算护盾
        /// </summary>
        public bool CalculateShield { get; set; } = true;

        /// <summary>
        /// 触发特效
        /// </summary>
        public bool TriggerEffects { get; set; } = true;

        /// <summary>
        /// 无视免疫
        /// </summary>
        public bool IgnoreImmune { get; set; } = false;

        /// <summary>
        /// 伤害基底（期望）
        /// </summary>
        internal double ExpectedDamage { get; set; } = 0;

        /// <summary>
        /// 特效伤害加成记录（乘区1：暴击和减伤计算前）
        /// </summary>
        internal Dictionary<Effect, double> BeforeDamageBonus { get; set; } = [];

        /// <summary>
        /// 暴击伤害
        /// </summary>
        internal double CriticalDamage { get; set; } = 0;

        /// <summary>
        /// 伤害减免
        /// </summary>
        internal double DefenseReduction { get; set; } = 0;

        /// <summary>
        /// 特效伤害加成记录（乘区2：暴击和减伤计算后）
        /// </summary>
        internal Dictionary<Effect, double> AfterDamageBonus { get; set; } = [];

        /// <summary>
        /// 最终伤害
        /// </summary>
        internal double FinalDamage { get; set; } = 0;

        /// <summary>
        /// 护盾减免
        /// </summary>
        internal double ShieldReduction { get; set; } = 0;

        /// <summary>
        /// 实际造成伤害
        /// </summary>
        internal double ActualDamage { get; set; } = 0;
    }
}
