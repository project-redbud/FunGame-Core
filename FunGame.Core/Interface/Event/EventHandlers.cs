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
        public event BeforeEventHandler? BeforeConnectEvent;
        public event AfterEventHandler? AfterConnectEvent;
        public event SucceedEventHandler? SucceedConnectEvent;
        public event FailedEventHandler? FailedConnectEvent;

        public EventResult OnBeforeConnectEvent(GeneralEventArgs e);
        public EventResult OnAfterConnectEvent(GeneralEventArgs e);
        public EventResult OnSucceedConnectEvent(GeneralEventArgs e);
        public EventResult OnFailedConnectEvent(GeneralEventArgs e);
    }

    public interface IDisconnectEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeDisconnectEvent;
        public event AfterEventHandler? AfterDisconnectEvent;
        public event SucceedEventHandler? SucceedDisconnectEvent;
        public event FailedEventHandler? FailedDisconnectEvent;

        public EventResult OnBeforeDisconnectEvent(GeneralEventArgs e);
        public EventResult OnAfterDisconnectEvent(GeneralEventArgs e);
        public EventResult OnSucceedDisconnectEvent(GeneralEventArgs e);
        public EventResult OnFailedDisconnectEvent(GeneralEventArgs e);
    }

    public interface ILoginEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLoginEvent;
        public event AfterEventHandler? AfterLoginEvent;
        public event SucceedEventHandler? SucceedLoginEvent;
        public event FailedEventHandler? FailedLoginEvent;

        public EventResult OnBeforeLoginEvent(GeneralEventArgs e);
        public EventResult OnAfterLoginEvent(GeneralEventArgs e);
        public EventResult OnSucceedLoginEvent(GeneralEventArgs e);
        public EventResult OnFailedLoginEvent(GeneralEventArgs e);
    }

    public interface ILogoutEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeLogoutEvent;
        public event AfterEventHandler? AfterLogoutEvent;
        public event SucceedEventHandler? SucceedLogoutEvent;
        public event FailedEventHandler? FailedLogoutEvent;

        public EventResult OnBeforeLogoutEvent(GeneralEventArgs e);
        public EventResult OnAfterLogoutEvent(GeneralEventArgs e);
        public EventResult OnSucceedLogoutEvent(GeneralEventArgs e);
        public EventResult OnFailedLogoutEvent(GeneralEventArgs e);
    }

    public interface IRegEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeRegEvent;
        public event AfterEventHandler? AfterRegEvent;
        public event SucceedEventHandler? SucceedRegEvent;
        public event FailedEventHandler? FailedRegEvent;

        public EventResult OnBeforeRegEvent(GeneralEventArgs e);
        public EventResult OnAfterRegEvent(GeneralEventArgs e);
        public EventResult OnSucceedRegEvent(GeneralEventArgs e);
        public EventResult OnFailedRegEvent(GeneralEventArgs e);
    }

    public interface IIntoRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeIntoRoomEvent;
        public event AfterEventHandler? AfterIntoRoomEvent;
        public event SucceedEventHandler? SucceedIntoRoomEvent;
        public event FailedEventHandler? FailedIntoRoomEvent;

        public EventResult OnBeforeIntoRoomEvent(GeneralEventArgs e);
        public EventResult OnAfterIntoRoomEvent(GeneralEventArgs e);
        public EventResult OnSucceedIntoRoomEvent(GeneralEventArgs e);
        public EventResult OnFailedIntoRoomEvent(GeneralEventArgs e);
    }

    public interface ISendTalkEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSendTalkEvent;
        public event AfterEventHandler? AfterSendTalkEvent;
        public event SucceedEventHandler? SucceedSendTalkEvent;
        public event FailedEventHandler? FailedSendTalkEvent;

        public EventResult OnBeforeSendTalkEvent(GeneralEventArgs e);
        public EventResult OnAfterSendTalkEvent(GeneralEventArgs e);
        public EventResult OnSucceedSendTalkEvent(GeneralEventArgs e);
        public EventResult OnFailedSendTalkEvent(GeneralEventArgs e);
    }

    public interface ICreateRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeCreateRoomEvent;
        public event AfterEventHandler? AfterCreateRoomEvent;
        public event SucceedEventHandler? SucceedCreateRoomEvent;
        public event FailedEventHandler? FailedCreateRoomEvent;

        public EventResult OnBeforeCreateRoomEvent(GeneralEventArgs e);
        public EventResult OnAfterCreateRoomEvent(GeneralEventArgs e);
        public EventResult OnSucceedCreateRoomEvent(GeneralEventArgs e);
        public EventResult OnFailedCreateRoomEvent(GeneralEventArgs e);
    }

    public interface IQuitRoomEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeQuitRoomEvent;
        public event AfterEventHandler? AfterQuitRoomEvent;
        public event SucceedEventHandler? SucceedQuitRoomEvent;
        public event FailedEventHandler? FailedQuitRoomEvent;

        public EventResult OnBeforeQuitRoomEvent(GeneralEventArgs e);
        public EventResult OnAfterQuitRoomEvent(GeneralEventArgs e);
        public EventResult OnSucceedQuitRoomEvent(GeneralEventArgs e);
        public EventResult OnFailedQuitRoomEvent(GeneralEventArgs e);
    }

    public interface IChangeRoomSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeRoomSettingEvent;
        public event AfterEventHandler? AfterChangeRoomSettingEvent;
        public event SucceedEventHandler? SucceedChangeRoomSettingEvent;
        public event FailedEventHandler? FailedChangeRoomSettingEvent;

        public EventResult OnBeforeChangeRoomSettingEvent(GeneralEventArgs e);
        public EventResult OnAfterChangeRoomSettingEvent(GeneralEventArgs e);
        public EventResult OnSucceedChangeRoomSettingEvent(GeneralEventArgs e);
        public EventResult OnFailedChangeRoomSettingEvent(GeneralEventArgs e);
    }

    public interface IStartMatchEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartMatchEvent;
        public event AfterEventHandler? AfterStartMatchEvent;
        public event SucceedEventHandler? SucceedStartMatchEvent;
        public event FailedEventHandler? FailedStartMatchEvent;

        public EventResult OnBeforeStartMatchEvent(GeneralEventArgs e);
        public EventResult OnAfterStartMatchEvent(GeneralEventArgs e);
        public EventResult OnSucceedStartMatchEvent(GeneralEventArgs e);
        public EventResult OnFailedStartMatchEvent(GeneralEventArgs e);
    }

    public interface IStartGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeStartGameEvent;
        public event AfterEventHandler? AfterStartGameEvent;
        public event SucceedEventHandler? SucceedStartGameEvent;
        public event FailedEventHandler? FailedStartGameEvent;

        public EventResult OnBeforeStartGameEvent(GeneralEventArgs e);
        public EventResult OnAfterStartGameEvent(GeneralEventArgs e);
        public EventResult OnSucceedStartGameEvent(GeneralEventArgs e);
        public EventResult OnFailedStartGameEvent(GeneralEventArgs e);
    }

    public interface IChangeProfileEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeProfileEvent;
        public event AfterEventHandler? AfterChangeProfileEvent;
        public event SucceedEventHandler? SucceedChangeProfileEvent;
        public event FailedEventHandler? FailedChangeProfileEvent;

        public EventResult OnBeforeChangeProfileEvent(GeneralEventArgs e);
        public EventResult OnAfterChangeProfileEvent(GeneralEventArgs e);
        public EventResult OnSucceedChangeProfileEvent(GeneralEventArgs e);
        public EventResult OnFailedChangeProfileEvent(GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeChangeAccountSettingEvent;
        public event AfterEventHandler? AfterChangeAccountSettingEvent;
        public event SucceedEventHandler? SucceedChangeAccountSettingEvent;
        public event FailedEventHandler? FailedChangeAccountSettingEvent;

        public EventResult OnBeforeChangeAccountSettingEvent(GeneralEventArgs e);
        public EventResult OnAfterChangeAccountSettingEvent(GeneralEventArgs e);
        public EventResult OnSucceedChangeAccountSettingEvent(GeneralEventArgs e);
        public EventResult OnFailedChangeAccountSettingEvent(GeneralEventArgs e);
    }

    public interface IOpenInventoryEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenInventoryEvent;
        public event AfterEventHandler? AfterOpenInventoryEvent;
        public event SucceedEventHandler? SucceedOpenInventoryEvent;
        public event FailedEventHandler? FailedOpenInventoryEvent;

        public EventResult OnBeforeOpenInventoryEvent(GeneralEventArgs e);
        public EventResult OnAfterOpenInventoryEvent(GeneralEventArgs e);
        public EventResult OnSucceedOpenInventoryEvent(GeneralEventArgs e);
        public EventResult OnFailedOpenInventoryEvent(GeneralEventArgs e);
    }

    public interface ISignInEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeSignInEvent;
        public event AfterEventHandler? AfterSignInEvent;
        public event SucceedEventHandler? SucceedSignInEvent;
        public event FailedEventHandler? FailedSignInEvent;

        public EventResult OnBeforeSignInEvent(GeneralEventArgs e);
        public EventResult OnAfterSignInEvent(GeneralEventArgs e);
        public EventResult OnSucceedSignInEvent(GeneralEventArgs e);
        public EventResult OnFailedSignInEvent(GeneralEventArgs e);
    }

    public interface IOpenStoreEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeOpenStoreEvent;
        public event AfterEventHandler? AfterOpenStoreEvent;
        public event SucceedEventHandler? SucceedOpenStoreEvent;
        public event FailedEventHandler? FailedOpenStoreEvent;

        public EventResult OnBeforeOpenStoreEvent(GeneralEventArgs e);
        public EventResult OnAfterOpenStoreEvent(GeneralEventArgs e);
        public EventResult OnSucceedOpenStoreEvent(GeneralEventArgs e);
        public EventResult OnFailedOpenStoreEvent(GeneralEventArgs e);
    }

    public interface IBuyItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeBuyItemEvent;
        public event AfterEventHandler? AfterBuyItemEvent;
        public event SucceedEventHandler? SucceedBuyItemEvent;
        public event FailedEventHandler? FailedBuyItemEvent;

        public EventResult OnBeforeBuyItemEvent(GeneralEventArgs e);
        public EventResult OnAfterBuyItemEvent(GeneralEventArgs e);
        public EventResult OnSucceedBuyItemEvent(GeneralEventArgs e);
        public EventResult OnFailedBuyItemEvent(GeneralEventArgs e);
    }

    public interface IShowRankingEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeShowRankingEvent;
        public event AfterEventHandler? AfterShowRankingEvent;
        public event SucceedEventHandler? SucceedShowRankingEvent;
        public event FailedEventHandler? FailedShowRankingEvent;

        public EventResult OnBeforeShowRankingEvent(GeneralEventArgs e);
        public EventResult OnAfterShowRankingEvent(GeneralEventArgs e);
        public EventResult OnSucceedShowRankingEvent(GeneralEventArgs e);
        public EventResult OnFailedShowRankingEvent(GeneralEventArgs e);
    }

    public interface IUseItemEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeUseItemEvent;
        public event AfterEventHandler? AfterUseItemEvent;
        public event SucceedEventHandler? SucceedUseItemEvent;
        public event FailedEventHandler? FailedUseItemEvent;

        public EventResult OnBeforeUseItemEvent(GeneralEventArgs e);
        public EventResult OnAfterUseItemEvent(GeneralEventArgs e);
        public EventResult OnSucceedUseItemEvent(GeneralEventArgs e);
        public EventResult OnFailedUseItemEvent(GeneralEventArgs e);
    }

    public interface IEndGameEventHandler : IEventHandler
    {
        public event BeforeEventHandler? BeforeEndGameEvent;
        public event AfterEventHandler? AfterEndGameEvent;
        public event SucceedEventHandler? SucceedEndGameEvent;
        public event FailedEventHandler? FailedEndGameEvent;

        public EventResult OnBeforeEndGameEvent(GeneralEventArgs e);
        public EventResult OnAfterEndGameEvent(GeneralEventArgs e);
        public EventResult OnSucceedEndGameEvent(GeneralEventArgs e);
        public EventResult OnFailedEndGameEvent(GeneralEventArgs e);
    }
}
