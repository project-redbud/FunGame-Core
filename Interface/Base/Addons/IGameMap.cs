using Milimoe.FunGame.Core.Api.Utility;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IGameMap : IAddon
    {
        public GameModuleLoader? ModuleLoader { get; }
    }
}
