using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class InventoryFactory : IFactory<Inventory>
    {
        public Type EntityType => typeof(Inventory);

        public Inventory Create()
        {
            return new Inventory(General.UnknownUserInstance);
        }

        public static Inventory Create(User user)
        {
            return new Inventory(user);
        }
    }
}
