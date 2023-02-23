using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;

namespace Milimoe.FunGame.Desktop.Model
{
    /// <summary>
    /// 请不要越过Controller直接调用Model中的方法。
    /// </summary>
    public class LoginModel
    {
        public static bool LoginAccount(params object[]? objs)
        {
            try
            {
                Core.Library.Common.Network.Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string password = "";
                    string autokey = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) password = (string)objs[1];
                    if (objs.Length > 2) autokey = (string)objs[2];
                    if (Socket.Send(SocketMessageType.Login, username, password, autokey) == SocketResult.Success)
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

        public static bool CheckLogin(params object[]? objs)
        {
            try
            {
                Core.Library.Common.Network.Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    Guid key = Guid.Empty;
                    if (objs.Length > 0) key = (Guid)objs[0];
                    if (Socket.Send(SocketMessageType.CheckLogin, key) == SocketResult.Success)
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
