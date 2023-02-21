using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEventHandler, IDisconnectEventHandler, ILogoutEventHandler
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
    }
}
