using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.General
{
    public class PassiveItem : Item
    {
        public PassiveSkill? Skill { get; set; } = null;

        internal PassiveItem()
        {
            Active = false;
        }

        internal PassiveItem(string Name)
        {
            Active = false;
            this.Name = Name;
        }
    }
}
