using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public readonly struct GamingObject(Room room, List<User> users, IServerModel roomMaster, Dictionary<string, IServerModel> serverModels)
    {
        public Room Room { get; } = room;
        public List<User> Users { get; } = users;
        public IServerModel RoomMaster { get; } = roomMaster;
        public Dictionary<string, IServerModel> All { get; } = serverModels;
    }
}
