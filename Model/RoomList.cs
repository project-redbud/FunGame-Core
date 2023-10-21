using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class RoomList : IEnumerable
    {
        private readonly Dictionary<string, Room> _List = new();
        private readonly Dictionary<string, List<User>> _PlayerList = new();

        public int Count => _List.Count;

        public List<Room> ListRoom => _List.Values.ToList();

        public List<string> ListRoomID => _List.Keys.ToList();

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

        public void RemoveRoom(Room Room) => RemoveRoom(Room.Roomid);

        public void IntoRoom(string RoomID, User User)
        {
            if (RoomID != "-1" && User.Id != 0)
            {
                GetPlayerList(RoomID).Add(User);
            }
        }

        public void QuitRoom(string RoomID, User User)
        {
            if (RoomID != "-1" && User.Id != 0)
            {
                GetPlayerList(RoomID).Remove(User);
            }
        }

        public Room GetRoom(string RoomID) => _List.ContainsKey(RoomID) ? _List[RoomID] : General.HallInstance;

        public bool IsExist(string RoomID) => _List.ContainsKey(RoomID);

        public User GetRoomMaster(string RoomID)
        {
            foreach (Room room in ListRoom.Where(r => r.Roomid == RoomID && r.RoomMaster != null))
            {
                return room.RoomMaster;
            }
            return General.UnknownUserInstance;
        }

        public void SetRoomMaster(string RoomID, User User)
        {
            if (RoomID != "-1" && User.Id != 0)
            {
                this[RoomID].RoomMaster = User;
            }
        }

        public List<User> GetPlayerList(string RoomID) => _PlayerList.ContainsKey(RoomID) ? _PlayerList[RoomID] : new();

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
