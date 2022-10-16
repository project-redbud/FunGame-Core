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

        internal ActiveItem()
        {
            Active = true;
        }

        internal ActiveItem(string Name)
        {
            Active = true;
            this.Name = Name;
        }
    }
}
