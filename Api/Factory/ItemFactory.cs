using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class ItemFactory : IFactory<Item>
    {
        public Type EntityType => _EntityType;

        private Type _EntityType = typeof(Item);

        public Item Create(ItemType type = ItemType.Passive)
        {
            _EntityType = typeof(Item);
            return type switch
            {
                ItemType.Passive => new(false),
                ItemType.Active => new(true),
                _ => new(false)
            };
        }

        public Item Create()
        {
            return Create(ItemType.Passive);
        }
    }
}
