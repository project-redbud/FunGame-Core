using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 插件需要实现什么事件就继承什么接口
    /// </summary>
    public interface IConnectEvent
    {
        public EventResult BeforeConnectEvent(object sender, ConnectEventArgs e);
        public EventResult AfterConnectEvent(object sender, ConnectEventArgs e);
        public EventResult SucceedConnectEvent(object sender, ConnectEventArgs e);
        public EventResult FailedConnectEvent(object sender, ConnectEventArgs e);
    }

    public interface IDisconnectEvent
    {
        public EventResult BeforeDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult AfterDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedDisconnectEvent(object sender, GeneralEventArgs e);
        public EventResult FailedDisconnectEvent(object sender, GeneralEventArgs e);
    }

    public interface ILoginEvent
    {
        public EventResult BeforeLoginEvent(object sender, LoginEventArgs e);
        public EventResult AfterLoginEvent(object sender, LoginEventArgs e);
        public EventResult SucceedLoginEvent(object sender, LoginEventArgs e);
        public EventResult FailedLoginEvent(object sender, LoginEventArgs e);
    }

    public interface ILogoutEvent
    {
        public EventResult BeforeLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult AfterLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedLogoutEvent(object sender, GeneralEventArgs e);
        public EventResult FailedLogoutEvent(object sender, GeneralEventArgs e);
    }

    public interface IRegEvent
    {
        public EventResult BeforeRegEvent(object sender, RegisterEventArgs e);
        public EventResult AfterRegEvent(object sender, RegisterEventArgs e);
        public EventResult SucceedRegEvent(object sender, RegisterEventArgs e);
        public EventResult FailedRegEvent(object sender, RegisterEventArgs e);
    }

    public interface IIntoRoomEvent
    {
        public EventResult BeforeIntoRoomEvent(object sender, RoomEventArgs e);
        public EventResult AfterIntoRoomEvent(object sender, RoomEventArgs e);
        public EventResult SucceedIntoRoomEvent(object sender, RoomEventArgs e);
        public EventResult FailedIntoRoomEvent(object sender, RoomEventArgs e);
    }

    public interface ISendTalkEvent
    {
        public EventResult BeforeSendTalkEvent(object sender, SendTalkEventArgs e);
        public EventResult AfterSendTalkEvent(object sender, SendTalkEventArgs e);
        public EventResult SucceedSendTalkEvent(object sender, SendTalkEventArgs e);
        public EventResult FailedSendTalkEvent(object sender, SendTalkEventArgs e);
    }

    public interface ICreateRoomEvent
    {
        public EventResult BeforeCreateRoomEvent(object sender, RoomEventArgs e);
        public EventResult AfterCreateRoomEvent(object sender, RoomEventArgs e);
        public EventResult SucceedCreateRoomEvent(object sender, RoomEventArgs e);
        public EventResult FailedCreateRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IQuitRoomEvent
    {
        public EventResult BeforeQuitRoomEvent(object sender, RoomEventArgs e);
        public EventResult AfterQuitRoomEvent(object sender, RoomEventArgs e);
        public EventResult SucceedQuitRoomEvent(object sender, RoomEventArgs e);
        public EventResult FailedQuitRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IChangeRoomSettingEvent
    {
        public EventResult BeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult AfterChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public EventResult FailedChangeRoomSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEvent
    {
        public EventResult BeforeStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult AfterStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedStartMatchEvent(object sender, GeneralEventArgs e);
        public EventResult FailedStartMatchEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEvent
    {
        public EventResult BeforeStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult AfterStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedStartGameEvent(object sender, GeneralEventArgs e);
        public EventResult FailedStartGameEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEvent
    {
        public EventResult BeforeChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult AfterChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedChangeProfileEvent(object sender, GeneralEventArgs e);
        public EventResult FailedChangeProfileEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEvent
    {
        public EventResult BeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult AfterChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public EventResult FailedChangeAccountSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEvent
    {
        public EventResult BeforeOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult AfterOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedOpenInventoryEvent(object sender, GeneralEventArgs e);
        public EventResult FailedOpenInventoryEvent(object sender, GeneralEventArgs e);
    }

    public interface ISignInEvent
    {
        public EventResult BeforeSignInEvent(object sender, GeneralEventArgs e);
        public EventResult AfterSignInEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedSignInEvent(object sender, GeneralEventArgs e);
        public EventResult FailedSignInEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEvent
    {
        public EventResult BeforeOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult AfterOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedOpenStoreEvent(object sender, GeneralEventArgs e);
        public EventResult FailedOpenStoreEvent(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEvent
    {
        public EventResult BeforeBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult AfterBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedBuyItemEvent(object sender, GeneralEventArgs e);
        public EventResult FailedBuyItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEvent
    {
        public EventResult BeforeShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult AfterShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedShowRankingEvent(object sender, GeneralEventArgs e);
        public EventResult FailedShowRankingEvent(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEvent
    {
        public EventResult BeforeUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult AfterUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedUseItemEvent(object sender, GeneralEventArgs e);
        public EventResult FailedUseItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEvent
    {
        public EventResult BeforeEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult AfterEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult SucceedEndGameEvent(object sender, GeneralEventArgs e);
        public EventResult FailedEndGameEvent(object sender, GeneralEventArgs e);
    }
}
