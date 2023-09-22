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

        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public string Version => throw new NotImplementedException();

        public string Author => throw new NotImplementedException();

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

        public void OnBeforeConnectEvent(ConnectEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterConnectEvent(ConnectEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedConnectEvent(ConnectEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedConnectEvent(ConnectEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeLoginEvent(LoginEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterLoginEvent(LoginEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedLoginEvent(LoginEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedLoginEvent(LoginEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterLogoutEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedLogoutEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeRegEvent(RegisterEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterRegEvent(RegisterEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedRegEvent(RegisterEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedRegEvent(RegisterEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeIntoRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterIntoRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedIntoRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedIntoRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeSendTalkEvent(SendTalkEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterSendTalkEvent(SendTalkEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedSendTalkEvent(SendTalkEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedSendTalkEvent(SendTalkEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeCreateRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterCreateRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedCreateRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedCreateRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeQuitRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterQuitRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedQuitRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedQuitRoomEvent(RoomEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeChangeRoomSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterChangeRoomSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeStartGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterStartGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedStartGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedStartGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeChangeProfileEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterChangeProfileEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedChangeProfileEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedChangeProfileEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeChangeAccountSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterChangeAccountSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeSignInEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterSignInEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedSignInEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedSignInEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeBuyItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterBuyItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedBuyItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedBuyItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeShowRankingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterShowRankingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedShowRankingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedShowRankingEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeUseItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterUseItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedUseItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedUseItemEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnBeforeEndGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnAfterEndGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnSucceedEndGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFailedEndGameEvent(GeneralEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
