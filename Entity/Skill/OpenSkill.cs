using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 用于动态扩展技能，<see cref="Description"/> 返回所有特效的描述
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="character"></param>
    public class OpenSkill(long id, string name, Character? character = null) : Skill(SkillType.Passive, character)
    {
        public override long Id => id;
        public override string Name => name;
        public override string Description => string.Join("\r\n", Effects);

        public override IEnumerable<Effect> AddInactiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
