using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class RoomList : IEnumerable<Room>
    {
        private readonly Dictionary<string, Room> _list = [];
        private readonly Dictionary<string, List<User>> _userList = [];
        private readonly Dictionary<string, List<User>> _readyUserList = [];

        public Room this[string roomid] => GetRoom(roomid);

        public int Count => _list.Count;

        public int GetUserCount(string roomid) => this[roomid].UserAndIsReady.Count;

        public int GetReadyUserCount(string roomid) => GetReadyUserList(roomid).Count;

        public List<Room> ListRoom => [.. _list.Values];

        public List<string> ListRoomID => [.. _list.Keys];

        public User GetRoomMaster(string roomid) => this[roomid].RoomMaster;

        public List<User> GetUsers(string roomid) => [.. this[roomid].UserAndIsReady.Keys];

        public List<User> GetReadyUserList(string roomid) => [.. this[roomid].UserAndIsReady.Where(kv => kv.Value && kv.Key.Id != GetRoomMaster(roomid).Id).Select(kv => kv.Key)];

        public List<User> GetNotReadyUserList(string roomid) => [.. this[roomid].UserAndIsReady.Where(kv => !kv.Value && kv.Key.Id != GetRoomMaster(roomid).Id).Select(kv => kv.Key)];

        public void Clear()
        {
            _list.Clear();
            _userList.Clear();
            _readyUserList.Clear();
        }

        public void AddRoom(Room room)
        {
            _list.Add(room.Roomid, room);
            _userList.Add(room.Roomid, []);
            _readyUserList.Add(room.Roomid, []);
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
            _list.Remove(roomid);
            _userList.Remove(roomid);
            _readyUserList.Remove(roomid);
        }

        public void RemoveRoom(Room room) => RemoveRoom(room.Roomid);

        public void IntoRoom(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                if (!this[roomid].UserAndIsReady.TryAdd(user, false))
                {
                    this[roomid].UserAndIsReady[user] = false;
                }
            }
        }

        public void QuitRoom(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                this[roomid].UserAndIsReady.Remove(user);
            }
        }

        public void SetReady(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0 && this[roomid].UserAndIsReady.ContainsKey(user))
            {
                this[roomid].UserAndIsReady[user] = true;
            }
        }

        public void CancelReady(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0 && this[roomid].UserAndIsReady.ContainsKey(user))
            {
                this[roomid].UserAndIsReady[user] = false;
            }
        }

        public Room GetRoom(string roomid) => _list.TryGetValue(roomid, out Room? room) ? room : General.HallInstance;

        public bool Exists(string roomid) => _list.ContainsKey(roomid);

        public void SetRoomMaster(string roomid, User user)
        {
            if (roomid != "-1" && user.Id != 0)
            {
                this[roomid].RoomMaster = user;
            }
        }

        public IEnumerator<Room> GetEnumerator()
        {
            foreach (Room room in ListRoom)
            {
                yield return room;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
