using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class PassiveItem : Item
    {
        public PassiveSkill? Skill { get; set; } = null;

        internal PassiveItem()
        {
            Active = false;
        }

        internal PassiveItem(int id, string name)
        {
            Active = false;
            Id = id;
            Name = name;
        }

        public override bool Equals(IBaseEntity? other)
        {
            if (other != null && other.Guid == this.Guid)
                return true;
            else return false;
        }
    }
}
