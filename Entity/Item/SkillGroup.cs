namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 物品只有一个主动技能，但是可以有很多个被动技能
    /// <para>魔法卡包具有很多个魔法技能</para>
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

        /// <summary>
        /// 魔法技能组
        /// </summary>
        public HashSet<Skill> Magics { get; set; } = [];
    }
}
