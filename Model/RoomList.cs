using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class RoomList : IEnumerable
    {
        private readonly Hashtable _List = new();
        private readonly Hashtable _PlayerList = new();

        public int Count => _List.Count;

        public List<Room> ListRoom => _List.Values.Cast<Room>().ToList();

        public List<string> ListRoomID => _List.Keys.Cast<string>().ToList();

        public RoomList()
        {

        }

        public Room this[string RoomID] => GetRoom(RoomID);

        public void Clear()
        {
            _List.Clear();
            _PlayerList.Clear();
        }

        public void AddRoom(Room Room)
        {
            _List.Add(Room.Roomid, Room);
            _PlayerList.Add(Room.Roomid, new List<User>());
        }

        public void AddRooms(List<Room> Rooms)
        {
            foreach (Room Room in Rooms)
            {
                AddRoom(Room);
            }
        }

        public void RemoveRoom(string RoomID)
        {
            _List.Remove(RoomID);
            _PlayerList.Remove(RoomID);
        }

        public void RemoveRoom(Room Room)
        {
            RemoveRoom(Room.Roomid);
        }

        public void IntoRoom(string RoomID, User Player)
        {
            if (RoomID == "-1" || Player.Id == 0) return;
            GetPlayerList(RoomID).Add(Player);
        }

        public void QuitRoom(string RoomID, User Player)
        {
            if (RoomID == "-1" || Player.Id == 0) return;
            GetPlayerList(RoomID).Remove(Player);
        }

        public Room GetRoom(string RoomID)
        {
            Room? room = null;
            if (_List.ContainsKey(RoomID))
            {
                room = (Room?)_List[RoomID];
            }
            room ??= General.HallInstance;
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

        public void SetRoomMaster(string RoomID, User User)
        {
            Room? room = this[RoomID];
            if (room != null)
            {
                room.RoomMaster = User;
            }
        }

        public List<User> GetPlayerList(string RoomID)
        {
            List<User>? list = new();
            if (_PlayerList.ContainsKey(RoomID) && _PlayerList[RoomID] != null)
            {
                list = (List<User>?)_PlayerList[RoomID];
            }
            list ??= new();
            return list;
        }

        public int GetPlayerCount(string RoomID) => GetPlayerList(RoomID).Count;

        public IEnumerator GetEnumerator()
        {
            foreach (Room room in ListRoom)
            {
                yield return room;
            }
        }
    }
}
