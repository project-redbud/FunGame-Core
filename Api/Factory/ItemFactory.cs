using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class ItemFactory
    {
        internal static Milimoe.FunGame.Core.Entity.Item? GetInstance(ItemType type, int id, string name)
        {
            Milimoe.FunGame.Core.Entity.Item? item = null;
            switch (type)
            {
                case ItemType.Active:
                    item = new Milimoe.FunGame.Core.Entity.ActiveItem(id, name);
                    break;
                case ItemType.Passive:
                    item = new Milimoe.FunGame.Core.Entity.PassiveItem(id, name);
                    break;
            }
            return item;
        }
    }
}
