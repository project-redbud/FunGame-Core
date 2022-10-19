using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class RegEvent : IRegEvent
    {
        public event IEvent.BeforeEvent? BeforeRegEvent;
        public event IEvent.AfterEvent? AfterRegEvent;
        public event IEvent.SucceedEvent? SucceedRegEvent;
        public event IEvent.FailedEvent? FailedRegEvent;

        public virtual Enum.EventResult OnBeforeRegEvent(GeneralEventArgs e)
        {
            if (BeforeRegEvent != null)
            {
                return BeforeRegEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterRegEvent(GeneralEventArgs e)
        {
            if (AfterRegEvent != null)
            {
                return AfterRegEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedRegEvent(GeneralEventArgs e)
        {
            if (SucceedRegEvent != null)
            {
                return SucceedRegEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedRegEvent(GeneralEventArgs e)
        {
            if (FailedRegEvent != null)
            {
                return FailedRegEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
