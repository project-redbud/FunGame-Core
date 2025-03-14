using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IGameModuleServer : IAddon, IAddonController<IGameModuleServer>, IGameModuleDepend
    {
        public bool StartServer(GamingObject obj, params object[] args);

        public Task<Dictionary<string, object>> GamingMessageHandler(IServerModel model, GamingType type, Dictionary<string, object> data);
    }
}
