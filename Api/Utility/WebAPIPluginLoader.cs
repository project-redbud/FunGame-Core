using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class WebAPIPluginLoader
    {
        /// <summary>
        /// 已读取的插件列表
        /// <para>key 是 <see cref="WebAPIPlugin.Name"/></para>
        /// </summary>
        public Dictionary<string, WebAPIPlugin> Plugins { get; } = [];

        /// <summary>
        /// 已加载的插件DLL名称对应的路径
        /// </summary>
        public static Dictionary<string, string> PluginFilePaths => new(AddonManager.PluginFilePaths);

        private WebAPIPluginLoader()
        {

        }

        /// <summary>
        /// 构建一个插件读取器并读取插件
        /// </summary>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static WebAPIPluginLoader LoadPlugins(params object[] otherobjs)
        {
            WebAPIPluginLoader loader = new();
            AddonManager.LoadWebAPIPlugins(loader.Plugins, otherobjs);
            foreach (WebAPIPlugin plugin in loader.Plugins.Values.ToList())
            {
                // 如果插件加载后需要执行代码，请重写AfterLoad方法
                plugin.AfterLoad(loader);
            }
            return loader;
        }

        public WebAPIPlugin this[string name]
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
