namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 技能和它的目标结构体
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="targets"></param>
    public struct SkillTarget(Skill skill, List<Character> targets)
    {
        /// <summary>
        /// 技能实例
        /// </summary>
        public Skill Skill { get; set; } = skill;

        /// <summary>
        /// 技能的目标列表
        /// </summary>
        public List<Character> Targets { get; set; } = targets;
    }
}
