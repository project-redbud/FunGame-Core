using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class SendTalkEvent : ISendTalkEvent
    {
        public event IEvent.BeforeEvent? BeforeSendTalkEvent;
        public event IEvent.AfterEvent? AfterSendTalkEvent;
        public event IEvent.SucceedEvent? SucceedSendTalkEvent;
        public event IEvent.FailedEvent? FailedSendTalkEvent;

        public virtual EventResult OnBeforeSendTalkEvent(GeneralEventArgs e)
        {
            if (BeforeSendTalkEvent != null)
            {
                return BeforeSendTalkEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterSendTalkEvent(GeneralEventArgs e)
        {
            if (AfterSendTalkEvent != null)
            {
                return AfterSendTalkEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedSendTalkEvent(GeneralEventArgs e)
        {
            if (SucceedSendTalkEvent != null)
            {
                return SucceedSendTalkEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedSendTalkEvent(GeneralEventArgs e)
        {
            if (FailedSendTalkEvent != null)
            {
                return FailedSendTalkEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
