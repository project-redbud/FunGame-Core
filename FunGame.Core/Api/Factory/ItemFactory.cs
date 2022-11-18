using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class ItemFactory
    {
        internal static Milimoe.FunGame.Core.Entity.Item? GetInstance(ItemType type, string Name)
        {
            Milimoe.FunGame.Core.Entity.Item? item = null;
            switch (type)
            {
                case ItemType.Active:
                    item = new Milimoe.FunGame.Core.Entity.ActiveItem(Name);
                    break;
                case ItemType.Passive:
                    item = new Milimoe.FunGame.Core.Entity.PassiveItem(Name);
                    break;
            }
            return item;
        }
    }
}
