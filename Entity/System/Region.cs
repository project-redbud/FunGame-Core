using System.Text;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Region : BaseEntity
    {
        public string Description { get; set; } = "";
        public HashSet<Character> Organisms { get; } = [];
        public HashSet<Item> Crops { get; } = [];
        public string Weather { get; set; } = "";
        public int Temperature { get; set; } = 15;
        public RarityType Difficulty { get; set; } = RarityType.OneStar;

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

            if (Organisms.Count > 0)
            {
                builder.AppendLine($"== 生物 ==");
                builder.AppendLine(string.Join("，", Organisms.Select(o => o.Name)));
            }
            
            if (Organisms.Count > 0)
            {
                builder.AppendLine($"== 作物 ==");
                builder.AppendLine(string.Join("，", Crops.Select(c => c.Name)));
            }

            builder.AppendLine($"探索难度：{CharacterSet.GetRarityTypeName(Difficulty)}");

            return builder.ToString().Trim();
        }
    }
}
