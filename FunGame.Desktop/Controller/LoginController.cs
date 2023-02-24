using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController
    {
        public static bool LoginAccount(params object[]? objs)
        {
            RunTime.Login?.OnBeforeLoginEvent(new GeneralEventArgs());
            bool result = LoginModel.LoginAccount(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
            }
            return result;
        }

        public static bool CheckLogin(params object[]? objs)
        {
            bool result = LoginModel.CheckLogin(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
            }
            return result;
        }
    }
}
