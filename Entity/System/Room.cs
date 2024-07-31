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
        public GameStatistics Statistics { get; set; }

        internal Room()
        {
            Statistics = new(this);
        }

        internal Room(long Id = 0, string Roomid = "-1", DateTime? CreateTime = null, User? RoomMaster = null, RoomType RoomType = RoomType.All, string GameModule = "", string GameMap = "", RoomState RoomState = RoomState.Created, bool IsRank = false, string Password = "")
        {
            this.Id = Id;
            this.Roomid = Roomid;
            this.CreateTime = CreateTime ?? General.DefaultTime;
            this.RoomMaster = RoomMaster ?? General.UnknownUserInstance;
            this.RoomType = RoomType;
            this.GameModule = GameModule;
            this.GameMap = GameMap;
            this.RoomState = RoomState;
            this.IsRank = IsRank;
            this.Password = Password;
            Statistics = new(this);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Room r && r.Roomid == Roomid;
        }
    }
}
