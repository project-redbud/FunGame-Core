using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class StartMatchEvent : IStartMatchEvent
    {
        public event IEvent.BeforeEvent? BeforeStartMatchEvent;
        public event IEvent.AfterEvent? AfterStartMatchEvent;
        public event IEvent.SucceedEvent? SucceedStartMatchEvent;
        public event IEvent.FailedEvent? FailedStartMatchEvent;

        public virtual EventResult OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            if (BeforeStartMatchEvent != null)
            {
                return BeforeStartMatchEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            if (AfterStartMatchEvent != null)
            {
                return AfterStartMatchEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            if (SucceedStartMatchEvent != null)
            {
                return SucceedStartMatchEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            if (FailedStartMatchEvent != null)
            {
                return FailedStartMatchEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
