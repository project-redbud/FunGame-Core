using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseReg : GeneralForm, IRegEventHandler
    {
        public event IEventHandler.BeforeEventHandler? BeforeReg;
        public event IEventHandler.AfterEventHandler? AfterReg;
        public event IEventHandler.SucceedEventHandler? SucceedReg;
        public event IEventHandler.FailedEventHandler? FailedReg;

        public EventResult OnAfterRegEvent(GeneralEventArgs e)
        {
            if (AfterReg != null)
            {
                return AfterReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeRegEvent(GeneralEventArgs e)
        {
            if (BeforeReg != null)
            {
                return BeforeReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedRegEvent(GeneralEventArgs e)
        {
            if (FailedReg != null)
            {
                return FailedReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedRegEvent(GeneralEventArgs e)
        {
            if (SucceedReg != null)
            {
                return SucceedReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
