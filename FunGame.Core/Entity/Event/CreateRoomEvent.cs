using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class CreateRoomEvent : ICreateRoomEvent
    {
        public event IEvent.BeforeEvent? BeforeCreateRoomEvent;
        public event IEvent.AfterEvent? AfterCreateRoomEvent;
        public event IEvent.SucceedEvent? SucceedCreateRoomEvent;
        public event IEvent.FailedEvent? FailedCreateRoomEvent;

        public virtual Enum.EventResult OnBeforeCreateRoomEvent(GeneralEventArgs e)
        {
            if (BeforeCreateRoomEvent != null)
            {
                return BeforeCreateRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterCreateRoomEvent(GeneralEventArgs e)
        {
            if (AfterCreateRoomEvent != null)
            {
                return AfterCreateRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedCreateRoomEvent(GeneralEventArgs e)
        {
            if (SucceedCreateRoomEvent != null)
            {
                return SucceedCreateRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedCreateRoomEvent(GeneralEventArgs e)
        {
            if (FailedCreateRoomEvent != null)
            {
                return FailedCreateRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
