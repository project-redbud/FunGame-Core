using System.Reflection;
using Milimoe.FunGame.Core.Library.Common.Plugin;

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
        internal static Dictionary<string, BasePlugin> LoadPlugins(Dictionary<string, BasePlugin> plugins, params object[] objs)
        {
            if (!Directory.Exists("plugins")) return plugins;

            string[] dlls = Directory.GetFiles("plugins", "*.dll");

            foreach (string dll in dlls)
            {
                // 加载目录下所有的DLL
                Assembly assembly = Assembly.LoadFrom(dll);

                // 遍历DLL中继承了BasePlugin的类型
                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(BasePlugin))))
                {
                    BasePlugin? instance = (BasePlugin?)Activator.CreateInstance(type);
                    if (instance != null && instance.Load(objs))
                    {
                        plugins.TryAdd(instance.Name, instance);
                    }
                }
            }

            return plugins;
        }
    }
}