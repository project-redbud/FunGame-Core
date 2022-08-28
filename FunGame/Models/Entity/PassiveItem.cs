using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
{
    public class PassiveItem : Item
    {
        public PassiveSkill? Skill { get; set; } = null;

        public PassiveItem()
        {
            Active = false;
        }
    }
}
