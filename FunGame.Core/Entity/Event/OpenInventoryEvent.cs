using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class OpenInventoryEvent : IOpenInventoryEvent
    {
        public event IEvent.BeforeEvent? BeforeOpenInventoryEvent;
        public event IEvent.AfterEvent? AfterOpenInventoryEvent;
        public event IEvent.SucceedEvent? SucceedOpenInventoryEvent;
        public event IEvent.FailedEvent? FailedOpenInventoryEvent;

        public virtual Enum.EventResult OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            if (BeforeOpenInventoryEvent != null)
            {
                return BeforeOpenInventoryEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            if (AfterOpenInventoryEvent != null)
            {
                return AfterOpenInventoryEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (SucceedOpenInventoryEvent != null)
            {
                return SucceedOpenInventoryEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (FailedOpenInventoryEvent != null)
            {
                return FailedOpenInventoryEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
