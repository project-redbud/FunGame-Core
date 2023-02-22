using Milimoe.FunGame.Core.Library.Common.Event;
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

        public static bool LoginAccount(params object[]? objs)
        {
            try
            {
                Core.Library.Common.Network.Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string password = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) password = (string)objs[1];
                    if (Socket.Send(SocketMessageType.Login, username, password) == SocketResult.Success)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                e.GetErrorInfo();
            }
            return false;
        }

        public bool LoginAccount(string username, string password)
        {
            try
            {
                if (LoginModel.LoginAccount(username, password))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                e.GetErrorInfo();
            }
            return false;
        }
    }
}
