using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class ChangeProfileEvent : IChangeProfileEvent
    {
        public event IEvent.BeforeEvent? BeforeChangeProfileEvent;
        public event IEvent.AfterEvent? AfterChangeProfileEvent;
        public event IEvent.SucceedEvent? SucceedChangeProfileEvent;
        public event IEvent.FailedEvent? FailedChangeProfileEvent;

        public virtual Enum.EventResult OnBeforeChangeProfileEvent(GeneralEventArgs e)
        {
            if (BeforeChangeProfileEvent != null)
            {
                return BeforeChangeProfileEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterChangeProfileEvent(GeneralEventArgs e)
        {
            if (AfterChangeProfileEvent != null)
            {
                return AfterChangeProfileEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedChangeProfileEvent(GeneralEventArgs e)
        {
            if (SucceedChangeProfileEvent != null)
            {
                return SucceedChangeProfileEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedChangeProfileEvent(GeneralEventArgs e)
        {
            if (FailedChangeProfileEvent != null)
            {
                return FailedChangeProfileEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
