using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class OpenInventoryEvent : IOpenInventoryEvent
    {
        public event IEvent.BeforeEvent? BeforeOpenInventoryEvent;
        public event IEvent.AfterEvent? AfterOpenInventoryEvent;
        public event IEvent.SucceedEvent? SucceedOpenInventoryEvent;
        public event IEvent.FailedEvent? FailedOpenInventoryEvent;

        public virtual EventResult OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            if (BeforeOpenInventoryEvent != null)
            {
                return BeforeOpenInventoryEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            if (AfterOpenInventoryEvent != null)
            {
                return AfterOpenInventoryEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (SucceedOpenInventoryEvent != null)
            {
                return SucceedOpenInventoryEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (FailedOpenInventoryEvent != null)
            {
                return FailedOpenInventoryEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
