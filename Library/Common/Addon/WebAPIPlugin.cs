using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Common.Event;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class WebAPIPlugin : IAddon, IAddonController<IAddon>
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
        /// 包含了一些常用方法的控制器
        /// </summary>
        public ServerAddonController<IAddon> Controller
        {
            get => _Controller ?? throw new NotImplementedException();
            internal set => _Controller = value;
        }

        /// <summary>
        /// base控制器
        /// </summary>
        BaseAddonController<IAddon> IAddonController<IAddon>.Controller
        {
            get => Controller;
            set => _Controller = (ServerAddonController<IAddon>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private ServerAddonController<IAddon>? _Controller;

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool _isLoaded = false;

        /// <summary>
        /// 加载插件
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (_isLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此插件
            if (BeforeLoad(objs))
            {
                // 插件加载后，不允许再次加载此插件
                _isLoaded = true;
                // 触发绑定事件
                BindEvent();
            }
            return _isLoaded;
        }

        /// <summary>
        /// 接收服务器控制台的输入
        /// </summary>
        /// <param name="input"></param>
        public abstract void ProcessInput(string input);

        /// <summary>
        /// 插件完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(WebAPIPluginLoader loader, params object[] objs)
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此插件
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad(params object[] objs)
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

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e)
        {
            BeforeConnect?.Invoke(sender, e);
        }

        public void OnAfterConnectEvent(object sender, ConnectEventArgs e)
        {
            AfterConnect?.Invoke(sender, e);
        }

        public void OnSucceedConnectEvent(object sender, ConnectEventArgs e)
        {
            SucceedConnect?.Invoke(sender, e);
        }

        public void OnFailedConnectEvent(object sender, ConnectEventArgs e)
        {
            FailedConnect?.Invoke(sender, e);
        }

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            BeforeDisconnect?.Invoke(sender, e);
        }

        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            AfterDisconnect?.Invoke(sender, e);
        }

        public void OnSucceedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            SucceedDisconnect?.Invoke(sender, e);
        }

        public void OnFailedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            FailedDisconnect?.Invoke(sender, e);
        }

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e)
        {
            BeforeLogin?.Invoke(sender, e);
        }

        public void OnAfterLoginEvent(object sender, LoginEventArgs e)
        {
            AfterLogin?.Invoke(sender, e);
        }

        public void OnSucceedLoginEvent(object sender, LoginEventArgs e)
        {
            SucceedLogin?.Invoke(sender, e);
        }

        public void OnFailedLoginEvent(object sender, LoginEventArgs e)
        {
            FailedLogin?.Invoke(sender, e);
        }

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            BeforeLogout?.Invoke(sender, e);
        }

        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            AfterLogout?.Invoke(sender, e);
        }

        public void OnSucceedLogoutEvent(object sender, GeneralEventArgs e)
        {
            SucceedLogout?.Invoke(sender, e);
        }

        public void OnFailedLogoutEvent(object sender, GeneralEventArgs e)
        {
            FailedLogout?.Invoke(sender, e);
        }

        public void OnBeforeRegEvent(object sender, RegisterEventArgs e)
        {
            BeforeReg?.Invoke(sender, e);
        }

        public void OnAfterRegEvent(object sender, RegisterEventArgs e)
        {
            AfterReg?.Invoke(sender, e);
        }

        public void OnSucceedRegEvent(object sender, RegisterEventArgs e)
        {
            SucceedReg?.Invoke(sender, e);
        }

        public void OnFailedRegEvent(object sender, RegisterEventArgs e)
        {
            FailedReg?.Invoke(sender, e);
        }

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeIntoRoom?.Invoke(sender, e);
        }

        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e)
        {
            AfterIntoRoom?.Invoke(sender, e);
        }

        public void OnSucceedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            SucceedIntoRoom?.Invoke(sender, e);
        }

        public void OnFailedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            FailedIntoRoom?.Invoke(sender, e);
        }

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            BeforeSendTalk?.Invoke(sender, e);
        }

        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            AfterSendTalk?.Invoke(sender, e);
        }

        public void OnSucceedSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            SucceedSendTalk?.Invoke(sender, e);
        }

        public void OnFailedSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            FailedSendTalk?.Invoke(sender, e);
        }

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeCreateRoom?.Invoke(sender, e);
        }

        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e)
        {
            AfterCreateRoom?.Invoke(sender, e);
        }

        public void OnSucceedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            SucceedCreateRoom?.Invoke(sender, e);
        }

        public void OnFailedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            FailedCreateRoom?.Invoke(sender, e);
        }

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeQuitRoom?.Invoke(sender, e);
        }

        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e)
        {
            AfterQuitRoom?.Invoke(sender, e);
        }

        public void OnSucceedQuitRoomEvent(object sender, RoomEventArgs e)
        {
            SucceedQuitRoom?.Invoke(sender, e);
        }

        public void OnFailedQuitRoomEvent(object sender, RoomEventArgs e)
        {
            FailedQuitRoom?.Invoke(sender, e);
        }

        public void OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            BeforeChangeRoomSetting?.Invoke(sender, e);
        }

        public void OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            AfterChangeRoomSetting?.Invoke(sender, e);
        }

        public void OnSucceedChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            SucceedChangeRoomSetting?.Invoke(sender, e);
        }

        public void OnFailedChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            FailedChangeRoomSetting?.Invoke(sender, e);
        }

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartMatch?.Invoke(sender, e);
        }

        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e)
        {
            AfterStartMatch?.Invoke(sender, e);
        }

        public void OnSucceedStartMatchEvent(object sender, GeneralEventArgs e)
        {
            SucceedStartMatch?.Invoke(sender, e);
        }

        public void OnFailedStartMatchEvent(object sender, GeneralEventArgs e)
        {
            FailedStartMatch?.Invoke(sender, e);
        }

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartGame?.Invoke(sender, e);
        }

        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e)
        {
            AfterStartGame?.Invoke(sender, e);
        }

        public void OnSucceedStartGameEvent(object sender, GeneralEventArgs e)
        {
            SucceedStartGame?.Invoke(sender, e);
        }

        public void OnFailedStartGameEvent(object sender, GeneralEventArgs e)
        {
            FailedStartGame?.Invoke(sender, e);
        }

        public void OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            BeforeChangeProfile?.Invoke(sender, e);
        }

        public void OnAfterChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            AfterChangeProfile?.Invoke(sender, e);
        }

        public void OnSucceedChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            SucceedChangeProfile?.Invoke(sender, e);
        }

        public void OnFailedChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            FailedChangeProfile?.Invoke(sender, e);
        }

        public void OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            BeforeChangeAccountSetting?.Invoke(sender, e);
        }

        public void OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            AfterChangeAccountSetting?.Invoke(sender, e);
        }

        public void OnSucceedChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            SucceedChangeAccountSetting?.Invoke(sender, e);
        }

        public void OnFailedChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            FailedChangeAccountSetting?.Invoke(sender, e);
        }

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenInventory?.Invoke(sender, e);
        }

        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenInventory?.Invoke(sender, e);
        }

        public void OnSucceedOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            SucceedOpenInventory?.Invoke(sender, e);
        }

        public void OnFailedOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            FailedOpenInventory?.Invoke(sender, e);
        }

        public void OnBeforeSignInEvent(object sender, GeneralEventArgs e)
        {
            BeforeSignIn?.Invoke(sender, e);
        }

        public void OnAfterSignInEvent(object sender, GeneralEventArgs e)
        {
            AfterSignIn?.Invoke(sender, e);
        }

        public void OnSucceedSignInEvent(object sender, GeneralEventArgs e)
        {
            SucceedSignIn?.Invoke(sender, e);
        }

        public void OnFailedSignInEvent(object sender, GeneralEventArgs e)
        {
            FailedSignIn?.Invoke(sender, e);
        }

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenStore?.Invoke(sender, e);
        }

        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenStore?.Invoke(sender, e);
        }

        public void OnSucceedOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            SucceedOpenStore?.Invoke(sender, e);
        }

        public void OnFailedOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            FailedOpenStore?.Invoke(sender, e);
        }

        public void OnBeforeBuyItemEvent(object sender, GeneralEventArgs e)
        {
            BeforeBuyItem?.Invoke(sender, e);
        }

        public void OnAfterBuyItemEvent(object sender, GeneralEventArgs e)
        {
            AfterBuyItem?.Invoke(sender, e);
        }

        public void OnSucceedBuyItemEvent(object sender, GeneralEventArgs e)
        {
            SucceedBuyItem?.Invoke(sender, e);
        }

        public void OnFailedBuyItemEvent(object sender, GeneralEventArgs e)
        {
            FailedBuyItem?.Invoke(sender, e);
        }

        public void OnBeforeShowRankingEvent(object sender, GeneralEventArgs e)
        {
            BeforeShowRanking?.Invoke(sender, e);
        }

        public void OnAfterShowRankingEvent(object sender, GeneralEventArgs e)
        {
            AfterShowRanking?.Invoke(sender, e);
        }

        public void OnSucceedShowRankingEvent(object sender, GeneralEventArgs e)
        {
            SucceedShowRanking?.Invoke(sender, e);
        }

        public void OnFailedShowRankingEvent(object sender, GeneralEventArgs e)
        {
            FailedShowRanking?.Invoke(sender, e);
        }

        public void OnBeforeUseItemEvent(object sender, GeneralEventArgs e)
        {
            BeforeUseItem?.Invoke(sender, e);
        }

        public void OnAfterUseItemEvent(object sender, GeneralEventArgs e)
        {
            AfterUseItem?.Invoke(sender, e);
        }

        public void OnSucceedUseItemEvent(object sender, GeneralEventArgs e)
        {
            SucceedUseItem?.Invoke(sender, e);
        }

        public void OnFailedUseItemEvent(object sender, GeneralEventArgs e)
        {
            FailedUseItem?.Invoke(sender, e);
        }

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeEndGame?.Invoke(sender, e);
        }

        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e)
        {
            AfterEndGame?.Invoke(sender, e);
        }

        public void OnSucceedEndGameEvent(object sender, GeneralEventArgs e)
        {
            SucceedEndGame?.Invoke(sender, e);
        }

        public void OnFailedEndGameEvent(object sender, GeneralEventArgs e)
        {
            FailedEndGame?.Invoke(sender, e);
        }
    }
}
