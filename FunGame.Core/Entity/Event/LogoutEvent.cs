using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class LogoutEvent : ILogoutEvent
    {
        public event IEvent.BeforeEvent? BeforeLogoutEvent;
        public event IEvent.AfterEvent? AfterLogoutEvent;
        public event IEvent.SucceedEvent? SucceedLogoutEvent;
        public event IEvent.FailedEvent? FailedLogoutEvent;

        public virtual Enum.EventResult OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            if (BeforeLogoutEvent != null)
            {
                return BeforeLogoutEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterLogoutEvent(GeneralEventArgs e)
        {
            if (AfterLogoutEvent != null)
            {
                return AfterLogoutEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            if (SucceedLogoutEvent != null)
            {
                return SucceedLogoutEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedLogoutEvent(GeneralEventArgs e)
        {
            if (FailedLogoutEvent != null)
            {
                return FailedLogoutEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
