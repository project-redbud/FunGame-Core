using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class ServerPluginLoader
    {
        /// <summary>
        /// 已读取的插件列表
        /// <para>key 是 <see cref="ServerPlugin.Name"/></para>
        /// </summary>
        public Dictionary<string, ServerPlugin> Plugins { get; } = [];

        /// <summary>
        /// 已加载的插件DLL名称对应的路径
        /// </summary>
        public static Dictionary<string, string> PluginFilePaths => new(AddonManager.PluginFilePaths);

        private ServerPluginLoader()
        {

        }

        /// <summary>
        /// 构建一个插件读取器并读取插件
        /// </summary>
        /// <param name="delegates">用于构建 <see cref="Controller.BaseAddonController{T}"/></param>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static ServerPluginLoader LoadPlugins(Dictionary<string, object> delegates, params object[] otherobjs)
        {
            ServerPluginLoader loader = new();
            AddonManager.LoadServerPlugins(loader.Plugins, delegates, otherobjs);
            foreach (ServerPlugin plugin in loader.Plugins.Values.ToList())
            {
                // 如果插件加载后需要执行代码，请重写AfterLoad方法
                plugin.AfterLoad(loader);
            }
            return loader;
        }

        public ServerPlugin this[string name]
        {
            get
            {
                return Plugins[name];
            }
            set
            {
                Plugins.TryAdd(name, value);
            }
        }
    }
}
