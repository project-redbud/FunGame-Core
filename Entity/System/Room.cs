using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Room : BaseEntity
    {
        public override long Id { get => base.Id ; set => base.Id = value; }
        public string Roomid { get; set; } = "-1";
        public DateTime CreateTime { get; set; } = General.DefaultTime;
        public Dictionary<string, User> Players { get; set; } = new();
        public User? RoomMaster { get; set; }
        public RoomType RoomType { get; set; }
        public RoomState RoomState { get; set; }
        public bool HasPass => Password.Trim() != "";
        public string Password { get; set; } = "";
        public GameStatistics? Statistics { get; set; } = null;

        internal Room()
        {
            
        }

        internal Room(long Id = 0, string Roomid = "-1", DateTime? CreateTime = null, User? RoomMaster = null, RoomType RoomType = RoomType.All, RoomState RoomState = RoomState.Created, string Password = "")
        {
            this.Id = Id;
            this.Roomid = Roomid;
            this.CreateTime = CreateTime ?? General.DefaultTime;
            this.RoomMaster = RoomMaster;
            this.RoomType = RoomType;
            this.RoomState = RoomState;
            this.Password = Password;
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
