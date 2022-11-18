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
    }

    public interface IDisconnectEvent : IEvent
    {
        public event BeforeEvent BeforeDisconnectEvent;
        public event AfterEvent AfterDisconnectEvent;
        public event SucceedEvent SucceedDisconnectEvent;
        public event FailedEvent FailedDisconnectEvent;
    }

    public interface ILoginEvent : IEvent
    {
        public event BeforeEvent BeforeLoginEvent;
        public event AfterEvent AfterLoginEvent;
        public event SucceedEvent SucceedLoginEvent;
        public event FailedEvent FailedLoginEvent;
    }

    public interface ILogoutEvent : IEvent
    {
        public event BeforeEvent BeforeLogoutEvent;
        public event AfterEvent AfterLogoutEvent;
        public event SucceedEvent SucceedLogoutEvent;
        public event FailedEvent FailedLogoutEvent;
    }

    public interface IRegEvent : IEvent
    {
        public event BeforeEvent BeforeRegEvent;
        public event AfterEvent AfterRegEvent;
        public event SucceedEvent SucceedRegEvent;
        public event FailedEvent FailedRegEvent;
    }

    public interface IIntoRoomEvent : IEvent
    {
        public event BeforeEvent BeforeIntoRoomEvent;
        public event AfterEvent AfterIntoRoomEvent;
        public event SucceedEvent SucceedIntoRoomEvent;
        public event FailedEvent FailedIntoRoomEvent;
    }

    public interface ISendTalkEvent : IEvent
    {
        public event BeforeEvent BeforeSendTalkEvent;
        public event AfterEvent AfterSendTalkEvent;
        public event SucceedEvent SucceedSendTalkEvent;
        public event FailedEvent FailedSendTalkEvent;
    }

    public interface ICreateRoomEvent : IEvent
    {
        public event BeforeEvent BeforeCreateRoomEvent;
        public event AfterEvent AfterCreateRoomEvent;
        public event SucceedEvent SucceedCreateRoomEvent;
        public event FailedEvent FailedCreateRoomEvent;
    }

    public interface IQuitRoomEvent : IEvent
    {
        public event BeforeEvent BeforeQuitRoomEvent;
        public event AfterEvent AfterQuitRoomEvent;
        public event SucceedEvent SucceedQuitRoomEvent;
        public event FailedEvent FailedQuitRoomEvent;
    }

    public interface IChangeRoomSettingEvent : IEvent
    {
        public event BeforeEvent BeforeChangeRoomSettingEvent;
        public event AfterEvent AfterChangeRoomSettingEvent;
        public event SucceedEvent SucceedChangeRoomSettingEvent;
        public event FailedEvent FailedChangeRoomSettingEvent;
    }

    public interface IStartMatchEvent : IEvent
    {
        public event BeforeEvent BeforeStartMatchEvent;
        public event AfterEvent AfterStartMatchEvent;
        public event SucceedEvent SucceedStartMatchEvent;
        public event FailedEvent FailedStartMatchEvent;
    }

    public interface IStartGameEvent : IEvent
    {
        public event BeforeEvent BeforeStartGameEvent;
        public event AfterEvent AfterStartGameEvent;
        public event SucceedEvent SucceedStartGameEvent;
        public event FailedEvent FailedStartGameEvent;
    }

    public interface IChangeProfileEvent : IEvent
    {
        public event BeforeEvent BeforeChangeProfileEvent;
        public event AfterEvent AfterChangeProfileEvent;
        public event SucceedEvent SucceedChangeProfileEvent;
        public event FailedEvent FailedChangeProfileEvent;
    }

    public interface IChangeAccountSettingEvent : IEvent
    {
        public event BeforeEvent BeforeChangeAccountSettingEvent;
        public event AfterEvent AfterChangeAccountSettingEvent;
        public event SucceedEvent SucceedChangeAccountSettingEvent;
        public event FailedEvent FailedChangeAccountSettingEvent;
    }

    public interface IOpenInventoryEvent : IEvent
    {
        public event BeforeEvent BeforeOpenInventoryEvent;
        public event AfterEvent AfterOpenInventoryEvent;
        public event SucceedEvent SucceedOpenInventoryEvent;
        public event FailedEvent FailedOpenInventoryEvent;
    }

    public interface ISignInEvent : IEvent
    {
        public event BeforeEvent BeforeSignInEvent;
        public event AfterEvent AfterSignInEvent;
        public event SucceedEvent SucceedSignInEvent;
        public event FailedEvent FailedSignInEvent;
    }

    public interface IOpenStoreEvent : IEvent
    {
        public event BeforeEvent BeforeOpenStoreEvent;
        public event AfterEvent AfterOpenStoreEvent;
        public event SucceedEvent SucceedOpenStoreEvent;
        public event FailedEvent FailedOpenStoreEvent;
    }

    public interface IBuyItemEvent : IEvent
    {
        public event BeforeEvent BeforeBuyItemEvent;
        public event AfterEvent AfterBuyItemEvent;
        public event SucceedEvent SucceedBuyItemEvent;
        public event FailedEvent FailedBuyItemEvent;
    }

    public interface IShowRankingEvent : IEvent
    {
        public event BeforeEvent BeforeShowRankingEvent;
        public event AfterEvent AfterShowRankingEvent;
        public event SucceedEvent SucceedShowRankingEvent;
        public event FailedEvent FailedShowRankingEvent;
    }

    public interface IUseItemEvent : IEvent
    {
        public event BeforeEvent BeforeUseItemEvent;
        public event AfterEvent AfterUseItemEvent;
        public event SucceedEvent SucceedUseItemEvent;
        public event FailedEvent FailedUseItemEvent;
    }

    public interface IEndGameEvent : IEvent
    {
        public event BeforeEvent BeforeEndGameEvent;
        public event AfterEvent AfterEndGameEvent;
        public event SucceedEvent SucceedEndGameEvent;
        public event FailedEvent FailedEndGameEvent;
    }
}
