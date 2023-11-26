using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class PluginLoader
    {
        public Dictionary<string, Plugin> Plugins { get; } = [];

        private PluginLoader()
        {

        }

        public static PluginLoader LoadPlugins(params object[] objs)
        {
            PluginLoader loader = new();
            AddonManager.LoadPlugins(loader.Plugins, objs);
            return loader;
        }

        public Plugin this[string name]
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

        public void OnSucceedConnectEvent(object sender, ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedConnectEvent(sender, e);
            });
        }

        public void OnFailedConnectEvent(object sender, ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedConnectEvent(sender, e);
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

        public void OnSucceedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedDisconnectEvent(sender, e);
            });
        }

        public void OnFailedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedDisconnectEvent(sender, e);
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

        public void OnSucceedLoginEvent(object sender, LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedLoginEvent(sender, e);
            });
        }

        public void OnFailedLoginEvent(object sender, LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedLoginEvent(sender, e);
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

        public void OnSucceedLogoutEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedLogoutEvent(sender, e);
            });
        }

        public void OnFailedLogoutEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedLogoutEvent(sender, e);
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

        public void OnSucceedRegEvent(object sender, RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedRegEvent(sender, e);
            });
        }

        public void OnFailedRegEvent(object sender, RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedRegEvent(sender, e);
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

        public void OnSucceedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedIntoRoomEvent(sender, e);
            });
        }

        public void OnFailedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedIntoRoomEvent(sender, e);
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

        public void OnSucceedSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedSendTalkEvent(sender, e);
            });
        }

        public void OnFailedSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedSendTalkEvent(sender, e);
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

        public void OnSucceedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedCreateRoomEvent(sender, e);
            });
        }

        public void OnFailedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedCreateRoomEvent(sender, e);
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

        public void OnSucceedQuitRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedQuitRoomEvent(sender, e);
            });
        }

        public void OnFailedQuitRoomEvent(object sender, RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedQuitRoomEvent(sender, e);
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

        public void OnSucceedChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedChangeRoomSettingEvent(sender, e);
            });
        }

        public void OnFailedChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedChangeRoomSettingEvent(sender, e);
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

        public void OnSucceedStartMatchEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedStartMatchEvent(sender, e);
            });
        }

        public void OnFailedStartMatchEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedStartMatchEvent(sender, e);
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

        public void OnSucceedStartGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedStartGameEvent(sender, e);
            });
        }

        public void OnFailedStartGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedStartGameEvent(sender, e);
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

        public void OnSucceedChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedChangeProfileEvent(sender, e);
            });
        }

        public void OnFailedChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedChangeProfileEvent(sender, e);
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

        public void OnSucceedChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedChangeAccountSettingEvent(sender, e);
            });
        }

        public void OnFailedChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedChangeAccountSettingEvent(sender, e);
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

        public void OnSucceedOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedOpenInventoryEvent(sender, e);
            });
        }

        public void OnFailedOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedOpenInventoryEvent(sender, e);
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

        public void OnSucceedSignInEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedSignInEvent(sender, e);
            });
        }

        public void OnFailedSignInEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedSignInEvent(sender, e);
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

        public void OnSucceedOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedOpenStoreEvent(sender, e);
            });
        }

        public void OnFailedOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedOpenStoreEvent(sender, e);
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

        public void OnSucceedBuyItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedBuyItemEvent(sender, e);
            });
        }

        public void OnFailedBuyItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedBuyItemEvent(sender, e);
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

        public void OnSucceedShowRankingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedShowRankingEvent(sender, e);
            });
        }

        public void OnFailedShowRankingEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedShowRankingEvent(sender, e);
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

        public void OnSucceedUseItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedUseItemEvent(sender, e);
            });
        }

        public void OnFailedUseItemEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedUseItemEvent(sender, e);
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

        public void OnSucceedEndGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedEndGameEvent(sender, e);
            });
        }

        public void OnFailedEndGameEvent(object sender, GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedEndGameEvent(sender, e);
            });
        }
    }
}
