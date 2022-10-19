using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class QuitRoomEvent : IQuitRoomEvent
    {
        public event IEvent.BeforeEvent? BeforeQuitRoomEvent;
        public event IEvent.AfterEvent? AfterQuitRoomEvent;
        public event IEvent.SucceedEvent? SucceedQuitRoomEvent;
        public event IEvent.FailedEvent? FailedQuitRoomEvent;

        public virtual Enum.EventResult OnBeforeQuitRoomEvent(GeneralEventArgs e)
        {
            if (BeforeQuitRoomEvent != null)
            {
                return BeforeQuitRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterQuitRoomEvent(GeneralEventArgs e)
        {
            if (AfterQuitRoomEvent != null)
            {
                return AfterQuitRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedQuitRoomEvent(GeneralEventArgs e)
        {
            if (SucceedQuitRoomEvent != null)
            {
                return SucceedQuitRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedQuitRoomEvent(GeneralEventArgs e)
        {
            if (FailedQuitRoomEvent != null)
            {
                return FailedQuitRoomEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
