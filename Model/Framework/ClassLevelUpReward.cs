namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业升级路线图：某一职业等级的升级奖励
    /// <para/>字段语义忠实于原始设定：
    /// <para/>「职业技能选择权」按类别计数；被动选择与数值提升是同一档的两种选法；
    /// <para/>「职业技能等级+1（魔法额外+1）」落在 SkillLevelUp / MagicExtraLevel。
    /// </summary>
    /// <param name="level">职业等级（1–10）</param>
    /// <param name="inherentPassive">获得流派固有被动的数量（1 / 6 级，与 SubClass 固有被动门槛一致）</param>
    /// <param name="activeSkillChoices">职业技能选择权（战技/魔法/爆发技 通用）</param>
    /// <param name="passiveChoices">被动选择权（可从职业技能池选被动）</param>
    /// <param name="canNumericBoost">该级是否可用「数值提升」替代被动选择</param>
    /// <param name="skillLevelUp">该级职业池主动技能（战技/爆发技）的等级增量（未习得的池技能同样提升，习得后即继承水位）</param>
    /// <param name="magicExtraLevel">该级魔法的额外等级增量（魔法成长快于战技）</param>
    /// <param name="numericBoost">该级数值提升每次可分配的额度，null 时回落到 <see cref="EquilibriumConstant.NumericBoostBudget"/></param>
    public class ClassLevelUpReward(int level, int inherentPassive = 0, int activeSkillChoices = 0, int passiveChoices = 0, bool canNumericBoost = false, int skillLevelUp = 0, int magicExtraLevel = 0, ClassAttributeBudget? numericBoost = null)
    {
        /// <summary>
        /// 职业等级
        /// </summary>
        public int Level { get; } = level;

        /// <summary>
        /// 获得流派固有被动的数量（1 / 6 级，与 <see cref="Entity.SubClass"/> 固有被动门槛一致）
        /// </summary>
        public int InherentPassive { get; } = inherentPassive;

        /// <summary>
        /// 职业技能选择权（战技 / 魔法 / 爆发技 通用）
        /// </summary>
        public int ActiveSkillChoices { get; } = activeSkillChoices;

        /// <summary>
        /// 被动选择权
        /// </summary>
        public int PassiveChoices { get; } = passiveChoices;

        /// <summary>
        /// 该级是否可用「数值提升」替代被动选择（4 / 9 级）
        /// </summary>
        public bool CanNumericBoost { get; } = canNumericBoost;

        /// <summary>
        /// 职业池主动技能（战技 / 爆发技）的等级增量
        /// <para>作用范围为整个职业池：未习得的池技能也一并提升，之后习得时直接继承当前水位</para>
        /// <para>职业被动按设定恒为 1 级，不参与提级</para>
        /// </summary>
        public int SkillLevelUp { get; } = skillLevelUp;

        /// <summary>
        /// 魔法的额外等级增量（魔法成长快于战技；最终等级由技能类型上限钳制）
        /// </summary>
        public int MagicExtraLevel { get; } = magicExtraLevel;

        /// <summary>
        /// 数值提升每次可分配的额度（属性点总额 + 成长总额，不受模板上下限约束）
        /// <para/>null 表示交由 <see cref="EquilibriumConstant.NumericBoostBudget"/> 决定，便于整体调平衡
        /// </summary>
        public ClassAttributeBudget? NumericBoost { get; } = numericBoost;

        /// <summary>
        /// 构建 1→10 级默认路线图（实验用；数值平衡可整体替换 <see cref="EquilibriumConstant.ClassLevelUpRewards"/>）
        /// </summary>
        /// <returns>key = 职业等级</returns>
        public static Dictionary<int, ClassLevelUpReward> BuildDefaultTable()
        {
            return new Dictionary<int, ClassLevelUpReward>
            {
                [1] = new(1, inherentPassive: 1),
                [2] = new(2, activeSkillChoices: 2),
                [3] = new(3, skillLevelUp: 1, magicExtraLevel: 1),
                [4] = new(4, passiveChoices: 1, canNumericBoost: true),
                [5] = new(5, activeSkillChoices: 2, skillLevelUp: 1, magicExtraLevel: 1),
                [6] = new(6, inherentPassive: 1),
                [7] = new(7, skillLevelUp: 1, magicExtraLevel: 1),
                [8] = new(8, activeSkillChoices: 2, skillLevelUp: 1, magicExtraLevel: 1),
                [9] = new(9, passiveChoices: 2, canNumericBoost: true),
                [10] = new(10, activeSkillChoices: 2, skillLevelUp: 1, magicExtraLevel: 1)
            };
        }
    }
}
