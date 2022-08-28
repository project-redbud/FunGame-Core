using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Models.Entity
{
    public class Room
    {
        public int Id { get; set; }
        public string Roomid { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
        public Hashtable PlayerList { get; set; } = new Hashtable();
        public User? RoomMaster { get; set; }
        public int RoomType { get; set; }
        public int RoomState { get; set; }
        public GameStatistics? Statistics { get; set; } = null;

        public Room(User? master = null)
        {
            if (master != null) RoomMaster = master;
        }

        public Room(string roomid, User? master = null)
        {
            Roomid = roomid;
            if (master != null) RoomMaster = master;
        }
    }
}
