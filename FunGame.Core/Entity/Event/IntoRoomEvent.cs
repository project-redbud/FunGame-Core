using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class IntoRoomEvent : IIntoRoomEvent
    {
        public event IEvent.BeforeEvent? BeforeIntoRoomEvent;
        public event IEvent.AfterEvent? AfterIntoRoomEvent;
        public event IEvent.SucceedEvent? SucceedIntoRoomEvent;
        public event IEvent.FailedEvent? FailedIntoRoomEvent;

        public virtual Enum.EventResult OnBeforeIntoRoomEvent(GeneralEventArgs e)
        {
            if (BeforeIntoRoomEvent != null)
            {
                return BeforeIntoRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterIntoRoomEvent(GeneralEventArgs e)
        {
            if (AfterIntoRoomEvent != null)
            {
                return AfterIntoRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedIntoRoomEvent(GeneralEventArgs e)
        {
            if (SucceedIntoRoomEvent != null)
            {
                return SucceedIntoRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedIntoRoomEvent(GeneralEventArgs e)
        {
            if (FailedIntoRoomEvent != null)
            {
                return FailedIntoRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
