using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Core.Library.Server
{
    public class RoomList
    {
        public ServerSocket Server => _Server;
        public int Count => _List.Count;

        private readonly ServerSocket _Server;
        private readonly Hashtable _List = new();

        public RoomList(ServerSocket Server)
        {
            _Server = Server;
        }

        public Room? this[string RoomID] => GetRoom(RoomID);

        public Hashtable GetHashTable => _List;

        public List<Room> GetRoomList => _List.Values.Cast<Room>().ToList();
        
        public List<string> GetRoomIDList => _List.Keys.Cast<string>().ToList();

        public void Clear()
        {
            _List.Clear();
        }

        public void AddRoom(Room Room)
        {
            _List.Add(Room.Roomid, Room);
        }
        
        public void AddRooms(List<Room> Rooms)
        {
            foreach (Room Room in Rooms)
            {
                _List.Add(Room.Roomid, Room);
            }
        }
        
        public void RemoveRoom(Room Room)
        {
            _List.Remove(Room.Roomid);
        }

        public Room? GetRoom(string RoomID)
        {
            Room? room = null;
            if (_List.ContainsKey(RoomID))
            {
                room = (Room?)_List[RoomID];
            }
            return room;
        }
    }
}
