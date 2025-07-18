﻿using System.Text;
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
        public bool AutoRefresh { get; set; } = false;
        public DateTime NextRefreshDate { get; set; } = DateTime.MinValue;
        public int RefreshInterval { get; set; } = 1; // Days

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
                builder.AppendLine($"营业时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)} 至 {EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)}");
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
            foreach (Goods goods in Goods.Values)
            {
                builder.AppendLine(goods.ToString());
            }
            builder.AppendLine("提示：使用【商店查看+序号】查看物品详细信息，使用【商店购买+序号】购买物品（指令在 2 分钟内可用）。");
            if (AutoRefresh)
            {
                builder.AppendLine($"商品将在 {NextRefreshDate.ToString(General.GeneralDateTimeFormatChinese)} 刷新。");
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
            goods.SetPrice(GameplayEquilibriumConstant.InGameCurrency, item.Price);
            Goods.Add(id, goods);
        }

        public void AddItems(IEnumerable<Item> items, int stock)
        {
            foreach (Item item in items)
            {
                AddItem(item, stock);
            }
        }

        public void SetPrice(long id, string needy, double price)
        {
            if (Goods.TryGetValue(id, out Goods? goods) && goods != null)
            {
                goods.SetPrice(needy, price);
            }
        }

        public bool GetPrice(long id, string needy, out double price)
        {
            price = 0;
            if (Goods.TryGetValue(id, out Goods? goods) && goods != null)
            {
                return goods.GetPrice(needy, out price);
            }
            return false;
        }

        public double GetPrice(long id)
        {
            double price = 0;
            if (Goods.TryGetValue(id, out Goods? goods) && goods != null)
            {
                goods.GetPrice(GameplayEquilibriumConstant.InGameCurrency, out price);
            }
            return price;
        }

        public void UpdateRefreshTime(DateTime? time = null)
        {
            if (AutoRefresh)
            {
                time ??= DateTime.Now;
                NextRefreshDate = time.Value.AddDays(RefreshInterval);
            }
            else
            {
                NextRefreshDate = DateTime.MinValue;
            }
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Store && other.GetIdName() == GetIdName();
        }
    }
}
