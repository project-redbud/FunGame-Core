using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Entity
{
    public class Inventory
    {
        /// <summary>
        /// 库存 ID 与用户 ID 绑定
        /// </summary>
        public long Id => User.Id;

        /// <summary>
        /// 库存的名称，默认为 “<see cref="User.Username"/>的库存”；可更改
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 库存属于哪个玩家
        /// </summary>
        public User User { get; }

        /// <summary>
        /// 玩家持有 <see cref="EquilibriumConstant.InGameCurrency"/> 的数量
        /// </summary>
        public double Credits { get; set; } = 0;

        /// <summary>
        /// 玩家持有 <see cref="EquilibriumConstant.InGameMaterial"/> 的数量
        /// </summary>
        public double Materials { get; set; } = 0;

        /// <summary>
        /// 玩家拥有的角色
        /// </summary>
        public HashSet<Character> Characters { get; } = [];

        /// <summary>
        /// 玩家拥有的物品
        /// </summary>
        public HashSet<Item> Items { get; } = [];

        /// <summary>
        /// 主战角色
        /// </summary>
        public Character MainCharacter
        {
            get
            {
                if (_character != null)
                {
                    return _character;
                }
                else if (Characters.Count > 0)
                {
                    _character = Characters.First();
                    return _character;
                }
                return Factory.GetCharacter();
            }
            set
            {
                _character = value;
            }
        }

        /// <summary>
        /// 小队
        /// </summary>
        public HashSet<long> Squad { get; set; } = [];
        
        /// <summary>
        /// 练级中的角色
        /// </summary>
        public Dictionary<long, DateTime> Training { get; set; } = [];

        private Character? _character;

        internal Inventory(User user)
        {
            User = user;
            Name = user.Username + "的库存";
        }

        public override string ToString()
        {
            return Name + $"（{User}）";
        }

        public string ToString(bool showAll)
        {
            StringBuilder builder = new();

            builder.AppendLine($"☆★☆ {Name} ☆★☆");
            builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameCurrency}：{Credits:0.00}");
            builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameMaterial}：{Materials:0.00}");

            builder.AppendLine($"======= 角色 =======");
            Character[] characters = [.. Characters];
            for (int i = 1; i <= characters.Length; i++)
            {
                Character character = characters[i - 1];
                if (showAll)
                {
                    builder.AppendLine($"===== 第 {i} 个角色 =====");
                    builder.AppendLine($"{character.GetInfo(false)}");
                }
                else
                {
                    builder.AppendLine($"{i}. {character.ToStringWithLevelWithOutUser()}");
                }
            }
            
            builder.AppendLine($"======= 物品 =======");
            Item[] items = [.. Items];
            for (int i = 1; i <= items.Length; i++)
            {
                Item item = items[i - 1];
                if (showAll)
                {
                    builder.AppendLine($"===== 第 {i} 个物品 =====");
                }
                else
                {
                    builder.AppendLine($"{i}. [{ItemSet.GetQualityTypeName(item.QualityType)}|{ItemSet.GetItemTypeName(item.ItemType)}] {item.Name}");
                }
                builder.AppendLine($"{item.ToStringInventory(showAll).Trim()}");
                if (showAll) builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
