using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using System.Collections;

namespace Milimoe.FunGame.Core.Entity
{
    public class Room : BaseEntity
    {
        public string Roomid { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
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

        internal Room(User? master = null)
        {
            if (master != null) RoomMaster = master;
        }
        
        internal Room(string roomid, User? master = null)
        {
            Roomid = roomid;
            if (master != null) RoomMaster = master;
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
