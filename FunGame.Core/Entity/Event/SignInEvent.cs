using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class SignInEvent : ISignInEvent
    {
        public event IEvent.BeforeEvent? BeforeSignInEvent;
        public event IEvent.AfterEvent? AfterSignInEvent;
        public event IEvent.SucceedEvent? SucceedSignInEvent;
        public event IEvent.FailedEvent? FailedSignInEvent;

        public virtual Enum.EventResult OnBeforeSignInEvent(GeneralEventArgs e)
        {
            if (BeforeSignInEvent != null)
            {
                return BeforeSignInEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterSignInEvent(GeneralEventArgs e)
        {
            if (AfterSignInEvent != null)
            {
                return AfterSignInEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedSignInEvent(GeneralEventArgs e)
        {
            if (SucceedSignInEvent != null)
            {
                return SucceedSignInEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedSignInEvent(GeneralEventArgs e)
        {
            if (FailedSignInEvent != null)
            {
                return FailedSignInEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
