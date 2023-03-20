using System.Data;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class SkillFactory
    {
        internal static Skill GetInstance(DataRow? DataRow, SkillType type = SkillType.Passive)
        {
            Skill skill = type switch
            {
                SkillType.Active => new ActiveSkill(DataRow),
                _ => new PassiveSkill(DataRow)
            };
            return skill;
        }
    }
}
