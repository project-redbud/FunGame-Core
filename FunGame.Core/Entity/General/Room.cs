using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.General
{
    public class Room
    {
        public int Id { get; set; }
        public string Roomid { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
        public Hashtable PlayerList { get; set; } = new Hashtable();
        public User? RoomMaster { get; set; }
        public Milimoe.FunGame.Core.Entity.Enum.RoomType RoomType { get; set; }
        public Milimoe.FunGame.Core.Entity.Enum.RoomState RoomState { get; set; }
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
