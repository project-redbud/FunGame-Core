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
            get => _controller ?? throw new NotImplementedException();
            internal set => _controller = value;
        }

        /// <summary>
        /// base控制器
        /// </summary>
        BaseAddonController<IAddon> IAddonController<IAddon>.Controller
        {
            get => Controller;
            set => _controller = (ServerAddonController<IAddon>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private ServerAddonController<IAddon>? _controller;

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
        /// 当 Web API 服务启动完成后触发
        /// </summary>
        /// <returns></returns>
        public virtual void OnWebAPIStarted(params object[] objs)
        {

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
            }

            if (this is IDisconnectEvent)
            {
                IDisconnectEvent bind = (IDisconnectEvent)this;
                BeforeDisconnect += bind.BeforeDisconnectEvent;
                AfterDisconnect += bind.AfterDisconnectEvent;
            }

            if (this is ILoginEvent)
            {
                ILoginEvent bind = (ILoginEvent)this;
                BeforeLogin += bind.BeforeLoginEvent;
                AfterLogin += bind.AfterLoginEvent;
            }

            if (this is ILogoutEvent)
            {
                ILogoutEvent bind = (ILogoutEvent)this;
                BeforeLogout += bind.BeforeLogoutEvent;
                AfterLogout += bind.AfterLogoutEvent;
            }

            if (this is IRegEvent)
            {
                IRegEvent bind = (IRegEvent)this;
                BeforeReg += bind.BeforeRegEvent;
                AfterReg += bind.AfterRegEvent;
            }

            if (this is IIntoRoomEvent)
            {
                IIntoRoomEvent bind = (IIntoRoomEvent)this;
                BeforeIntoRoom += bind.BeforeIntoRoomEvent;
                AfterIntoRoom += bind.AfterIntoRoomEvent;
            }

            if (this is ISendTalkEvent)
            {
                ISendTalkEvent bind = (ISendTalkEvent)this;
                BeforeSendTalk += bind.BeforeSendTalkEvent;
                AfterSendTalk += bind.AfterSendTalkEvent;
            }

            if (this is ICreateRoomEvent)
            {
                ICreateRoomEvent bind = (ICreateRoomEvent)this;
                BeforeCreateRoom += bind.BeforeCreateRoomEvent;
                AfterCreateRoom += bind.AfterCreateRoomEvent;
            }

            if (this is IQuitRoomEvent)
            {
                IQuitRoomEvent bind = (IQuitRoomEvent)this;
                BeforeQuitRoom += bind.BeforeQuitRoomEvent;
                AfterQuitRoom += bind.AfterQuitRoomEvent;
            }

            if (this is IChangeRoomSettingEvent)
            {
                IChangeRoomSettingEvent bind = (IChangeRoomSettingEvent)this;
                BeforeChangeRoomSetting += bind.BeforeChangeRoomSettingEvent;
                AfterChangeRoomSetting += bind.AfterChangeRoomSettingEvent;
            }

            if (this is IStartMatchEvent)
            {
                IStartMatchEvent bind = (IStartMatchEvent)this;
                BeforeStartMatch += bind.BeforeStartMatchEvent;
                AfterStartMatch += bind.AfterStartMatchEvent;
            }

            if (this is IStartGameEvent)
            {
                IStartGameEvent bind = (IStartGameEvent)this;
                BeforeStartGame += bind.BeforeStartGameEvent;
                AfterStartGame += bind.AfterStartGameEvent;
            }

            if (this is IChangeProfileEvent)
            {
                IChangeProfileEvent bind = (IChangeProfileEvent)this;
                BeforeChangeProfile += bind.BeforeChangeProfileEvent;
                AfterChangeProfile += bind.AfterChangeProfileEvent;
            }

            if (this is IChangeAccountSettingEvent)
            {
                IChangeAccountSettingEvent bind = (IChangeAccountSettingEvent)this;
                BeforeChangeAccountSetting += bind.BeforeChangeAccountSettingEvent;
                AfterChangeAccountSetting += bind.AfterChangeAccountSettingEvent;
            }

            if (this is IOpenInventoryEvent)
            {
                IOpenInventoryEvent bind = (IOpenInventoryEvent)this;
                BeforeOpenInventory += bind.BeforeOpenInventoryEvent;
                AfterOpenInventory += bind.AfterOpenInventoryEvent;
            }

            if (this is ISignInEvent)
            {
                ISignInEvent bind = (ISignInEvent)this;
                BeforeSignIn += bind.BeforeSignInEvent;
                AfterSignIn += bind.AfterSignInEvent;
            }

            if (this is IOpenStoreEvent)
            {
                IOpenStoreEvent bind = (IOpenStoreEvent)this;
                BeforeOpenStore += bind.BeforeOpenStoreEvent;
                AfterOpenStore += bind.AfterOpenStoreEvent;
            }

            if (this is IBuyItemEvent)
            {
                IBuyItemEvent bind = (IBuyItemEvent)this;
                BeforeBuyItem += bind.BeforeBuyItemEvent;
                AfterBuyItem += bind.AfterBuyItemEvent;
            }

            if (this is IShowRankingEvent)
            {
                IShowRankingEvent bind = (IShowRankingEvent)this;
                BeforeShowRanking += bind.BeforeShowRankingEvent;
                AfterShowRanking += bind.AfterShowRankingEvent;
            }

            if (this is IUseItemEvent)
            {
                IUseItemEvent bind = (IUseItemEvent)this;
                BeforeUseItem += bind.BeforeUseItemEvent;
                AfterUseItem += bind.AfterUseItemEvent;
            }

            if (this is IEndGameEvent)
            {
                IEndGameEvent bind = (IEndGameEvent)this;
                BeforeEndGame += bind.BeforeEndGameEvent;
                AfterEndGame += bind.AfterEndGameEvent;
            }
        }

        public event IConnectEventHandler.BeforeEventHandler? BeforeConnect;
        public event IConnectEventHandler.AfterEventHandler? AfterConnect;
        public event IEventHandler.BeforeEventHandler? BeforeDisconnect;
        public event IEventHandler.AfterEventHandler? AfterDisconnect;
        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;
        public event IEventHandler.BeforeEventHandler? BeforeLogout;
        public event IEventHandler.AfterEventHandler? AfterLogout;
        public event IRegEventHandler.BeforeEventHandler? BeforeReg;
        public event IRegEventHandler.AfterEventHandler? AfterReg;
        public event IIntoRoomEventHandler.BeforeEventHandler? BeforeIntoRoom;
        public event IIntoRoomEventHandler.AfterEventHandler? AfterIntoRoom;
        public event ISendTalkEventHandler.BeforeEventHandler? BeforeSendTalk;
        public event ISendTalkEventHandler.AfterEventHandler? AfterSendTalk;
        public event ICreateRoomEventHandler.BeforeEventHandler? BeforeCreateRoom;
        public event ICreateRoomEventHandler.AfterEventHandler? AfterCreateRoom;
        public event IQuitRoomEventHandler.BeforeEventHandler? BeforeQuitRoom;
        public event IQuitRoomEventHandler.AfterEventHandler? AfterQuitRoom;
        public event IEventHandler.BeforeEventHandler? BeforeChangeRoomSetting;
        public event IEventHandler.AfterEventHandler? AfterChangeRoomSetting;
        public event IEventHandler.BeforeEventHandler? BeforeStartMatch;
        public event IEventHandler.AfterEventHandler? AfterStartMatch;
        public event IEventHandler.BeforeEventHandler? BeforeStartGame;
        public event IEventHandler.AfterEventHandler? AfterStartGame;
        public event IEventHandler.BeforeEventHandler? BeforeChangeProfile;
        public event IEventHandler.AfterEventHandler? AfterChangeProfile;
        public event IEventHandler.BeforeEventHandler? BeforeChangeAccountSetting;
        public event IEventHandler.AfterEventHandler? AfterChangeAccountSetting;
        public event IEventHandler.BeforeEventHandler? BeforeOpenInventory;
        public event IEventHandler.AfterEventHandler? AfterOpenInventory;
        public event IEventHandler.BeforeEventHandler? BeforeSignIn;
        public event IEventHandler.AfterEventHandler? AfterSignIn;
        public event IEventHandler.BeforeEventHandler? BeforeOpenStore;
        public event IEventHandler.AfterEventHandler? AfterOpenStore;
        public event IEventHandler.BeforeEventHandler? BeforeBuyItem;
        public event IEventHandler.AfterEventHandler? AfterBuyItem;
        public event IEventHandler.BeforeEventHandler? BeforeShowRanking;
        public event IEventHandler.AfterEventHandler? AfterShowRanking;
        public event IEventHandler.BeforeEventHandler? BeforeUseItem;
        public event IEventHandler.AfterEventHandler? AfterUseItem;
        public event IEventHandler.BeforeEventHandler? BeforeEndGame;
        public event IEventHandler.AfterEventHandler? AfterEndGame;

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e)
        {
            BeforeConnect?.Invoke(sender, e);
        }

        public void OnAfterConnectEvent(object sender, ConnectEventArgs e)
        {
            AfterConnect?.Invoke(sender, e);
        }

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            BeforeDisconnect?.Invoke(sender, e);
        }

        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            AfterDisconnect?.Invoke(sender, e);
        }

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e)
        {
            BeforeLogin?.Invoke(sender, e);
        }

        public void OnAfterLoginEvent(object sender, LoginEventArgs e)
        {
            AfterLogin?.Invoke(sender, e);
        }

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            BeforeLogout?.Invoke(sender, e);
        }

        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            AfterLogout?.Invoke(sender, e);
        }

        public void OnBeforeRegEvent(object sender, RegisterEventArgs e)
        {
            BeforeReg?.Invoke(sender, e);
        }

        public void OnAfterRegEvent(object sender, RegisterEventArgs e)
        {
            AfterReg?.Invoke(sender, e);
        }

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeIntoRoom?.Invoke(sender, e);
        }

        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e)
        {
            AfterIntoRoom?.Invoke(sender, e);
        }

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            BeforeSendTalk?.Invoke(sender, e);
        }

        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            AfterSendTalk?.Invoke(sender, e);
        }

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeCreateRoom?.Invoke(sender, e);
        }

        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e)
        {
            AfterCreateRoom?.Invoke(sender, e);
        }

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeQuitRoom?.Invoke(sender, e);
        }

        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e)
        {
            AfterQuitRoom?.Invoke(sender, e);
        }

        public void OnBeforeChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            BeforeChangeRoomSetting?.Invoke(sender, e);
        }

        public void OnAfterChangeRoomSettingEvent(object sender, GeneralEventArgs e)
        {
            AfterChangeRoomSetting?.Invoke(sender, e);
        }

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartMatch?.Invoke(sender, e);
        }

        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e)
        {
            AfterStartMatch?.Invoke(sender, e);
        }

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartGame?.Invoke(sender, e);
        }

        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e)
        {
            AfterStartGame?.Invoke(sender, e);
        }

        public void OnBeforeChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            BeforeChangeProfile?.Invoke(sender, e);
        }

        public void OnAfterChangeProfileEvent(object sender, GeneralEventArgs e)
        {
            AfterChangeProfile?.Invoke(sender, e);
        }

        public void OnBeforeChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            BeforeChangeAccountSetting?.Invoke(sender, e);
        }

        public void OnAfterChangeAccountSettingEvent(object sender, GeneralEventArgs e)
        {
            AfterChangeAccountSetting?.Invoke(sender, e);
        }

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenInventory?.Invoke(sender, e);
        }

        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenInventory?.Invoke(sender, e);
        }

        public void OnBeforeSignInEvent(object sender, GeneralEventArgs e)
        {
            BeforeSignIn?.Invoke(sender, e);
        }

        public void OnAfterSignInEvent(object sender, GeneralEventArgs e)
        {
            AfterSignIn?.Invoke(sender, e);
        }

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenStore?.Invoke(sender, e);
        }

        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenStore?.Invoke(sender, e);
        }

        public void OnBeforeBuyItemEvent(object sender, GeneralEventArgs e)
        {
            BeforeBuyItem?.Invoke(sender, e);
        }

        public void OnAfterBuyItemEvent(object sender, GeneralEventArgs e)
        {
            AfterBuyItem?.Invoke(sender, e);
        }

        public void OnBeforeShowRankingEvent(object sender, GeneralEventArgs e)
        {
            BeforeShowRanking?.Invoke(sender, e);
        }

        public void OnAfterShowRankingEvent(object sender, GeneralEventArgs e)
        {
            AfterShowRanking?.Invoke(sender, e);
        }

        public void OnBeforeUseItemEvent(object sender, GeneralEventArgs e)
        {
            BeforeUseItem?.Invoke(sender, e);
        }

        public void OnAfterUseItemEvent(object sender, GeneralEventArgs e)
        {
            AfterUseItem?.Invoke(sender, e);
        }

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeEndGame?.Invoke(sender, e);
        }

        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e)
        {
            AfterEndGame?.Invoke(sender, e);
        }
    }
}
