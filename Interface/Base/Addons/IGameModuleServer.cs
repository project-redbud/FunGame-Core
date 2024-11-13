using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IGameModuleServer : IAddon, IAddonController<IGameModuleServer>, IGameModuleDepend
    {
        public bool StartServer(string GameModule, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> ServerModels, params object[] args);

        public Task<Dictionary<string, object>> GamingMessageHandler(string username, GamingType type, Dictionary<string, object> data);
    }
}
