using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class SkillFactory
    {
        internal static Milimoe.FunGame.Core.Entity.General.Skill? GetInstance(Milimoe.FunGame.Core.Entity.Enum.SkillType type, string Name)
        {
            Milimoe.FunGame.Core.Entity.General.Skill? skill = null;
            switch (type)
            {
                case Entity.Enum.SkillType.Active:
                    skill = new Milimoe.FunGame.Core.Entity.General.ActiveSkill(Name);
                    break;
                case Entity.Enum.SkillType.Passive:
                    skill = new Milimoe.FunGame.Core.Entity.General.PassiveSkill(Name);
                    break;
            }
            return skill;
        }
    }
}
