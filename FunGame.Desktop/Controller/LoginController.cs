using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;
using System.Windows.Forms;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController : ILogin
    {
        private Login Login { get; }

        public LoginController(Login Login)
        {
            this.Login = Login;
        }

        public static bool LoginAccount(params object[]? objs)
        {
            RunTime.Login?.OnBeforeLoginEvent(new GeneralEventArgs());
            bool result = LoginModel.LoginAccount(objs);
            if (!result)
            {
                RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
            }
            RunTime.Login?.OnAfterLoginEvent(new GeneralEventArgs());
            return result;
        }

        public bool LoginAccount(string username, string password)
        {
            return LoginController.LoginAccount(username, password);
        }

        public static bool CheckLogin(params object[]? objs)
        {
            bool result = LoginModel.CheckLogin(objs);
            if (result)
            {
                RunTime.Login?.OnSucceedLoginEvent(new GeneralEventArgs());
            }
            else
            {
                RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
            }
            return result;
        }

        public bool CheckLogin(Guid key)
        {
            return LoginController.CheckLogin(key);
        }
    }
}
