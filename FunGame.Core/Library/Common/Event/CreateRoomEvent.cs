using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class CreateRoomEvent : ICreateRoomEvent
    {
        public event IEvent.BeforeEvent? BeforeCreateRoomEvent;
        public event IEvent.AfterEvent? AfterCreateRoomEvent;
        public event IEvent.SucceedEvent? SucceedCreateRoomEvent;
        public event IEvent.FailedEvent? FailedCreateRoomEvent;

        public virtual EventResult OnBeforeCreateRoomEvent(GeneralEventArgs e)
        {
            if (BeforeCreateRoomEvent != null)
            {
                return BeforeCreateRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterCreateRoomEvent(GeneralEventArgs e)
        {
            if (AfterCreateRoomEvent != null)
            {
                return AfterCreateRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedCreateRoomEvent(GeneralEventArgs e)
        {
            if (SucceedCreateRoomEvent != null)
            {
                return SucceedCreateRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedCreateRoomEvent(GeneralEventArgs e)
        {
            if (FailedCreateRoomEvent != null)
            {
                return FailedCreateRoomEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
