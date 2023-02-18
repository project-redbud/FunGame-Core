using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEvent, IDisconnectEvent, ILogoutEvent
    {
        public event IEvent.BeforeEvent? BeforeConnectEvent;
        public event IEvent.AfterEvent? AfterConnectEvent;
        public event IEvent.SucceedEvent? SucceedConnectEvent;
        public event IEvent.FailedEvent? FailedConnectEvent;

        public EventResult OnAfterConnectEvent(object sender, GeneralEventArgs e)
        {
            if (AfterConnectEvent != null)
            {
                return AfterConnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeConnectEvent(object sender, GeneralEventArgs e)
        {
            if (BeforeConnectEvent != null)
            {
                return BeforeConnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedConnectEvent(object sender, GeneralEventArgs e)
        {
            if (SucceedConnectEvent != null)
            {
                return SucceedConnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedConnectEvent(object sender, GeneralEventArgs e)
        {
            if (FailedConnectEvent != null)
            {
                return FailedConnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEvent.BeforeEvent? BeforeDisconnectEvent;
        public event IEvent.AfterEvent? AfterDisconnectEvent;
        public event IEvent.SucceedEvent? SucceedDisconnectEvent;
        public event IEvent.FailedEvent? FailedDisconnectEvent;

        public EventResult OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (AfterDisconnectEvent != null)
            {
                return AfterDisconnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (BeforeDisconnectEvent != null)
            {
                return BeforeDisconnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (FailedDisconnectEvent != null)
            {
                return FailedDisconnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (SucceedDisconnectEvent != null)
            {
                return SucceedDisconnectEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEvent.BeforeEvent? BeforeLogoutEvent;
        public event IEvent.AfterEvent? AfterLogoutEvent;
        public event IEvent.SucceedEvent? SucceedLogoutEvent;
        public event IEvent.FailedEvent? FailedLogoutEvent;

        public EventResult OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (AfterLogoutEvent != null)
            {
                return AfterLogoutEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (BeforeLogoutEvent != null)
            {
                return BeforeLogoutEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (FailedLogoutEvent != null)
            {
                return FailedLogoutEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (SucceedLogoutEvent != null)
            {
                return SucceedLogoutEvent(sender, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
