using Milimoe.FunGame.Core.Library.Common.Plugin;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class PluginLoader
    {
        public Dictionary<string, BasePlugin> Plugins { get; } = new();

        private PluginLoader()
        {

        }
        
        private PluginLoader(Dictionary<string, BasePlugin> plugins)
        {
            Plugins = plugins;
        }

        public static PluginLoader LoadPlugins()
        {
            PluginLoader loader = new();
            PluginManager.LoadPlugins(loader.Plugins);
            return loader;
        }

        public static PluginLoader LoadPlugins(Dictionary<string, BasePlugin> plugins)
        {
            PluginLoader loader = new(plugins);
            PluginManager.LoadPlugins(loader.Plugins);
            return loader;
        }
    }
}
