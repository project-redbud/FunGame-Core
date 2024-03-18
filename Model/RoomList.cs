using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class RoomList : IEnumerable<Room>
    {
        private readonly Dictionary<string, Room> _List = [];
        private readonly Dictionary<string, List<User>> _PlayerList = [];
        private readonly Dictionary<string, List<User>> _ReadyPlayerList = [];

        public Room this[string roomid] => GetRoom(roomid);

        public int Count => _List.Count;

        public int GetPlayerCount(string roomid) => GetPlayerList(roomid).Count;

        public int GetReadyPlayerCount(string roomid) => GetReadyPlayerList(roomid).Count;

        public List<Room> ListRoom => _List.Values.ToList();

        public List<string> ListRoomID => _List.Keys.ToList();

        public List<User> GetPlayerList(string roomid) => _PlayerList.TryGetValue(roomid, out List<User>? user) ? user : [];

        public List<User> GetReadyPlayerList(string roomid) => _ReadyPlayerList.TryGetValue(roomid, out List<User>? user) ? user : [];

        public List<User> GetNotReadyPlayerList(string roomid) => _PlayerList.TryGetValue(roomid, out List<User>? user) ? user.Except(GetReadyPlayerList(roomid)).Except([this[roomid].RoomMaster]).ToList() : [];

        public void Clear()
        {
            _List.Clear();
            _PlayerList.Clear();
            _ReadyPlayerList.Clear();
        }

        public void AddRoom(Room room)
        {
            _List.Add(room.Roomid, room);
            _PlayerList.Add(room.Roomid, []);
            _ReadyPlayerList.Add(room.Roomid, []);
        }

        public void AddRooms(List<Room> rooms)
        {
            foreach (Room room in rooms)
            {
                AddRoom(room);
            }
        }

        public void RemoveRoom(string roomid)
        {
            _List.Remove(roomid);
            _PlayerList.Remove(roomid);
            _ReadyPlayerList.Remove(roomid);
        }

        public void RemoveRoom(Room room) => RemoveRoom(room.Roomid);

        public void IntoRoom(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                GetPlayerList(roomid).Add(user);
            }
        }

        public void QuitRoom(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                GetPlayerList(roomid).Remove(user);
            }
        }

        public void SetReady(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                GetReadyPlayerList(roomid).Add(user);
            }
        }

        public void CancelReady(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                GetReadyPlayerList(roomid).Remove(user);
            }
        }

        public Room GetRoom(string roomid) => _List.TryGetValue(roomid, out Room? room) ? room : General.HallInstance;

        public bool IsExist(string roomid) => _List.ContainsKey(roomid);

        public User GetRoomMaster(string roomid)
        {
            foreach (Room room in ListRoom.Where(r => r.Roomid == roomid && r.RoomMaster != null))
            {
                return room.RoomMaster;
            }
            return General.UnknownUserInstance;
        }

        public void SetRoomMaster(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                this[roomid].RoomMaster = user;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (Room room in ListRoom)
            {
                yield return room;
            }
        }

        IEnumerator<Room> IEnumerable<Room>.GetEnumerator()
        {
            foreach (Room room in ListRoom)
            {
                yield return room;
            }
        }
    }
}
