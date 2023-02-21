using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RegisterController : IReg
    {
        RegisterModel RegisterModel { get; }

        public RegisterController(Register Register)
        {
            RegisterModel = new RegisterModel(Register);
        }

        public bool Reg()
        {
            return RegisterModel.Reg();
        }
    }
}
