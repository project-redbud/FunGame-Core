using System.Collections;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class GameModuleLoader
    {
        /// <summary>
        /// 适用于客户端的模组集
        /// </summary>
        public Dictionary<string, GameModule> Modules { get; } = [];

        /// <summary>
        /// 适用于服务器的模组集
        /// </summary>
        public Dictionary<string, GameModuleServer> ServerModules { get; } = [];

        /// <summary>
        /// 游戏地图集
        /// </summary>
        public Dictionary<string, GameMap> Maps { get; } = [];

        /// <summary>
        /// 角色表
        /// </summary>
        public Dictionary<string, CharacterModule> Characters { get; } = [];

        /// <summary>
        /// 技能表
        /// </summary>
        public Dictionary<string, SkillModule> Skills { get; } = [];

        /// <summary>
        /// 物品表
        /// </summary>
        public Dictionary<string, ItemModule> Items { get; } = [];

        /// <summary>
        /// 客户端模组与服务器模组的关联字典
        /// </summary>
        public Dictionary<GameModule, GameModuleServer?> AssociatedServers { get; } = [];

        private GameModuleLoader() { }

        /// <summary>
        /// 传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Desktop"/> 时，仅读取 <seealso cref="Modules"/></para>
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Server"/> 时，都会读取，并且生成关联字典 <see cref="AssociatedServers"/></para>
        /// <seealso cref="Maps"/> 都会读取
        /// </summary>
        /// <param name="runtime">传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器</param>
        /// <param name="delegates">用于构建 <see cref="Controller.AddonController{T}"/></param>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static GameModuleLoader LoadGameModules(FunGameInfo.FunGame runtime, Hashtable delegates, params object[] otherobjs)
        {
            GameModuleLoader loader = new();
            if (runtime == FunGameInfo.FunGame.FunGame_Desktop)
            {
                AddonManager.LoadGameModules(loader.Modules, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
            }
            else if (runtime == FunGameInfo.FunGame.FunGame_Server)
            {
                AddonManager.LoadGameModulesForServer(loader.Modules, loader.ServerModules, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                foreach (GameModule module in loader.Modules.Values)
                {
                    // AssociatedServerModuleName 已经存包含 IsConnectToOtherServerModule 的判断，因此无需重复判断
                    if (loader.ServerModules.TryGetValue(module.AssociatedServerModuleName, out GameModuleServer? server) && server != null)
                    {
                        loader.AssociatedServers.Add(module, server);
                    }
                    else loader.AssociatedServers.Add(module, null); // 服务器获取GameModuleServer时需要判断是否存在模组。
                }
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
        public GameModule this[string name]
        {
            get
            {
                return Modules[name];
            }
            set
            {
                Modules.TryAdd(name, value);
            }
        }

        /// <summary>
        /// 获取对应名称的服务器模组实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameModuleServer GetServerMode(string name)
        {
            return ServerModules[name];
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
