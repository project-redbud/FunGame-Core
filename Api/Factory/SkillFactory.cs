using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class SkillFactory : IFactory<Skill>
    {
        public Type EntityType => _EntityType;

        private Type _EntityType = typeof(Skill);

        internal Skill Create(SkillType type = SkillType.Passive)
        {
            _EntityType = typeof(Skill);
            return type switch
            {
                SkillType.Passive => new(false),
                SkillType.Active => new(true),
                _ => new(false)
            };
        }

        public Skill Create()
        {
            return Create(SkillType.Passive);
        }
    }
}
