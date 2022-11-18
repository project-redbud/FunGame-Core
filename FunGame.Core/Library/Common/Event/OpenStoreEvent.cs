using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class OpenStoreEvent : IOpenStoreEvent
    {
        public event IEvent.BeforeEvent? BeforeOpenStoreEvent;
        public event IEvent.AfterEvent? AfterOpenStoreEvent;
        public event IEvent.SucceedEvent? SucceedOpenStoreEvent;
        public event IEvent.FailedEvent? FailedOpenStoreEvent;

        public virtual EventResult OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            if (BeforeOpenStoreEvent != null)
            {
                return BeforeOpenStoreEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            if (AfterOpenStoreEvent != null)
            {
                return AfterOpenStoreEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            if (SucceedOpenStoreEvent != null)
            {
                return SucceedOpenStoreEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            if (FailedOpenStoreEvent != null)
            {
                return FailedOpenStoreEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
