using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Factory
{
    public class ItemFactory
    {
        public static Milimoe.FunGame.Core.Entity.General.Item? GetInstance(Milimoe.FunGame.Core.Entity.Enum.ItemType type, string Name)
        {
            Milimoe.FunGame.Core.Entity.General.Item? item = null;
            switch (type)
            {
                case Entity.Enum.ItemType.Active:
                    item = new Milimoe.FunGame.Core.Entity.General.ActiveItem(Name);
                    break;
                case Entity.Enum.ItemType.Passive:
                    item = new Milimoe.FunGame.Core.Entity.General.PassiveItem(Name);
                    break;
            }
            return item;
        }
    }
}
