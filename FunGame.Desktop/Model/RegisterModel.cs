using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class RegisterModel : IReg
    {
        private readonly Register Register;
        private Core.Library.Common.Network.Socket? Socket;

        public RegisterModel(Register register)
        {
            Register = register;
            Socket = RunTime.Socket;
        }

        public bool Reg()
        {
            return true;
        }
    }
}
