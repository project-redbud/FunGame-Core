using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Interface.Base.Addons;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Service
{
    /// <summary>
    /// 支持热更新、可卸载、可重载插件/模组
    /// 支持插件跨 DLL 引用模组，通过全局访问最新实例
    /// </summary>
    internal static class HotLoadAddonManager
    {
        /// <summary>
        /// 已加载的插件DLL名称对应的路径
        /// </summary>
        internal static Dictionary<string, string> PluginFilePaths { get; } = [];

        /// <summary>
        /// 已加载的模组DLL名称对应的路径
        /// </summary>
        internal static Dictionary<string, string> ModuleFilePaths { get; } = [];

        /// <summary>
        /// 已加载的插件
        /// </summary>
        internal static Dictionary<string, IAddon> Plugins { get; } = [];

        /// <summary>
        /// 已加载的模组
        /// </summary>
        internal static Dictionary<string, IAddon> Modules { get; } = [];

        /// <summary>
        /// key = 文件路径（全小写），value = 当前加载上下文 + 程序集 + 根实例
        /// </summary>
        private static readonly ConcurrentDictionary<string, DLLAddonEntry> _loadedDLLs = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 即将清除的上下文
        /// </summary>
        private static readonly List<WeakReference<AssemblyLoadContext>> _contextsToClean = [];

        /// <summary>
        /// 尝试加载或重新加载某个 DLL，返回是否成功加载/更新
        /// </summary>
        /// <param name="fullPath">DLL 完整路径</param>
        /// <param name="newInstances">新加载的实例列表</param>
        /// <param name="isPlugin">是否为插件</param>
        /// <param name="isModule">是否为模组</param>
        /// <returns>是否成功（无变化不成功）</returns>
        private static bool TryLoadOrReload(string fullPath, out List<IAddon> newInstances, bool isPlugin, bool isModule)
        {
            newInstances = [];

            if (!File.Exists(fullPath)) return false;

            string key = fullPath.ToLowerInvariant();
            DateTime currentWriteTime = File.GetLastWriteTimeUtc(fullPath);

            // 文件无变化 → 返回现有实例
            if (_loadedDLLs.TryGetValue(key, out DLLAddonEntry? entry) && entry.LastWriteTimeUtc == currentWriteTime)
            {
                foreach (AddonSubEntry sub in entry.Addons)
                {
                    if (sub.Weak.TryGetTarget(out IAddon? target) && target != null)
                        newInstances.Add(target);
                }
                return false;
            }

            // 需要卸载旧 DLL
            if (_loadedDLLs.TryRemove(key, out DLLAddonEntry? oldEntry))
            {
                try
                {
                    foreach (AddonSubEntry sub in oldEntry.Addons)
                    {
                        if (sub.Instance is IHotReloadAware aware)
                        {
                            aware.OnBeforeUnload();
                        }
                        sub.Instance.UnLoad();
                        Plugins.Remove(sub.Name);
                        Modules.Remove(sub.Name);
                    }
                    oldEntry.Context.Unload();
                    GC.Collect(2, GCCollectionMode.Forced, true);
                }
                catch (Exception e)
                {
                    throw new AddonUnloadException(fullPath, e);
                }
                finally
                {
                    lock (_contextsToClean)
                    {
                        _contextsToClean.Add(new WeakReference<AssemblyLoadContext>(oldEntry.Context));
                    }
                }
            }

            // 加载新 DLL
            try
            {
                string dllName = Path.GetFileNameWithoutExtension(fullPath);
                AddonLoadContext ctx = new(dllName, fullPath);

                using FileStream fs = new(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                Assembly assembly = ctx.LoadFromStream(fs);
                string filename = assembly.GetName().Name?.Trim() ?? "";

                Type[] addonTypes = [.. assembly.GetTypes().Where(t => !t.IsAbstract && typeof(IAddon).IsAssignableFrom(t) && (typeof(Plugin).IsAssignableFrom(t) || typeof(ServerPlugin).IsAssignableFrom(t) ||
                    typeof(WebAPIPlugin).IsAssignableFrom(t) || typeof(GameModule).IsAssignableFrom(t) || typeof(GameModuleServer).IsAssignableFrom(t) || typeof(CharacterModule).IsAssignableFrom(t) ||
                    typeof(SkillModule).IsAssignableFrom(t) || typeof(ItemModule).IsAssignableFrom(t) || typeof(GameMap).IsAssignableFrom(t)))];

                DLLAddonEntry newEntry = new()
                {
                    Context = ctx,
                    Assembly = assembly,
                    LastWriteTimeUtc = currentWriteTime
                };

                foreach (Type addonType in addonTypes)
                {
                    try
                    {
                        if (Activator.CreateInstance(addonType) is not IAddon instance) continue;

                        // 为了安全起见，未实现此接口的不允许使用热更新模式加载
                        if (instance is not IHotReloadAware aware) continue;

                        string addonName = instance.Name?.Trim() ?? addonType.Name;
                        AddonSubEntry sub = new()
                        {
                            Instance = instance,
                            Weak = new WeakReference<IAddon>(instance),
                            Name = addonName
                        };

                        newEntry.Addons.Add(sub);
                        newInstances.Add(instance);

                        if (isPlugin) Plugins[addonName] = instance;
                        if (isModule) Modules[addonName] = instance;
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }

                if (newInstances.Count > 0)
                {
                    _loadedDLLs[key] = newEntry;
                    if (isPlugin) PluginFilePaths[filename] = key;
                    if (isModule) ModuleFilePaths[filename] = key;
                }
                return true;
            }
            catch (Exception e)
            {
                throw new AddonLoadException(fullPath, e);
            }
        }

        /// <summary>
        /// 从 plugins 目录加载所有插件
        /// </summary>
        /// <param name="plugins">插件字典</param>
        /// <param name="delegates">委托字典</param>
        /// <param name="otherobjs">其他参数</param>
        /// <returns>被更新的插件列表</returns>
        internal static List<Plugin> LoadPlugins(Dictionary<string, Plugin> plugins, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            List<Plugin> updated = [];
            if (!Directory.Exists(ReflectionSet.PluginFolderPath)) return updated;

            string[] dlls = Directory.GetFiles(ReflectionSet.PluginFolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in dlls)
            {
                bool loaded = TryLoadOrReload(dll, out List<IAddon> instances, true, false);
                foreach (IAddon instance in instances)
                {
                    string name = instance.Name.Trim();
                    if (instance is Plugin pluginInstance)
                    {
                        // 热更新无变化的文件时，不会再触发 Load 方法
                        if ((loaded || !plugins.ContainsKey(name)) && pluginInstance.Load(otherobjs))
                        {
                            pluginInstance.Controller = new(pluginInstance, delegates);
                            updated.Add(pluginInstance);
                        }
                        plugins[name] = pluginInstance;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// 从 plugins 目录加载所有 Server 插件
        /// </summary>
        /// <param name="plugins">插件字典</param>
        /// <param name="delegates">委托字典</param>
        /// <param name="otherobjs">其他参数</param>
        /// <returns>被更新的插件列表</returns>
        internal static List<ServerPlugin> LoadServerPlugins(Dictionary<string, ServerPlugin> plugins, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            List<ServerPlugin> updated = [];
            if (!Directory.Exists(ReflectionSet.PluginFolderPath)) return updated;

            string[] dlls = Directory.GetFiles(ReflectionSet.PluginFolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in dlls)
            {
                bool loaded = TryLoadOrReload(dll, out List<IAddon> instances, true, false);
                foreach (IAddon instance in instances)
                {
                    string name = instance.Name.Trim();
                    if (instance is ServerPlugin pluginInstance)
                    {
                        // 热更新无变化的文件时，不会再触发 Load 方法
                        if ((loaded || !plugins.ContainsKey(name)) && pluginInstance.Load(otherobjs))
                        {
                            pluginInstance.Controller = new(pluginInstance, delegates);
                            updated.Add(pluginInstance);
                        }
                        plugins[name] = pluginInstance;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// 从 plugins 目录加载所有 WebAPI 插件
        /// </summary>
        /// <param name="plugins">插件字典</param>
        /// <param name="delegates">委托字典</param>
        /// <param name="otherobjs">其他参数</param>
        /// <returns>被更新的插件列表</returns>
        internal static List<WebAPIPlugin> LoadWebAPIPlugins(Dictionary<string, WebAPIPlugin> plugins, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            List<WebAPIPlugin> updated = [];
            if (!Directory.Exists(ReflectionSet.PluginFolderPath)) return updated;

            string[] dlls = Directory.GetFiles(ReflectionSet.PluginFolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in dlls)
            {
                bool loaded = TryLoadOrReload(dll, out List<IAddon> instances, true, false);
                foreach (IAddon instance in instances)
                {
                    string name = instance.Name.Trim();
                    if (instance is WebAPIPlugin pluginInstance)
                    {
                        // 热更新无变化的文件时，不会再触发 Load 方法
                        if ((loaded || !plugins.ContainsKey(name)) && pluginInstance.Load(otherobjs))
                        {
                            pluginInstance.Controller = new(pluginInstance, delegates);
                            updated.Add(pluginInstance);
                        }
                        plugins[name] = pluginInstance;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// 从 modules 目录加载所有模组
        /// </summary>
        /// <param name="modules">模组字典</param>
        /// <param name="characters">角色字典</param>
        /// <param name="skills">技能字典</param>
        /// <param name="items">物品字典</param>
        /// <param name="delegates">委托字典</param>
        /// <param name="otherobjs">其他参数</param>
        /// <returns>被更新的模组列表</returns>
        internal static List<GameModule> LoadGameModules(Dictionary<string, GameModule> modules, Dictionary<string, CharacterModule> characters, Dictionary<string, SkillModule> skills, Dictionary<string, ItemModule> items, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            List<GameModule> updated = [];
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return updated;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in dlls)
            {
                bool loaded = TryLoadOrReload(dll, out List<IAddon> instances, false, true);
                foreach (IAddon instance in instances)
                {
                    string name = instance.Name.Trim();
                    if (instance is GameModule moduleInstance)
                    {
                        // 热更新无变化的文件时，不会再触发 Load 方法
                        if ((loaded || !modules.ContainsKey(name)) && instance.Load(otherobjs))
                        {
                            moduleInstance.Controller = new(moduleInstance, delegates);
                        }
                        modules[name] = moduleInstance;
                    }
                    else if (instance is CharacterModule charInstance)
                    {
                        if (loaded || !characters.ContainsKey(name))
                        {
                            instance.Load(otherobjs);
                        }
                        characters[name] = charInstance;
                    }
                    else if (instance is SkillModule skillInstance)
                    {
                        if (loaded || !skills.ContainsKey(name))
                        {
                            instance.Load(otherobjs);
                        }
                        skills[name] = skillInstance;
                    }
                    else if (instance is ItemModule itemInstance)
                    {
                        if (loaded || !items.ContainsKey(name))
                        {
                            instance.Load(otherobjs);
                        }
                        items[name] = itemInstance;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// 从 modules 目录加载所有适用于服务器的模组
        /// </summary>
        /// <param name="servers">服务器模组字典</param>
        /// <param name="characters">角色字典</param>
        /// <param name="skills">技能字典</param>
        /// <param name="items">物品字典</param>
        /// <param name="delegates">委托字典</param>
        /// <param name="otherobjs">其他参数</param>
        /// <returns>被更新的服务器模组列表</returns>
        internal static List<GameModuleServer> LoadGameModulesForServer(Dictionary<string, GameModuleServer> servers, Dictionary<string, CharacterModule> characters, Dictionary<string, SkillModule> skills, Dictionary<string, ItemModule> items, Dictionary<string, object> delegates, params object[] otherobjs)
        {
            List<GameModuleServer> updated = [];
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return updated;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in dlls)
            {
                bool loaded = TryLoadOrReload(dll, out List<IAddon> instances, false, true);
                foreach (IAddon instance in instances)
                {
                    string name = instance.Name.Trim();
                    if (instance is GameModuleServer serversInstance)
                    {
                        // 热更新无变化的文件时，不会再触发 Load 方法
                        if ((loaded || !servers.ContainsKey(name)) && instance.Load(otherobjs))
                        {
                            serversInstance.Controller = new(serversInstance, delegates);
                        }
                        servers[name] = serversInstance;
                    }
                    else if (instance is CharacterModule charInstance)
                    {
                        if (loaded || !characters.ContainsKey(name))
                        {
                            instance.Load(otherobjs);
                        }
                        characters[name] = charInstance;
                    }
                    else if (instance is SkillModule skillInstance)
                    {
                        if (loaded || !skills.ContainsKey(name))
                        {
                            instance.Load(otherobjs);
                        }
                        skills[name] = skillInstance;
                    }
                    else if (instance is ItemModule itemInstance)
                    {
                        if (loaded || !items.ContainsKey(name))
                        {
                            instance.Load(otherobjs);
                        }
                        items[name] = itemInstance;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// 从 maps 目录加载所有地图
        /// </summary>
        /// <param name="maps">地图字典</param>
        /// <param name="objs">其他参数</param>
        /// <returns>被更新的地图列表</returns>
        internal static List<GameMap> LoadGameMaps(Dictionary<string, GameMap> maps, params object[] objs)
        {
            List<GameMap> updated = [];
            if (!Directory.Exists(ReflectionSet.GameMapFolderPath)) return updated;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameMapFolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in dlls)
            {
                bool loaded = TryLoadOrReload(dll, out List<IAddon> instances, false, true);
                foreach (IAddon instance in instances)
                {
                    string name = instance.Name.Trim();
                    if (instance is GameMap mapInstance)
                    {
                        // 热更新无变化的文件时，不会再触发 Load 方法
                        if ((loaded || !maps.ContainsKey(name)) && mapInstance.Load(objs))
                        {
                            updated.Add(mapInstance);
                        }
                        maps[name] = mapInstance;
                    }
                }
            }

            return updated;
        }

        /// <summary>
        /// 在任务计划中定期执行
        /// </summary>
        internal static void CleanUnusedContexts()
        {
            lock (_contextsToClean)
            {
                for (int i = _contextsToClean.Count - 1; i >= 0; i--)
                {
                    if (!_contextsToClean[i].TryGetTarget(out AssemblyLoadContext? ctx) || ctx.IsCollectible == false)
                    {
                        _contextsToClean.RemoveAt(i);
                        continue;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    if (!ctx.IsCollectible)
                    {
                        _contextsToClean.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 热更新 DLL
        /// </summary>
        /// <param name="filePath">DLL 完整路径</param>
        /// <returns>是否成功热更新</returns>
        internal static bool HotReload(string filePath)
        {
            return TryLoadOrReload(filePath, out _, isPlugin: Directory.GetParent(filePath)?.FullName == ReflectionSet.PluginFolderPath, isModule: Directory.GetParent(filePath)?.FullName == ReflectionSet.GameModuleFolderPath);
        }

        /// <summary>
        /// 尝试获取当前最新的实例
        /// </summary>
        /// <typeparam name="T">预期类型</typeparam>
        /// <param name="addonName">插件/模组名称</param>
        /// <param name="instance">最新的实例</param>
        /// <returns>是否找到</returns>
        internal static bool TryGetLiveInstance<T>(string addonName, out T? instance) where T : class, IAddon
        {
            instance = null;

            // 所有的 Plugin 都继承自 IPlugin -> IAddon，除此之外都是 IAddon，所以先从 Plugin 里找
            if (typeof(T).IsSubclassOf(typeof(IPlugin)))
            {
                if (Plugins.FirstOrDefault(kv => kv.Key == addonName).Value is T plugin)
                {
                    instance = plugin;
                }
            }
            else if (Modules.FirstOrDefault(kv => kv.Key == addonName).Value is T module)
            {
                instance = module;
            }

            if (instance != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 每个加载项都拥有独立的可收集 AssemblyLoadContext
        /// </summary>
        private class AddonLoadContext(string addonName, string filePath) : AssemblyLoadContext($"{addonName}_{Guid.NewGuid():N}", isCollectible: true)
        {
            public string AddonName { get; } = addonName;
            public string FilePath { get; } = filePath;
            public DateTime LastWriteTimeUtc { get; } = File.GetLastWriteTimeUtc(filePath);
            public AssemblyDependencyResolver Resolver { get; } = new(filePath);

            /// <summary>
            /// 读取加载项的依赖项
            /// </summary>
            protected override Assembly? Load(AssemblyName assemblyName)
            {
                string? assemblyPath = Resolver.ResolveAssemblyToPath(assemblyName);

                if (assemblyPath != null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }

            /// <summary>
            /// 读取 native dll 依赖项
            /// </summary>
            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string? nativePath = Resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (nativePath != null)
                {
                    return LoadUnmanagedDllFromPath(nativePath);
                }
                return base.LoadUnmanagedDll(unmanagedDllName);
            }
        }

        /// <summary>
        /// 记录 DLL 信息
        /// </summary>
        private class DLLAddonEntry
        {
            public required AddonLoadContext Context { get; set; }
            public required Assembly Assembly { get; set; }
            public required DateTime LastWriteTimeUtc { get; set; }
            public List<AddonSubEntry> Addons { get; } = [];
        }

        /// <summary>
        /// 记录加载项信息
        /// </summary>
        private class AddonSubEntry
        {
            public required IAddon Instance { get; set; }
            public required WeakReference<IAddon> Weak { get; set; }
            public required string Name { get; set; }
        }
    }
}
