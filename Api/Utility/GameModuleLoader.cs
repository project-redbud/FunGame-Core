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
        public Dictionary<string, GameModuleServer> ModuleServers { get; } = [];

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
        /// 已加载的模组DLL名称对应的路径
        /// </summary>
        public static Dictionary<string, string> ModuleFilePaths => new(AddonManager.ModuleFilePaths);

        private GameModuleLoader() { }

        /// <summary>
        /// 传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Desktop"/> 时，仅读取 <seealso cref="Modules"/></para>
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Server"/> 时，仅读取 <seealso cref="ModuleServers"/></para>
        /// <seealso cref="Maps"/> 都会读取
        /// </summary>
        /// <param name="runtime">传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器</param>
        /// <param name="delegates">用于构建 <see cref="Controller.AddonController{T}"/></param>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static GameModuleLoader LoadGameModules(FunGameInfo.FunGame runtime, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            GameModuleLoader loader = new();
            if (runtime == FunGameInfo.FunGame.FunGame_Desktop)
            {
                AddonManager.LoadGameModules(loader.Modules, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
                foreach (GameModule module in loader.Modules.Values.ToList())
                {
                    // 读取模组的依赖集合
                    module.GameModuleDepend.GetDependencies(loader);
                    // 如果模组加载后需要执行代码，请重写AfterLoad方法
                    module.AfterLoad(loader);
                }
            }
            else if (runtime == FunGameInfo.FunGame.FunGame_Server)
            {
                AddonManager.LoadGameModulesForServer(loader.ModuleServers, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
                foreach (GameModuleServer server in loader.ModuleServers.Values.ToList())
                {
                    server.GameModuleDepend.GetDependencies(loader);
                    server.AfterLoad(loader);
                }
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
            return ModuleServers[name];
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
