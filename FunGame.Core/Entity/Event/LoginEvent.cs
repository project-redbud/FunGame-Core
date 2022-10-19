using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class LoginEvent : ILoginEvent
    {
        public event IEvent.BeforeEvent? BeforeLoginEvent;
        public event IEvent.AfterEvent? AfterLoginEvent;
        public event IEvent.SucceedEvent? SucceedLoginEvent;
        public event IEvent.FailedEvent? FailedLoginEvent;

        public virtual Enum.EventResult OnBeforeLoginEvent(GeneralEventArgs e)
        {
            if (BeforeLoginEvent != null)
            {
                return BeforeLoginEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterLoginEvent(GeneralEventArgs e)
        {
            if (AfterLoginEvent != null)
            {
                return AfterLoginEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedLoginEvent(GeneralEventArgs e)
        {
            if (SucceedLoginEvent != null)
            {
                return SucceedLoginEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedLoginEvent(GeneralEventArgs e)
        {
            if (FailedLoginEvent != null)
            {
                return FailedLoginEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
