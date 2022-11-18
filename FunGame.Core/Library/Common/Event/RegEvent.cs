using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RegEvent : IRegEvent
    {
        public event IEvent.BeforeEvent? BeforeRegEvent;
        public event IEvent.AfterEvent? AfterRegEvent;
        public event IEvent.SucceedEvent? SucceedRegEvent;
        public event IEvent.FailedEvent? FailedRegEvent;

        public virtual EventResult OnBeforeRegEvent(GeneralEventArgs e)
        {
            if (BeforeRegEvent != null)
            {
                return BeforeRegEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterRegEvent(GeneralEventArgs e)
        {
            if (AfterRegEvent != null)
            {
                return AfterRegEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedRegEvent(GeneralEventArgs e)
        {
            if (SucceedRegEvent != null)
            {
                return SucceedRegEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedRegEvent(GeneralEventArgs e)
        {
            if (FailedRegEvent != null)
            {
                return FailedRegEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
