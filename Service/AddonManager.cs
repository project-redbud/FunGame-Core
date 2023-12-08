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
        internal static Dictionary<string, Plugin> LoadPlugins(Dictionary<string, Plugin> plugins, Delegate[] delegates, params object[] otherobjs)
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
        /// 从gamemodes目录加载所有模组
        /// </summary>
        /// <param name="gamemodes"></param>
        /// <param name="characters"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameMode> LoadGameModes(Dictionary<string, GameMode> gamemodes, List<Character> characters, List<Skill> skills, List<Item> items, Delegate[] delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModeFolderPath)) return gamemodes;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModeFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameMode))))
                {
                    GameMode? instance = (GameMode?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(otherobjs) && instance.Name.Trim() != "")
                    {
                        instance.Controller = new(instance, delegates);
                        gamemodes.TryAdd(instance.Name, instance);
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

            return gamemodes;
        }

        /// <summary>
        /// 从gamemodes目录加载所有适用于服务器的模组
        /// </summary>
        /// <param name="gamemodes"></param>
        /// <param name="characters"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        /// <param name="delegates"></param>
        /// <param name="otherobjs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameModeServer> LoadGameModesForServer(Dictionary<string, GameModeServer> gamemodes, List<Character> characters, List<Skill> skills, List<Item> items, Delegate[] delegates, params object[] otherobjs)
        {
            if (!Directory.Exists(ReflectionSet.GameModeFolderPath)) return gamemodes;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModeFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameModeServer))))
                {
                    GameModeServer? instance = (GameModeServer?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(otherobjs) && instance.Name.Trim() != "")
                    {
                        instance.Controller = new(instance, delegates);
                        gamemodes.TryAdd(instance.Name, instance);
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

            return gamemodes;
        }

        /// <summary>
        /// 从gamemaps目录加载所有地图
        /// </summary>
        /// <param name="gamemaps"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameMap> LoadGameMaps(Dictionary<string, GameMap> gamemaps, params object[] objs)
        {
            if (!Directory.Exists(ReflectionSet.GameMapFolderPath)) return gamemaps;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameMapFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameMap))))
                {
                    GameMap? instance = (GameMap?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(objs) && instance.Name.Trim() != "")
                    {
                        gamemaps.TryAdd(instance.Name, instance);
                    }
                }
            }

            return gamemaps;
        }
    }
}