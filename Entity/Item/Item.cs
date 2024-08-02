using System;
using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class Item : BaseEntity, IItem
    {
        public string Describe { get; set; } = "";
        public decimal Price { get; set; }
        public char Key { get; set; }
        public bool Active { get; set; }
        public bool Enable { get; set; }
        public Character? Character { get; set; } = null;
        public Skill? Skill { get; set; } = null;

        internal Item(bool active = false)
        {
            Active = active;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Item c && c.Name == Name;
        }
    }
}
