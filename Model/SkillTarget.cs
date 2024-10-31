using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Model
{
    public struct SkillTarget(Skill skill, List<Character> targets)
    {
        public Skill Skill { get; set; } = skill;
        public List<Character> Targets { get; set; } = targets;
    }
}
