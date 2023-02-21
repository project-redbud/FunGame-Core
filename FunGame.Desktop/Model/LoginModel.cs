using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class LoginModel : ILogin
    {
        private readonly Login Login;
        private Core.Library.Common.Network.Socket? Socket;

        public LoginModel(Login login)
        {
            Login = login;
            Socket = RunTime.Socket;
        }

        public bool LoginAccount()
        {
            try
            {
                if (Socket != null && Socket.Send(SocketMessageType.Login, "Mili", "OK") == SocketResult.Success)
                    return true;
            }
            catch (Exception e)
            {
                e.GetErrorInfo();
            }
            return false;
        }
    }
}
