using System.Collections;
using System.Reflection;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Service
{
    internal class AddonManager
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
        /// 从plugins目录加载所有插件
        /// </summary>
        /// <param name="plugins"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        /// <returns></returns>
        internal static Dictionary<string, Plugin> LoadPlugins(Dictionary<string, Plugin> plugins, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.PluginFolderPath)) return plugins;

            string[] dlls = Directory.GetFiles(ReflectionSet.PluginFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                // 加载目录下所有的DLL
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Plugin))))
                {
                    if (AddAddonInstances(type, plugins, (instance) =>
                    {
                        if (instance.Load(otherobjs))
                        {
                            instance.Controller = new(instance, delegates);
                            return true;
                        }
                        return false;
                    }))
                    {
                        AddDictionary(PluginFilePaths, assembly, dll);
                    }
                }
            }

            return plugins;
        }

        /// <summary>
        /// 从modules目录加载所有模组
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="characters"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameModule> LoadGameModules(Dictionary<string, GameModule> modules, Dictionary<string, CharacterModule> characters, Dictionary<string, SkillModule> skills, Dictionary<string, ItemModule> items, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return modules;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => typeof(IAddon).IsAssignableFrom(type)))
                {
                    bool isAdded = false;

                    if (type.IsSubclassOf(typeof(GameModule)))
                    {
                        isAdded = AddAddonInstances(type, modules, (instance) =>
                        {
                            if (instance.Load(otherobjs))
                            {
                                instance.Controller = new(instance, delegates);
                                return true;
                            }
                            return false;
                        });
                    }
                    else if (type.IsSubclassOf(typeof(CharacterModule)))
                    {
                        isAdded = AddAddonInstances(type, characters, (instance) => instance.Load(otherobjs));
                    }
                    else if (type.IsSubclassOf(typeof(SkillModule)))
                    {
                        isAdded = AddAddonInstances(type, skills, (instance) => instance.Load(otherobjs));
                    }
                    else if (type.IsSubclassOf(typeof(ItemModule)))
                    {
                        isAdded = AddAddonInstances(type, items, (instance) => instance.Load(otherobjs));
                    }

                    if (isAdded)
                    {
                        AddDictionary(ModuleFilePaths, assembly, dll);
                    }
                }
            }

            return modules;
        }

        /// <summary>
        /// 从modules目录加载所有适用于服务器的模组
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="servers"></param>
        /// <param name="characters"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameModuleServer> LoadGameModulesForServer(Dictionary<string, GameModule> modules, Dictionary<string, GameModuleServer> servers, Dictionary<string, CharacterModule> characters, Dictionary<string, SkillModule> skills, Dictionary<string, ItemModule> items, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return servers;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => typeof(IAddon).IsAssignableFrom(type)))
                {
                    bool isAdded = false;

                    if (type.IsSubclassOf(typeof(GameModule)))
                    {
                        isAdded = AddAddonInstances(type, modules, (instance) =>
                        {
                            if (instance.Load(otherobjs))
                            {
                                instance.Controller = new(instance, delegates);
                                return true;
                            }
                            return false;
                        });
                    }
                    else if (type.IsSubclassOf(typeof(GameModuleServer)))
                    {
                        isAdded = AddAddonInstances(type, servers, (instance) =>
                        {
                            if (instance.Load(otherobjs))
                            {
                                instance.Controller = new(instance, delegates);
                                return true;
                            }
                            return false;
                        });
                    }
                    else if (type.IsSubclassOf(typeof(CharacterModule)))
                    {
                        isAdded = AddAddonInstances(type, characters, (instance) => instance.Load(otherobjs));
                    }
                    else if (type.IsSubclassOf(typeof(SkillModule)))
                    {
                        isAdded = AddAddonInstances(type, skills, (instance) => instance.Load(otherobjs));
                    }
                    else if (type.IsSubclassOf(typeof(ItemModule)))
                    {
                        isAdded = AddAddonInstances(type, items, (instance) => instance.Load(otherobjs));
                    }

                    if (isAdded)
                    {
                        AddDictionary(ModuleFilePaths, assembly, dll);
                    }
                }
            }

            return servers;
        }

        /// <summary>
        /// 从maps目录加载所有地图
        /// </summary>
        /// <param name="maps"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameMap> LoadGameMaps(Dictionary<string, GameMap> maps, params object[] objs)
        {
            if (!Directory.Exists(ReflectionSet.GameMapFolderPath)) return maps;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameMapFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameMap))))
                {
                    if (AddAddonInstances(type, maps, (instance) => instance.Load(objs)))
                    {
                        AddDictionary(ModuleFilePaths, assembly, dll);
                    }
                }
            }

            return maps;
        }

        /// <summary>
        /// 添加构造好的模组类实例到字典中
        /// </summary>
        /// <typeparam name="T">加载的类型</typeparam>
        /// <param name="type">循环程序集的类型</param>
        /// <param name="dictionary">实例的字典</param>
        /// <param name="isadd">加载时触发的检查方法，返回false不添加</param>
        private static bool AddAddonInstances<T>(Type type, Dictionary<string, T> dictionary, Func<T, bool>? isadd = null) where T : IAddon
        {
            bool isAdded = false;
            T? instance = (T?)Activator.CreateInstance(type);
            if (instance != null)
            {
                string name = instance.Name;
                if (!string.IsNullOrWhiteSpace(name) && (isadd == null || isadd(instance)))
                {
                    dictionary.TryAdd(name.Trim(), instance);
                    isAdded = true;
                }
            }
            return isAdded;
        }

        private static void AddDictionary(Dictionary<string, string> dict, Assembly assembly, string dllpath)
        {
            string filename = assembly.GetName().Name?.Trim() ?? "";
            if (filename != "") dict.TryAdd(filename, dllpath);
        }
    }
}
