using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Plugin
{
    public abstract class BasePlugin : IPlugin, IConnectEventHandler, IDisconnectEventHandler, ILoginEventHandler, ILogoutEventHandler, IRegEventHandler, IIntoRoomEventHandler, ISendTalkEventHandler,
        ICreateRoomEventHandler, IQuitRoomEventHandler, IChangeRoomSettingEventHandler, IStartMatchEventHandler, IStartGameEventHandler, IChangeProfileEventHandler, IChangeAccountSettingEventHandler,
        IOpenInventoryEventHandler, ISignInEventHandler, IOpenStoreEventHandler, IBuyItemEventHandler, IShowRankingEventHandler, IUseItemEventHandler, IEndGameEventHandler
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name => _Name;

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description => _Description;

        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version => _Version;

        /// <summary>
        /// 插件作者
        /// </summary>
        public string Author => _Author;

        /**
         * Private
         */
        protected string _Name = "Plugin";
        protected string _Description = "My First Plugin";
        protected string _Version = "1.0.0";
        protected string _Author = "FunGamer";

        /// <summary>
        /// 读取插件时触发的方法
        /// </summary>
        public void Load()
        {
            if (!BeforeLoad())
            {
                return;
            }

            BindEvent();

            AfterLoad();
        }

        /// <summary>
        /// 插件读取后做的事
        /// </summary>
        protected virtual void AfterLoad()
        {

        }

        /// <summary>
        /// 允许返回false来停止读取此插件
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
            Type type = GetType();

            if (type == typeof(IConnectEvent))
            {
                IConnectEvent bind = (IConnectEvent)this;
                BeforeConnect += bind.BeforeConnectEvent;
                AfterConnect += bind.AfterConnectEvent;
                SucceedConnect += bind.SucceedConnectEvent;
                FailedConnect += bind.FailedConnectEvent;
            }

            if (type == typeof(IDisconnectEvent))
            {
                IDisconnectEvent bind = (IDisconnectEvent)this;
                BeforeDisconnect += bind.BeforeDisconnectEvent;
                AfterDisconnect += bind.AfterDisconnectEvent;
                SucceedDisconnect += bind.SucceedDisconnectEvent;
                FailedDisconnect += bind.FailedDisconnectEvent;
            }

            if (type == typeof(ILoginEvent))
            {
                ILoginEvent bind = (ILoginEvent)this;
                BeforeLogin += bind.BeforeLoginEvent;
                AfterLogin += bind.AfterLoginEvent;
                SucceedLogin += bind.SucceedLoginEvent;
                FailedLogin += bind.FailedLoginEvent;
            }

            if (type == typeof(ILogoutEvent))
            {
                ILogoutEvent bind = (ILogoutEvent)this;
                BeforeLogout += bind.BeforeLogoutEvent;
                AfterLogout += bind.AfterLogoutEvent;
                SucceedLogout += bind.SucceedLogoutEvent;
                FailedLogout += bind.FailedLogoutEvent;
            }

            if (type == typeof(IRegEvent))
            {
                IRegEvent bind = (IRegEvent)this;
                BeforeReg += bind.BeforeRegEvent;
                AfterReg += bind.AfterRegEvent;
                SucceedReg += bind.SucceedRegEvent;
                FailedReg += bind.FailedRegEvent;
            }

            if (type == typeof(IIntoRoomEvent))
            {
                IIntoRoomEvent bind = (IIntoRoomEvent)this;
                BeforeIntoRoom += bind.BeforeIntoRoomEvent;
                AfterIntoRoom += bind.AfterIntoRoomEvent;
                SucceedIntoRoom += bind.SucceedIntoRoomEvent;
                FailedIntoRoom += bind.FailedIntoRoomEvent;
            }

            if (type == typeof(ISendTalkEvent))
            {
                ISendTalkEvent bind = (ISendTalkEvent)this;
                BeforeSendTalk += bind.BeforeSendTalkEvent;
                AfterSendTalk += bind.AfterSendTalkEvent;
                SucceedSendTalk += bind.SucceedSendTalkEvent;
                FailedSendTalk += bind.FailedSendTalkEvent;
            }

            if (type == typeof(ICreateRoomEvent))
            {
                ICreateRoomEvent bind = (ICreateRoomEvent)this;
                BeforeCreateRoom += bind.BeforeCreateRoomEvent;
                AfterCreateRoom += bind.AfterCreateRoomEvent;
                SucceedCreateRoom += bind.SucceedCreateRoomEvent;
                FailedCreateRoom += bind.FailedCreateRoomEvent;
            }

            if (type == typeof(IQuitRoomEvent))
            {
                IQuitRoomEvent bind = (IQuitRoomEvent)this;
                BeforeQuitRoom += bind.BeforeQuitRoomEvent;
                AfterQuitRoom += bind.AfterQuitRoomEvent;
                SucceedQuitRoom += bind.SucceedQuitRoomEvent;
                FailedQuitRoom += bind.FailedQuitRoomEvent;
            }

            if (type == typeof(IChangeRoomSettingEvent))
            {
                IChangeRoomSettingEvent bind = (IChangeRoomSettingEvent)this;
                BeforeChangeRoomSetting += bind.BeforeChangeRoomSettingEvent;
                AfterChangeRoomSetting += bind.AfterChangeRoomSettingEvent;
                SucceedChangeRoomSetting += bind.SucceedChangeRoomSettingEvent;
                FailedChangeRoomSetting += bind.FailedChangeRoomSettingEvent;
            }

            if (type == typeof(IStartMatchEvent))
            {
                IStartMatchEvent bind = (IStartMatchEvent)this;
                BeforeStartMatch += bind.BeforeStartMatchEvent;
                AfterStartMatch += bind.AfterStartMatchEvent;
                SucceedStartMatch += bind.SucceedStartMatchEvent;
                FailedStartMatch += bind.FailedStartMatchEvent;
            }

            if (type == typeof(IStartGameEvent))
            {
                IStartGameEvent bind = (IStartGameEvent)this;
                BeforeStartGame += bind.BeforeStartGameEvent;
                AfterStartGame += bind.AfterStartGameEvent;
                SucceedStartGame += bind.SucceedStartGameEvent;
                FailedStartGame += bind.FailedStartGameEvent;
            }

            if (type == typeof(IChangeProfileEvent))
            {
                IChangeProfileEvent bind = (IChangeProfileEvent)this;
                BeforeChangeProfile += bind.BeforeChangeProfileEvent;
                AfterChangeProfile += bind.AfterChangeProfileEvent;
                SucceedChangeProfile += bind.SucceedChangeProfileEvent;
                FailedChangeProfile += bind.FailedChangeProfileEvent;
            }

            if (type == typeof(IChangeAccountSettingEvent))
            {
                IChangeAccountSettingEvent bind = (IChangeAccountSettingEvent)this;
                BeforeChangeAccountSetting += bind.BeforeChangeAccountSettingEvent;
                AfterChangeAccountSetting += bind.AfterChangeAccountSettingEvent;
                SucceedChangeAccountSetting += bind.SucceedChangeAccountSettingEvent;
                FailedChangeAccountSetting += bind.FailedChangeAccountSettingEvent;
            }

            if (type == typeof(IOpenInventoryEvent))
            {
                IOpenInventoryEvent bind = (IOpenInventoryEvent)this;
                BeforeOpenInventory += bind.BeforeOpenInventoryEvent;
                AfterOpenInventory += bind.AfterOpenInventoryEvent;
                SucceedOpenInventory += bind.SucceedOpenInventoryEvent;
                FailedOpenInventory += bind.FailedOpenInventoryEvent;
            }

            if (type == typeof(ISignInEvent))
            {
                ISignInEvent bind = (ISignInEvent)this;
                BeforeSignIn += bind.BeforeSignInEvent;
                AfterSignIn += bind.AfterSignInEvent;
                SucceedSignIn += bind.SucceedSignInEvent;
                FailedSignIn += bind.FailedSignInEvent;
            }

            if (type == typeof(IOpenStoreEvent))
            {
                IOpenStoreEvent bind = (IOpenStoreEvent)this;
                BeforeOpenStore += bind.BeforeOpenStoreEvent;
                AfterOpenStore += bind.AfterOpenStoreEvent;
                SucceedOpenStore += bind.SucceedOpenStoreEvent;
                FailedOpenStore += bind.FailedOpenStoreEvent;
            }

            if (type == typeof(IBuyItemEvent))
            {
                IBuyItemEvent bind = (IBuyItemEvent)this;
                BeforeBuyItem += bind.BeforeBuyItemEvent;
                AfterBuyItem += bind.AfterBuyItemEvent;
                SucceedBuyItem += bind.SucceedBuyItemEvent;
                FailedBuyItem += bind.FailedBuyItemEvent;
            }

            if (type == typeof(IShowRankingEvent))
            {
                IShowRankingEvent bind = (IShowRankingEvent)this;
                BeforeShowRanking += bind.BeforeShowRankingEvent;
                AfterShowRanking += bind.AfterShowRankingEvent;
                SucceedShowRanking += bind.SucceedShowRankingEvent;
                FailedShowRanking += bind.FailedShowRankingEvent;
            }

            if (type == typeof(IUseItemEvent))
            {
                IUseItemEvent bind = (IUseItemEvent)this;
                BeforeUseItem += bind.BeforeUseItemEvent;
                AfterUseItem += bind.AfterUseItemEvent;
                SucceedUseItem += bind.SucceedUseItemEvent;
                FailedUseItem += bind.FailedUseItemEvent;
            }

            if (type == typeof(IEndGameEvent))
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

        public EventResult OnBeforeConnectEvent(ConnectEventArgs e)
        {
            return BeforeConnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterConnectEvent(ConnectEventArgs e)
        {
            return AfterConnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedConnectEvent(ConnectEventArgs e)
        {
            return SucceedConnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedConnectEvent(ConnectEventArgs e)
        {
            return FailedConnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            return BeforeDisconnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            return AfterDisconnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            return SucceedDisconnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            return FailedDisconnect?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLoginEvent(LoginEventArgs e)
        {
            return BeforeLogin?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterLoginEvent(LoginEventArgs e)
        {
            return AfterLogin?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLoginEvent(LoginEventArgs e)
        {
            return SucceedLogin?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedLoginEvent(LoginEventArgs e)
        {
            return FailedLogin?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            return BeforeLogout?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterLogoutEvent(GeneralEventArgs e)
        {
            return AfterLogout?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            return SucceedLogout?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedLogoutEvent(GeneralEventArgs e)
        {
            return FailedLogout?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeRegEvent(RegisterEventArgs e)
        {
            return BeforeReg?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterRegEvent(RegisterEventArgs e)
        {
            return AfterReg?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedRegEvent(RegisterEventArgs e)
        {
            return SucceedReg?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedRegEvent(RegisterEventArgs e)
        {
            return FailedReg?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeIntoRoomEvent(RoomEventArgs e)
        {
            return BeforeIntoRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterIntoRoomEvent(RoomEventArgs e)
        {
            return AfterIntoRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedIntoRoomEvent(RoomEventArgs e)
        {
            return SucceedIntoRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedIntoRoomEvent(RoomEventArgs e)
        {
            return FailedIntoRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeSendTalkEvent(SendTalkEventArgs e)
        {
            return BeforeSendTalk?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterSendTalkEvent(SendTalkEventArgs e)
        {
            return AfterSendTalk?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedSendTalkEvent(SendTalkEventArgs e)
        {
            return SucceedSendTalk?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedSendTalkEvent(SendTalkEventArgs e)
        {
            return FailedSendTalk?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeCreateRoomEvent(RoomEventArgs e)
        {
            return BeforeCreateRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterCreateRoomEvent(RoomEventArgs e)
        {
            return AfterCreateRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedCreateRoomEvent(RoomEventArgs e)
        {
            return SucceedCreateRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedCreateRoomEvent(RoomEventArgs e)
        {
            return FailedCreateRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeQuitRoomEvent(RoomEventArgs e)
        {
            return BeforeQuitRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterQuitRoomEvent(RoomEventArgs e)
        {
            return AfterQuitRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedQuitRoomEvent(RoomEventArgs e)
        {
            return SucceedQuitRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedQuitRoomEvent(RoomEventArgs e)
        {
            return FailedQuitRoom?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeChangeRoomSettingEvent(GeneralEventArgs e)
        {
            return BeforeChangeRoomSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterChangeRoomSettingEvent(GeneralEventArgs e)
        {
            return AfterChangeRoomSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            return SucceedChangeRoomSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            return FailedChangeRoomSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            return BeforeStartMatch?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            return AfterStartMatch?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            return SucceedStartMatch?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            return FailedStartMatch?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeStartGameEvent(GeneralEventArgs e)
        {
            return BeforeStartGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterStartGameEvent(GeneralEventArgs e)
        {
            return AfterStartGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedStartGameEvent(GeneralEventArgs e)
        {
            return SucceedStartGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedStartGameEvent(GeneralEventArgs e)
        {
            return FailedStartGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeChangeProfileEvent(GeneralEventArgs e)
        {
            return BeforeChangeProfile?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterChangeProfileEvent(GeneralEventArgs e)
        {
            return AfterChangeProfile?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedChangeProfileEvent(GeneralEventArgs e)
        {
            return SucceedChangeProfile?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedChangeProfileEvent(GeneralEventArgs e)
        {
            return FailedChangeProfile?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeChangeAccountSettingEvent(GeneralEventArgs e)
        {
            return BeforeChangeAccountSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterChangeAccountSettingEvent(GeneralEventArgs e)
        {
            return AfterChangeAccountSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            return SucceedChangeAccountSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            return FailedChangeAccountSetting?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            return BeforeOpenInventory?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            return AfterOpenInventory?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            return SucceedOpenInventory?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            return FailedOpenInventory?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeSignInEvent(GeneralEventArgs e)
        {
            return BeforeSignIn?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterSignInEvent(GeneralEventArgs e)
        {
            return AfterSignIn?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedSignInEvent(GeneralEventArgs e)
        {
            return SucceedSignIn?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedSignInEvent(GeneralEventArgs e)
        {
            return FailedSignIn?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            return BeforeOpenStore?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            return AfterOpenStore?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            return SucceedOpenStore?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            return FailedOpenStore?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeBuyItemEvent(GeneralEventArgs e)
        {
            return BeforeBuyItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterBuyItemEvent(GeneralEventArgs e)
        {
            return AfterBuyItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedBuyItemEvent(GeneralEventArgs e)
        {
            return SucceedBuyItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedBuyItemEvent(GeneralEventArgs e)
        {
            return FailedBuyItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeShowRankingEvent(GeneralEventArgs e)
        {
            return BeforeShowRanking?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterShowRankingEvent(GeneralEventArgs e)
        {
            return AfterShowRanking?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedShowRankingEvent(GeneralEventArgs e)
        {
            return SucceedShowRanking?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedShowRankingEvent(GeneralEventArgs e)
        {
            return FailedShowRanking?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeUseItemEvent(GeneralEventArgs e)
        {
            return BeforeUseItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterUseItemEvent(GeneralEventArgs e)
        {
            return AfterUseItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedUseItemEvent(GeneralEventArgs e)
        {
            return SucceedUseItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedUseItemEvent(GeneralEventArgs e)
        {
            return FailedUseItem?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnBeforeEndGameEvent(GeneralEventArgs e)
        {
            return BeforeEndGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnAfterEndGameEvent(GeneralEventArgs e)
        {
            return AfterEndGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnSucceedEndGameEvent(GeneralEventArgs e)
        {
            return SucceedEndGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }

        public EventResult OnFailedEndGameEvent(GeneralEventArgs e)
        {
            return FailedEndGame?.Invoke(this, e) ?? EventResult.NoEventImplement;
        }
    }
}
