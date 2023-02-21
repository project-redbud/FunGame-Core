using Milimoe.FunGame.Core.Library.Constant;
using System.Collections;

namespace Milimoe.FunGame.Core.Entity
{
    public class Room
    {
        public int Id { get; set; }
        public string Roomid { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
        public Hashtable PlayerList { get; set; } = new Hashtable();
        public User? RoomMaster { get; set; }
        public RoomType RoomType { get; set; }
        public RoomState RoomState { get; set; }
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
    }
}
