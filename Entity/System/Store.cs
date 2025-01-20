using System.Text;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Store : BaseEntity
    {
        public User User { get; set; } = General.UnknownUserInstance;
        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;
        public Dictionary<long, Goods> Goods { get; } = [];

        public Store(string name, User? user = null)
        {
            Name = name;
            if (user != null)
            {
                User = user;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"☆★☆ {Name} ☆★☆");
            if (StartTime.HasValue && EndTime.HasValue)
            {
                builder.AppendLine($"营业时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)}至{EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)}");
            }
            else if (StartTime.HasValue && !EndTime.HasValue)
            {
                builder.AppendLine($"开始营业时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)}");
            }
            else if (!StartTime.HasValue && EndTime.HasValue)
            {
                builder.AppendLine($"停止营业时间：{EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)}");
            }
            else
            {
                builder.AppendLine($"[ 24H ] 全年无休，永久开放");
            }
            builder.AppendLine($"☆--- 商品列表 ---☆");
            foreach (Goods good in Goods.Values)
            {
                builder.AppendLine($"{good.Id}. {good.Name}");
                builder.AppendLine($"商品描述：{good.Description}");
                builder.AppendLine($"商品售价：{string.Join("、", good.Prices.Select(kv => $"{kv.Value} {kv.Key}"))}");
                builder.AppendLine($"包含物品：{string.Join("、", good.Items.Select(i => $"[{ItemSet.GetQualityTypeName(i.QualityType)}|{ItemSet.GetItemTypeName(i.ItemType)}] {i.Name}"))}");
                builder.AppendLine($"剩余库存：{good.Stock}");
            }

            return builder.ToString().Trim();
        }

        public void AddItem(Item item, int stock, string name = "", string description = "")
        {
            long id = Goods.Count > 0 ? Goods.Keys.Max() + 1 : 1;
            if (name.Trim() == "")
            {
                name = item.Name;
            }
            if (description.Trim() == "")
            {
                description = item.Description;
            }
            Goods goods = new(id, item, stock, name, description);
            if (item.Price > 0)
            {
                goods.SetPrice(General.GameplayEquilibriumConstant.InGameCurrency, item.Price);
            }
            Goods.Add(id, goods);
        }

        public void AddItems(IEnumerable<Item> items, int stock)
        {
            foreach (Item item in items)
            {
                AddItem(item, stock);
            }
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Store && other.GetIdName() == GetIdName();
        }
    }

    public class Goods
    {
        public long Id { get; set; }
        public List<Item> Items { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, double> Prices { get; } = [];
        public int Stock { get; set; }

        public Goods(long id, Item item, int stock, string name, string description, Dictionary<string, double>? prices = null)
        {
            Id = id;
            Items = [item];
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

        public void SetPrice(string needy, double price)
        {
            Prices[needy] = price;
        }

        public bool GetPrice(string needy, out double price)
        {
            price = -1;
            if (Prices.TryGetValue(needy, out double temp) && temp > 0)
            {
                price = temp;
                return true;
            }
            return false;
        }
    }
}
