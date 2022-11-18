using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ConnectEvent : IConnectEvent
    {
        public event IEvent.BeforeEvent? BeforeConnectEvent;
        public event IEvent.AfterEvent? AfterConnectEvent;
        public event IEvent.SucceedEvent? SucceedConnectEvent;
        public event IEvent.FailedEvent? FailedConnectEvent;

        public virtual EventResult OnBeforeConnectEvent(GeneralEventArgs e)
        {
            if (BeforeConnectEvent != null)
            {
                return BeforeConnectEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterConnectEvent(GeneralEventArgs e)
        {
            if (AfterConnectEvent != null)
            {
                return AfterConnectEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedConnectEvent(GeneralEventArgs e)
        {
            if (SucceedConnectEvent != null)
            {
                return SucceedConnectEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedConnectEvent(GeneralEventArgs e)
        {
            if (FailedConnectEvent != null)
            {
                return FailedConnectEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
