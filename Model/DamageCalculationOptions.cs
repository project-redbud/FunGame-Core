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
    }
}
