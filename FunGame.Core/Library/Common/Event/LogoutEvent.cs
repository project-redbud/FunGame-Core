using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class LogoutEvent : ILogoutEvent
    {
        public event IEvent.BeforeEvent? BeforeLogoutEvent;
        public event IEvent.AfterEvent? AfterLogoutEvent;
        public event IEvent.SucceedEvent? SucceedLogoutEvent;
        public event IEvent.FailedEvent? FailedLogoutEvent;

        public virtual EventResult OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            if (BeforeLogoutEvent != null)
            {
                return BeforeLogoutEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterLogoutEvent(GeneralEventArgs e)
        {
            if (AfterLogoutEvent != null)
            {
                return AfterLogoutEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            if (SucceedLogoutEvent != null)
            {
                return SucceedLogoutEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedLogoutEvent(GeneralEventArgs e)
        {
            if (FailedLogoutEvent != null)
            {
                return FailedLogoutEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
