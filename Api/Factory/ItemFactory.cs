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
            switch (type)
            {
                case ItemType.Passive:
                    _EntityType = typeof(PassiveItem);
                    return PassiveItem.GetInstance();
                case ItemType.Active:
                default:
                    _EntityType = typeof(ActiveItem);
                    return PassiveItem.GetInstance();
            }
        }

        public Item Create()
        {
            return Create(ItemType.Passive);
        }
    }
}
