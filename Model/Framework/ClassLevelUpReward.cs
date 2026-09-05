namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业升级路线图：某一职业等级的升级奖励（1→10 级默认表随库附带做实验，可整体替换）
    /// <para>字段语义忠实于原始设定（2026-01-28）：
    /// 「职业技能选择权」按类别计数；被动选择与数值提升是同一档的两种选法；
    /// 「职业技能等级+1（魔法额外+1）」落在 SkillLevelUp / MagicExtraLevel。</para>
    /// </summary>
    /// <param name="level">职业等级（1–10）</param>
    /// <param name="inherentPassive">获得流派固有被动的数量（1 / 6 级，与 SubClass 固有被动门槛一致）</param>
    /// <param name="activeSkillChoices">职业技能选择权（战技/魔法/爆发技 通用）</param>
    /// <param name="passiveChoices">被动选择权（可从职业技能池选被动）</param>
    /// <param name="canNumericBoost">该级是否可用「数值提升」替代被动选择</param>
    /// <param name="skillLevelUp">该级已学职业技能（战技/爆发技）的等级增量</param>
    /// <param name="magicExtraLevel">该级魔法的额外等级增量（魔法成长快于战技）</param>
    public class ClassLevelUpReward(int level, int inherentPassive = 0, int activeSkillChoices = 0, int passiveChoices = 0, bool canNumericBoost = false, int skillLevelUp = 0, int magicExtraLevel = 0)
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
        /// 已学职业技能（战技 / 爆发技）的等级增量
        /// </summary>
        public int SkillLevelUp { get; } = skillLevelUp;

        /// <summary>
        /// 魔法的额外等级增量（魔法成长快于战技；最终等级由技能类型上限钳制）
        /// </summary>
        public int MagicExtraLevel { get; } = magicExtraLevel;

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
