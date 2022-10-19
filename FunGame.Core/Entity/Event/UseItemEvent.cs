using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class UseItemEvent : IUseItemEvent
    {
        public event IEvent.BeforeEvent? BeforeUseItemEvent;
        public event IEvent.AfterEvent? AfterUseItemEvent;
        public event IEvent.SucceedEvent? SucceedUseItemEvent;
        public event IEvent.FailedEvent? FailedUseItemEvent;

        public virtual Enum.EventResult OnBeforeUseItemEvent(GeneralEventArgs e)
        {
            if (BeforeUseItemEvent != null)
            {
                return BeforeUseItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterUseItemEvent(GeneralEventArgs e)
        {
            if (AfterUseItemEvent != null)
            {
                return AfterUseItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedUseItemEvent(GeneralEventArgs e)
        {
            if (SucceedUseItemEvent != null)
            {
                return SucceedUseItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedUseItemEvent(GeneralEventArgs e)
        {
            if (FailedUseItemEvent != null)
            {
                return FailedUseItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
