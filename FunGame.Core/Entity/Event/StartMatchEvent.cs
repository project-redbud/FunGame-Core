using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class StartMatchEvent : IStartMatchEvent
    {
        public event IEvent.BeforeEvent? BeforeStartMatchEvent;
        public event IEvent.AfterEvent? AfterStartMatchEvent;
        public event IEvent.SucceedEvent? SucceedStartMatchEvent;
        public event IEvent.FailedEvent? FailedStartMatchEvent;

        public virtual Enum.EventResult OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            if (BeforeStartMatchEvent != null)
            {
                return BeforeStartMatchEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            if (AfterStartMatchEvent != null)
            {
                return AfterStartMatchEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            if (SucceedStartMatchEvent != null)
            {
                return SucceedStartMatchEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            if (FailedStartMatchEvent != null)
            {
                return FailedStartMatchEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
