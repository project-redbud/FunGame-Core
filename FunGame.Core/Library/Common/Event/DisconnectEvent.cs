using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class DisconnectEvent : IDisconnectEvent
    {
        public event IEvent.BeforeEvent? BeforeDisconnectEvent;
        public event IEvent.AfterEvent? AfterDisconnectEvent;
        public event IEvent.SucceedEvent? SucceedDisconnectEvent;
        public event IEvent.FailedEvent? FailedDisconnectEvent;

        public virtual EventResult OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            if (BeforeDisconnectEvent != null)
            {
                return BeforeDisconnectEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            if (AfterDisconnectEvent != null)
            {
                return AfterDisconnectEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            if (SucceedDisconnectEvent != null)
            {
                return SucceedDisconnectEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            if (FailedDisconnectEvent != null)
            {
                return FailedDisconnectEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
