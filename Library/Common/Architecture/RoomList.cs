using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public class RoomList : IEnumerable
    {
        private readonly Hashtable _List = new();

        public int Count => _List.Count;

        public List<Room> ListRoom => _List.Values.Cast<Room>().ToList();

        public List<string> ListRoomID => _List.Keys.Cast<string>().ToList();

        public RoomList()
        {

        }
        
        public Room? this[string RoomID] => GetRoom(RoomID);

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

        public bool IsExist(string RoomID)
        {
            return _List.ContainsKey(RoomID);
        }

        public User GetRoomMaster(string RoomID)
        {
            foreach (Room room in ListRoom)
            {
                if (room.Roomid == RoomID && room.RoomMaster != null)
                {
                    return room.RoomMaster;
                }
            }
            return General.UnknownUserInstance;
        }

        public IEnumerator GetEnumerator()
        {
            foreach(Room room in ListRoom)
            {
                yield return room;
            }
        }
    }
}
