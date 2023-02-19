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
        public event BeforeEventHandler? BeforeConnectEventHandler;
        public event AfterEventHandler? AfterConnectEventHandler;
        public event SucceedEventHandler? SucceedConnectEventHandler;
        public event FailedEventHandler? FailedConnectEventHandler;

        public EventResult OnBeforeConnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterConnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedConnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedConnectEvent(object sender, GeneralEventArgs e);
    }

    public interface IDisconnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeDisconnectEventHandler;
        public event AfterEventHandler? AfterDisconnectEventHandler;
        public event SucceedEventHandler? SucceedDisconnectEventHandler;
        public event FailedEventHandler? FailedDisconnectEventHandler;

        public EventResult OnBeforeDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedDisconnectEvent(object sender, GeneralEventArgs e);
    }

    public interface ILoginEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLoginEventHandler;
        public event AfterEventHandler? AfterLoginEventHandler;
        public event SucceedEventHandler? SucceedLoginEventHandler;
        public event FailedEventHandler? FailedLoginEventHandler;

        public EventResult OnBeforeLoginEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterLoginEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedLoginEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedLoginEvent(object sender, GeneralEventArgs e);
    }

    public interface ILogoutEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogoutEventHandler;
        public event AfterEventHandler? AfterLogoutEventHandler;
        public event SucceedEventHandler? SucceedLogoutEventHandler;
        public event FailedEventHandler? FailedLogoutEventHandler;

        public EventResult OnBeforeLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedLogoutEvent(object sender, GeneralEventArgs e);
    }

    public interface IRegEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeRegEventHandler;
        public event AfterEventHandler? AfterRegEventHandler;
        public event SucceedEventHandler? SucceedRegEventHandler;
        public event FailedEventHandler? FailedRegEventHandler;

        public EventResult OnBeforeRegEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterRegEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedRegEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedRegEvent(object sender, GeneralEventArgs e);
    }

    public interface IIntoRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeIntoRoomEventHandler;
        public event AfterEventHandler? AfterIntoRoomEventHandler;
        public event SucceedEventHandler? SucceedIntoRoomEventHandler;
        public event FailedEventHandler? FailedIntoRoomEventHandler;

        public EventResult OnBeforeIntoRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterIntoRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedIntoRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedIntoRoomEvent(object sender, GeneralEventArgs e);
    }

    public interface ISendTalkEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSendTalkEventHandler;
        public event AfterEventHandler? AfterSendTalkEventHandler;
        public event SucceedEventHandler? SucceedSendTalkEventHandler;
        public event FailedEventHandler? FailedSendTalkEventHandler;

        public EventResult OnBeforeSendTalkEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterSendTalkEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedSendTalkEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedSendTalkEvent(object sender, GeneralEventArgs e);
    }

    public interface ICreateRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeCreateRoomEventHandler;
        public event AfterEventHandler? AfterCreateRoomEventHandler;
        public event SucceedEventHandler? SucceedCreateRoomEventHandler;
        public event FailedEventHandler? FailedCreateRoomEventHandler;

        public EventResult OnBeforeCreateRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterCreateRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedCreateRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedCreateRoomEvent(object sender, GeneralEventArgs e);
    }

    public interface IQuitRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeQuitRoomEventHandler;
        public event AfterEventHandler? AfterQuitRoomEventHandler;
        public event SucceedEventHandler? SucceedQuitRoomEventHandler;
        public event FailedEventHandler? FailedQuitRoomEventHandler;

        public EventResult OnBeforeQuitRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterQuitRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedQuitRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedQuitRoomEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeRoomSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeRoomSettingEventHandler;
        public event AfterEventHandler? AfterChangeRoomSettingEventHandler;
        public event SucceedEventHandler? SucceedChangeRoomSettingEventHandler;
        public event FailedEventHandler? FailedChangeRoomSettingEventHandler;

        public EventResult OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartMatchEventHandler;
        public event AfterEventHandler? AfterStartMatchEventHandler;
        public event SucceedEventHandler? SucceedStartMatchEventHandler;
        public event FailedEventHandler? FailedStartMatchEventHandler;

        public EventResult OnBeforeStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedStartMatchEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartGameEventHandler;
        public event AfterEventHandler? AfterStartGameEventHandler;
        public event SucceedEventHandler? SucceedStartGameEventHandler;
        public event FailedEventHandler? FailedStartGameEventHandler;

        public EventResult OnBeforeStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedStartGameEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeProfileEventHandler;
        public event AfterEventHandler? AfterChangeProfileEventHandler;
        public event SucceedEventHandler? SucceedChangeProfileEventHandler;
        public event FailedEventHandler? FailedChangeProfileEventHandler;

        public EventResult OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedChangeProfileEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeAccountSettingEventHandler;
        public event AfterEventHandler? AfterChangeAccountSettingEventHandler;
        public event SucceedEventHandler? SucceedChangeAccountSettingEventHandler;
        public event FailedEventHandler? FailedChangeAccountSettingEventHandler;

        public EventResult OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenInventoryEventHandler;
        public event AfterEventHandler? AfterOpenInventoryEventHandler;
        public event SucceedEventHandler? SucceedOpenInventoryEventHandler;
        public event FailedEventHandler? FailedOpenInventoryEventHandler;

        public EventResult OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedOpenInventoryEvent(object sender, GeneralEventArgs e);
    }

    public interface ISignInEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSignInEventHandler;
        public event AfterEventHandler? AfterSignInEventHandler;
        public event SucceedEventHandler? SucceedSignInEventHandler;
        public event FailedEventHandler? FailedSignInEventHandler;

        public EventResult OnBeforeSignInEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterSignInEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedSignInEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedSignInEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenStoreEventHandler;
        public event AfterEventHandler? AfterOpenStoreEventHandler;
        public event SucceedEventHandler? SucceedOpenStoreEventHandler;
        public event FailedEventHandler? FailedOpenStoreEventHandler;

        public EventResult OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedOpenStoreEvent(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeBuyItemEventHandler;
        public event AfterEventHandler? AfterBuyItemEventHandler;
        public event SucceedEventHandler? SucceedBuyItemEventHandler;
        public event FailedEventHandler? FailedBuyItemEventHandler;

        public EventResult OnBeforeBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedBuyItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeShowRankingEventHandler;
        public event AfterEventHandler? AfterShowRankingEventHandler;
        public event SucceedEventHandler? SucceedShowRankingEventHandler;
        public event FailedEventHandler? FailedShowRankingEventHandler;

        public EventResult OnBeforeShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedShowRankingEvent(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeUseItemEventHandler;
        public event AfterEventHandler? AfterUseItemEventHandler;
        public event SucceedEventHandler? SucceedUseItemEventHandler;
        public event FailedEventHandler? FailedUseItemEventHandler;

        public EventResult OnBeforeUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedUseItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeEndGameEventHandler;
        public event AfterEventHandler? AfterEndGameEventHandler;
        public event SucceedEventHandler? SucceedEndGameEventHandler;
        public event FailedEventHandler? FailedEndGameEventHandler;

        public EventResult OnBeforeEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedEndGameEvent(object sender, GeneralEventArgs e);
    }
}
