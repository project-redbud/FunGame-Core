using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Api.EntityFactory
{
    internal class SkillFactory : IFactory<Skill>
    {
        public Type EntityType => typeof(Skill);

        public Skill Create()
        {
            return new();
        }
    }
}
