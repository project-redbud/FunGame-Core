using System.Data;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    /// <summary>
    /// 请不要越过Controller直接调用Model中的方法。
    /// </summary>
    public class LoginModel : BaseModel
    {
        public LoginModel() : base(RunTime.Socket)
        {

        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                SocketMessageType type = SocketObject.SocketType;
                object[] objs = SocketObject.Parameters;
                switch (SocketObject.SocketType)
                {
                    case SocketMessageType.Login:
                        SocketHandler_Login(SocketObject);
                        break;

                    case SocketMessageType.CheckLogin:
                        SocketHandler_CheckLogin(SocketObject);
                        break;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        public static bool LoginAccount(params object[]? objs)
        {
            try
            {
                Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string password = "";
                    string autokey = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) password = (string)objs[1];
                    if (objs.Length > 2) autokey = (string)objs[2];
                    password = password.Encrypt(username);
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
                Socket? Socket = RunTime.Socket;
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

        private static void SocketHandler_Login(SocketObject SocketObject)
        {
            Guid key = Guid.Empty;
            string? msg = "";
            // 返回一个Key，再发回去给服务器就行了
            if (SocketObject.Length > 0) key = SocketObject.GetParam<Guid>(0);
            if (SocketObject.Length > 1) msg = SocketObject.GetParam<string>(1);
            // 如果返回了msg，说明验证错误。
            if (msg != null && msg.Trim() != "")
            {
                ShowMessage.ErrorMessage(msg, "登录失败");
                RunTime.Login?.OnFailedLoginEvent(Login.EventArgs);
                RunTime.Login?.OnAfterLoginEvent(Login.EventArgs);
            }
            else
            {
                if (key != Guid.Empty)
                {
                    Config.Guid_LoginKey = key;
                    LoginController.CheckLogin(key);
                }
                else
                {
                    ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                    RunTime.Login?.OnFailedLoginEvent(Login.EventArgs);
                    RunTime.Login?.OnAfterLoginEvent(Login.EventArgs);
                }
            }
        }

        private static void SocketHandler_CheckLogin(SocketObject SocketObject)
        {
            // 返回构造User对象的DataTable
            object[] objs = SocketObject.Parameters;
            if (objs != null && objs.Length > 0)
            {
                DataSet? DataSet = SocketObject.GetParam<DataSet>(0);
                // 创建User对象并返回到Main
                RunTime.Main?.UpdateUI(MainInvokeType.SetUser, new object[] { Factory.GetInstance<User>(DataSet) });
                RunTime.Login?.OnSucceedLoginEvent(Login.EventArgs);
                RunTime.Login?.OnAfterLoginEvent(Login.EventArgs);
                return;
            }
            ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
            RunTime.Login?.OnFailedLoginEvent(Login.EventArgs);
            RunTime.Login?.OnAfterLoginEvent(Login.EventArgs);
        }

    }
}
