using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Room : BaseEntity
    {
        public static readonly Room Empty = new();
        public override long Id { get => base.Id; set => base.Id = value; }
        public string Roomid { get; set; } = "-1";
        public DateTime CreateTime { get; set; } = General.DefaultTime;
        public User RoomMaster { get; set; } = General.UnknownUserInstance;
        public RoomType RoomType { get; set; } = RoomType.All;
        public string GameModule { get; set; } = "";
        public string GameMap { get; set; } = "";
        public RoomState RoomState { get; set; }
        public bool IsRank { get; set; } = false;
        public bool HasPass => Password.Trim() != "";
        public string Password { get; set; } = "";
        public int MaxUsers { get; set; } = 0;
        public Dictionary<User, bool> UserAndIsReady { get; } = [];
        public GameStatistics Statistics { get; set; }

        internal Room()
        {
            Statistics = new(this);
        }

        internal Room(long id = 0, string roomid = "-1", DateTime? createTime = null, User? roomMaster = null, RoomType roomType = RoomType.All, string gameModule = "", string gameMap = "", RoomState roomState = RoomState.Created, bool isRank = false, string password = "", int maxUsers = 4)
        {
            Id = id;
            Roomid = roomid;
            CreateTime = createTime ?? General.DefaultTime;
            RoomMaster = roomMaster ?? General.UnknownUserInstance;
            RoomType = roomType;
            GameModule = gameModule;
            GameMap = gameMap;
            RoomState = roomState;
            IsRank = isRank;
            Password = password;
            MaxUsers = maxUsers;
            Statistics = new(this);
        }

        public override string ToString()
        {
            return $"[ {Roomid} ] {Name}";
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Room r && r.Roomid == Roomid;
        }
    }
}
