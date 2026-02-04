using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class Plugin : IPlugin
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
        /// 记录该插件的加载器
        /// </summary>
        public PluginLoader? PluginLoader { get; set; } = null;

        /// <summary>
        /// 包含了一些常用方法的控制器
        /// </summary>
        public AddonController<IPlugin> Controller
        {
            get => _controller ?? throw new NotImplementedException();
            internal set => _controller = value;
        }

        /// <summary>
        /// base控制器，没有DataRequest
        /// </summary>
        BaseAddonController<IPlugin> IAddonController<IPlugin>.Controller
        {
            get => Controller;
            set => _controller = (AddonController<IPlugin>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private AddonController<IPlugin>? _controller;

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
                // 初始化此插件（传入委托或者Model）
                Init(objs);
                // 触发绑定事件
                BindEvent();
            }
            return _isLoaded;
        }

        /// <summary>
        /// 卸载模组
        /// </summary>
        /// <param name="objs"></param>
        public void UnLoad(params object[] objs)
        {
            BindEvent(false);
        }

        /// <summary>
        /// 插件完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(PluginLoader loader, params object[] objs)
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
        /// 传递一些插件可以用参数
        /// </summary>
        private void Init(params object[] objs)
        {
            if (objs.Length > 0) _session = (Session)objs[0];
            if (objs.Length > 1) _config = (FunGameConfig)objs[1];
        }

        /// <summary>
        /// Session对象
        /// </summary>
        protected Session _session = new();

        /// <summary>
        /// Config对象
        /// </summary>
        protected FunGameConfig _config = new();

        /// <summary>
        /// 绑定事件。在<see cref="BeforeLoad"/>后触发
        /// </summary>
        private void BindEvent(bool isAdd = true)
        {
            if (this is IConnectEvent connect)
            {
                if (isAdd)
                {
                    BeforeConnect += connect.BeforeConnectEvent;
                    AfterConnect += connect.AfterConnectEvent;
                }
                else
                {
                    BeforeConnect -= connect.BeforeConnectEvent;
                    AfterConnect -= connect.AfterConnectEvent;
                }
            }

            if (this is IDisconnectEvent disconnect)
            {
                if (isAdd)
                {
                    BeforeDisconnect += disconnect.BeforeDisconnectEvent;
                    AfterDisconnect += disconnect.AfterDisconnectEvent;
                }
                else
                {
                    BeforeDisconnect -= disconnect.BeforeDisconnectEvent;
                    AfterDisconnect -= disconnect.AfterDisconnectEvent;
                }
            }

            if (this is ILoginEvent login)
            {
                if (isAdd)
                {
                    BeforeLogin += login.BeforeLoginEvent;
                    AfterLogin += login.AfterLoginEvent;
                }
                else
                {
                    BeforeLogin -= login.BeforeLoginEvent;
                    AfterLogin -= login.AfterLoginEvent;
                }
            }

            if (this is ILogoutEvent logout)
            {
                if (isAdd)
                {
                    BeforeLogout += logout.BeforeLogoutEvent;
                    AfterLogout += logout.AfterLogoutEvent;
                }
                else
                {
                    BeforeLogout -= logout.BeforeLogoutEvent;
                    AfterLogout -= logout.AfterLogoutEvent;
                }
            }

            if (this is IRegEvent reg)
            {
                if (isAdd)
                {
                    BeforeReg += reg.BeforeRegEvent;
                    AfterReg += reg.AfterRegEvent;
                }
                else
                {
                    BeforeReg -= reg.BeforeRegEvent;
                    AfterReg -= reg.AfterRegEvent;
                }
            }

            if (this is IIntoRoomEvent intoRoom)
            {
                if (isAdd)
                {
                    BeforeIntoRoom += intoRoom.BeforeIntoRoomEvent;
                    AfterIntoRoom += intoRoom.AfterIntoRoomEvent;
                }
                else
                {
                    BeforeIntoRoom -= intoRoom.BeforeIntoRoomEvent;
                    AfterIntoRoom -= intoRoom.AfterIntoRoomEvent;
                }
            }

            if (this is ISendTalkEvent sendTalk)
            {
                if (isAdd)
                {
                    BeforeSendTalk += sendTalk.BeforeSendTalkEvent;
                    AfterSendTalk += sendTalk.AfterSendTalkEvent;
                }
                else
                {
                    BeforeSendTalk -= sendTalk.BeforeSendTalkEvent;
                    AfterSendTalk -= sendTalk.AfterSendTalkEvent;
                }
            }

            if (this is ICreateRoomEvent createRoom)
            {
                if (isAdd)
                {
                    BeforeCreateRoom += createRoom.BeforeCreateRoomEvent;
                    AfterCreateRoom += createRoom.AfterCreateRoomEvent;
                }
                else
                {
                    BeforeCreateRoom -= createRoom.BeforeCreateRoomEvent;
                    AfterCreateRoom -= createRoom.AfterCreateRoomEvent;
                }
            }

            if (this is IQuitRoomEvent quitRoom)
            {
                if (isAdd)
                {
                    BeforeQuitRoom += quitRoom.BeforeQuitRoomEvent;
                    AfterQuitRoom += quitRoom.AfterQuitRoomEvent;
                }
                else
                {
                    BeforeQuitRoom -= quitRoom.BeforeQuitRoomEvent;
                    AfterQuitRoom -= quitRoom.AfterQuitRoomEvent;
                }
            }

            if (this is IChangeRoomSettingEvent changeRoomSetting)
            {
                if (isAdd)
                {
                    BeforeChangeRoomSetting += changeRoomSetting.BeforeChangeRoomSettingEvent;
                    AfterChangeRoomSetting += changeRoomSetting.AfterChangeRoomSettingEvent;
                }
                else
                {
                    BeforeChangeRoomSetting -= changeRoomSetting.BeforeChangeRoomSettingEvent;
                    AfterChangeRoomSetting -= changeRoomSetting.AfterChangeRoomSettingEvent;
                }
            }

            if (this is IStartMatchEvent startMatch)
            {
                if (isAdd)
                {
                    BeforeStartMatch += startMatch.BeforeStartMatchEvent;
                    AfterStartMatch += startMatch.AfterStartMatchEvent;
                }
                else
                {
                    BeforeStartMatch -= startMatch.BeforeStartMatchEvent;
                    AfterStartMatch -= startMatch.AfterStartMatchEvent;
                }
            }

            if (this is IStartGameEvent startGame)
            {
                if (isAdd)
                {
                    BeforeStartGame += startGame.BeforeStartGameEvent;
                    AfterStartGame += startGame.AfterStartGameEvent;
                }
                else
                {
                    BeforeStartGame -= startGame.BeforeStartGameEvent;
                    AfterStartGame -= startGame.AfterStartGameEvent;
                }
            }

            if (this is IChangeProfileEvent changeProfile)
            {
                if (isAdd)
                {
                    BeforeChangeProfile += changeProfile.BeforeChangeProfileEvent;
                    AfterChangeProfile += changeProfile.AfterChangeProfileEvent;
                }
                else
                {
                    BeforeChangeProfile -= changeProfile.BeforeChangeProfileEvent;
                    AfterChangeProfile -= changeProfile.AfterChangeProfileEvent;
                }
            }

            if (this is IChangeAccountSettingEvent changeAccountSetting)
            {
                if (isAdd)
                {
                    BeforeChangeAccountSetting += changeAccountSetting.BeforeChangeAccountSettingEvent;
                    AfterChangeAccountSetting += changeAccountSetting.AfterChangeAccountSettingEvent;
                }
                else
                {
                    BeforeChangeAccountSetting -= changeAccountSetting.BeforeChangeAccountSettingEvent;
                    AfterChangeAccountSetting -= changeAccountSetting.AfterChangeAccountSettingEvent;
                }
            }

            if (this is IOpenInventoryEvent openInventory)
            {
                if (isAdd)
                {
                    BeforeOpenInventory += openInventory.BeforeOpenInventoryEvent;
                    AfterOpenInventory += openInventory.AfterOpenInventoryEvent;
                }
                else
                {
                    BeforeOpenInventory -= openInventory.BeforeOpenInventoryEvent;
                    AfterOpenInventory -= openInventory.AfterOpenInventoryEvent;
                }
            }

            if (this is ISignInEvent signIn)
            {
                if (isAdd)
                {
                    BeforeSignIn += signIn.BeforeSignInEvent;
                    AfterSignIn += signIn.AfterSignInEvent;
                }
                else
                {
                    BeforeSignIn -= signIn.BeforeSignInEvent;
                    AfterSignIn -= signIn.AfterSignInEvent;
                }
            }

            if (this is IOpenStoreEvent openStore)
            {
                if (isAdd)
                {
                    BeforeOpenStore += openStore.BeforeOpenStoreEvent;
                    AfterOpenStore += openStore.AfterOpenStoreEvent;
                }
                else
                {
                    BeforeOpenStore -= openStore.BeforeOpenStoreEvent;
                    AfterOpenStore -= openStore.AfterOpenStoreEvent;
                }
            }

            if (this is IBuyItemEvent buyItem)
            {
                if (isAdd)
                {
                    BeforeBuyItem += buyItem.BeforeBuyItemEvent;
                    AfterBuyItem += buyItem.AfterBuyItemEvent;
                }
                else
                {
                    BeforeBuyItem -= buyItem.BeforeBuyItemEvent;
                    AfterBuyItem -= buyItem.AfterBuyItemEvent;
                }
            }

            if (this is IShowRankingEvent showRanking)
            {
                if (isAdd)
                {
                    BeforeShowRanking += showRanking.BeforeShowRankingEvent;
                    AfterShowRanking += showRanking.AfterShowRankingEvent;
                }
                else
                {
                    BeforeShowRanking -= showRanking.BeforeShowRankingEvent;
                    AfterShowRanking -= showRanking.AfterShowRankingEvent;
                }
            }

            if (this is IUseItemEvent useItem)
            {
                if (isAdd)
                {
                    BeforeUseItem += useItem.BeforeUseItemEvent;
                    AfterUseItem += useItem.AfterUseItemEvent;
                }
                else
                {
                    BeforeUseItem -= useItem.BeforeUseItemEvent;
                    AfterUseItem -= useItem.AfterUseItemEvent;
                }
            }

            if (this is IEndGameEvent endGame)
            {
                if (isAdd)
                {
                    BeforeEndGame += endGame.BeforeEndGameEvent;
                    AfterEndGame += endGame.AfterEndGameEvent;
                }
                else
                {
                    BeforeEndGame -= endGame.BeforeEndGameEvent;
                    AfterEndGame -= endGame.AfterEndGameEvent;
                }
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
