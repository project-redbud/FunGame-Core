using System.Collections;
using System.Reflection;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Service
{
    internal class AddonManager
    {
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
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    AddAddonInstances(type, plugins, (instance, name) =>
                    {
                        if (instance.Load(otherobjs))
                        {
                            instance.Controller = new(instance, delegates);
                            return true;
                        }
                        return false;
                    });
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
        internal static Dictionary<string, GameModule> LoadGameModules(Dictionary<string, GameModule> modules, Dictionary<string, List<Character>> characters, Dictionary<string, List<Skill>> skills, Dictionary<string, List<Item>> items, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return modules;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (typeof(GameModule).IsAssignableFrom(type))
                    {
                        AddAddonInstances(type, modules, (instance, name) =>
                        {
                            if (instance.Load(otherobjs))
                            {
                                instance.Controller = new(instance, delegates);
                                return true;
                            }
                            return false;
                        });
                    }
                    else if (typeof(Character).IsAssignableFrom(type))
                    {
                        AddEntityInstances(dll, type, characters);
                    }
                    else if (typeof(Skill).IsAssignableFrom(type))
                    {
                        AddEntityInstances(dll, type, skills);
                    }
                    else if (typeof(Item).IsAssignableFrom(type))
                    {
                        AddEntityInstances(dll, type, items);
                    }
                }
            }

            return modules;
        }

        /// <summary>
        /// 从modules目录加载所有适用于服务器的模组
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="characters"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameModuleServer> LoadGameModulesForServer(Dictionary<string, GameModuleServer> modules, Dictionary<string, List<Character>> characters, Dictionary<string, List<Skill>> skills, Dictionary<string, List<Item>> items, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return modules;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (typeof(GameModule).IsAssignableFrom(type))
                    {
                        AddAddonInstances(type, modules, (instance, name) =>
                        {
                            if (instance.Load(otherobjs))
                            {
                                instance.Controller = new(instance, delegates);
                                return true;
                            }
                            return false;
                        });
                    }
                    else if (typeof(Character).IsAssignableFrom(type))
                    {
                        AddEntityInstances(dll, type, characters);
                    }
                    else if (typeof(Skill).IsAssignableFrom(type))
                    {
                        AddEntityInstances(dll, type, skills);
                    }
                    else if (typeof(Item).IsAssignableFrom(type))
                    {
                        AddEntityInstances(dll, type, items);
                    }
                }
            }

            return modules;
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
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    AddAddonInstances(type, maps, (instance, name) =>
                    {
                        if (instance.Load(objs))
                        {
                            return true;
                        }
                        return false;
                    });
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
        /// <param name="action">如果加载后要触发动作，可以传入一个函数</param>
        private static void AddAddonInstances<T>(Type type, Dictionary<string, T> dictionary, Func<T, string, bool>? action = null) where T : IAddon
        {
            T? instance = (T?)Activator.CreateInstance(type);
            if (instance != null)
            {
                string name = instance.Name;
                if (!string.IsNullOrWhiteSpace(name) && (action == null || action(instance, name)))
                {
                    dictionary.TryAdd(name.Trim(), instance);
                }
            }
        }

        /// <summary>
        /// 添加构造好的实体类实例到字典中
        /// </summary>
        /// <typeparam name="T">加载的类型</typeparam>
        /// <param name="dll">程序集的名称</param>
        /// <param name="type">循环程序集的类型</param>
        /// <param name="dictionary">实例的字典</param>
        private static void AddEntityInstances<T>(string dll, Type type, Dictionary<string, List<T>> dictionary) where T : IBaseEntity
        {
            T? instance = (T?)Activator.CreateInstance(type);
            if (instance != null)
            {
                string dllname = dll.Trim();
                if (!string.IsNullOrWhiteSpace(dllname))
                {
                    if (dictionary.TryGetValue(dllname, out List<T>? list) && list != null)
                    {
                        list.Add(instance);
                    }
                    else dictionary.TryAdd(dllname, []);
                }
            }
        }
    }
}