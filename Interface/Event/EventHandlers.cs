using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

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

        public void OnBeforeConnectEvent(ConnectEventArgs e);
        public void OnAfterConnectEvent(ConnectEventArgs e);
        public void OnSucceedConnectEvent(ConnectEventArgs e);
        public void OnFailedConnectEvent(ConnectEventArgs e);
    }

    public interface IDisconnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeDisconnect;
        public event AfterEventHandler? AfterDisconnect;
        public event SucceedEventHandler? SucceedDisconnect;
        public event FailedEventHandler? FailedDisconnect;

        public void OnBeforeDisconnectEvent(GeneralEventArgs e);
        public void OnAfterDisconnectEvent(GeneralEventArgs e);
        public void OnSucceedDisconnectEvent(GeneralEventArgs e);
        public void OnFailedDisconnectEvent(GeneralEventArgs e);
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

        public void OnBeforeLoginEvent(LoginEventArgs e);
        public void OnAfterLoginEvent(LoginEventArgs e);
        public void OnSucceedLoginEvent(LoginEventArgs e);
        public void OnFailedLoginEvent(LoginEventArgs e);
    }

    public interface ILogoutEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogout;
        public event AfterEventHandler? AfterLogout;
        public event SucceedEventHandler? SucceedLogout;
        public event FailedEventHandler? FailedLogout;

        public void OnBeforeLogoutEvent(GeneralEventArgs e);
        public void OnAfterLogoutEvent(GeneralEventArgs e);
        public void OnSucceedLogoutEvent(GeneralEventArgs e);
        public void OnFailedLogoutEvent(GeneralEventArgs e);
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

        public void OnBeforeRegEvent(RegisterEventArgs e);
        public void OnAfterRegEvent(RegisterEventArgs e);
        public void OnSucceedRegEvent(RegisterEventArgs e);
        public void OnFailedRegEvent(RegisterEventArgs e);
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

        public void OnBeforeIntoRoomEvent(RoomEventArgs e);
        public void OnAfterIntoRoomEvent(RoomEventArgs e);
        public void OnSucceedIntoRoomEvent(RoomEventArgs e);
        public void OnFailedIntoRoomEvent(RoomEventArgs e);
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

        public void OnBeforeSendTalkEvent(SendTalkEventArgs e);
        public void OnAfterSendTalkEvent(SendTalkEventArgs e);
        public void OnSucceedSendTalkEvent(SendTalkEventArgs e);
        public void OnFailedSendTalkEvent(SendTalkEventArgs e);
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

        public void OnBeforeCreateRoomEvent(RoomEventArgs e);
        public void OnAfterCreateRoomEvent(RoomEventArgs e);
        public void OnSucceedCreateRoomEvent(RoomEventArgs e);
        public void OnFailedCreateRoomEvent(RoomEventArgs e);
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

        public void OnBeforeQuitRoomEvent(RoomEventArgs e);
        public void OnAfterQuitRoomEvent(RoomEventArgs e);
        public void OnSucceedQuitRoomEvent(RoomEventArgs e);
        public void OnFailedQuitRoomEvent(RoomEventArgs e);
    }

    public interface IChangeRoomSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeRoomSetting;
        public event AfterEventHandler? AfterChangeRoomSetting;
        public event SucceedEventHandler? SucceedChangeRoomSetting;
        public event FailedEventHandler? FailedChangeRoomSetting;

        public void OnBeforeChangeRoomSettingEvent(GeneralEventArgs e);
        public void OnAfterChangeRoomSettingEvent(GeneralEventArgs e);
        public void OnSucceedChangeRoomSettingEvent(GeneralEventArgs e);
        public void OnFailedChangeRoomSettingEvent(GeneralEventArgs e);
    }

    public interface IStartMatchEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartMatch;
        public event AfterEventHandler? AfterStartMatch;
        public event SucceedEventHandler? SucceedStartMatch;
        public event FailedEventHandler? FailedStartMatch;

        public void OnBeforeStartMatchEvent(GeneralEventArgs e);
        public void OnAfterStartMatchEvent(GeneralEventArgs e);
        public void OnSucceedStartMatchEvent(GeneralEventArgs e);
        public void OnFailedStartMatchEvent(GeneralEventArgs e);
    }

    public interface IStartGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartGame;
        public event AfterEventHandler? AfterStartGame;
        public event SucceedEventHandler? SucceedStartGame;
        public event FailedEventHandler? FailedStartGame;

        public void OnBeforeStartGameEvent(GeneralEventArgs e);
        public void OnAfterStartGameEvent(GeneralEventArgs e);
        public void OnSucceedStartGameEvent(GeneralEventArgs e);
        public void OnFailedStartGameEvent(GeneralEventArgs e);
    }

    public interface IChangeProfileEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeProfile;
        public event AfterEventHandler? AfterChangeProfile;
        public event SucceedEventHandler? SucceedChangeProfile;
        public event FailedEventHandler? FailedChangeProfile;

        public void OnBeforeChangeProfileEvent(GeneralEventArgs e);
        public void OnAfterChangeProfileEvent(GeneralEventArgs e);
        public void OnSucceedChangeProfileEvent(GeneralEventArgs e);
        public void OnFailedChangeProfileEvent(GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeAccountSetting;
        public event AfterEventHandler? AfterChangeAccountSetting;
        public event SucceedEventHandler? SucceedChangeAccountSetting;
        public event FailedEventHandler? FailedChangeAccountSetting;

        public void OnBeforeChangeAccountSettingEvent(GeneralEventArgs e);
        public void OnAfterChangeAccountSettingEvent(GeneralEventArgs e);
        public void OnSucceedChangeAccountSettingEvent(GeneralEventArgs e);
        public void OnFailedChangeAccountSettingEvent(GeneralEventArgs e);
    }

    public interface IOpenInventoryEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenInventory;
        public event AfterEventHandler? AfterOpenInventory;
        public event SucceedEventHandler? SucceedOpenInventory;
        public event FailedEventHandler? FailedOpenInventory;

        public void OnBeforeOpenInventoryEvent(GeneralEventArgs e);
        public void OnAfterOpenInventoryEvent(GeneralEventArgs e);
        public void OnSucceedOpenInventoryEvent(GeneralEventArgs e);
        public void OnFailedOpenInventoryEvent(GeneralEventArgs e);
    }

    public interface ISignInEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSignIn;
        public event AfterEventHandler? AfterSignIn;
        public event SucceedEventHandler? SucceedSignIn;
        public event FailedEventHandler? FailedSignIn;

        public void OnBeforeSignInEvent(GeneralEventArgs e);
        public void OnAfterSignInEvent(GeneralEventArgs e);
        public void OnSucceedSignInEvent(GeneralEventArgs e);
        public void OnFailedSignInEvent(GeneralEventArgs e);
    }

    public interface IOpenStoreEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenStore;
        public event AfterEventHandler? AfterOpenStore;
        public event SucceedEventHandler? SucceedOpenStore;
        public event FailedEventHandler? FailedOpenStore;

        public void OnBeforeOpenStoreEvent(GeneralEventArgs e);
        public void OnAfterOpenStoreEvent(GeneralEventArgs e);
        public void OnSucceedOpenStoreEvent(GeneralEventArgs e);
        public void OnFailedOpenStoreEvent(GeneralEventArgs e);
    }

    public interface IBuyItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeBuyItem;
        public event AfterEventHandler? AfterBuyItem;
        public event SucceedEventHandler? SucceedBuyItem;
        public event FailedEventHandler? FailedBuyItem;

        public void OnBeforeBuyItemEvent(GeneralEventArgs e);
        public void OnAfterBuyItemEvent(GeneralEventArgs e);
        public void OnSucceedBuyItemEvent(GeneralEventArgs e);
        public void OnFailedBuyItemEvent(GeneralEventArgs e);
    }

    public interface IShowRankingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeShowRanking;
        public event AfterEventHandler? AfterShowRanking;
        public event SucceedEventHandler? SucceedShowRanking;
        public event FailedEventHandler? FailedShowRanking;

        public void OnBeforeShowRankingEvent(GeneralEventArgs e);
        public void OnAfterShowRankingEvent(GeneralEventArgs e);
        public void OnSucceedShowRankingEvent(GeneralEventArgs e);
        public void OnFailedShowRankingEvent(GeneralEventArgs e);
    }

    public interface IUseItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeUseItem;
        public event AfterEventHandler? AfterUseItem;
        public event SucceedEventHandler? SucceedUseItem;
        public event FailedEventHandler? FailedUseItem;

        public void OnBeforeUseItemEvent(GeneralEventArgs e);
        public void OnAfterUseItemEvent(GeneralEventArgs e);
        public void OnSucceedUseItemEvent(GeneralEventArgs e);
        public void OnFailedUseItemEvent(GeneralEventArgs e);
    }

    public interface IEndGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeEndGame;
        public event AfterEventHandler? AfterEndGame;
        public event SucceedEventHandler? SucceedEndGame;
        public event FailedEventHandler? FailedEndGame;

        public void OnBeforeEndGameEvent(GeneralEventArgs e);
        public void OnAfterEndGameEvent(GeneralEventArgs e);
        public void OnSucceedEndGameEvent(GeneralEventArgs e);
        public void OnFailedEndGameEvent(GeneralEventArgs e);
    }
}
