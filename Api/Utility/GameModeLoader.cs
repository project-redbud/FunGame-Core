using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class GameModeLoader
    {
        public Dictionary<string, GameMode> Modes { get; } = [];
        public Dictionary<string, GameMap> Maps { get; } = [];

        private GameModeLoader()
        {

        }

        public static GameModeLoader LoadPlugins(params object[] objs)
        {
            GameModeLoader loader = new();
            AddonManager.LoadGameModes(loader.Modes, objs);
            AddonManager.LoadGameMaps(loader.Maps, objs);
            return loader;
        }
    }
}
