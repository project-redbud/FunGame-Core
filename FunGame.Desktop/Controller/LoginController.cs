using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController : BaseController
    {
        private readonly LoginModel LoginModel;

        public LoginController()
        {
            LoginModel = new LoginModel();
        }

        public override void Dispose()
        {
            LoginModel.Dispose();
        }

        public static bool LoginAccount(params object[]? objs)
        {
            LoginEventArgs LoginEventArgs = new(objs);
            if (RunTime.Login?.OnBeforeLoginEvent(LoginEventArgs) == Core.Library.Constant.EventResult.Fail) return false;
            bool result = LoginModel.LoginAccountAsync(objs).Result;
            if (result) RunTime.Login?.OnSucceedLoginEvent(LoginEventArgs);
            else RunTime.Login?.OnFailedLoginEvent(LoginEventArgs);
            RunTime.Login?.OnAfterLoginEvent(LoginEventArgs);
            return result;
        }
    }
}
