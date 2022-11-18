using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class QuitRoomEvent : IQuitRoomEvent
    {
        public event IEvent.BeforeEvent? BeforeQuitRoomEvent;
        public event IEvent.AfterEvent? AfterQuitRoomEvent;
        public event IEvent.SucceedEvent? SucceedQuitRoomEvent;
        public event IEvent.FailedEvent? FailedQuitRoomEvent;

        public virtual EventResult OnBeforeQuitRoomEvent(GeneralEventArgs e)
        {
            if (BeforeQuitRoomEvent != null)
            {
                return BeforeQuitRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterQuitRoomEvent(GeneralEventArgs e)
        {
            if (AfterQuitRoomEvent != null)
            {
                return AfterQuitRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedQuitRoomEvent(GeneralEventArgs e)
        {
            if (SucceedQuitRoomEvent != null)
            {
                return SucceedQuitRoomEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedQuitRoomEvent(GeneralEventArgs e)
        {
            if (FailedQuitRoomEvent != null)
            {
                return FailedQuitRoomEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
