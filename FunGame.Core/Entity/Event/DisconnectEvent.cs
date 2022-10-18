using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class DisconnectEvent : IDisconnectEvent
    {
        public event IEvent.BeforeEvent? BeforeDisconnectEvent;
        public event IEvent.AfterEvent? AfterDisconnectEvent;
        public event IEvent.SucceedEvent? SucceedDisconnectEvent;
        public event IEvent.FailedEvent? FailedDisconnectEvent;

        public virtual Enum.EventResult OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            if (BeforeDisconnectEvent != null)
            {
                return BeforeDisconnectEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            if (AfterDisconnectEvent != null)
            {
                return AfterDisconnectEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            if (SucceedDisconnectEvent != null)
            {
                return SucceedDisconnectEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            if (FailedDisconnectEvent != null)
            {
                return FailedDisconnectEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
