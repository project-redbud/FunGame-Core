using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController : ILogin
    {
        LoginModel LoginModel { get; }

        public LoginController(Login Login)
        {
            LoginModel = new LoginModel(Login);
        }

        public bool LoginAccount()
        {
            return LoginModel.LoginAccount();
        }
    }
}
