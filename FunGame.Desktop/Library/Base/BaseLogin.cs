using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseLogin : GeneralForm, ILoginEventHandler
    {
        public event IEventHandler.BeforeEventHandler? BeforeLogin;
        public event IEventHandler.AfterEventHandler? AfterLogin;
        public event IEventHandler.SucceedEventHandler? SucceedLogin;
        public event IEventHandler.FailedEventHandler? FailedLogin;

        public EventResult OnAfterLoginEvent(GeneralEventArgs e)
        {
            if (AfterLogin != null)
            {
                return AfterLogin(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLoginEvent(GeneralEventArgs e)
        {
            if (BeforeLogin != null)
            {
                return BeforeLogin(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedLoginEvent(GeneralEventArgs e)
        {
            if (FailedLogin != null)
            {
                return FailedLogin(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLoginEvent(GeneralEventArgs e)
        {
            if (SucceedLogin != null)
            {
                return SucceedLogin(this, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
