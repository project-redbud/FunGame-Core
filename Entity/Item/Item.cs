using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class Item : BaseEntity, IItem
    {
        public string Describe { get; set; } = "";
        public double Price { get; set; }
        public char Key { get; set; }
        public bool IsActive { get; set; }
        public bool Enable { get; set; }
        public Character? Character { get; set; } = null;
        public Skill? Skill { get; set; } = null;

        internal Item(bool active = false)
        {
            IsActive = active;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Item c && c.Name == Name;
        }
    }
}
