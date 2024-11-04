using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 用于动态扩展技能，<see cref="Description"/> 返回所有特效的描述
    /// </summary>
    public class OpenSkill : Skill
    {
        public override long Id { get; set; }
        public override string Name { get; set; }
        public override string Description => string.Join("\r\n", Effects);

        public OpenSkill(long id, string name, Dictionary<string, object> args, Character? character = null) : base(SkillType.Passive, character)
        {
            Id = id;
            Name = name;
            foreach (string str in args.Keys)
            {
                Values[str] = args[str];
                switch (str)
                {
                    default:
                        break;
                }
            }
        }

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
