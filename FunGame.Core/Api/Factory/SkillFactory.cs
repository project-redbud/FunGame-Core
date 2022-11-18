using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class SkillFactory
    {
        internal static Skill? GetInstance(SkillType type, string Name)
        {
            Skill? skill = null;
            switch (type)
            {
                case SkillType.Active:
                    skill = new Milimoe.FunGame.Core.Entity.ActiveSkill(Name);
                    break;
                case SkillType.Passive:
                    skill = new Milimoe.FunGame.Core.Entity.PassiveSkill(Name);
                    break;
            }
            return skill;
        }
    }
}
