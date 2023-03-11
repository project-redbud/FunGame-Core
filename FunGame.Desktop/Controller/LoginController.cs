using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

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
            if (RunTime.Login?.OnBeforeLoginEvent(Login.EventArgs) == Core.Library.Constant.EventResult.Fail) return false;
            bool result = LoginModel.LoginAccount(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                RunTime.Login?.OnFailedLoginEvent(Login.EventArgs);
            }
            return result;
        }

        public static bool CheckLogin(params object[]? objs)
        {
            bool result = LoginModel.CheckLogin(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                RunTime.Login?.OnFailedLoginEvent(Login.EventArgs);
            }
            return result;
        }
    }
}
