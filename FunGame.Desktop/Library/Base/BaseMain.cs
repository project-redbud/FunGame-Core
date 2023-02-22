using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEventHandler, IDisconnectEventHandler, ILogoutEventHandler, IIntoRoomEventHandler, ISendTalkEventHandler,
        ICreateRoomEventHandler, IQuitRoomEventHandler, IStartMatchEventHandler, IStartGameEventHandler, IOpenInventoryEventHandler, IOpenStoreEventHandler
    {
        public event IEventHandler.BeforeEventHandler? BeforeConnect;
        public event IEventHandler.AfterEventHandler? AfterConnect;
        public event IEventHandler.SucceedEventHandler? SucceedConnect;
        public event IEventHandler.FailedEventHandler? FailedConnect;

        public EventResult OnAfterConnectEvent(GeneralEventArgs e)
        {
            if (AfterConnect != null)
            {
                return AfterConnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeConnectEvent(GeneralEventArgs e)
        {
            if (BeforeConnect != null)
            {
                return BeforeConnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedConnectEvent(GeneralEventArgs e)
        {
            if (SucceedConnect != null)
            {
                return SucceedConnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedConnectEvent(GeneralEventArgs e)
        {
            if (FailedConnect != null)
            {
                return FailedConnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeDisconnect;
        public event IEventHandler.AfterEventHandler? AfterDisconnect;
        public event IEventHandler.SucceedEventHandler? SucceedDisconnect;
        public event IEventHandler.FailedEventHandler? FailedDisconnect;

        public EventResult OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            if (AfterDisconnect != null)
            {
                return AfterDisconnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            if (BeforeDisconnect != null)
            {
                return BeforeDisconnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            if (FailedDisconnect != null)
            {
                return FailedDisconnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            if (SucceedDisconnect != null)
            {
                return SucceedDisconnect(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeLogout;
        public event IEventHandler.AfterEventHandler? AfterLogout;
        public event IEventHandler.SucceedEventHandler? SucceedLogout;
        public event IEventHandler.FailedEventHandler? FailedLogout;

        public EventResult OnAfterLogoutEvent(GeneralEventArgs e)
        {
            if (AfterLogout != null)
            {
                return AfterLogout(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            if (BeforeLogout != null)
            {
                return BeforeLogout(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedLogoutEvent(GeneralEventArgs e)
        {
            if (FailedLogout != null)
            {
                return FailedLogout(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            if (SucceedLogout != null)
            {
                return SucceedLogout(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeIntoRoom;
        public event IEventHandler.AfterEventHandler? AfterIntoRoom;
        public event IEventHandler.SucceedEventHandler? SucceedIntoRoom;
        public event IEventHandler.FailedEventHandler? FailedIntoRoom;

        public EventResult OnBeforeIntoRoomEvent(GeneralEventArgs e)
        {
            if (BeforeIntoRoom != null)
            {
                return BeforeIntoRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterIntoRoomEvent(GeneralEventArgs e)
        {
            if (AfterIntoRoom != null)
            {
                return AfterIntoRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedIntoRoomEvent(GeneralEventArgs e)
        {
            if (SucceedIntoRoom != null)
            {
                return SucceedIntoRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedIntoRoomEvent(GeneralEventArgs e)
        {
            if (FailedIntoRoom != null)
            {
                return FailedIntoRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeSendTalk;
        public event IEventHandler.AfterEventHandler? AfterSendTalk;
        public event IEventHandler.SucceedEventHandler? SucceedSendTalk;
        public event IEventHandler.FailedEventHandler? FailedSendTalk;

        public EventResult OnBeforeSendTalkEvent(GeneralEventArgs e)
        {
            if (BeforeSendTalk != null)
            {
                return BeforeSendTalk(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterSendTalkEvent(GeneralEventArgs e)
        {
            if (AfterSendTalk != null)
            {
                return AfterSendTalk(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedSendTalkEvent(GeneralEventArgs e)
        {
            if (SucceedSendTalk != null)
            {
                return SucceedSendTalk(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedSendTalkEvent(GeneralEventArgs e)
        {
            if (FailedSendTalk != null)
            {
                return FailedSendTalk(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeCreateRoom;
        public event IEventHandler.AfterEventHandler? AfterCreateRoom;
        public event IEventHandler.SucceedEventHandler? SucceedCreateRoom;
        public event IEventHandler.FailedEventHandler? FailedCreateRoom;

        public EventResult OnBeforeCreateRoomEvent(GeneralEventArgs e)
        {
            if (BeforeCreateRoom != null)
            {
                return BeforeCreateRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterCreateRoomEvent(GeneralEventArgs e)
        {
            if (AfterCreateRoom != null)
            {
                return AfterCreateRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedCreateRoomEvent(GeneralEventArgs e)
        {
            if (SucceedCreateRoom != null)
            {
                return SucceedCreateRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedCreateRoomEvent(GeneralEventArgs e)
        {
            if (FailedCreateRoom != null)
            {
                return FailedCreateRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeQuitRoom;
        public event IEventHandler.AfterEventHandler? AfterQuitRoom;
        public event IEventHandler.SucceedEventHandler? SucceedQuitRoom;
        public event IEventHandler.FailedEventHandler? FailedQuitRoom;

        public EventResult OnBeforeQuitRoomEvent(GeneralEventArgs e)
        {
            if (BeforeQuitRoom != null)
            {
                return BeforeQuitRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterQuitRoomEvent(GeneralEventArgs e)
        {
            if (AfterQuitRoom != null)
            {
                return AfterQuitRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedQuitRoomEvent(GeneralEventArgs e)
        {
            if (SucceedQuitRoom != null)
            {
                return SucceedQuitRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedQuitRoomEvent(GeneralEventArgs e)
        {
            if (FailedQuitRoom != null)
            {
                return FailedQuitRoom(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartMatch;
        public event IEventHandler.AfterEventHandler? AfterStartMatch;
        public event IEventHandler.SucceedEventHandler? SucceedStartMatch;
        public event IEventHandler.FailedEventHandler? FailedStartMatch;

        public EventResult OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            if (BeforeStartMatch != null)
            {
                return BeforeStartMatch(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            if (AfterStartMatch != null)
            {
                return AfterStartMatch(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            if (SucceedStartMatch != null)
            {
                return SucceedStartMatch(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            if (FailedStartMatch != null)
            {
                return FailedStartMatch(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartGame;
        public event IEventHandler.AfterEventHandler? AfterStartGame;
        public event IEventHandler.SucceedEventHandler? SucceedStartGame;
        public event IEventHandler.FailedEventHandler? FailedStartGame;

        public EventResult OnBeforeStartGameEvent(GeneralEventArgs e)
        {
            if (BeforeStartGame != null)
            {
                return BeforeStartGame(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterStartGameEvent(GeneralEventArgs e)
        {
            if (AfterStartGame != null)
            {
                return AfterStartGame(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedStartGameEvent(GeneralEventArgs e)
        {
            if (SucceedStartGame != null)
            {
                return SucceedStartGame(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedStartGameEvent(GeneralEventArgs e)
        {
            if (FailedStartGame != null)
            {
                return FailedStartGame(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenInventory;
        public event IEventHandler.AfterEventHandler? AfterOpenInventory;
        public event IEventHandler.SucceedEventHandler? SucceedOpenInventory;
        public event IEventHandler.FailedEventHandler? FailedOpenInventory;

        public EventResult OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            if (BeforeOpenInventory != null)
            {
                return BeforeOpenInventory(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            if (AfterOpenInventory != null)
            {
                return AfterOpenInventory(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (SucceedOpenInventory != null)
            {
                return SucceedOpenInventory(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (FailedOpenInventory != null)
            {
                return FailedOpenInventory(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenStore;
        public event IEventHandler.AfterEventHandler? AfterOpenStore;
        public event IEventHandler.SucceedEventHandler? SucceedOpenStore;
        public event IEventHandler.FailedEventHandler? FailedOpenStore;

        public EventResult OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            if (BeforeOpenStore != null)
            {
                return BeforeOpenStore(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            if (AfterOpenStore != null)
            {
                return AfterOpenStore(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            if (SucceedOpenStore != null)
            {
                return SucceedOpenStore(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            if (FailedOpenStore != null)
            {
                return FailedOpenStore(this, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
