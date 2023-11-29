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
            switch (type)
            {
                case SkillType.Passive:
                    _EntityType = typeof(PassiveSkill);
                    return PassiveSkill.GetInstance();
                case SkillType.Active:
                default:
                    _EntityType = typeof(ActiveSkill);
                    return ActiveSkill.GetInstance();
            }
        }

        public Skill Create()
        {
            return Create(SkillType.Passive);
        }
    }
}
