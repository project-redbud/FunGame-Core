using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 插件需要实现什么事件就继承什么接口
    /// </summary>
    public interface IConnectEvent
    {
        public EventResult BeforeConnect(object sender, GeneralEventArgs e);
        public EventResult AfterConnect(object sender, GeneralEventArgs e);
        public EventResult SucceedConnect(object sender, GeneralEventArgs e);
        public EventResult FailedConnect(object sender, GeneralEventArgs e);
    }

    public interface IDisconnectEvent
    {
        public EventResult BeforeDisconnect(object sender, GeneralEventArgs e);
        public EventResult AfterDisconnect(object sender, GeneralEventArgs e);
        public EventResult SucceedDisconnect(object sender, GeneralEventArgs e);
        public EventResult FailedDisconnect(object sender, GeneralEventArgs e);
    }

    public interface ILoginEvent
    {
        public EventResult BeforeLogin(object sender, GeneralEventArgs e);
        public EventResult AfterLogin(object sender, GeneralEventArgs e);
        public EventResult SucceedLogin(object sender, GeneralEventArgs e);
        public EventResult FailedLogin(object sender, GeneralEventArgs e);
    }

    public interface ILogoutEvent
    {
        public EventResult BeforeLogout(object sender, GeneralEventArgs e);
        public EventResult AfterLogout(object sender, GeneralEventArgs e);
        public EventResult SucceedLogout(object sender, GeneralEventArgs e);
        public EventResult FailedLogout(object sender, GeneralEventArgs e);
    }

    public interface IRegEvent
    {
        public EventResult BeforeReg(object sender, GeneralEventArgs e);
        public EventResult AfterReg(object sender, GeneralEventArgs e);
        public EventResult SucceedReg(object sender, GeneralEventArgs e);
        public EventResult FailedReg(object sender, GeneralEventArgs e);
    }

    public interface IIntoRoomEvent
    {
        public EventResult BeforeIntoRoom(object sender, GeneralEventArgs e);
        public EventResult AfterIntoRoom(object sender, GeneralEventArgs e);
        public EventResult SucceedIntoRoom(object sender, GeneralEventArgs e);
        public EventResult FailedIntoRoom(object sender, GeneralEventArgs e);
    }

    public interface ISendTalkEvent
    {
        public EventResult BeforeSendTalk(object sender, GeneralEventArgs e);
        public EventResult AfterSendTalk(object sender, GeneralEventArgs e);
        public EventResult SucceedSendTalk(object sender, GeneralEventArgs e);
        public EventResult FailedSendTalk(object sender, GeneralEventArgs e);
    }

    public interface ICreateRoomEvent
    {
        public EventResult BeforeCreateRoom(object sender, GeneralEventArgs e);
        public EventResult AfterCreateRoom(object sender, GeneralEventArgs e);
        public EventResult SucceedCreateRoom(object sender, GeneralEventArgs e);
        public EventResult FailedCreateRoom(object sender, GeneralEventArgs e);
    }

    public interface IQuitRoomEvent
    {
        public EventResult BeforeQuitRoom(object sender, GeneralEventArgs e);
        public EventResult AfterQuitRoom(object sender, GeneralEventArgs e);
        public EventResult SucceedQuitRoom(object sender, GeneralEventArgs e);
        public EventResult FailedQuitRoom(object sender, GeneralEventArgs e);
    }

    public interface IChangeRoomSettingEvent
    {
        public EventResult BeforeChangeRoomSetting(object sender, GeneralEventArgs e);
        public EventResult AfterChangeRoomSetting(object sender, GeneralEventArgs e);
        public EventResult SucceedChangeRoomSetting(object sender, GeneralEventArgs e);
        public EventResult FailedChangeRoomSetting(object sender, GeneralEventArgs e);
    }

    public interface IStartMatchEvent
    {
        public EventResult BeforeStartMatch(object sender, GeneralEventArgs e);
        public EventResult AfterStartMatch(object sender, GeneralEventArgs e);
        public EventResult SucceedStartMatch(object sender, GeneralEventArgs e);
        public EventResult FailedStartMatch(object sender, GeneralEventArgs e);
    }

    public interface IStartGameEvent
    {
        public EventResult BeforeStartGame(object sender, GeneralEventArgs e);
        public EventResult AfterStartGame(object sender, GeneralEventArgs e);
        public EventResult SucceedStartGame(object sender, GeneralEventArgs e);
        public EventResult FailedStartGame(object sender, GeneralEventArgs e);
    }

    public interface IChangeProfileEvent
    {
        public EventResult BeforeChangeProfile(object sender, GeneralEventArgs e);
        public EventResult AfterChangeProfile(object sender, GeneralEventArgs e);
        public EventResult SucceedChangeProfile(object sender, GeneralEventArgs e);
        public EventResult FailedChangeProfile(object sender, GeneralEventArgs e);
    }

    public interface IChangeAccountSettingEvent
    {
        public EventResult BeforeChangeAccountSetting(object sender, GeneralEventArgs e);
        public EventResult AfterChangeAccountSetting(object sender, GeneralEventArgs e);
        public EventResult SucceedChangeAccountSetting(object sender, GeneralEventArgs e);
        public EventResult FailedChangeAccountSetting(object sender, GeneralEventArgs e);
    }

    public interface IOpenInventoryEvent
    {
        public EventResult BeforeOpenInventory(object sender, GeneralEventArgs e);
        public EventResult AfterOpenInventory(object sender, GeneralEventArgs e);
        public EventResult SucceedOpenInventory(object sender, GeneralEventArgs e);
        public EventResult FailedOpenInventory(object sender, GeneralEventArgs e);
    }

    public interface ISignInEvent
    {
        public EventResult BeforeSignIn(object sender, GeneralEventArgs e);
        public EventResult AfterSignIn(object sender, GeneralEventArgs e);
        public EventResult SucceedSignIn(object sender, GeneralEventArgs e);
        public EventResult FailedSignIn(object sender, GeneralEventArgs e);
    }

    public interface IOpenStoreEvent
    {
        public EventResult BeforeOpenStore(object sender, GeneralEventArgs e);
        public EventResult AfterOpenStore(object sender, GeneralEventArgs e);
        public EventResult SucceedOpenStore(object sender, GeneralEventArgs e);
        public EventResult FailedOpenStore(object sender, GeneralEventArgs e);
    }

    public interface IBuyItemEvent
    {
        public EventResult BeforeBuyItem(object sender, GeneralEventArgs e);
        public EventResult AfterBuyItem(object sender, GeneralEventArgs e);
        public EventResult SucceedBuyItem(object sender, GeneralEventArgs e);
        public EventResult FailedBuyItem(object sender, GeneralEventArgs e);
    }

    public interface IShowRankingEvent
    {
        public EventResult BeforeShowRanking(object sender, GeneralEventArgs e);
        public EventResult AfterShowRanking(object sender, GeneralEventArgs e);
        public EventResult SucceedShowRanking(object sender, GeneralEventArgs e);
        public EventResult FailedShowRanking(object sender, GeneralEventArgs e);
    }

    public interface IUseItemEvent
    {
        public EventResult BeforeUseItem(object sender, GeneralEventArgs e);
        public EventResult AfterUseItem(object sender, GeneralEventArgs e);
        public EventResult SucceedUseItem(object sender, GeneralEventArgs e);
        public EventResult FailedUseItem(object sender, GeneralEventArgs e);
    }

    public interface IEndGameEvent
    {
        public EventResult BeforeEndGame(object sender, GeneralEventArgs e);
        public EventResult AfterEndGame(object sender, GeneralEventArgs e);
        public EventResult SucceedEndGame(object sender, GeneralEventArgs e);
        public EventResult FailedEndGame(object sender, GeneralEventArgs e);
    }
}
