using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.General
{
    public class ActiveItem : Item
    {
        public ActiveSkill? Skill { get; set; } = null;

        public ActiveItem()
        {
            Active = true;
        }
    }
}
