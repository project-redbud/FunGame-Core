using Milimoe.FunGame.Core.Library.Common.Event;

// 插件需要实现什么事件就继承什么接口
namespace Milimoe.FunGame.Core.Interface
{
    public interface IConnectEvent
    {
        public void BeforeConnectEvent(object sender, ConnectEventArgs e);
        public void AfterConnectEvent(object sender, ConnectEventArgs e);
    }

    public interface IDisconnectEvent
    {
        public void BeforeDisconnectEvent(object sender, GeneralEventArgs e);
        public void AfterDisconnectEvent(object sender, GeneralEventArgs e);
    }

    public interface ILoginEvent
    {
        public void BeforeLoginEvent(object sender, LoginEventArgs e);
        public void AfterLoginEvent(object sender, LoginEventArgs e);
    }

    public interface ILogoutEvent
    {
        public void BeforeLogoutEvent(object sender, GeneralEventArgs e);
        public void AfterLogoutEvent(object sender, GeneralEventArgs e);
    }

    public interface IRegEvent
    {
        public void BeforeRegEvent(object sender, RegisterEventArgs e);
        public void AfterRegEvent(object sender, RegisterEventArgs e);
    }

    public interface IIntoRoomEvent
    {
        public void BeforeIntoRoomEvent(object sender, RoomEventArgs e);
        public void AfterIntoRoomEvent(object sender, RoomEventArgs e);
    }

    public interface ISendTalkEvent
    {
        public void BeforeSendTalkEvent(object sender, SendTalkEventArgs e);
        public void AfterSendTalkEvent(object sender, SendTalkEventArgs e);
    }

    public interface ICreateRoomEvent
    {
        public void BeforeCreateRoomEvent(object sender, RoomEventArgs e);
        public void AfterCreateRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IQuitRoomEvent
    {
        public void BeforeQuitRoomEvent(object sender, RoomEventArgs e);
        public void AfterQuitRoomEvent(object sender, RoomEventArgs e);
    }

    public interface IChangeRoomSettingEvent
    {
        public void BeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e);
        public void AfterChangeRoomSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEvent
    {
        public void BeforeStartMatchEvent(object sender, GeneralEventArgs e);
        public void AfterStartMatchEvent(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEvent
    {
        public void BeforeStartGameEvent(object sender, GeneralEventArgs e);
        public void AfterStartGameEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEvent
    {
        public void BeforeChangeProfileEvent(object sender, GeneralEventArgs e);
        public void AfterChangeProfileEvent(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEvent
    {
        public void BeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e);
        public void AfterChangeAccountSettingEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEvent
    {
        public void BeforeOpenInventoryEvent(object sender, GeneralEventArgs e);
        public void AfterOpenInventoryEvent(object sender, GeneralEventArgs e);
    }

    public interface ISignInEvent
    {
        public void BeforeSignInEvent(object sender, GeneralEventArgs e);
        public void AfterSignInEvent(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEvent
    {
        public void BeforeOpenStoreEvent(object sender, GeneralEventArgs e);
        public void AfterOpenStoreEvent(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEvent
    {
        public void BeforeBuyItemEvent(object sender, GeneralEventArgs e);
        public void AfterBuyItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEvent
    {
        public void BeforeShowRankingEvent(object sender, GeneralEventArgs e);
        public void AfterShowRankingEvent(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEvent
    {
        public void BeforeUseItemEvent(object sender, GeneralEventArgs e);
        public void AfterUseItemEvent(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEvent
    {
        public void BeforeEndGameEvent(object sender, GeneralEventArgs e);
        public void AfterEndGameEvent(object sender, GeneralEventArgs e);
    }
}
