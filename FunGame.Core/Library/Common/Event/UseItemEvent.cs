using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class UseItemEvent : IUseItemEvent
    {
        public event IEvent.BeforeEvent? BeforeUseItemEvent;
        public event IEvent.AfterEvent? AfterUseItemEvent;
        public event IEvent.SucceedEvent? SucceedUseItemEvent;
        public event IEvent.FailedEvent? FailedUseItemEvent;

        public virtual EventResult OnBeforeUseItemEvent(GeneralEventArgs e)
        {
            if (BeforeUseItemEvent != null)
            {
                return BeforeUseItemEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterUseItemEvent(GeneralEventArgs e)
        {
            if (AfterUseItemEvent != null)
            {
                return AfterUseItemEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedUseItemEvent(GeneralEventArgs e)
        {
            if (SucceedUseItemEvent != null)
            {
                return SucceedUseItemEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedUseItemEvent(GeneralEventArgs e)
        {
            if (FailedUseItemEvent != null)
            {
                return FailedUseItemEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
