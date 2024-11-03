namespace Milimoe.FunGame.Core.Entity
{
    public struct SkillTarget(Skill skill, List<Character> targets)
    {
        public Skill Skill { get; set; } = skill;
        public List<Character> Targets { get; set; } = targets;
    }
}
