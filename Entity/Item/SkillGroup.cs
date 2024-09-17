namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 物品只有一个主动技能，但是可以有很多个被动技能
    /// </summary>
    public class SkillGroup
    {
        /// <summary>
        /// 唯一的主动技能
        /// </summary>
        public Skill? Active { get; set; } = null;

        /// <summary>
        /// 被动技能组
        /// </summary>
        public HashSet<Skill> Passives { get; set; } = [];
    }
}
