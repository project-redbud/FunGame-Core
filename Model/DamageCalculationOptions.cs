using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 精准的分步控制伤害计算选项
    /// </summary>
    public class DamageCalculationOptions(Character character)
    {
        /// <summary>
        /// 伤害来源
        /// </summary>
        public Character Character { get; set; } = character;

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
        /// 如果来源是技能
        /// </summary>
        public Skill? Skill { get; set; } = null;

        /// <summary>
        /// 是魔法技能
        /// </summary>
        public bool IsMagicSkill => Skill?.IsMagic ?? false;

        /// <summary>
        /// 魔法瓶颈
        /// </summary>
        public double MagicBottleneck => IsMagicSkill ? (Skill?.MagicBottleneck ?? 0) : 0;

        /// <summary>
        /// 魔法效能%
        /// </summary>
        public double MagicEfficacy => IsMagicSkill ? (Skill?.MagicEfficacy ?? 1) : 1;

        /// <summary>
        /// 施法者智力
        /// </summary>
        public double INT => Character.INT;

        /** == 调试数据记录 == **/

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
        /// 魔法效能修正
        /// </summary>
        internal double MagicEfficacyDamage { get; set; } = 0;

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
