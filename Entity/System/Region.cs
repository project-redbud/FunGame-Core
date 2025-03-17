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
        public string Weather { get; set; } = "";
        public int Temperature { get; set; } = 15;
        public Dictionary<string, int> Weathers { get; } = [];
        public RarityType Difficulty { get; set; } = RarityType.OneStar;

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
            builder.AppendLine($"温度：{Temperature} °C");
            builder.AppendLine($"{Description}");

            if (Characters.Count > 0)
            {
                builder.AppendLine($"== 头目 ==");
                builder.AppendLine(string.Join("，", Characters.Select(o => o.Name)));
            }

            if (Units.Count > 0)
            {
                builder.AppendLine($"== 生物 ==");
                builder.AppendLine(string.Join("，", Units.Select(o => o.Name)));
            }

            if (Crops.Count > 0)
            {
                builder.AppendLine($"== 作物 ==");
                builder.AppendLine(string.Join("，", Crops.Select(c => c.Name)));
            }

            builder.AppendLine($"探索难度：{CharacterSet.GetRarityTypeName(Difficulty)}");

            return builder.ToString().Trim();
        }
    }
}
