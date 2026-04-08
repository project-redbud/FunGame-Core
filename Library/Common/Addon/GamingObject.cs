using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public class GamingObject(Room room, List<User> users, IServerModel roomMaster, Dictionary<string, IServerModel> serverModels)
    {
        public bool Running { get; set; } = true;
        public Room Room { get; } = room;
        public List<User> Users { get; } = users;
        public IServerModel RoomMaster { get; } = roomMaster;
        public Dictionary<string, IServerModel> All { get; } = serverModels;

        public bool HasUser(long id)
        {
            return Users.Any(u => u.Id == id);
        }

        public void UserReconnect(User newUser)
        {
            Users.RemoveAll(u => u.Id == newUser.Id);
            Users.Add(newUser);
        }
    }
}
