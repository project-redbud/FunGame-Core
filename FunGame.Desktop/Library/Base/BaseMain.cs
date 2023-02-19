using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEventHandler, IDisconnectEventHandler, ILogoutEventHandler
    {
        public event IEventHandler.BeforeEventHandler? BeforeConnectEventHandler;
        public event IEventHandler.AfterEventHandler? AfterConnectEventHandler;
        public event IEventHandler.SucceedEventHandler? SucceedConnectEventHandler;
        public event IEventHandler.FailedEventHandler? FailedConnectEventHandler;

        public EventResult OnAfterConnectEvent(object sender, GeneralEventArgs e)
        {
            if (AfterConnectEventHandler != null)
            {
                return AfterConnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeConnectEvent(object sender, GeneralEventArgs e)
        {
            if (BeforeConnectEventHandler != null)
            {
                return BeforeConnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedConnectEvent(object sender, GeneralEventArgs e)
        {
            if (SucceedConnectEventHandler != null)
            {
                return SucceedConnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedConnectEvent(object sender, GeneralEventArgs e)
        {
            if (FailedConnectEventHandler != null)
            {
                return FailedConnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeDisconnectEventHandler;
        public event IEventHandler.AfterEventHandler? AfterDisconnectEventHandler;
        public event IEventHandler.SucceedEventHandler? SucceedDisconnectEventHandler;
        public event IEventHandler.FailedEventHandler? FailedDisconnectEventHandler;

        public EventResult OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (AfterDisconnectEventHandler != null)
            {
                return AfterDisconnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (BeforeDisconnectEventHandler != null)
            {
                return BeforeDisconnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (FailedDisconnectEventHandler != null)
            {
                return FailedDisconnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            if (SucceedDisconnectEventHandler != null)
            {
                return SucceedDisconnectEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public event IEventHandler.BeforeEventHandler? BeforeLogoutEventHandler;
        public event IEventHandler.AfterEventHandler? AfterLogoutEventHandler;
        public event IEventHandler.SucceedEventHandler? SucceedLogoutEventHandler;
        public event IEventHandler.FailedEventHandler? FailedLogoutEventHandler;

        public EventResult OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (AfterLogoutEventHandler != null)
            {
                return AfterLogoutEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (BeforeLogoutEventHandler != null)
            {
                return BeforeLogoutEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (FailedLogoutEventHandler != null)
            {
                return FailedLogoutEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLogoutEvent(object sender, GeneralEventArgs e)
        {
            if (SucceedLogoutEventHandler != null)
            {
                return SucceedLogoutEventHandler(sender, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
