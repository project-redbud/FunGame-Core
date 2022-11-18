using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ShowRankingEvent : IShowRankingEvent
    {
        public event IEvent.BeforeEvent? BeforeShowRankingEvent;
        public event IEvent.AfterEvent? AfterShowRankingEvent;
        public event IEvent.SucceedEvent? SucceedShowRankingEvent;
        public event IEvent.FailedEvent? FailedShowRankingEvent;

        public virtual EventResult OnBeforeShowRankingEvent(GeneralEventArgs e)
        {
            if (BeforeShowRankingEvent != null)
            {
                return BeforeShowRankingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterShowRankingEvent(GeneralEventArgs e)
        {
            if (AfterShowRankingEvent != null)
            {
                return AfterShowRankingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedShowRankingEvent(GeneralEventArgs e)
        {
            if (SucceedShowRankingEvent != null)
            {
                return SucceedShowRankingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedShowRankingEvent(GeneralEventArgs e)
        {
            if (FailedShowRankingEvent != null)
            {
                return FailedShowRankingEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
