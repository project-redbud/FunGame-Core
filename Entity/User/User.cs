using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class User : BaseEntity
    {
        public static readonly User Empty = new();
        public string Username { get; set; } = "";
        public DateTime RegTime { get; set; }
        public DateTime LastTime { get; set; }
        public string Email { get; set; } = "";
        public string NickName { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        public bool IsOperator { get; set; } = false;
        public bool IsEnable { get; set; } = true;
        public double GameTime { get; set; } = 0;
        public string AutoKey { get; set; } = "";
        public UserStatistics Statistics { get; }
        public Inventory Inventory { get; }

        internal User()
        {
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
    }
}
