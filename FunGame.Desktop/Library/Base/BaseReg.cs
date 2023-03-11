using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseReg : GeneralForm, IRegEventHandler
    {
        public event IRegEventHandler.BeforeEventHandler? BeforeReg;
        public event IRegEventHandler.AfterEventHandler? AfterReg;
        public event IRegEventHandler.SucceedEventHandler? SucceedReg;
        public event IRegEventHandler.FailedEventHandler? FailedReg;

        public EventResult OnAfterRegEvent(RegisterEventArgs e)
        {
            if (AfterReg != null)
            {
                return AfterReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeRegEvent(RegisterEventArgs e)
        {
            if (BeforeReg != null)
            {
                return BeforeReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedRegEvent(RegisterEventArgs e)
        {
            if (FailedReg != null)
            {
                return FailedReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedRegEvent(RegisterEventArgs e)
        {
            if (SucceedReg != null)
            {
                return SucceedReg(this, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
