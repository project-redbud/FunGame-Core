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

        internal Room(DataSet? DsRoom, DataSet? DsUser)
        {
            if (DsRoom != null && DsRoom.Tables.Count > 0 && DsRoom.Tables[0].Rows.Count > 0)
            {
                DataRow row = DsRoom.Tables[0].Rows[0];
                Id = (long)row[RoomQuery.Column_ID];
                Roomid = (string)row[RoomQuery.Column_RoomID];
                CreateTime = (DateTime)row[RoomQuery.Column_CreateTime];
                RoomMaster = Api.Utility.Factory.GetInstance<User>(DsUser);
                RoomType = (RoomType)Convert.ToInt32(row[RoomQuery.Column_RoomType]);
                RoomState = (RoomState)Convert.ToInt32(row[RoomQuery.Column_RoomState]);
                Password = (string)row[RoomQuery.Column_Password];
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
