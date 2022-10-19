using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class BuyItemEvent : IBuyItemEvent
    {
        public event IEvent.BeforeEvent? BeforeBuyItemEvent;
        public event IEvent.AfterEvent? AfterBuyItemEvent;
        public event IEvent.SucceedEvent? SucceedBuyItemEvent;
        public event IEvent.FailedEvent? FailedBuyItemEvent;

        public virtual Enum.EventResult OnBeforeBuyItemEvent(GeneralEventArgs e)
        {
            if (BeforeBuyItemEvent != null)
            {
                return BeforeBuyItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterBuyItemEvent(GeneralEventArgs e)
        {
            if (AfterBuyItemEvent != null)
            {
                return AfterBuyItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedBuyItemEvent(GeneralEventArgs e)
        {
            if (SucceedBuyItemEvent != null)
            {
                return SucceedBuyItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedBuyItemEvent(GeneralEventArgs e)
        {
            if (FailedBuyItemEvent != null)
            {
                return FailedBuyItemEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
