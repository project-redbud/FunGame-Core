using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Core.Library.Exception;

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

        public static async Task<bool> LoginAccount(params object[]? objs)
        {
            bool result = false;

            try
            {
                LoginEventArgs LoginEventArgs = new(objs);
                if (RunTime.Login?.OnBeforeLoginEvent(LoginEventArgs) == Core.Library.Constant.EventResult.Fail) return false;

                result = await LoginModel.LoginAccountAsync(objs);

                if (result) RunTime.Login?.OnSucceedLoginEvent(LoginEventArgs);
                else RunTime.Login?.OnFailedLoginEvent(LoginEventArgs);
                RunTime.Login?.OnAfterLoginEvent(LoginEventArgs);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }
    }
}
