using System.Reflection;
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
        /// <param name="objs"></param>
        /// <returns></returns>
        internal static Dictionary<string, Plugin> LoadPlugins(Dictionary<string, Plugin> plugins, params object[] objs)
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
                    if (instance != null && instance.Load(objs))
                    {
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
        /// <param name="objs"></param>
        /// <returns></returns>
        internal static Dictionary<string, GameMode> LoadGameModes(Dictionary<string, GameMode> gamemodes, params object[] objs)
        {
            if (!Directory.Exists(ReflectionSet.GameModeFolderPath)) return gamemodes;

            string[] dlls = Directory.GetFiles(ReflectionSet.GameModeFolderPath, "*.dll");

            foreach (string dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(GameMode))))
                {
                    GameMode? instance = (GameMode?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(objs))
                    {
                        gamemodes.TryAdd(instance.Name, instance);
                    }
                }
            }

            return gamemodes;
        }

        /// <summary>
        /// 从gamemaps目录加载所有模组
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
                    if (instance != null && instance.Load(objs))
                    {
                        gamemaps.TryAdd(instance.Name, instance);
                    }
                }
            }

            return gamemaps;
        }
    }
}