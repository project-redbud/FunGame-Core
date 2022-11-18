using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class EndGameEvent : IEndGameEvent
    {
        public event IEvent.BeforeEvent? BeforeEndGameEvent;
        public event IEvent.AfterEvent? AfterEndGameEvent;
        public event IEvent.SucceedEvent? SucceedEndGameEvent;
        public event IEvent.FailedEvent? FailedEndGameEvent;

        public virtual EventResult OnBeforeEndGameEvent(GeneralEventArgs e)
        {
            if (BeforeEndGameEvent != null)
            {
                return BeforeEndGameEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterEndGameEvent(GeneralEventArgs e)
        {
            if (AfterEndGameEvent != null)
            {
                return AfterEndGameEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedEndGameEvent(GeneralEventArgs e)
        {
            if (SucceedEndGameEvent != null)
            {
                return SucceedEndGameEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedEndGameEvent(GeneralEventArgs e)
        {
            if (FailedEndGameEvent != null)
            {
                return FailedEndGameEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
