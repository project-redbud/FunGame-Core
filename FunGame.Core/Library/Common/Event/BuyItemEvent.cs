using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class BuyItemEvent : IBuyItemEvent
    {
        public event IEvent.BeforeEvent? BeforeBuyItemEvent;
        public event IEvent.AfterEvent? AfterBuyItemEvent;
        public event IEvent.SucceedEvent? SucceedBuyItemEvent;
        public event IEvent.FailedEvent? FailedBuyItemEvent;

        public virtual EventResult OnBeforeBuyItemEvent(GeneralEventArgs e)
        {
            if (BeforeBuyItemEvent != null)
            {
                return BeforeBuyItemEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterBuyItemEvent(GeneralEventArgs e)
        {
            if (AfterBuyItemEvent != null)
            {
                return AfterBuyItemEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedBuyItemEvent(GeneralEventArgs e)
        {
            if (SucceedBuyItemEvent != null)
            {
                return SucceedBuyItemEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedBuyItemEvent(GeneralEventArgs e)
        {
            if (FailedBuyItemEvent != null)
            {
                return FailedBuyItemEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
