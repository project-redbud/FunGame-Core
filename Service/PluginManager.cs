using System.Reflection;
using Milimoe.FunGame.Core.Library.Common.Plugin;

namespace Milimoe.FunGame.Core.Service
{
    internal class PluginManager
    {
        /// <summary>
        /// 从plugins目录加载所有插件
        /// </summary>
        /// <param name="plugins"></param>
        /// <returns></returns>
        public static Dictionary<string, BasePlugin> LoadPlugins(Dictionary<string, BasePlugin> plugins)
        {
            string directory = "plugins";

            // 获取目录中所有的 DLL 文件路径
            string[] dlls = Directory.GetFiles(directory, "*.dll");

            foreach (string dll in dlls)
            {
                // 加载 DLL
                Assembly assembly = Assembly.LoadFrom(dll);

                // 遍历 DLL 中的类型
                foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(BasePlugin))))
                {
                    BasePlugin? instance = (BasePlugin?)Activator.CreateInstance(type);
                    if (instance != null)
                    {
                        plugins.TryAdd(instance.Name, instance);
                    }
                }
            }

            return plugins;
        }
    }
}