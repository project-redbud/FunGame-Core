using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Plugin;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class PluginLoader
    {
        public Dictionary<string, BasePlugin> Plugins { get; } = new();

        private PluginLoader()
        {

        }

        public static PluginLoader LoadPlugins()
        {
            PluginLoader loader = new();
            PluginManager.LoadPlugins(loader.Plugins);
            return loader;
        }

        public void OnBeforeConnectEvent(ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeConnectEvent(e);
            });
        }

        public void OnAfterConnectEvent(ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterConnectEvent(e);
            });
        }

        public void OnSucceedConnectEvent(ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedConnectEvent(e);
            });
        }

        public void OnFailedConnectEvent(ConnectEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedConnectEvent(e);
            });
        }

        public void OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeDisconnectEvent(e);
            });
        }

        public void OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterDisconnectEvent(e);
            });
        }

        public void OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedDisconnectEvent(e);
            });
        }

        public void OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedDisconnectEvent(e);
            });
        }

        public void OnBeforeLoginEvent(LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeLoginEvent(e);
            });
        }

        public void OnAfterLoginEvent(LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterLoginEvent(e);
            });
        }

        public void OnSucceedLoginEvent(LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedLoginEvent(e);
            });
        }

        public void OnFailedLoginEvent(LoginEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedLoginEvent(e);
            });
        }

        public void OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeLogoutEvent(e);
            });
        }

        public void OnAfterLogoutEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterLogoutEvent(e);
            });
        }

        public void OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedLogoutEvent(e);
            });
        }

        public void OnFailedLogoutEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedLogoutEvent(e);
            });
        }

        public void OnBeforeRegEvent(RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeRegEvent(e);
            });
        }

        public void OnAfterRegEvent(RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterRegEvent(e);
            });
        }

        public void OnSucceedRegEvent(RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedRegEvent(e);
            });
        }

        public void OnFailedRegEvent(RegisterEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedRegEvent(e);
            });
        }

        public void OnBeforeIntoRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeIntoRoomEvent(e);
            });
        }

        public void OnAfterIntoRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterIntoRoomEvent(e);
            });
        }

        public void OnSucceedIntoRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedIntoRoomEvent(e);
            });
        }

        public void OnFailedIntoRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedIntoRoomEvent(e);
            });
        }

        public void OnBeforeSendTalkEvent(SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeSendTalkEvent(e);
            });
        }

        public void OnAfterSendTalkEvent(SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterSendTalkEvent(e);
            });
        }

        public void OnSucceedSendTalkEvent(SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedSendTalkEvent(e);
            });
        }

        public void OnFailedSendTalkEvent(SendTalkEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedSendTalkEvent(e);
            });
        }

        public void OnBeforeCreateRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeCreateRoomEvent(e);
            });
        }

        public void OnAfterCreateRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterCreateRoomEvent(e);
            });
        }

        public void OnSucceedCreateRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedCreateRoomEvent(e);
            });
        }

        public void OnFailedCreateRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedCreateRoomEvent(e);
            });
        }

        public void OnBeforeQuitRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeQuitRoomEvent(e);
            });
        }

        public void OnAfterQuitRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterQuitRoomEvent(e);
            });
        }

        public void OnSucceedQuitRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedQuitRoomEvent(e);
            });
        }

        public void OnFailedQuitRoomEvent(RoomEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedQuitRoomEvent(e);
            });
        }

        public void OnBeforeChangeRoomSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeChangeRoomSettingEvent(e);
            });
        }

        public void OnAfterChangeRoomSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterChangeRoomSettingEvent(e);
            });
        }

        public void OnSucceedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedChangeRoomSettingEvent(e);
            });
        }

        public void OnFailedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedChangeRoomSettingEvent(e);
            });
        }

        public void OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeStartMatchEvent(e);
            });
        }

        public void OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterStartMatchEvent(e);
            });
        }

        public void OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedStartMatchEvent(e);
            });
        }

        public void OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedStartMatchEvent(e);
            });
        }

        public void OnBeforeStartGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeStartGameEvent(e);
            });
        }

        public void OnAfterStartGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterStartGameEvent(e);
            });
        }

        public void OnSucceedStartGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedStartGameEvent(e);
            });
        }

        public void OnFailedStartGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedStartGameEvent(e);
            });
        }

        public void OnBeforeChangeProfileEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeChangeProfileEvent(e);
            });
        }

        public void OnAfterChangeProfileEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterChangeProfileEvent(e);
            });
        }

        public void OnSucceedChangeProfileEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedChangeProfileEvent(e);
            });
        }

        public void OnFailedChangeProfileEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedChangeProfileEvent(e);
            });
        }

        public void OnBeforeChangeAccountSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeChangeAccountSettingEvent(e);
            });
        }

        public void OnAfterChangeAccountSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterChangeAccountSettingEvent(e);
            });
        }

        public void OnSucceedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedChangeAccountSettingEvent(e);
            });
        }

        public void OnFailedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedChangeAccountSettingEvent(e);
            });
        }

        public void OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeOpenInventoryEvent(e);
            });
        }

        public void OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterOpenInventoryEvent(e);
            });
        }

        public void OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedOpenInventoryEvent(e);
            });
        }

        public void OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedOpenInventoryEvent(e);
            });
        }

        public void OnBeforeSignInEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeSignInEvent(e);
            });
        }

        public void OnAfterSignInEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterSignInEvent(e);
            });
        }

        public void OnSucceedSignInEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedSignInEvent(e);
            });
        }

        public void OnFailedSignInEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedSignInEvent(e);
            });
        }

        public void OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeOpenStoreEvent(e);
            });
        }

        public void OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterOpenStoreEvent(e);
            });
        }

        public void OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedOpenStoreEvent(e);
            });
        }

        public void OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedOpenStoreEvent(e);
            });
        }

        public void OnBeforeBuyItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeBuyItemEvent(e);
            });
        }

        public void OnAfterBuyItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterBuyItemEvent(e);
            });
        }

        public void OnSucceedBuyItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedBuyItemEvent(e);
            });
        }

        public void OnFailedBuyItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedBuyItemEvent(e);
            });
        }

        public void OnBeforeShowRankingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeShowRankingEvent(e);
            });
        }

        public void OnAfterShowRankingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterShowRankingEvent(e);
            });
        }

        public void OnSucceedShowRankingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedShowRankingEvent(e);
            });
        }

        public void OnFailedShowRankingEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedShowRankingEvent(e);
            });
        }

        public void OnBeforeUseItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeUseItemEvent(e);
            });
        }

        public void OnAfterUseItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterUseItemEvent(e);
            });
        }

        public void OnSucceedUseItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedUseItemEvent(e);
            });
        }

        public void OnFailedUseItemEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedUseItemEvent(e);
            });
        }

        public void OnBeforeEndGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnBeforeEndGameEvent(e);
            });
        }

        public void OnAfterEndGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnAfterEndGameEvent(e);
            });
        }

        public void OnSucceedEndGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnSucceedEndGameEvent(e);
            });
        }

        public void OnFailedEndGameEvent(GeneralEventArgs e)
        {
            Parallel.ForEach(Plugins.Values, plugin =>
            {
                plugin.OnFailedEndGameEvent(e);
            });
        }
    }
}
