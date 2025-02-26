using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
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
        /// <param name="delegates">用于构建 <see cref="Controller.BaseAddonController{T}"/></param>
        /// <param name="otherobjs">其他需要传入给插件初始化的对象</param>
        /// <returns></returns>
        public static WebAPIPluginLoader LoadPlugins(Dictionary<string, object> delegates, params object[] otherobjs)
        {
            WebAPIPluginLoader loader = new();
            AddonManager.LoadWebAPIPlugins(loader.Plugins, delegates, otherobjs);
            foreach (WebAPIPlugin plugin in loader.Plugins.Values.ToList())
            {
                // 如果插件加载后需要执行代码，请重写AfterLoad方法
                plugin.AfterLoad(loader, otherobjs);
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

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeConnectEvent(sender, e);
            });
        }

        public void OnAfterConnectEvent(object sender, ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterConnectEvent(sender, e);
            });
        }

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeDisconnectEvent(sender, e);
            });
        }

        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterDisconnectEvent(sender, e);
            });
        }

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeLoginEvent(sender, e);
            });
        }

        public void OnAfterLoginEvent(object sender, LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterLoginEvent(sender, e);
            });
        }

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeLogoutEvent(sender, e);
            });
        }

        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterLogoutEvent(sender, e);
            });
        }

        public void OnBeforeRegEvent(object sender, RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeRegEvent(sender, e);
            });
        }

        public void OnAfterRegEvent(object sender, RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterRegEvent(sender, e);
            });
        }

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeIntoRoomEvent(sender, e);
            });
        }

        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterIntoRoomEvent(sender, e);
            });
        }

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeSendTalkEvent(sender, e);
            });
        }

        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterSendTalkEvent(sender, e);
            });
        }

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeCreateRoomEvent(sender, e);
            });
        }

        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterCreateRoomEvent(sender, e);
            });
        }

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeQuitRoomEvent(sender, e);
            });
        }

        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterQuitRoomEvent(sender, e);
            });
        }

        public void OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeChangeRoomSettingEvent(sender, e);
            });
        }

        public void OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterChangeRoomSettingEvent(sender, e);
            });
        }

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeStartMatchEvent(sender, e);
            });
        }

        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterStartMatchEvent(sender, e);
            });
        }

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeStartGameEvent(sender, e);
            });
        }

        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterStartGameEvent(sender, e);
            });
        }

        public void OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeChangeProfileEvent(sender, e);
            });
        }

        public void OnAfterChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterChangeProfileEvent(sender, e);
            });
        }

        public void OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeChangeAccountSettingEvent(sender, e);
            });
        }

        public void OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterChangeAccountSettingEvent(sender, e);
            });
        }

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeOpenInventoryEvent(sender, e);
            });
        }

        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterOpenInventoryEvent(sender, e);
            });
        }

        public void OnBeforeSignInEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeSignInEvent(sender, e);
            });
        }

        public void OnAfterSignInEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterSignInEvent(sender, e);
            });
        }

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeOpenStoreEvent(sender, e);
            });
        }

        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterOpenStoreEvent(sender, e);
            });
        }

        public void OnBeforeBuyItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeBuyItemEvent(sender, e);
            });
        }

        public void OnAfterBuyItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterBuyItemEvent(sender, e);
            });
        }

        public void OnBeforeShowRankingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeShowRankingEvent(sender, e);
            });
        }

        public void OnAfterShowRankingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterShowRankingEvent(sender, e);
            });
        }

        public void OnBeforeUseItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeUseItemEvent(sender, e);
            });
        }

        public void OnAfterUseItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterUseItemEvent(sender, e);
            });
        }

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeEndGameEvent(sender, e);
            });
        }

        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterEndGameEvent(sender, e);
            });
        }
    }
}
