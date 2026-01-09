using Milimoe.FunGame.Core.Library.Common.Addon;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 技能和它的目标结构体
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="targets"></param>
    /// <param name="grids"></param>
    public struct SkillTarget(Skill skill, List<Character> targets, List<Grid> grids)
    {
        /// <summary>
        /// 技能实例
        /// </summary>
        public Skill Skill { get; set; } = skill;

        /// <summary>
        /// 指向性技能的目标列表
        /// </summary>
        public List<Character> Targets { get; set; } = targets;

        /// <summary>
        /// 非指向性技能的目标列表
        /// </summary>
        public List<Grid> TargetGrids { get; set; } = grids;
    }
}
