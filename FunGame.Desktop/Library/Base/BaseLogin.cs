using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseLogin : GeneralForm, ILoginEventHandler
    {
        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;
        public event ILoginEventHandler.SucceedEventHandler? SucceedLogin;
        public event ILoginEventHandler.FailedEventHandler? FailedLogin;

        public EventResult OnAfterLoginEvent(LoginEventArgs e)
        {
            if (AfterLogin != null)
            {
                return AfterLogin.Invoke(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnBeforeLoginEvent(LoginEventArgs e)
        {
            if (BeforeLogin != null)
            {
                return BeforeLogin.Invoke(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnFailedLoginEvent(LoginEventArgs e)
        {
            if (FailedLogin != null)
            {
                return FailedLogin.Invoke(this, e);
            }
            else return EventResult.NoEventImplement;
        }

        public EventResult OnSucceedLoginEvent(LoginEventArgs e)
        {
            if (SucceedLogin != null)
            {
                return SucceedLogin.Invoke(this, e);
            }
            else return EventResult.NoEventImplement;
        }
    }
}
