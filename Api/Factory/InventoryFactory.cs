using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class InventoryFactory : IFactory<Inventory>
    {
        public Type EntityType => typeof(Inventory);

        public Inventory Create()
        {
            return new Inventory();
        }
    }
}
