using System.Collections;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class GameModeLoader
    {
        /// <summary>
        /// 适用于客户端的模组集
        /// </summary>
        public Dictionary<string, GameMode> Modes { get; } = [];

        /// <summary>
        /// 适用于服务器的模组集
        /// </summary>
        public Dictionary<string, GameModeServer> ServerModes { get; } = [];

        /// <summary>
        /// 游戏地图集
        /// </summary>
        public Dictionary<string, GameMap> Maps { get; } = [];

        /// <summary>
        /// 角色表
        /// </summary>
        public List<Character> Characters { get; } = [];

        /// <summary>
        /// 技能表
        /// </summary>
        public List<Skill> Skills { get; } = [];

        /// <summary>
        /// 物品表
        /// </summary>
        public List<Item> Items { get; } = [];

        private GameModeLoader()
        {

        }

        /// <summary>
        /// 传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Desktop"/> 时，仅读取 <seealso cref="Modes"/></para>
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Server"/> 时，仅读取 <seealso cref="ServerModes"/></para>
        /// <seealso cref="Maps"/> 都会读取
        /// </summary>
        /// <param name="runtime">传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器</param>
        /// <param name="delegates">用于构建 <see cref="Controller.AddonController"/></param>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static GameModeLoader LoadGameModes(FunGameInfo.FunGame runtime, Hashtable delegates, params object[] otherobjs)
        {
            GameModeLoader loader = new();
            if (runtime == FunGameInfo.FunGame.FunGame_Desktop)
            {
                AddonManager.LoadGameModes(loader.Modes, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
            }
            else if (runtime == FunGameInfo.FunGame.FunGame_Server)
            {
                AddonManager.LoadGameModesForServer(loader.ServerModes, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
            }
            return loader;
        }

        /// <summary>
        /// 获取对应名称的模组实例
        /// <para>如果需要取得服务器模组的实例，请调用 <see cref="GetServerMode"/></para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取对应名称的服务器模组实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameModeServer GetServerMode(string name)
        {
            return ServerModes[name];
        }

        /// <summary>
        /// 获取对应名称的游戏地图
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameMap GetGameMap(string name)
        {
            return Maps[name];
        }
    }
}
