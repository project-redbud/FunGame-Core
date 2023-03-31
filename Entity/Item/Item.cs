using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public abstract class Item : BaseEntity, IItem
    {
        public string Describe { get; set; } = "";
        public decimal Price { get; set; }
        public char Key { get; set; }
        public bool Active { get; set; }
        public bool Enable { get; set; }
        public Character? Character { get; set; } = null;

        public override IEnumerator<IBaseEntity> GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
