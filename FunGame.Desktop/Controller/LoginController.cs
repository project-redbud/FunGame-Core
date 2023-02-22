using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController : ILogin
    {
        private LoginModel LoginModel { get; }
        private Login Login { get; }

        public LoginController(Login Login)
        {
            this.Login = Login;
            LoginModel = new LoginModel(Login);
        }

        public static bool LoginAccount(params object[]? objs)
        {
            return LoginModel.LoginAccount(objs);
        }

        public bool LoginAccount(string username, string password)
        {
            Login.OnBeforeLoginEvent(new GeneralEventArgs());
            bool result = LoginModel.LoginAccount(username, password);
            if (result)
            {
                Login.OnSucceedLoginEvent(new GeneralEventArgs());
            }
            else
            {
                Login.OnFailedLoginEvent(new GeneralEventArgs());
            }
            Login.OnAfterLoginEvent(new GeneralEventArgs());
            return result;
        }
    }
}
