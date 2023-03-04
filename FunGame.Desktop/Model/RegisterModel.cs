using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;

namespace Milimoe.FunGame.Desktop.Model
{
    public class RegisterModel
    {
        public static bool Reg(params object[]? objs)
        {
            try
            {
                Core.Library.Common.Network.Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string email = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) email = (string)objs[1];
                    if (Socket.Send(SocketMessageType.Reg, username, email) == SocketResult.Success)
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

        public static bool CheckReg(params object[]? objs)
        {
            try
            {
                Core.Library.Common.Network.Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string password = "";
                    string email = "";
                    string verifycode = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) password = (string)objs[1];
                    password = password.Encrypt(username);
                    if (objs.Length > 2) email = (string)objs[2];
                    if (objs.Length > 3) verifycode = (string)objs[3];
                    if (Socket.Send(SocketMessageType.CheckReg, username, password, email, verifycode) == SocketResult.Success)
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
    }
}
