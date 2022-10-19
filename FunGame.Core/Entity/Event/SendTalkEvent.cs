using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class SendTalkEvent : ISendTalkEvent
    {
        public event IEvent.BeforeEvent? BeforeSendTalkEvent;
        public event IEvent.AfterEvent? AfterSendTalkEvent;
        public event IEvent.SucceedEvent? SucceedSendTalkEvent;
        public event IEvent.FailedEvent? FailedSendTalkEvent;

        public virtual Enum.EventResult OnBeforeSendTalkEvent(GeneralEventArgs e)
        {
            if (BeforeSendTalkEvent != null)
            {
                return BeforeSendTalkEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterSendTalkEvent(GeneralEventArgs e)
        {
            if (AfterSendTalkEvent != null)
            {
                return AfterSendTalkEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedSendTalkEvent(GeneralEventArgs e)
        {
            if (SucceedSendTalkEvent != null)
            {
                return SucceedSendTalkEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedSendTalkEvent(GeneralEventArgs e)
        {
            if (FailedSendTalkEvent != null)
            {
                return FailedSendTalkEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
