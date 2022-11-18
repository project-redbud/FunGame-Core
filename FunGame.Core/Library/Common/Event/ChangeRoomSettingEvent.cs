using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ChangeRoomSettingEvent : IChangeRoomSettingEvent
    {
        public event IEvent.BeforeEvent? BeforeChangeRoomSettingEvent;
        public event IEvent.AfterEvent? AfterChangeRoomSettingEvent;
        public event IEvent.SucceedEvent? SucceedChangeRoomSettingEvent;
        public event IEvent.FailedEvent? FailedChangeRoomSettingEvent;

        public virtual EventResult OnBeforeChangeRoomSettingEvent(GeneralEventArgs e)
        {
            if (BeforeChangeRoomSettingEvent != null)
            {
                return BeforeChangeRoomSettingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnAfterChangeRoomSettingEvent(GeneralEventArgs e)
        {
            if (AfterChangeRoomSettingEvent != null)
            {
                return AfterChangeRoomSettingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnSucceedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            if (SucceedChangeRoomSettingEvent != null)
            {
                return SucceedChangeRoomSettingEvent(this, e);
            }

            return EventResult.Fail;
        }

        public virtual EventResult OnFailedChangeRoomSettingEvent(GeneralEventArgs e)
        {
            if (FailedChangeRoomSettingEvent != null)
            {
                return FailedChangeRoomSettingEvent(this, e);
            }

            return EventResult.Fail;
        }
    }
}
