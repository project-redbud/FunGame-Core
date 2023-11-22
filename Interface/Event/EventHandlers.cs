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
        public delegate void SucceedEventHandler(object sender, GeneralEventArgs e);
        public delegate void FailedEventHandler(object sender, GeneralEventArgs e);
    }

    public interface IConnectEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, ConnectEventArgs e);
        public new delegate void AfterEventHandler(object sender, ConnectEventArgs e);
        public new delegate void SucceedEventHandler(object sender, ConnectEventArgs e);
        public new delegate void FailedEventHandler(object sender, ConnectEventArgs e);

        public event BeforeEventHandler? BeforeConnect;
        public event AfterEventHandler? AfterConnect;
        public event SucceedEventHandler? SucceedConnect;
        public event FailedEventHandler? FailedConnect;

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e);
        public void OnAfterConnectEvent(object sender, ConnectEventArgs e);
        public void OnSucceedConnectEvent(object sender, ConnectEventArgs e);
        public void OnFailedConnectEvent(object sender, ConnectEventArgs e);
    }

    public interface IDisconnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeDisconnect;
        public event AfterEventHandler? AfterDisconnect;
        public event SucceedEventHandler? SucceedDisconnect;
        public event FailedEventHandler? FailedDisconnect;

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e);
        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e);
        public void OnSucceedDisconnectEvent(object sender, GeneralEventArgs e);
        public void OnFailedDisconnectEvent(object sender, GeneralEventArgs e);
    }

    public interface ILoginEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, LoginEventArgs e);
        public new delegate void AfterEventHandler(object sender, LoginEventArgs e);
        public new delegate void SucceedEventHandler(object sender, LoginEventArgs e);
        public new delegate void FailedEventHandler(object sender, LoginEventArgs e);

        public event BeforeEventHandler? BeforeLogin;
        public event AfterEventHandler? AfterLogin;
        public event SucceedEventHandler? SucceedLogin;
        public event FailedEventHandler? FailedLogin;

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e);
        public void OnAfterLoginEvent(object sender, LoginEventArgs e);
        public void OnSucceedLoginEvent(object sender, LoginEventArgs e);
        public void OnFailedLoginEvent(object sender, LoginEventArgs e);
    }

    public interface ILogoutEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogout;
        public event AfterEventHandler? AfterLogout;
        public event SucceedEventHandler? SucceedLogout;
        public event FailedEventHandler? FailedLogout;

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e);
        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e);
        public void OnSucceedLogoutEvent(object sender, GeneralEventArgs e);
        public void OnFailedLogoutEvent(object sender, GeneralEventArgs e);
    }

    public interface IRegEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RegisterEventArgs e);
        public new delegate void AfterEventHandler(object sender, RegisterEventArgs e);
        public new delegate void SucceedEventHandler(object sender, RegisterEventArgs e);
        public new delegate void FailedEventHandler(object sender, RegisterEventArgs e);

        public event BeforeEventHandler? BeforeReg;
        public event AfterEventHandler? AfterReg;
        public event SucceedEventHandler? SucceedReg;
        public event FailedEventHandler? FailedReg;

        public void OnBeforeRegEvent(object sender, RegisterEventArgs e);
        public void OnAfterRegEvent(object sender, RegisterEventArgs e);
        public void OnSucceedRegEvent(object sender, RegisterEventArgs e);
        public void OnFailedRegEvent(object sender, RegisterEventArgs e);
    }

    public interface IIntoRoomEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RoomEventArgs e);
        public new delegate void AfterEventHandler(object sender, RoomEventArgs e);
        public new delegate void SucceedEventHandler(object sender, RoomEventArgs e);
        public new delegate void FailedEventHandler(object sender, RoomEventArgs e);

        public event BeforeEventHandler? BeforeIntoRoom;
        public event AfterEventHandler? AfterIntoRoom;
        public event SucceedEventHandler? SucceedIntoRoom;
        public event FailedEventHandler? FailedIntoRoom;

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e);
        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e);
        public void OnSucceedIntoRoomEvent(object sender, RoomEventArgs e);
        public void OnFailedIntoRoomEvent(object sender, RoomEventArgs e);
    }

    public interface ISendTalkEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, SendTalkEventArgs e);
        public new delegate void AfterEventHandler(object sender, SendTalkEventArgs e);
        public new delegate void SucceedEventHandler(object sender, SendTalkEventArgs e);
        public new delegate void FailedEventHandler(object sender, SendTalkEventArgs e);

        public event BeforeEventHandler? BeforeSendTalk;
        public event AfterEventHandler? AfterSendTalk;
        public event SucceedEventHandler? SucceedSendTalk;
        public event FailedEventHandler? FailedSendTalk;

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e);
        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e);
        public void OnSucceedSendTalkEvent(object sender, SendTalkEventArgs e);
        public void OnFailedSendTalkEvent(object sender, SendTalkEventArgs e);
    }

    public interface ICreateRoomEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RoomEventArgs e);
        public new delegate void AfterEventHandler(object sender, RoomEventArgs e);
        public new delegate void SucceedEventHandler(object sender, RoomEventArgs e);
        public new delegate void FailedEventHandler(object sender, RoomEventArgs e);

        public event BeforeEventHandler? BeforeCreateRoom;
        public event AfterEventHandler? AfterCreateRoom;
        public event SucceedEventHandler? SucceedCreateRoom;
        public event FailedEventHandler? FailedCreateRoom;

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e);
        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e);
        public void OnSucceedCreateRoomEvent(object sender, RoomEventArgs e);
        public void OnFailedCreateRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IQuitRoomEventHandler : IEventHandler
    {
        public new delegate void BeforeEventHandler(object sender, RoomEventArgs e);
        public new delegate void AfterEventHandler(object sender, RoomEventArgs e);
        public new delegate void SucceedEventHandler(object sender, RoomEventArgs e);
        public new delegate void FailedEventHandler(object sender, RoomEventArgs e);

        public event BeforeEventHandler? BeforeQuitRoom;
        public event AfterEventHandler? AfterQuitRoom;
        public event SucceedEventHandler? SucceedQuitRoom;
        public event FailedEventHandler? FailedQuitRoom;

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e);
        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e);
        public void OnSucceedQuitRoomEvent(object sender, RoomEventArgs e);
        public void OnFailedQuitRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IChangeRoomSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeRoomSetting;
        public event AfterEventHandler? AfterChangeRoomSetting;
        public event SucceedEventHandler? SucceedChangeRoomSetting;
        public event FailedEventHandler? FailedChangeRoomSetting;

        public void OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public void OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public void OnSucceedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public void OnFailedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartMatch;
        public event AfterEventHandler? AfterStartMatch;
        public event SucceedEventHandler? SucceedStartMatch;
        public event FailedEventHandler? FailedStartMatch;

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e);
        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e);
        public void OnSucceedStartMatchEvent(object sender, GeneralEventArgs e);
        public void OnFailedStartMatchEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartGame;
        public event AfterEventHandler? AfterStartGame;
        public event SucceedEventHandler? SucceedStartGame;
        public event FailedEventHandler? FailedStartGame;

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e);
        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e);
        public void OnSucceedStartGameEvent(object sender, GeneralEventArgs e);
        public void OnFailedStartGameEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeProfile;
        public event AfterEventHandler? AfterChangeProfile;
        public event SucceedEventHandler? SucceedChangeProfile;
        public event FailedEventHandler? FailedChangeProfile;

        public void OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e);
        public void OnAfterChangeProfileEvent(object sender, GeneralEventArgs e);
        public void OnSucceedChangeProfileEvent(object sender, GeneralEventArgs e);
        public void OnFailedChangeProfileEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeAccountSetting;
        public event AfterEventHandler? AfterChangeAccountSetting;
        public event SucceedEventHandler? SucceedChangeAccountSetting;
        public event FailedEventHandler? FailedChangeAccountSetting;

        public void OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public void OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public void OnSucceedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public void OnFailedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenInventory;
        public event AfterEventHandler? AfterOpenInventory;
        public event SucceedEventHandler? SucceedOpenInventory;
        public event FailedEventHandler? FailedOpenInventory;

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e);
        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e);
        public void OnSucceedOpenInventoryEvent(object sender, GeneralEventArgs e);
        public void OnFailedOpenInventoryEvent(object sender, GeneralEventArgs e);
    }

    public interface ISignInEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSignIn;
        public event AfterEventHandler? AfterSignIn;
        public event SucceedEventHandler? SucceedSignIn;
        public event FailedEventHandler? FailedSignIn;

        public void OnBeforeSignInEvent(object sender, GeneralEventArgs e);
        public void OnAfterSignInEvent(object sender, GeneralEventArgs e);
        public void OnSucceedSignInEvent(object sender, GeneralEventArgs e);
        public void OnFailedSignInEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenStore;
        public event AfterEventHandler? AfterOpenStore;
        public event SucceedEventHandler? SucceedOpenStore;
        public event FailedEventHandler? FailedOpenStore;

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e);
        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e);
        public void OnSucceedOpenStoreEvent(object sender, GeneralEventArgs e);
        public void OnFailedOpenStoreEvent(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeBuyItem;
        public event AfterEventHandler? AfterBuyItem;
        public event SucceedEventHandler? SucceedBuyItem;
        public event FailedEventHandler? FailedBuyItem;

        public void OnBeforeBuyItemEvent(object sender, GeneralEventArgs e);
        public void OnAfterBuyItemEvent(object sender, GeneralEventArgs e);
        public void OnSucceedBuyItemEvent(object sender, GeneralEventArgs e);
        public void OnFailedBuyItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeShowRanking;
        public event AfterEventHandler? AfterShowRanking;
        public event SucceedEventHandler? SucceedShowRanking;
        public event FailedEventHandler? FailedShowRanking;

        public void OnBeforeShowRankingEvent(object sender, GeneralEventArgs e);
        public void OnAfterShowRankingEvent(object sender, GeneralEventArgs e);
        public void OnSucceedShowRankingEvent(object sender, GeneralEventArgs e);
        public void OnFailedShowRankingEvent(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeUseItem;
        public event AfterEventHandler? AfterUseItem;
        public event SucceedEventHandler? SucceedUseItem;
        public event FailedEventHandler? FailedUseItem;

        public void OnBeforeUseItemEvent(object sender, GeneralEventArgs e);
        public void OnAfterUseItemEvent(object sender, GeneralEventArgs e);
        public void OnSucceedUseItemEvent(object sender, GeneralEventArgs e);
        public void OnFailedUseItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeEndGame;
        public event AfterEventHandler? AfterEndGame;
        public event SucceedEventHandler? SucceedEndGame;
        public event FailedEventHandler? FailedEndGame;

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e);
        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e);
        public void OnSucceedEndGameEvent(object sender, GeneralEventArgs e);
        public void OnFailedEndGameEvent(object sender, GeneralEventArgs e);
    }
}
