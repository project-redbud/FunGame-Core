using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEventHandler, IDisconnectEventHandler, ILogoutEventHandler
    {
        public event IEventHandler.BeforeEventHandler? BeforeConnectEvent;
        public event IEventHandler.AfterEventHandler? AfterConnectEvent;
        public event IEventHandler.SucceedEventHandler? SucceedConnectEvent;
        public event IEventHandler.FailedEventHandler? FailedConnectEvent;

        public EventResult OnAfterConnectEvent(GeneralEventArgs e)
        {
            if (AfterConnectEvent != null)
            {
                return AfterConnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeConnectEvent(GeneralEventArgs e)
        {
            if (BeforeConnectEvent != null)
            {
                return BeforeConnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedConnectEvent(GeneralEventArgs e)
        {
            if (SucceedConnectEvent != null)
            {
                return SucceedConnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedConnectEvent(GeneralEventArgs e)
        {
            if (FailedConnectEvent != null)
            {
                return FailedConnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeDisconnectEvent;
        public event IEventHandler.AfterEventHandler? AfterDisconnectEvent;
        public event IEventHandler.SucceedEventHandler? SucceedDisconnectEvent;
        public event IEventHandler.FailedEventHandler? FailedDisconnectEvent;

        public EventResult OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            if (AfterDisconnectEvent != null)
            {
                return AfterDisconnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            if (BeforeDisconnectEvent != null)
            {
                return BeforeDisconnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            if (FailedDisconnectEvent != null)
            {
                return FailedDisconnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            if (SucceedDisconnectEvent != null)
            {
                return SucceedDisconnectEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeLogoutEvent;
        public event IEventHandler.AfterEventHandler? AfterLogoutEvent;
        public event IEventHandler.SucceedEventHandler? SucceedLogoutEvent;
        public event IEventHandler.FailedEventHandler? FailedLogoutEvent;

        public EventResult OnAfterLogoutEvent(GeneralEventArgs e)
        {
            if (AfterLogoutEvent != null)
            {
                return AfterLogoutEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            if (BeforeLogoutEvent != null)
            {
                return BeforeLogoutEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedLogoutEvent(GeneralEventArgs e)
        {
            if (FailedLogoutEvent != null)
            {
                return FailedLogoutEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            if (SucceedLogoutEvent != null)
            {
                return SucceedLogoutEvent(this, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
