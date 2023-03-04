using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 窗体继承这些接口便能实现事件，或为插件预留。
    /// </summary>
    public interface IEventHandler
    {
        public delegate EventResult BeforeEventHandler(object sender, GeneralEventArgs e);
        public delegate EventResult AfterEventHandler(object sender, GeneralEventArgs e);
        public delegate EventResult SucceedEventHandler(object sender, GeneralEventArgs e);
        public delegate EventResult FailedEventHandler(object sender, GeneralEventArgs e);
    }

    public interface IConnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeConnect;
        public event AfterEventHandler? AfterConnect;
        public event SucceedEventHandler? SucceedConnect;
        public event FailedEventHandler? FailedConnect;

        public EventResult OnBeforeConnectEvent(GeneralEventArgs e);
        public EventResult OnAfterConnectEvent(GeneralEventArgs e);
        public EventResult OnSucceedConnectEvent(GeneralEventArgs e);
        public EventResult OnFailedConnectEvent(GeneralEventArgs e);
    }

    public interface IDisconnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeDisconnect;
        public event AfterEventHandler? AfterDisconnect;
        public event SucceedEventHandler? SucceedDisconnect;
        public event FailedEventHandler? FailedDisconnect;

        public EventResult OnBeforeDisconnectEvent(GeneralEventArgs e);
        public EventResult OnAfterDisconnectEvent(GeneralEventArgs e);
        public EventResult OnSucceedDisconnectEvent(GeneralEventArgs e);
        public EventResult OnFailedDisconnectEvent(GeneralEventArgs e);
    }

    public interface ILoginEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogin;
        public event AfterEventHandler? AfterLogin;
        public event SucceedEventHandler? SucceedLogin;
        public event FailedEventHandler? FailedLogin;

        public EventResult OnBeforeLoginEvent(GeneralEventArgs e);
        public EventResult OnAfterLoginEvent(GeneralEventArgs e);
        public EventResult OnSucceedLoginEvent(GeneralEventArgs e);
        public EventResult OnFailedLoginEvent(GeneralEventArgs e);
    }

    public interface ILogoutEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogout;
        public event AfterEventHandler? AfterLogout;
        public event SucceedEventHandler? SucceedLogout;
        public event FailedEventHandler? FailedLogout;

        public EventResult OnBeforeLogoutEvent(GeneralEventArgs e);
        public EventResult OnAfterLogoutEvent(GeneralEventArgs e);
        public EventResult OnSucceedLogoutEvent(GeneralEventArgs e);
        public EventResult OnFailedLogoutEvent(GeneralEventArgs e);
    }

    public interface IRegEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeReg;
        public event AfterEventHandler? AfterReg;
        public event SucceedEventHandler? SucceedReg;
        public event FailedEventHandler? FailedReg;

        public EventResult OnBeforeRegEvent(RegisterEventArgs e);
        public EventResult OnAfterRegEvent(RegisterEventArgs e);
        public EventResult OnSucceedRegEvent(RegisterEventArgs e);
        public EventResult OnFailedRegEvent(RegisterEventArgs e);
    }

    public interface IIntoRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeIntoRoom;
        public event AfterEventHandler? AfterIntoRoom;
        public event SucceedEventHandler? SucceedIntoRoom;
        public event FailedEventHandler? FailedIntoRoom;

        public EventResult OnBeforeIntoRoomEvent(GeneralEventArgs e);
        public EventResult OnAfterIntoRoomEvent(GeneralEventArgs e);
        public EventResult OnSucceedIntoRoomEvent(GeneralEventArgs e);
        public EventResult OnFailedIntoRoomEvent(GeneralEventArgs e);
    }

    public interface ISendTalkEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSendTalk;
        public event AfterEventHandler? AfterSendTalk;
        public event SucceedEventHandler? SucceedSendTalk;
        public event FailedEventHandler? FailedSendTalk;

        public EventResult OnBeforeSendTalkEvent(GeneralEventArgs e);
        public EventResult OnAfterSendTalkEvent(GeneralEventArgs e);
        public EventResult OnSucceedSendTalkEvent(GeneralEventArgs e);
        public EventResult OnFailedSendTalkEvent(GeneralEventArgs e);
    }

    public interface ICreateRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeCreateRoom;
        public event AfterEventHandler? AfterCreateRoom;
        public event SucceedEventHandler? SucceedCreateRoom;
        public event FailedEventHandler? FailedCreateRoom;

        public EventResult OnBeforeCreateRoomEvent(GeneralEventArgs e);
        public EventResult OnAfterCreateRoomEvent(GeneralEventArgs e);
        public EventResult OnSucceedCreateRoomEvent(GeneralEventArgs e);
        public EventResult OnFailedCreateRoomEvent(GeneralEventArgs e);
    }

    public interface IQuitRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeQuitRoom;
        public event AfterEventHandler? AfterQuitRoom;
        public event SucceedEventHandler? SucceedQuitRoom;
        public event FailedEventHandler? FailedQuitRoom;

        public EventResult OnBeforeQuitRoomEvent(GeneralEventArgs e);
        public EventResult OnAfterQuitRoomEvent(GeneralEventArgs e);
        public EventResult OnSucceedQuitRoomEvent(GeneralEventArgs e);
        public EventResult OnFailedQuitRoomEvent(GeneralEventArgs e);
    }

    public interface IChangeRoomSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeRoomSetting;
        public event AfterEventHandler? AfterChangeRoomSetting;
        public event SucceedEventHandler? SucceedChangeRoomSetting;
        public event FailedEventHandler? FailedChangeRoomSetting;

        public EventResult OnBeforeChangeRoomSettingEvent(GeneralEventArgs e);
        public EventResult OnAfterChangeRoomSettingEvent(GeneralEventArgs e);
        public EventResult OnSucceedChangeRoomSettingEvent(GeneralEventArgs e);
        public EventResult OnFailedChangeRoomSettingEvent(GeneralEventArgs e);
    }

    public interface IStartMatchEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartMatch;
        public event AfterEventHandler? AfterStartMatch;
        public event SucceedEventHandler? SucceedStartMatch;
        public event FailedEventHandler? FailedStartMatch;

        public EventResult OnBeforeStartMatchEvent(GeneralEventArgs e);
        public EventResult OnAfterStartMatchEvent(GeneralEventArgs e);
        public EventResult OnSucceedStartMatchEvent(GeneralEventArgs e);
        public EventResult OnFailedStartMatchEvent(GeneralEventArgs e);
    }

    public interface IStartGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartGame;
        public event AfterEventHandler? AfterStartGame;
        public event SucceedEventHandler? SucceedStartGame;
        public event FailedEventHandler? FailedStartGame;

        public EventResult OnBeforeStartGameEvent(GeneralEventArgs e);
        public EventResult OnAfterStartGameEvent(GeneralEventArgs e);
        public EventResult OnSucceedStartGameEvent(GeneralEventArgs e);
        public EventResult OnFailedStartGameEvent(GeneralEventArgs e);
    }

    public interface IChangeProfileEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeProfile;
        public event AfterEventHandler? AfterChangeProfile;
        public event SucceedEventHandler? SucceedChangeProfile;
        public event FailedEventHandler? FailedChangeProfile;

        public EventResult OnBeforeChangeProfileEvent(GeneralEventArgs e);
        public EventResult OnAfterChangeProfileEvent(GeneralEventArgs e);
        public EventResult OnSucceedChangeProfileEvent(GeneralEventArgs e);
        public EventResult OnFailedChangeProfileEvent(GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeAccountSetting;
        public event AfterEventHandler? AfterChangeAccountSetting;
        public event SucceedEventHandler? SucceedChangeAccountSetting;
        public event FailedEventHandler? FailedChangeAccountSetting;

        public EventResult OnBeforeChangeAccountSettingEvent(GeneralEventArgs e);
        public EventResult OnAfterChangeAccountSettingEvent(GeneralEventArgs e);
        public EventResult OnSucceedChangeAccountSettingEvent(GeneralEventArgs e);
        public EventResult OnFailedChangeAccountSettingEvent(GeneralEventArgs e);
    }

    public interface IOpenInventoryEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenInventory;
        public event AfterEventHandler? AfterOpenInventory;
        public event SucceedEventHandler? SucceedOpenInventory;
        public event FailedEventHandler? FailedOpenInventory;

        public EventResult OnBeforeOpenInventoryEvent(GeneralEventArgs e);
        public EventResult OnAfterOpenInventoryEvent(GeneralEventArgs e);
        public EventResult OnSucceedOpenInventoryEvent(GeneralEventArgs e);
        public EventResult OnFailedOpenInventoryEvent(GeneralEventArgs e);
    }

    public interface ISignInEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSignIn;
        public event AfterEventHandler? AfterSignIn;
        public event SucceedEventHandler? SucceedSignIn;
        public event FailedEventHandler? FailedSignIn;

        public EventResult OnBeforeSignInEvent(GeneralEventArgs e);
        public EventResult OnAfterSignInEvent(GeneralEventArgs e);
        public EventResult OnSucceedSignInEvent(GeneralEventArgs e);
        public EventResult OnFailedSignInEvent(GeneralEventArgs e);
    }

    public interface IOpenStoreEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenStore;
        public event AfterEventHandler? AfterOpenStore;
        public event SucceedEventHandler? SucceedOpenStore;
        public event FailedEventHandler? FailedOpenStore;

        public EventResult OnBeforeOpenStoreEvent(GeneralEventArgs e);
        public EventResult OnAfterOpenStoreEvent(GeneralEventArgs e);
        public EventResult OnSucceedOpenStoreEvent(GeneralEventArgs e);
        public EventResult OnFailedOpenStoreEvent(GeneralEventArgs e);
    }

    public interface IBuyItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeBuyItem;
        public event AfterEventHandler? AfterBuyItem;
        public event SucceedEventHandler? SucceedBuyItem;
        public event FailedEventHandler? FailedBuyItem;

        public EventResult OnBeforeBuyItemEvent(GeneralEventArgs e);
        public EventResult OnAfterBuyItemEvent(GeneralEventArgs e);
        public EventResult OnSucceedBuyItemEvent(GeneralEventArgs e);
        public EventResult OnFailedBuyItemEvent(GeneralEventArgs e);
    }

    public interface IShowRankingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeShowRanking;
        public event AfterEventHandler? AfterShowRanking;
        public event SucceedEventHandler? SucceedShowRanking;
        public event FailedEventHandler? FailedShowRanking;

        public EventResult OnBeforeShowRankingEvent(GeneralEventArgs e);
        public EventResult OnAfterShowRankingEvent(GeneralEventArgs e);
        public EventResult OnSucceedShowRankingEvent(GeneralEventArgs e);
        public EventResult OnFailedShowRankingEvent(GeneralEventArgs e);
    }

    public interface IUseItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeUseItem;
        public event AfterEventHandler? AfterUseItem;
        public event SucceedEventHandler? SucceedUseItem;
        public event FailedEventHandler? FailedUseItem;

        public EventResult OnBeforeUseItemEvent(GeneralEventArgs e);
        public EventResult OnAfterUseItemEvent(GeneralEventArgs e);
        public EventResult OnSucceedUseItemEvent(GeneralEventArgs e);
        public EventResult OnFailedUseItemEvent(GeneralEventArgs e);
    }

    public interface IEndGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeEndGame;
        public event AfterEventHandler? AfterEndGame;
        public event SucceedEventHandler? SucceedEndGame;
        public event FailedEventHandler? FailedEndGame;

        public EventResult OnBeforeEndGameEvent(GeneralEventArgs e);
        public EventResult OnAfterEndGameEvent(GeneralEventArgs e);
        public EventResult OnSucceedEndGameEvent(GeneralEventArgs e);
        public EventResult OnFailedEndGameEvent(GeneralEventArgs e);
    }
}
