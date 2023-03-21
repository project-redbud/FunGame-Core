using System.Data;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class SkillFactory
    {
        internal static Skill GetInstance(DataSet? DataSet, SkillType type = SkillType.Passive, int Index = 0)
        {
            Skill skill = type switch
            {
                SkillType.Active => new ActiveSkill(DataSet, Index),
                _ => new PassiveSkill(DataSet, Index)
            };
            return skill;
        }
    }
}
