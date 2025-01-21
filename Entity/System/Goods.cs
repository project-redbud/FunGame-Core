using System.Text;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Goods
    {
        public long Id { get; set; } = 0;
        public List<Item> Items { get; } = [];
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, double> Prices { get; } = [];
        public int Stock { get; set; }

        public Goods() { }

        public Goods(long id, Item item, int stock, string name, string description, Dictionary<string, double>? prices = null)
        {
            Id = id;
            Items.Add(item);
            Stock = stock;
            Name = name;
            Description = description;
            if (prices != null) Prices = prices;
        }

        public Goods(long id, List<Item> items, int stock, string name, string description, Dictionary<string, double>? prices = null)
        {
            Id = id;
            Items = items;
            Stock = stock;
            Name = name;
            Description = description;
            if (prices != null) Prices = prices;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine($"{Id}. {Name}");
            builder.AppendLine($"商品描述：{Description}");
            builder.AppendLine($"商品售价：{(Prices.Count > 0 ? string.Join("、", Prices.Select(kv => $"{kv.Value} {kv.Key}")) : "免费")}");
            builder.AppendLine($"包含物品：{string.Join("、", Items.Select(i => $"[{ItemSet.GetQualityTypeName(i.QualityType)}|{ItemSet.GetItemTypeName(i.ItemType)}] {i.Name}"))}");
            builder.AppendLine($"剩余库存：{Stock}");
            return builder.ToString().Trim();
        }

        public void SetPrice(string needy, double price)
        {
            if (price > 0) Prices[needy] = price;
        }

        public bool GetPrice(string needy, out double price)
        {
            price = 0;
            if (Prices.TryGetValue(needy, out double temp) && temp > 0)
            {
                price = temp;
                return true;
            }
            return false;
        }
    }
}
