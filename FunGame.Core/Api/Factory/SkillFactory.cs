using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using System.Data;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class SkillFactory
    {
        internal static Skill GetInstance(DataSet? DataSet, SkillType type = SkillType.Passive)
        {
            Skill skill = type switch
            {
                SkillType.Active => new ActiveSkill(DataSet),
                _ => new PassiveSkill(DataSet)
            };
            return skill;
        }
    }
}
