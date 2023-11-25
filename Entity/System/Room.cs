using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
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
        public GameMode GameMode { get; set; }
        public GameMap GameMap => GameMode.Map;
        public RoomState RoomState { get; set; }
        public bool IsRank { get; set; } = false;
        public bool HasPass => Password.Trim() != "";
        public string Password { get; set; } = "";
        public GameStatistics Statistics { get; set; }

        internal Room()
        {
            GameMode = RoomSet.GetGameMode(RoomType);
            Statistics = new(this);
        }

        internal Room(long Id = 0, string Roomid = "-1", DateTime? CreateTime = null, User? RoomMaster = null, RoomType RoomType = RoomType.All, GameMode? GameMode = null, RoomState RoomState = RoomState.Created, string Password = "")
        {
            this.Id = Id;
            this.Roomid = Roomid;
            this.CreateTime = CreateTime ?? General.DefaultTime;
            this.RoomMaster = RoomMaster ?? General.UnknownUserInstance;
            this.RoomType = RoomType;
            this.GameMode = GameMode ?? RoomSet.GetGameMode(RoomType);
            this.RoomState = RoomState;
            this.Password = Password;
            Statistics = new(this);
        }

        public bool Equals(Room other)
        {
            return Equals(other);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other?.Id == Id;
        }
    }
}
