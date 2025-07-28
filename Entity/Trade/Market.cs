using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Market : BaseEntity
    {
        public string Description { get; set; } = "";
        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;
        public DateTime? StartTimeOfDay { get; set; } = null;
        public DateTime? EndTimeOfDay { get; set; } = null;
        public Dictionary<long, MarketItem> MarketItems { get; } = [];

        public Market(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public void AddItem(User user, Item item, double price, int stock, string name = "")
        {
            if (MarketItems.Values.FirstOrDefault(m => m.Item.Id == item.Id && m.Item.Name == item.Name && m.Price == price && m.User == user.Id && m.Status == MarketItemState.Listed) is MarketItem marketItem)
            {
                marketItem.Stock += stock;
                marketItem.Item.Price = (marketItem.Item.Price + item.Price) / marketItem.Stock;
            }
            else
            {
                long id = MarketItems.Count > 0 ? MarketItems.Keys.Max() + 1 : 1;
                if (name.Trim() == "")
                {
                    name = item.Name;
                }
                marketItem = new()
                {
                    Id = id,
                    User = user.Id,
                    Username = user.Username,
                    Item = item,
                    Price = price,
                    Stock = stock,
                    Name = name
                };
                MarketItems.Add(id, marketItem);
            }
        }

        public void AddItems(User user, Item[] items, double price)
        {
            for (int index = 0; index < items.Length; index++)
            {
                Item item = items[index];
                AddItem(user, item, price, 1);
            }
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Market && other.GetIdName() == GetIdName();
        }
    }
}
