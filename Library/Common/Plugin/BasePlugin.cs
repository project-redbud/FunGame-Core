using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Plugin
{
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// 加载插件
        /// </summary>
        public bool Load()
        {
            if (IsLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此插件
            if (BeforeLoad())
            {
                // 插件加载后，不允许再次加载此插件
                IsLoaded = true;
                // 触发绑定事件
                BindEvent();
                // 如果加载后需要执行代码，请重写AfterLoad方法
                AfterLoad();
            }
            return IsLoaded;
        }

        /// <summary>
        /// 插件加载后需要做的事
        /// </summary>
        protected virtual void AfterLoad()
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此插件
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad()
        {
            return true;
        }

        /// <summary>
        /// 绑定事件。在<see cref="BeforeLoad"/>后触发
        /// </summary>
        private void BindEvent()
        {
            if (this is IConnectEvent)
            {
                IConnectEvent bind = (IConnectEvent)this;
                BeforeConnect += bind.BeforeConnectEvent;
                AfterConnect += bind.AfterConnectEvent;
                SucceedConnect += bind.SucceedConnectEvent;
                FailedConnect += bind.FailedConnectEvent;
            }

            if (this is IDisconnectEvent)
            {
                IDisconnectEvent bind = (IDisconnectEvent)this;
                BeforeDisconnect += bind.BeforeDisconnectEvent;
                AfterDisconnect += bind.AfterDisconnectEvent;
                SucceedDisconnect += bind.SucceedDisconnectEvent;
                FailedDisconnect += bind.FailedDisconnectEvent;
            }

            if (this is ILoginEvent)
            {
                ILoginEvent bind = (ILoginEvent)this;
                BeforeLogin += bind.BeforeLoginEvent;
                AfterLogin += bind.AfterLoginEvent;
                SucceedLogin += bind.SucceedLoginEvent;
                FailedLogin += bind.FailedLoginEvent;
            }

            if (this is ILogoutEvent)
            {
                ILogoutEvent bind = (ILogoutEvent)this;
                BeforeLogout += bind.BeforeLogoutEvent;
                AfterLogout += bind.AfterLogoutEvent;
                SucceedLogout += bind.SucceedLogoutEvent;
                FailedLogout += bind.FailedLogoutEvent;
            }

            if (this is IRegEvent)
            {
                IRegEvent bind = (IRegEvent)this;
                BeforeReg += bind.BeforeRegEvent;
                AfterReg += bind.AfterRegEvent;
                SucceedReg += bind.SucceedRegEvent;
                FailedReg += bind.FailedRegEvent;
            }

            if (this is IIntoRoomEvent)
            {
                IIntoRoomEvent bind = (IIntoRoomEvent)this;
                BeforeIntoRoom += bind.BeforeIntoRoomEvent;
                AfterIntoRoom += bind.AfterIntoRoomEvent;
                SucceedIntoRoom += bind.SucceedIntoRoomEvent;
                FailedIntoRoom += bind.FailedIntoRoomEvent;
            }

            if (this is ISendTalkEvent)
            {
                ISendTalkEvent bind = (ISendTalkEvent)this;
                BeforeSendTalk += bind.BeforeSendTalkEvent;
                AfterSendTalk += bind.AfterSendTalkEvent;
                SucceedSendTalk += bind.SucceedSendTalkEvent;
                FailedSendTalk += bind.FailedSendTalkEvent;
            }

            if (this is ICreateRoomEvent)
            {
                ICreateRoomEvent bind = (ICreateRoomEvent)this;
                BeforeCreateRoom += bind.BeforeCreateRoomEvent;
                AfterCreateRoom += bind.AfterCreateRoomEvent;
                SucceedCreateRoom += bind.SucceedCreateRoomEvent;
                FailedCreateRoom += bind.FailedCreateRoomEvent;
            }

            if (this is IQuitRoomEvent)
            {
                IQuitRoomEvent bind = (IQuitRoomEvent)this;
                BeforeQuitRoom += bind.BeforeQuitRoomEvent;
                AfterQuitRoom += bind.AfterQuitRoomEvent;
                SucceedQuitRoom += bind.SucceedQuitRoomEvent;
                FailedQuitRoom += bind.FailedQuitRoomEvent;
            }

            if (this is IChangeRoomSettingEvent)
            {
                IChangeRoomSettingEvent bind = (IChangeRoomSettingEvent)this;
                BeforeChangeRoomSetting += bind.BeforeChangeRoomSettingEvent;
                AfterChangeRoomSetting += bind.AfterChangeRoomSettingEvent;
                SucceedChangeRoomSetting += bind.SucceedChangeRoomSettingEvent;
                FailedChangeRoomSetting += bind.FailedChangeRoomSettingEvent;
            }

            if (this is IStartMatchEvent)
            {
                IStartMatchEvent bind = (IStartMatchEvent)this;
                BeforeStartMatch += bind.BeforeStartMatchEvent;
                AfterStartMatch += bind.AfterStartMatchEvent;
                SucceedStartMatch += bind.SucceedStartMatchEvent;
                FailedStartMatch += bind.FailedStartMatchEvent;
            }

            if (this is IStartGameEvent)
            {
                IStartGameEvent bind = (IStartGameEvent)this;
                BeforeStartGame += bind.BeforeStartGameEvent;
                AfterStartGame += bind.AfterStartGameEvent;
                SucceedStartGame += bind.SucceedStartGameEvent;
                FailedStartGame += bind.FailedStartGameEvent;
            }

            if (this is IChangeProfileEvent)
            {
                IChangeProfileEvent bind = (IChangeProfileEvent)this;
                BeforeChangeProfile += bind.BeforeChangeProfileEvent;
                AfterChangeProfile += bind.AfterChangeProfileEvent;
                SucceedChangeProfile += bind.SucceedChangeProfileEvent;
                FailedChangeProfile += bind.FailedChangeProfileEvent;
            }

            if (this is IChangeAccountSettingEvent)
            {
                IChangeAccountSettingEvent bind = (IChangeAccountSettingEvent)this;
                BeforeChangeAccountSetting += bind.BeforeChangeAccountSettingEvent;
                AfterChangeAccountSetting += bind.AfterChangeAccountSettingEvent;
                SucceedChangeAccountSetting += bind.SucceedChangeAccountSettingEvent;
                FailedChangeAccountSetting += bind.FailedChangeAccountSettingEvent;
            }

            if (this is IOpenInventoryEvent)
            {
                IOpenInventoryEvent bind = (IOpenInventoryEvent)this;
                BeforeOpenInventory += bind.BeforeOpenInventoryEvent;
                AfterOpenInventory += bind.AfterOpenInventoryEvent;
                SucceedOpenInventory += bind.SucceedOpenInventoryEvent;
                FailedOpenInventory += bind.FailedOpenInventoryEvent;
            }

            if (this is ISignInEvent)
            {
                ISignInEvent bind = (ISignInEvent)this;
                BeforeSignIn += bind.BeforeSignInEvent;
                AfterSignIn += bind.AfterSignInEvent;
                SucceedSignIn += bind.SucceedSignInEvent;
                FailedSignIn += bind.FailedSignInEvent;
            }

            if (this is IOpenStoreEvent)
            {
                IOpenStoreEvent bind = (IOpenStoreEvent)this;
                BeforeOpenStore += bind.BeforeOpenStoreEvent;
                AfterOpenStore += bind.AfterOpenStoreEvent;
                SucceedOpenStore += bind.SucceedOpenStoreEvent;
                FailedOpenStore += bind.FailedOpenStoreEvent;
            }

            if (this is IBuyItemEvent)
            {
                IBuyItemEvent bind = (IBuyItemEvent)this;
                BeforeBuyItem += bind.BeforeBuyItemEvent;
                AfterBuyItem += bind.AfterBuyItemEvent;
                SucceedBuyItem += bind.SucceedBuyItemEvent;
                FailedBuyItem += bind.FailedBuyItemEvent;
            }

            if (this is IShowRankingEvent)
            {
                IShowRankingEvent bind = (IShowRankingEvent)this;
                BeforeShowRanking += bind.BeforeShowRankingEvent;
                AfterShowRanking += bind.AfterShowRankingEvent;
                SucceedShowRanking += bind.SucceedShowRankingEvent;
                FailedShowRanking += bind.FailedShowRankingEvent;
            }

            if (this is IUseItemEvent)
            {
                IUseItemEvent bind = (IUseItemEvent)this;
                BeforeUseItem += bind.BeforeUseItemEvent;
                AfterUseItem += bind.AfterUseItemEvent;
                SucceedUseItem += bind.SucceedUseItemEvent;
                FailedUseItem += bind.FailedUseItemEvent;
            }

            if (this is IEndGameEvent)
            {
                IEndGameEvent bind = (IEndGameEvent)this;
                BeforeEndGame += bind.BeforeEndGameEvent;
                AfterEndGame += bind.AfterEndGameEvent;
                SucceedEndGame += bind.SucceedEndGameEvent;
                FailedEndGame += bind.FailedEndGameEvent;
            }
        }

        public event IConnectEventHandler.BeforeEventHandler? BeforeConnect;
        public event IConnectEventHandler.AfterEventHandler? AfterConnect;
        public event IConnectEventHandler.SucceedEventHandler? SucceedConnect;
        public event IConnectEventHandler.FailedEventHandler? FailedConnect;
        public event IEventHandler.BeforeEventHandler? BeforeDisconnect;
        public event IEventHandler.AfterEventHandler? AfterDisconnect;
        public event IEventHandler.SucceedEventHandler? SucceedDisconnect;
        public event IEventHandler.FailedEventHandler? FailedDisconnect;
        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;
        public event ILoginEventHandler.SucceedEventHandler? SucceedLogin;
        public event ILoginEventHandler.FailedEventHandler? FailedLogin;
        public event IEventHandler.BeforeEventHandler? BeforeLogout;
        public event IEventHandler.AfterEventHandler? AfterLogout;
        public event IEventHandler.SucceedEventHandler? SucceedLogout;
        public event IEventHandler.FailedEventHandler? FailedLogout;
        public event IRegEventHandler.BeforeEventHandler? BeforeReg;
        public event IRegEventHandler.AfterEventHandler? AfterReg;
        public event IRegEventHandler.SucceedEventHandler? SucceedReg;
        public event IRegEventHandler.FailedEventHandler? FailedReg;
        public event IIntoRoomEventHandler.BeforeEventHandler? BeforeIntoRoom;
        public event IIntoRoomEventHandler.AfterEventHandler? AfterIntoRoom;
        public event IIntoRoomEventHandler.SucceedEventHandler? SucceedIntoRoom;
        public event IIntoRoomEventHandler.FailedEventHandler? FailedIntoRoom;
        public event ISendTalkEventHandler.BeforeEventHandler? BeforeSendTalk;
        public event ISendTalkEventHandler.AfterEventHandler? AfterSendTalk;
        public event ISendTalkEventHandler.SucceedEventHandler? SucceedSendTalk;
        public event ISendTalkEventHandler.FailedEventHandler? FailedSendTalk;
        public event ICreateRoomEventHandler.BeforeEventHandler? BeforeCreateRoom;
        public event ICreateRoomEventHandler.AfterEventHandler? AfterCreateRoom;
        public event ICreateRoomEventHandler.SucceedEventHandler? SucceedCreateRoom;
        public event ICreateRoomEventHandler.FailedEventHandler? FailedCreateRoom;
        public event IQuitRoomEventHandler.BeforeEventHandler? BeforeQuitRoom;
        public event IQuitRoomEventHandler.AfterEventHandler? AfterQuitRoom;
        public event IQuitRoomEventHandler.SucceedEventHandler? SucceedQuitRoom;
        public event IQuitRoomEventHandler.FailedEventHandler? FailedQuitRoom;
        public event IEventHandler.BeforeEventHandler? BeforeChangeRoomSetting;
        public event IEventHandler.AfterEventHandler? AfterChangeRoomSetting;
        public event IEventHandler.SucceedEventHandler? SucceedChangeRoomSetting;
        public event IEventHandler.FailedEventHandler? FailedChangeRoomSetting;
        public event IEventHandler.BeforeEventHandler? BeforeStartMatch;
        public event IEventHandler.AfterEventHandler? AfterStartMatch;
        public event IEventHandler.SucceedEventHandler? SucceedStartMatch;
        public event IEventHandler.FailedEventHandler? FailedStartMatch;
        public event IEventHandler.BeforeEventHandler? BeforeStartGame;
        public event IEventHandler.AfterEventHandler? AfterStartGame;
        public event IEventHandler.SucceedEventHandler? SucceedStartGame;
        public event IEventHandler.FailedEventHandler? FailedStartGame;
        public event IEventHandler.BeforeEventHandler? BeforeChangeProfile;
        public event IEventHandler.AfterEventHandler? AfterChangeProfile;
        public event IEventHandler.SucceedEventHandler? SucceedChangeProfile;
        public event IEventHandler.FailedEventHandler? FailedChangeProfile;
        public event IEventHandler.BeforeEventHandler? BeforeChangeAccountSetting;
        public event IEventHandler.AfterEventHandler? AfterChangeAccountSetting;
        public event IEventHandler.SucceedEventHandler? SucceedChangeAccountSetting;
        public event IEventHandler.FailedEventHandler? FailedChangeAccountSetting;
        public event IEventHandler.BeforeEventHandler? BeforeOpenInventory;
        public event IEventHandler.AfterEventHandler? AfterOpenInventory;
        public event IEventHandler.SucceedEventHandler? SucceedOpenInventory;
        public event IEventHandler.FailedEventHandler? FailedOpenInventory;
        public event IEventHandler.BeforeEventHandler? BeforeSignIn;
        public event IEventHandler.AfterEventHandler? AfterSignIn;
        public event IEventHandler.SucceedEventHandler? SucceedSignIn;
        public event IEventHandler.FailedEventHandler? FailedSignIn;
        public event IEventHandler.BeforeEventHandler? BeforeOpenStore;
        public event IEventHandler.AfterEventHandler? AfterOpenStore;
        public event IEventHandler.SucceedEventHandler? SucceedOpenStore;
        public event IEventHandler.FailedEventHandler? FailedOpenStore;
        public event IEventHandler.BeforeEventHandler? BeforeBuyItem;
        public event IEventHandler.AfterEventHandler? AfterBuyItem;
        public event IEventHandler.SucceedEventHandler? SucceedBuyItem;
        public event IEventHandler.FailedEventHandler? FailedBuyItem;
        public event IEventHandler.BeforeEventHandler? BeforeShowRanking;
        public event IEventHandler.AfterEventHandler? AfterShowRanking;
        public event IEventHandler.SucceedEventHandler? SucceedShowRanking;
        public event IEventHandler.FailedEventHandler? FailedShowRanking;
        public event IEventHandler.BeforeEventHandler? BeforeUseItem;
        public event IEventHandler.AfterEventHandler? AfterUseItem;
        public event IEventHandler.SucceedEventHandler? SucceedUseItem;
        public event IEventHandler.FailedEventHandler? FailedUseItem;
        public event IEventHandler.BeforeEventHandler? BeforeEndGame;
        public event IEventHandler.AfterEventHandler? AfterEndGame;
        public event IEventHandler.SucceedEventHandler? SucceedEndGame;
        public event IEventHandler.FailedEventHandler? FailedEndGame;

        public void OnBeforeConnectEvent(ConnectEventArgs e)
        {
            BeforeConnect?.Invoke(this, e);
        }

        public void OnAfterConnectEvent(ConnectEventArgs e)
        {
            AfterConnect?.Invoke(this, e);
        }

        public void OnSucceedConnectEvent(ConnectEventArgs e)
        {
            SucceedConnect?.Invoke(this, e);
        }

        public void OnFailedConnectEvent(ConnectEventArgs e)
        {
            FailedConnect?.Invoke(this, e);
        }

        public void OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            BeforeDisconnect?.Invoke(this, e);
        }

        public void OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            AfterDisconnect?.Invoke(this, e);
        }

        public void OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            SucceedDisconnect?.Invoke(this, e);
        }

        public void OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            FailedDisconnect?.Invoke(this, e);
        }

        public void OnBeforeLoginEvent(LoginEventArgs e)
        {
            BeforeLogin?.Invoke(this, e);
        }

        public void OnAfterLoginEvent(LoginEventArgs e)
        {
            AfterLogin?.Invoke(this, e);
        }

        public void OnSucceedLoginEvent(LoginEventArgs e)
        {
            SucceedLogin?.Invoke(this, e);
        }

        public void OnFailedLoginEvent(LoginEventArgs e)
        {
            FailedLogin?.Invoke(this, e);
        }

        public void OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            BeforeLogout?.Invoke(this, e);
        }

        public void OnAfterLogoutEvent(GeneralEventArgs e)
        {
            AfterLogout?.Invoke(this, e);
        }

        public void OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            SucceedLogout?.Invoke(this, e);
        }

        public void OnFailedLogoutEvent(GeneralEventArgs e)
        {
            FailedLogout?.Invoke(this, e);
        }

        public void OnBeforeRegEvent(RegisterEventArgs e)
        {
            BeforeReg?.Invoke(this, e);
        }

        public void OnAfterRegEvent(RegisterEventArgs e)
        {
            AfterReg?.Invoke(this, e);
        }

        public void OnSucceedRegEvent(RegisterEventArgs e)
        {
            SucceedReg?.Invoke(this, e);
        }

        public void OnFailedRegEvent(RegisterEventArgs e)
        {
            FailedReg?.Invoke(this, e);
        }

        public void OnBeforeIntoRoomEvent(RoomEventArgs e)
        {
            BeforeIntoRoom?.Invoke(this, e);
        }

        public void OnAfterIntoRoomEvent(RoomEventArgs e)
        {
            AfterIntoRoom?.Invoke(this, e);
        }

        public void OnSucceedIntoRoomEvent(RoomEventArgs e)
        {
            SucceedIntoRoom?.Invoke(this, e);
        }

        public void OnFailedIntoRoomEvent(RoomEventArgs e)
        {
            FailedIntoRoom?.Invoke(this, e);
        }

        public void OnBeforeSendTalkEvent(SendTalkEventArgs e)
        {
            BeforeSendTalk?.Invoke(this, e);
        }

        public void OnAfterSendTalkEvent(SendTalkEventArgs e)
        {
            AfterSendTalk?.Invoke(this, e);
        }

        public void OnSucceedSendTalkEvent(SendTalkEventArgs e)
        {
            SucceedSendTalk?.Invoke(this, e);
        }

        public void OnFailedSendTalkEvent(SendTalkEventArgs e)
        {
            FailedSendTalk?.Invoke(this, e);
        }

        public void OnBeforeCreateRoomEvent(RoomEventArgs e)
        {
            BeforeCreateRoom?.Invoke(this, e);
        }

        public void OnAfterCreateRoomEvent(RoomEventArgs e)
        {
            AfterCreateRoom?.Invoke(this, e);
        }

        public void OnSucceedCreateRoomEvent(RoomEventArgs e)
        {
            SucceedCreateRoom?.Invoke(this, e);
        }

        public void OnFailedCreateRoomEvent(RoomEventArgs e)
        {
            FailedCreateRoom?.Invoke(this, e);
        }

        public void OnBeforeQuitRoomEvent(RoomEventArgs e)
        {
            BeforeQuitRoom?.Invoke(this, e);
        }

        public void OnAfterQuitRoomEvent(RoomEventArgs e)
        {
            AfterQuitRoom?.Invoke(this, e);
        }

        public void OnSucceedQuitRoomEvent(RoomEventArgs e)
        {
            SucceedQuitRoom?.Invoke(this, e);
        }

        public void OnFailedQuitRoomEvent(RoomEventArgs e)
        {
            FailedQuitRoom?.Invoke(this, e);
        }

        public void OnBeforeChangeRoomSettingEvent(GeneralEventArgs e)
        {
            BeforeChangeRoomSetting?.Invoke(this, e);
        }

        public void OnAfterChangeRoomSettingEvent(GeneralEventArgs e)
        {
            AfterChangeRoomSetting?.Invoke(this, e);
        }

        public void OnSucceedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            SucceedChangeRoomSetting?.Invoke(this, e);
        }

        public void OnFailedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            FailedChangeRoomSetting?.Invoke(this, e);
        }

        public void OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            BeforeStartMatch?.Invoke(this, e);
        }

        public void OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            AfterStartMatch?.Invoke(this, e);
        }

        public void OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            SucceedStartMatch?.Invoke(this, e);
        }

        public void OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            FailedStartMatch?.Invoke(this, e);
        }

        public void OnBeforeStartGameEvent(GeneralEventArgs e)
        {
            BeforeStartGame?.Invoke(this, e);
        }

        public void OnAfterStartGameEvent(GeneralEventArgs e)
        {
            AfterStartGame?.Invoke(this, e);
        }

        public void OnSucceedStartGameEvent(GeneralEventArgs e)
        {
            SucceedStartGame?.Invoke(this, e);
        }

        public void OnFailedStartGameEvent(GeneralEventArgs e)
        {
            FailedStartGame?.Invoke(this, e);
        }

        public void OnBeforeChangeProfileEvent(GeneralEventArgs e)
        {
            BeforeChangeProfile?.Invoke(this, e);
        }

        public void OnAfterChangeProfileEvent(GeneralEventArgs e)
        {
            AfterChangeProfile?.Invoke(this, e);
        }

        public void OnSucceedChangeProfileEvent(GeneralEventArgs e)
        {
            SucceedChangeProfile?.Invoke(this, e);
        }

        public void OnFailedChangeProfileEvent(GeneralEventArgs e)
        {
            FailedChangeProfile?.Invoke(this, e);
        }

        public void OnBeforeChangeAccountSettingEvent(GeneralEventArgs e)
        {
            BeforeChangeAccountSetting?.Invoke(this, e);
        }

        public void OnAfterChangeAccountSettingEvent(GeneralEventArgs e)
        {
            AfterChangeAccountSetting?.Invoke(this, e);
        }

        public void OnSucceedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            SucceedChangeAccountSetting?.Invoke(this, e);
        }

        public void OnFailedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            FailedChangeAccountSetting?.Invoke(this, e);
        }

        public void OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            BeforeOpenInventory?.Invoke(this, e);
        }

        public void OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            AfterOpenInventory?.Invoke(this, e);
        }

        public void OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            SucceedOpenInventory?.Invoke(this, e);
        }

        public void OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            FailedOpenInventory?.Invoke(this, e);
        }

        public void OnBeforeSignInEvent(GeneralEventArgs e)
        {
            BeforeSignIn?.Invoke(this, e);
        }

        public void OnAfterSignInEvent(GeneralEventArgs e)
        {
            AfterSignIn?.Invoke(this, e);
        }

        public void OnSucceedSignInEvent(GeneralEventArgs e)
        {
            SucceedSignIn?.Invoke(this, e);
        }

        public void OnFailedSignInEvent(GeneralEventArgs e)
        {
            FailedSignIn?.Invoke(this, e);
        }

        public void OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            BeforeOpenStore?.Invoke(this, e);
        }

        public void OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            AfterOpenStore?.Invoke(this, e);
        }

        public void OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            SucceedOpenStore?.Invoke(this, e);
        }

        public void OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            FailedOpenStore?.Invoke(this, e);
        }

        public void OnBeforeBuyItemEvent(GeneralEventArgs e)
        {
            BeforeBuyItem?.Invoke(this, e);
        }

        public void OnAfterBuyItemEvent(GeneralEventArgs e)
        {
            AfterBuyItem?.Invoke(this, e);
        }

        public void OnSucceedBuyItemEvent(GeneralEventArgs e)
        {
            SucceedBuyItem?.Invoke(this, e);
        }

        public void OnFailedBuyItemEvent(GeneralEventArgs e)
        {
            FailedBuyItem?.Invoke(this, e);
        }

        public void OnBeforeShowRankingEvent(GeneralEventArgs e)
        {
            BeforeShowRanking?.Invoke(this, e);
        }

        public void OnAfterShowRankingEvent(GeneralEventArgs e)
        {
            AfterShowRanking?.Invoke(this, e);
        }

        public void OnSucceedShowRankingEvent(GeneralEventArgs e)
        {
            SucceedShowRanking?.Invoke(this, e);
        }

        public void OnFailedShowRankingEvent(GeneralEventArgs e)
        {
            FailedShowRanking?.Invoke(this, e);
        }

        public void OnBeforeUseItemEvent(GeneralEventArgs e)
        {
            BeforeUseItem?.Invoke(this, e);
        }

        public void OnAfterUseItemEvent(GeneralEventArgs e)
        {
            AfterUseItem?.Invoke(this, e);
        }

        public void OnSucceedUseItemEvent(GeneralEventArgs e)
        {
            SucceedUseItem?.Invoke(this, e);
        }

        public void OnFailedUseItemEvent(GeneralEventArgs e)
        {
            FailedUseItem?.Invoke(this, e);
        }

        public void OnBeforeEndGameEvent(GeneralEventArgs e)
        {
            BeforeEndGame?.Invoke(this, e);
        }

        public void OnAfterEndGameEvent(GeneralEventArgs e)
        {
            AfterEndGame?.Invoke(this, e);
        }

        public void OnSucceedEndGameEvent(GeneralEventArgs e)
        {
            SucceedEndGame?.Invoke(this, e);
        }

        public void OnFailedEndGameEvent(GeneralEventArgs e)
        {
            FailedEndGame?.Invoke(this, e);
        }
    }
}
