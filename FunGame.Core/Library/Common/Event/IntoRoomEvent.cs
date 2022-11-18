using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class IntoRoomEvent : IIntoRoomEvent
    {
        public event IEvent.BeforeEvent? BeforeIntoRoomEvent;
        public event IEvent.AfterEvent? AfterIntoRoomEvent;
        public event IEvent.SucceedEvent? SucceedIntoRoomEvent;
        public event IEvent.FailedEvent? FailedIntoRoomEvent;

        public virtual EventResult OnBeforeIntoRoomEvent(GeneralEventArgs e)
        {
            if (BeforeIntoRoomEvent != null)
            {
                return BeforeIntoRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterIntoRoomEvent(GeneralEventArgs e)
        {
            if (AfterIntoRoomEvent != null)
            {
                return AfterIntoRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedIntoRoomEvent(GeneralEventArgs e)
        {
            if (SucceedIntoRoomEvent != null)
            {
                return SucceedIntoRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedIntoRoomEvent(GeneralEventArgs e)
        {
            if (FailedIntoRoomEvent != null)
            {
                return FailedIntoRoomEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
