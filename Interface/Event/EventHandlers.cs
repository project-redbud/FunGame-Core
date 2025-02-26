using Milimoe.FunGame.Core.Library.Common.Event;

namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 窗体继承这些接口便能实现事件，或为插件预留。
    /// </summary>
    public interface IEventHandler
    {
        public delegate void BeforeEventHandler(object sender, GeneralEventArgs e);
        public delegate void AfterEventHandler(object sender, GeneralEventArgs e);
    }

    public interface IConnectEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, ConnectEventArgs e);
        public new delegate void AfterEventHandler(object sender, ConnectEventArgs e);

        public event BeforeEventHandler? BeforeConnect;
        public event AfterEventHandler? AfterConnect;

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e);
        public void OnAfterConnectEvent(object sender, ConnectEventArgs e);
    }

    public interface IDisconnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeDisconnect;
        public event AfterEventHandler? AfterDisconnect;

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e);
        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e);
    }

    public interface ILoginEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, LoginEventArgs e);
        public new delegate void AfterEventHandler(object sender, LoginEventArgs e);

        public event BeforeEventHandler? BeforeLogin;
        public event AfterEventHandler? AfterLogin;

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e);
        public void OnAfterLoginEvent(object sender, LoginEventArgs e);
    }

    public interface ILogoutEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogout;
        public event AfterEventHandler? AfterLogout;

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e);
        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e);
    }

    public interface IRegEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RegisterEventArgs e);
        public new delegate void AfterEventHandler(object sender, RegisterEventArgs e);

        public event BeforeEventHandler? BeforeReg;
        public event AfterEventHandler? AfterReg;

        public void OnBeforeRegEvent(object sender, RegisterEventArgs e);
        public void OnAfterRegEvent(object sender, RegisterEventArgs e);
    }

    public interface IIntoRoomEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RoomEventArgs e);
        public new delegate void AfterEventHandler(object sender, RoomEventArgs e);

        public event BeforeEventHandler? BeforeIntoRoom;
        public event AfterEventHandler? AfterIntoRoom;

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e);
        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e);
    }

    public interface ISendTalkEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, SendTalkEventArgs e);
        public new delegate void AfterEventHandler(object sender, SendTalkEventArgs e);

        public event BeforeEventHandler? BeforeSendTalk;
        public event AfterEventHandler? AfterSendTalk;

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e);
        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e);
    }

    public interface ICreateRoomEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RoomEventArgs e);
        public new delegate void AfterEventHandler(object sender, RoomEventArgs e);

        public event BeforeEventHandler? BeforeCreateRoom;
        public event AfterEventHandler? AfterCreateRoom;

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e);
        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IQuitRoomEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RoomEventArgs e);
        public new delegate void AfterEventHandler(object sender, RoomEventArgs e);

        public event BeforeEventHandler? BeforeQuitRoom;
        public event AfterEventHandler? AfterQuitRoom;

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e);
        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IChangeRoomSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeRoomSetting;
        public event AfterEventHandler? AfterChangeRoomSetting;

        public void OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public void OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartMatch;
        public event AfterEventHandler? AfterStartMatch;

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e);
        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartGame;
        public event AfterEventHandler? AfterStartGame;

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e);
        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeProfile;
        public event AfterEventHandler? AfterChangeProfile;

        public void OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e);
        public void OnAfterChangeProfileEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeAccountSetting;
        public event AfterEventHandler? AfterChangeAccountSetting;

        public void OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public void OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenInventory;
        public event AfterEventHandler? AfterOpenInventory;

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e);
        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e);
    }

    public interface ISignInEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSignIn;
        public event AfterEventHandler? AfterSignIn;

        public void OnBeforeSignInEvent(object sender, GeneralEventArgs e);
        public void OnAfterSignInEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenStore;
        public event AfterEventHandler? AfterOpenStore;

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e);
        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeBuyItem;
        public event AfterEventHandler? AfterBuyItem;

        public void OnBeforeBuyItemEvent(object sender, GeneralEventArgs e);
        public void OnAfterBuyItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeShowRanking;
        public event AfterEventHandler? AfterShowRanking;

        public void OnBeforeShowRankingEvent(object sender, GeneralEventArgs e);
        public void OnAfterShowRankingEvent(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeUseItem;
        public event AfterEventHandler? AfterUseItem;

        public void OnBeforeUseItemEvent(object sender, GeneralEventArgs e);
        public void OnAfterUseItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeEndGame;
        public event AfterEventHandler? AfterEndGame;

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e);
        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e);
    }
}
