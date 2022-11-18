using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class LoginEvent : ILoginEvent
    {
        public event IEvent.BeforeEvent? BeforeLoginEvent;
        public event IEvent.AfterEvent? AfterLoginEvent;
        public event IEvent.SucceedEvent? SucceedLoginEvent;
        public event IEvent.FailedEvent? FailedLoginEvent;

        public virtual EventResult OnBeforeLoginEvent(GeneralEventArgs e)
        {
            if (BeforeLoginEvent != null)
            {
                return BeforeLoginEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterLoginEvent(GeneralEventArgs e)
        {
            if (AfterLoginEvent != null)
            {
                return AfterLoginEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedLoginEvent(GeneralEventArgs e)
        {
            if (SucceedLoginEvent != null)
            {
                return SucceedLoginEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedLoginEvent(GeneralEventArgs e)
        {
            if (FailedLoginEvent != null)
            {
                return FailedLoginEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
