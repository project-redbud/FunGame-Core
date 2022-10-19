using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class ShowRankingEvent : IShowRankingEvent
    {
        public event IEvent.BeforeEvent? BeforeShowRankingEvent;
        public event IEvent.AfterEvent? AfterShowRankingEvent;
        public event IEvent.SucceedEvent? SucceedShowRankingEvent;
        public event IEvent.FailedEvent? FailedShowRankingEvent;

        public virtual Enum.EventResult OnBeforeShowRankingEvent(GeneralEventArgs e)
        {
            if (BeforeShowRankingEvent != null)
            {
                return BeforeShowRankingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterShowRankingEvent(GeneralEventArgs e)
        {
            if (AfterShowRankingEvent != null)
            {
                return AfterShowRankingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedShowRankingEvent(GeneralEventArgs e)
        {
            if (SucceedShowRankingEvent != null)
            {
                return SucceedShowRankingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedShowRankingEvent(GeneralEventArgs e)
        {
            if (FailedShowRankingEvent != null)
            {
                return FailedShowRankingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
