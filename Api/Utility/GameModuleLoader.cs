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
        public Dictionary<string, string> ModuleFilePaths => IsHotLoadMode ? new(HotLoadAddonManager.ModuleFilePaths) : new(AddonManager.ModuleFilePaths);

        /// <summary>
        /// 使用可热更新的加载项模式
        /// </summary>
        public bool IsHotLoadMode { get; } = false;

        private GameModuleLoader(bool hotMode = false)
        {
            IsHotLoadMode = hotMode;
        }

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
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
                foreach (GameMap map in loader.Maps.Values.ToList())
                {
                    map.AfterLoad(loader, otherobjs);
                }
                AddonManager.LoadGameModules(loader.Modules, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                foreach (GameModule module in loader.Modules.Values.ToList())
                {
                    // 读取模组的依赖集合
                    module.GameModuleDepend.GetDependencies(loader);
                    // 如果模组加载后需要执行代码，请重写AfterLoad方法
                    module.AfterLoad(loader, otherobjs);
                }
            }
            else if (runtime == FunGameInfo.FunGame.FunGame_Server)
            {
                AddonManager.LoadGameMaps(loader.Maps, otherobjs);
                foreach (GameMap map in loader.Maps.Values.ToList())
                {
                    map.AfterLoad(loader, otherobjs);
                }
                AddonManager.LoadGameModulesForServer(loader.ModuleServers, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                foreach (GameModuleServer server in loader.ModuleServers.Values.ToList())
                {
                    server.GameModuleDepend.GetDependencies(loader);
                    server.AfterLoad(loader, otherobjs);
                }
            }
            return loader;
        }

        /// <summary>
        /// 传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器  [ 可热更新模式 ]
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Desktop"/> 时，仅读取 <seealso cref="Modules"/></para>
        /// <para>runtime = <see cref="FunGameInfo.FunGame.FunGame_Server"/> 时，仅读取 <seealso cref="ModuleServers"/></para>
        /// <seealso cref="Maps"/> 都会读取
        /// </summary>
        /// <param name="runtime">传入 <see cref="FunGameInfo.FunGame"/> 类型来创建指定端的模组读取器</param>
        /// <param name="delegates">用于构建 <see cref="Controller.AddonController{T}"/></param>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static GameModuleLoader LoadGameModulesByHotLoadMode(FunGameInfo.FunGame runtime, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            GameModuleLoader loader = new(true);
            if (runtime == FunGameInfo.FunGame.FunGame_Desktop)
            {
                List<GameMap> updated = HotLoadAddonManager.LoadGameMaps(loader.Maps, otherobjs);
                foreach (GameMap map in updated)
                {
                    map.AfterLoad(loader, otherobjs);
                }
                List<GameModule> updatedModule = HotLoadAddonManager.LoadGameModules(loader.Modules, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                foreach (GameModule module in updatedModule)
                {
                    // 读取模组的依赖集合
                    module.GameModuleDepend.GetDependencies(loader);
                    // 如果模组加载后需要执行代码，请重写AfterLoad方法
                    module.AfterLoad(loader, otherobjs);
                }
            }
            else if (runtime == FunGameInfo.FunGame.FunGame_Server)
            {
                List<GameMap> updated = HotLoadAddonManager.LoadGameMaps(loader.Maps, otherobjs);
                foreach (GameMap map in updated)
                {
                    map.AfterLoad(loader, otherobjs);
                }
                List<GameModuleServer> updatedServer = HotLoadAddonManager.LoadGameModulesForServer(loader.ModuleServers, loader.Characters, loader.Skills, loader.Items, delegates, otherobjs);
                foreach (GameModuleServer server in updatedServer)
                {
                    server.GameModuleDepend.GetDependencies(loader);
                    server.AfterLoad(loader, otherobjs);
                }
            }
            return loader;
        }

        /// <summary>
        /// 热更新
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        public void HotReload(FunGameInfo.FunGame runtime, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            if (!IsHotLoadMode) return;
            if (runtime == FunGameInfo.FunGame.FunGame_Desktop)
            {
                List<GameMap> updated = HotLoadAddonManager.LoadGameMaps(Maps, otherobjs);
                foreach (GameMap map in updated)
                {
                    map.AfterLoad(this, otherobjs);
                }
                List<GameModule> updatedModule = HotLoadAddonManager.LoadGameModules(Modules, Characters, Skills, Items, delegates, otherobjs);
                foreach (GameModule module in updatedModule)
                {
                    // 读取模组的依赖集合
                    module.GameModuleDepend.GetDependencies(this);
                    // 如果模组加载后需要执行代码，请重写AfterLoad方法
                    module.AfterLoad(this, otherobjs);
                }
            }
            else if (runtime == FunGameInfo.FunGame.FunGame_Server)
            {
                List<GameMap> updated = HotLoadAddonManager.LoadGameMaps(Maps, otherobjs);
                foreach (GameMap map in updated)
                {
                    map.AfterLoad(this, otherobjs);
                }
                List<GameModuleServer> updatedServer = HotLoadAddonManager.LoadGameModulesForServer(ModuleServers, Characters, Skills, Items, delegates, otherobjs);
                foreach (GameModuleServer server in updatedServer)
                {
                    server.GameModuleDepend.GetDependencies(this);
                    server.AfterLoad(this, otherobjs);
                }
            }
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
