using Milimoe.FunGame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class ChangeAccountSettingEvent : IChangeAccountSettingEvent
    {
        public event IEvent.BeforeEvent? BeforeChangeAccountSettingEvent;
        public event IEvent.AfterEvent? AfterChangeAccountSettingEvent;
        public event IEvent.SucceedEvent? SucceedChangeAccountSettingEvent;
        public event IEvent.FailedEvent? FailedChangeAccountSettingEvent;

        public virtual Enum.EventResult OnBeforeChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (BeforeChangeAccountSettingEvent != null)
            {
                return BeforeChangeAccountSettingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnAfterChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (AfterChangeAccountSettingEvent != null)
            {
                return AfterChangeAccountSettingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnSucceedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (SucceedChangeAccountSettingEvent != null)
            {
                return SucceedChangeAccountSettingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }

        public virtual Enum.EventResult OnFailedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (FailedChangeAccountSettingEvent != null)
            {
                return FailedChangeAccountSettingEvent(this, e);
            }

            return Enum.EventResult.Fail;
        }
    }
}
