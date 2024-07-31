using System.Collections;
using System.Reflection;
using Milimoe.FunGame.Core.Entity;
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

                // 遍历DLL中继承了Plugin的类型
                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Plugin))))
                {
                    Plugin? instance = (Plugin?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(otherobjs) && instance.Name.Trim() != "")
                    {
                        instance.Controller = new(instance, delegates);
                        plugins.TryAdd(instance.Name, instance);
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
        internal static Dictionary<string, GameModule> LoadGameModules(Dictionary<string, GameModule> modules, List<Character> characters, List<Skill> skills, List<Item> items, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return modules;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameModule))))
                {
                    GameModule? instance = (GameModule?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(otherobjs) && instance.Name.Trim() != "")
                    {
                        instance.Controller = new(instance, delegates);
                        modules.TryAdd(instance.Name, instance);
                    }
                }

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Character))))
                {
                    Character? instance = (Character?)Activator.CreateInstance(type);
                    if (instance != null && instance.Name.Trim() != "" && !characters.Contains(instance))
                    {
                        characters.Add(instance);
                    }
                }

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Skill))))
                {
                    Skill? instance = (Skill?)Activator.CreateInstance(type);
                    if (instance != null && instance.Name.Trim() != "" && !skills.Contains(instance))
                    {
                        skills.Add(instance);
                    }
                }

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Item))))
                {
                    Item? instance = (Item?)Activator.CreateInstance(type);
                    if (instance != null && instance.Name.Trim() != "" && !items.Contains(instance))
                    {
                        items.Add(instance);
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
        internal static Dictionary<string, GameModuleServer> LoadGameModulesForServer(Dictionary<string, GameModuleServer> modules, List<Character> characters, List<Skill> skills, List<Item> items, Hashtable delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModuleFolderPath)) return modules;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModuleFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameModuleServer))))
                {
                    GameModuleServer? instance = (GameModuleServer?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(otherobjs) && instance.Name.Trim() != "")
                    {
                        instance.Controller = new(instance, delegates);
                        modules.TryAdd(instance.Name, instance);
                    }
                }

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Character))))
                {
                    Character? instance = (Character?)Activator.CreateInstance(type);
                    if (instance != null && instance.Name.Trim() != "" && !characters.Contains(instance))
                    {
                        characters.Add(instance);
                    }
                }

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Skill))))
                {
                    Skill? instance = (Skill?)Activator.CreateInstance(type);
                    if (instance != null && instance.Name.Trim() != "" && !skills.Contains(instance))
                    {
                        skills.Add(instance);
                    }
                }

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(Item))))
                {
                    Item? instance = (Item?)Activator.CreateInstance(type);
                    if (instance != null && instance.Name.Trim() != "" && !items.Contains(instance))
                    {
                        items.Add(instance);
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

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameMap))))
                {
                    GameMap? instance = (GameMap?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(objs) && instance.Name.Trim() != "")
                    {
                        maps.TryAdd(instance.Name, instance);
                    }
                }
            }

            return maps;
        }
    }
}