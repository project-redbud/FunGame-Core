using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface
{
    public interface IEvent
    {
        public delegate EventResult BeforeEvent(object sender, GeneralEventArgs e);
        public delegate EventResult AfterEvent(object sender, GeneralEventArgs e);
        public delegate EventResult SucceedEvent(object sender, GeneralEventArgs e);
        public delegate EventResult FailedEvent(object sender, GeneralEventArgs e);
    }

    public interface IConnectEvent : IEvent
    {
        public event BeforeEvent BeforeConnectEvent;
        public event AfterEvent AfterConnectEvent;
        public event SucceedEvent SucceedConnectEvent;
        public event FailedEvent FailedConnectEvent;

        public EventResult OnBeforeConnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterConnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedConnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedConnectEvent(object sender, GeneralEventArgs e);
    }

    public interface IDisconnectEvent : IEvent
    {
        public event BeforeEvent BeforeDisconnectEvent;
        public event AfterEvent AfterDisconnectEvent;
        public event SucceedEvent SucceedDisconnectEvent;
        public event FailedEvent FailedDisconnectEvent;

        public EventResult OnBeforeDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedDisconnectEvent(object sender, GeneralEventArgs e);
    }

    public interface ILoginEvent : IEvent
    {
        public event BeforeEvent BeforeLoginEvent;
        public event AfterEvent AfterLoginEvent;
        public event SucceedEvent SucceedLoginEvent;
        public event FailedEvent FailedLoginEvent;

        public EventResult OnBeforeLoginEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterLoginEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedLoginEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedLoginEvent(object sender, GeneralEventArgs e);
    }

    public interface ILogoutEvent : IEvent
    {
        public event BeforeEvent BeforeLogoutEvent;
        public event AfterEvent AfterLogoutEvent;
        public event SucceedEvent SucceedLogoutEvent;
        public event FailedEvent FailedLogoutEvent;

        public EventResult OnBeforeLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedLogoutEvent(object sender, GeneralEventArgs e);
    }

    public interface IRegEvent : IEvent
    {
        public event BeforeEvent BeforeRegEvent;
        public event AfterEvent AfterRegEvent;
        public event SucceedEvent SucceedRegEvent;
        public event FailedEvent FailedRegEvent;

        public EventResult OnBeforeRegEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterRegEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedRegEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedRegEvent(object sender, GeneralEventArgs e);
    }

    public interface IIntoRoomEvent : IEvent
    {
        public event BeforeEvent BeforeIntoRoomEvent;
        public event AfterEvent AfterIntoRoomEvent;
        public event SucceedEvent SucceedIntoRoomEvent;
        public event FailedEvent FailedIntoRoomEvent;

        public EventResult OnBeforeIntoRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterIntoRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedIntoRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedIntoRoomEvent(object sender, GeneralEventArgs e);
    }

    public interface ISendTalkEvent : IEvent
    {
        public event BeforeEvent BeforeSendTalkEvent;
        public event AfterEvent AfterSendTalkEvent;
        public event SucceedEvent SucceedSendTalkEvent;
        public event FailedEvent FailedSendTalkEvent;

        public EventResult OnBeforeSendTalkEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterSendTalkEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedSendTalkEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedSendTalkEvent(object sender, GeneralEventArgs e);
    }

    public interface ICreateRoomEvent : IEvent
    {
        public event BeforeEvent BeforeCreateRoomEvent;
        public event AfterEvent AfterCreateRoomEvent;
        public event SucceedEvent SucceedCreateRoomEvent;
        public event FailedEvent FailedCreateRoomEvent;

        public EventResult OnBeforeCreateRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterCreateRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedCreateRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedCreateRoomEvent(object sender, GeneralEventArgs e);
    }

    public interface IQuitRoomEvent : IEvent
    {
        public event BeforeEvent BeforeQuitRoomEvent;
        public event AfterEvent AfterQuitRoomEvent;
        public event SucceedEvent SucceedQuitRoomEvent;
        public event FailedEvent FailedQuitRoomEvent;

        public EventResult OnBeforeQuitRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterQuitRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedQuitRoomEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedQuitRoomEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeRoomSettingEvent : IEvent
    {
        public event BeforeEvent BeforeChangeRoomSettingEvent;
        public event AfterEvent AfterChangeRoomSettingEvent;
        public event SucceedEvent SucceedChangeRoomSettingEvent;
        public event FailedEvent FailedChangeRoomSettingEvent;

        public EventResult OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEvent : IEvent
    {
        public event BeforeEvent BeforeStartMatchEvent;
        public event AfterEvent AfterStartMatchEvent;
        public event SucceedEvent SucceedStartMatchEvent;
        public event FailedEvent FailedStartMatchEvent;

        public EventResult OnBeforeStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedStartMatchEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEvent : IEvent
    {
        public event BeforeEvent BeforeStartGameEvent;
        public event AfterEvent AfterStartGameEvent;
        public event SucceedEvent SucceedStartGameEvent;
        public event FailedEvent FailedStartGameEvent;

        public EventResult OnBeforeStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedStartGameEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEvent : IEvent
    {
        public event BeforeEvent BeforeChangeProfileEvent;
        public event AfterEvent AfterChangeProfileEvent;
        public event SucceedEvent SucceedChangeProfileEvent;
        public event FailedEvent FailedChangeProfileEvent;

        public EventResult OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedChangeProfileEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEvent : IEvent
    {
        public event BeforeEvent BeforeChangeAccountSettingEvent;
        public event AfterEvent AfterChangeAccountSettingEvent;
        public event SucceedEvent SucceedChangeAccountSettingEvent;
        public event FailedEvent FailedChangeAccountSettingEvent;

        public EventResult OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEvent : IEvent
    {
        public event BeforeEvent BeforeOpenInventoryEvent;
        public event AfterEvent AfterOpenInventoryEvent;
        public event SucceedEvent SucceedOpenInventoryEvent;
        public event FailedEvent FailedOpenInventoryEvent;

        public EventResult OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedOpenInventoryEvent(object sender, GeneralEventArgs e);
    }

    public interface ISignInEvent : IEvent
    {
        public event BeforeEvent BeforeSignInEvent;
        public event AfterEvent AfterSignInEvent;
        public event SucceedEvent SucceedSignInEvent;
        public event FailedEvent FailedSignInEvent;

        public EventResult OnBeforeSignInEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterSignInEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedSignInEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedSignInEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEvent : IEvent
    {
        public event BeforeEvent BeforeOpenStoreEvent;
        public event AfterEvent AfterOpenStoreEvent;
        public event SucceedEvent SucceedOpenStoreEvent;
        public event FailedEvent FailedOpenStoreEvent;

        public EventResult OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedOpenStoreEvent(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEvent : IEvent
    {
        public event BeforeEvent BeforeBuyItemEvent;
        public event AfterEvent AfterBuyItemEvent;
        public event SucceedEvent SucceedBuyItemEvent;
        public event FailedEvent FailedBuyItemEvent;

        public EventResult OnBeforeBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedBuyItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEvent : IEvent
    {
        public event BeforeEvent BeforeShowRankingEvent;
        public event AfterEvent AfterShowRankingEvent;
        public event SucceedEvent SucceedShowRankingEvent;
        public event FailedEvent FailedShowRankingEvent;

        public EventResult OnBeforeShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedShowRankingEvent(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEvent : IEvent
    {
        public event BeforeEvent BeforeUseItemEvent;
        public event AfterEvent AfterUseItemEvent;
        public event SucceedEvent SucceedUseItemEvent;
        public event FailedEvent FailedUseItemEvent;

        public EventResult OnBeforeUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedUseItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEvent : IEvent
    {
        public event BeforeEvent BeforeEndGameEvent;
        public event AfterEvent AfterEndGameEvent;
        public event SucceedEvent SucceedEndGameEvent;
        public event FailedEvent FailedEndGameEvent;

        public EventResult OnBeforeEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnAfterEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnSucceedEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult OnFailedEndGameEvent(object sender, GeneralEventArgs e);
    }
}
