using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ChangeAccountSettingEvent : IChangeAccountSettingEvent
    {
        public event IEvent.BeforeEvent? BeforeChangeAccountSettingEvent;
        public event IEvent.AfterEvent? AfterChangeAccountSettingEvent;
        public event IEvent.SucceedEvent? SucceedChangeAccountSettingEvent;
        public event IEvent.FailedEvent? FailedChangeAccountSettingEvent;

        public virtual EventResult OnBeforeChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (BeforeChangeAccountSettingEvent != null)
            {
                return BeforeChangeAccountSettingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (AfterChangeAccountSettingEvent != null)
            {
                return AfterChangeAccountSettingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (SucceedChangeAccountSettingEvent != null)
            {
                return SucceedChangeAccountSettingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedChangeAccountSettingEvent(GeneralEventArgs e)
        {
            if (FailedChangeAccountSettingEvent != null)
            {
                return FailedChangeAccountSettingEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
