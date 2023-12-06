using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Interface
{
    public interface IGameModeServer : IAddon
    {
        public abstract bool StartServer(string GameMode, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> OthersServerModel, params object[] args);
    }
}
