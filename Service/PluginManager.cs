using System.Reflection;
using Milimoe.FunGame.Core.Library.Common.Plugin;

namespace Milimoe.FunGame.Core.Service
{
    internal class PluginManager
    {
        public static Dictionary<string, BasePlugin> LoadPlugin()
        {
            Dictionary<string, BasePlugin> plugins = new();
            string directory = "plugins";

            // 获取目录中所有的 DLL 文件路径
            string[] dlls = Directory.GetFiles(directory, "*.dll");

            foreach (string dll in dlls)
            {
                try
                {
                    // 加载 DLL
                    Assembly assembly = Assembly.LoadFrom(dll);

                    // 遍历 DLL 中的类型
                    foreach (Type type in assembly.GetTypes().AsEnumerable().Where(type => type.IsSubclassOf(typeof(BasePlugin))))
                    {
                        BasePlugin instance = Activator.CreateInstance<BasePlugin>();
                        plugins.Add(instance.Name, instance);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load DLL: {dll}");
                    Console.WriteLine($"Error: {e.Message}");
                }
            }

            return plugins;
        }
    }
}