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

        public List<Room> GetList()
        {
            return _List.Values.Cast<Room>().ToList();
        }

        public void AddRoom(Room Room)
        {
            _List.Add(Room.Name, Room);
        }
        
        public void RemoveRoom(Room Room)
        {
            _List.Remove(Room.Name);
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
