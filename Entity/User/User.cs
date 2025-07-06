using System.Text;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class User : BaseEntity
    {
        public static readonly User Empty = new();
        public override string Name => Username;
        public string Username { get; set; } = "";
        public DateTime RegTime { get; set; }
        public DateTime LastTime { get; set; }
        public OnlineState OnlineState { get; set; } = OnlineState.Offline;
        public string Email { get; set; } = "";
        public string NickName { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        public bool IsOperator { get; set; } = false;
        public bool IsEnable { get; set; } = true;
        public double GameTime { get; set; } = 0;
        public string AutoKey { get; set; } = "";
        public UserProfile Profile { get; }
        public UserStatistics Statistics { get; }
        public Inventory Inventory { get; }

        internal User()
        {
            Profile = new();
            Statistics = new(this);
            Inventory = new(this);
        }

        internal User(long Id = 0, string Username = "", DateTime? RegTime = null, DateTime? LastTime = null, string Email = "", string NickName = "", bool IsAdmin = false, bool IsOperator = false, bool IsEnable = true, double GameTime = 0, string AutoKey = "")
        {
            this.Id = Id;
            this.Username = Username;
            this.RegTime = RegTime ?? General.DefaultTime;
            this.LastTime = LastTime ?? General.DefaultTime;
            this.Email = Email;
            this.NickName = NickName;
            this.IsAdmin = IsAdmin;
            this.IsOperator = IsOperator;
            this.IsEnable = IsEnable;
            this.GameTime = GameTime;
            this.AutoKey = AutoKey;
            Profile = new();
            Statistics = new(this);
            Inventory = new(this);
        }

        internal User(UserType usertype)
        {
            switch (usertype)
            {
                case UserType.General:
                case UserType.Empty:
                    break;
                case UserType.Guest:
                    Id = UserSet.GuestUserId;
                    Username = UserSet.GuestUserName;
                    break;
                case UserType.LocalUser:
                    Id = UserSet.LocalUserId;
                    Username = UserSet.LocalUserName;
                    break;
            }
            Profile = new();
            Statistics = new(this);
            Inventory = new(this);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is User u && u.Id == Id;
        }

        public override string ToString()
        {
            string str = Username;
            if (NickName != "")
            {
                str += " ( " + NickName + " ) ";
            }
            return str;
        }

        public string GetUserInfo()
        {
            StringBuilder builder = new();

            builder.AppendLine($"���� {Username}�Ĵ浵��Ϣ ����");
            builder.AppendLine($"���� ID��{Id}");
            builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameCurrency}��{Inventory.Credits:0.00}");
            builder.AppendLine($"{General.GameplayEquilibriumConstant.InGameMaterial}��{Inventory.Materials:0.00}");
            builder.AppendLine($"��ɫ������{Inventory.Characters.Count}");
            builder.AppendLine($"��ս��ɫ��{Inventory.MainCharacter.ToStringWithLevelWithOutUser()}");
            Character[] squad = [.. Inventory.Characters.Where(c => Inventory.Squad.Contains(c.Id))];
            Dictionary<Character, int> characters = Inventory.Characters
                .Select((character, index) => new { character, index })
                .ToDictionary(x => x.character, x => x.index + 1);
            builder.AppendLine($"С�ӳ�Ա��{(squad.Length > 0 ? string.Join(" / ", squad.Select(c => $"[#{characters[c]}]{c.NickName}({c.Level})")) : "��")}");
            if (Inventory.Training.Count > 0)
            {
                builder.AppendLine($"����������{string.Join(" / ", Inventory.Characters.Where(c => Inventory.Training.ContainsKey(c.Id)).Select(c => c.ToStringWithLevelWithOutUser()))}");
            }
            builder.AppendLine($"��Ʒ������{Inventory.Items.Count}");
            builder.AppendLine($"ע��ʱ�䣺{RegTime.ToString(General.GeneralDateTimeFormatChinese)}");
            builder.AppendLine($"�����ʣ�{LastTime.ToString(General.GeneralDateTimeFormatChinese)}");

            return builder.ToString();
        }
    }
}
