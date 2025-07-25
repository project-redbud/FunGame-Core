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
        public int Quota { get; set; }
        public Dictionary<long, int> UsersBuyCount { get; } = [];
        public DateTime? ExpireTime { get; set; } = null;

        public Goods() { }

        public Goods(long id, Item item, int stock, string name, string description, Dictionary<string, double>? prices = null, int quota = 0)
        {
            Id = id;
            Items.Add(item);
            Stock = stock;
            Quota = quota;
            Name = name;
            Description = description;
            if (prices != null) Prices = prices;
        }

        public Goods(long id, List<Item> items, int stock, string name, string description, Dictionary<string, double>? prices = null, int quota = 0)
        {
            Id = id;
            Items = items;
            Stock = stock;
            Quota = quota;
            Name = name;
            Description = description;
            if (prices != null) Prices = prices;
        }

        public override string ToString()
        {
            return ToString(null);
        }
        
        public string ToString(User? user = null)
        {
            StringBuilder builder = new();
            builder.AppendLine($"{Id}. {Name}");
            if (ExpireTime.HasValue) builder.AppendLine($"限时购买：{ExpireTime.Value.ToString(General.GeneralDateTimeFormatChinese)} 截止");
            builder.AppendLine($"商品描述：{Description}");
            builder.AppendLine($"商品售价：{(Prices.Count > 0 ? string.Join("、", Prices.Select(kv => $"{kv.Value} {kv.Key}")) : "免费")}");
            builder.AppendLine($"包含物品：{string.Join("、", Items.Select(i => $"[{ItemSet.GetQualityTypeName(i.QualityType)}|{ItemSet.GetItemTypeName(i.ItemType)}] {i.Name}"))}");
            int buyCount = 0;
            if (user != null)
            {
                UsersBuyCount.TryGetValue(user.Id, out buyCount);
            }
            builder.AppendLine($"剩余库存：{(Stock == -1 ? "不限" : Stock)}（已购：{buyCount}）");
            if (Quota > 0)
            {
                builder.AppendLine($"限购数量：{Quota}");
            }
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
