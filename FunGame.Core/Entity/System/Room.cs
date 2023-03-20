using System.Data;
using System.Collections;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class Room : BaseEntity
    {
        public override long Id { get => base.Id ; set => base.Id = value; }
        public string Roomid { get; set; } = "";
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public Hashtable PlayerList { get; set; } = new Hashtable();
        public User? RoomMaster { get; set; }
        public RoomType RoomType { get; set; }
        public RoomState RoomState { get; set; }
        public bool HasPass
        {
            get
            {
                if (RoomType == RoomType.MixHasPass || RoomType == RoomType.TeamHasPass)
                    return true;
                else return false;
            }
        }
        public string Password { get; set; } = "";
        public GameStatistics? Statistics { get; set; } = null;

        internal Room(DataRow? DrRoom, DataRow? DrUser)
        {
            if (DrRoom != null)
            {
                Id = (long)DrRoom[RoomQuery.Column_ID];
                Roomid = (string)DrRoom[RoomQuery.Column_RoomID];
                CreateTime = (DateTime)DrRoom[RoomQuery.Column_CreateTime];
                RoomMaster = Api.Utility.Factory.GetInstance<User>(DrUser);
                RoomType = (RoomType)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomType]);
                RoomState = (RoomState)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomState]);
                Password = (string)DrRoom[RoomQuery.Column_Password];
            }
        }

        public bool Equals(Room other)
        {
            return other.Id == Id;
        }

        public override IEnumerator<Room> GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(IBaseEntity? other)
        {
            return Equals(other);
        }
    }
}
