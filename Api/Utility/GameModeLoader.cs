using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class GameModeLoader
    {
        public Dictionary<string, GameMode> Modes { get; } = [];
        public Dictionary<string, GameMap> Maps { get; } = [];
        public List<Character> Characters { get; } = [];
        public List<Skill> Skills { get; } = [];
        public List<Item> Items { get; } = [];

        private GameModeLoader()
        {

        }

        public static GameModeLoader LoadGameModes(params object[] objs)
        {
            GameModeLoader loader = new();
            AddonManager.LoadGameModes(loader.Modes, loader.Characters, loader.Skills, loader.Items, objs);
            AddonManager.LoadGameMaps(loader.Maps, objs);
            return loader;
        }

        public GameMode this[string name]
        {
            get
            {
                return Modes[name];
            }
            set
            {
                Modes.TryAdd(name, value);
            }
        }

        public GameMap GetGameMap(string name)
        {
            return Maps[name];
        }
    }
}
