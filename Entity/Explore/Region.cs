using System.Text;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Region : BaseEntity
    {
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public HashSet<Character> Characters { get; } = [];
        public HashSet<Unit> Units { get; } = [];
        public HashSet<Item> Crops { get; } = [];
        public HashSet<Item> Items { get; } = [];
        public string Weather { get; set; } = "";
        public int Temperature { get; set; } = 15;
        public Dictionary<string, int> Weathers { get; } = [];
        public RarityType Difficulty { get; set; } = RarityType.OneStar;
        public List<string> NPCs { get; set; } = [];
        public List<string> Areas { get; set; } = [];

        public bool ChangeWeather(string weather)
        {
            if (Weathers.TryGetValue(weather, out int temperature))
            {
                Weather = weather;
                Temperature = temperature;
                return true;
            }
            return false;
        }

        public bool ChangeRandomWeather()
        {
            if (Weathers.Count == 0) return false;
            Weather = Weathers.Keys.ElementAt(Random.Shared.Next(Weathers.Count));
            Temperature = Weathers[Weather];
            return true;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Region && other.GetIdName() == GetIdName();
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"☆--- {Name} ---☆");
            builder.AppendLine($"编号：{Id}");
            builder.AppendLine($"天气：{Weather}");
            builder.AppendLine($"温度：{Temperature} ℃");
            builder.AppendLine($"{Description}");

            if (Characters.Count > 0)
            {
                builder.AppendLine($"== 头目 ==");
                builder.AppendLine(string.Join("，", Characters.Select(c => c.Name)));
            }

            if (Units.Count > 0)
            {
                builder.AppendLine($"== 生物 ==");
                builder.AppendLine(string.Join("，", Units.Select(u => u.Name)));
            }

            if (Crops.Count > 0)
            {
                builder.AppendLine($"== 作物 ==");
                builder.AppendLine(string.Join("，", Crops.Select(c => c.Name + (c.Description != "" ? $"：{c.Description}" : "") + (c.BackgroundStory != "" ? $"\"{c.BackgroundStory}\"" : ""))));
            }

            if (Items.Count > 0)
            {
                builder.AppendLine($"== 掉落 ==");
                builder.AppendLine(string.Join("，", Items.Select(i =>
                {
                    string itemquality = ItemSet.GetQualityTypeName(i.QualityType);
                    string itemtype = ItemSet.GetItemTypeName(i.ItemType) + (i.ItemType == ItemType.Weapon && i.WeaponType != WeaponType.None ? "-" + ItemSet.GetWeaponTypeName(i.WeaponType) : "");
                    if (itemtype != "") itemtype = $"|{itemtype}";
                    return $"[{itemquality + itemtype}]{i.Name}";
                })));
            }

            builder.AppendLine($"探索难度：{CharacterSet.GetRarityTypeName(Difficulty)}");

            return builder.ToString().Trim();
        }
    }
}
