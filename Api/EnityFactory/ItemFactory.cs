using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Api.EntityFactory
{
    internal class ItemFactory : IFactory<Item>
    {
        public Type EntityType => typeof(Item);

        public Item Create()
        {
            return new();
        }
    }
}
