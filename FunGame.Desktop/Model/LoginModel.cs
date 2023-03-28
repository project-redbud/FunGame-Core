using System.Data;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Model
{
    /// <summary>
    /// 请不要越过Controller直接调用Model中的方法。
    /// </summary>
    public class LoginModel : BaseModel
    {
        private static SocketObject Work;
        private static bool Working = false;
        private static bool Success = false;

        public LoginModel() : base(RunTime.Socket)
        {

        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                Work = SocketObject;
                Success = true;
                Working = false;
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        public static async Task<bool> LoginAccountAsync(params object[]? objs)
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
                        SetWorking();
                        await Task.Factory.StartNew(async() =>
                        {
                            while (true)
                            {
                                if (!Working) break;
                            }
                            if (Success)
                            {
                                await CheckLoginAsync(Work);
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
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
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            return false;
        }

        private static async Task<bool> CheckLoginAsync(SocketObject SocketObject)
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
                return false;
            }
            else
            {
                if (key != Guid.Empty)
                {
                    Config.Guid_LoginKey = key;
                    Socket Socket = RunTime.Socket!;
                    if (Socket.Send(SocketMessageType.CheckLogin, key) == SocketResult.Success)
                    {
                        SetWorking();
                        await Task.Factory.StartNew(() =>
                        {
                            while (true)
                            {
                                if (!Working) break;
                            }
                            if (Success)
                            {
                                SocketHandler_CheckLogin(Work);
                            }
                        });
                        return true;
                    }
                }
                else
                {
                    ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                    return false;
                }
            }
            return true;
        }

        private static bool SocketHandler_CheckLogin(SocketObject SocketObject)
        {
            // 返回构造User对象的DataTable
            object[] objs = SocketObject.Parameters;
            if (objs != null && objs.Length > 0)
            {
                DataSet? DataSet = SocketObject.GetParam<DataSet>(0);
                // 创建User对象并返回到Main
                RunTime.Main?.UpdateUI(MainInvokeType.SetUser, new object[] { Factory.GetInstance<User>(DataSet) });
                return true;
            }
            ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
            return false;
        }

        private static void SetWorking()
        {
            Working = true;
            Success = false;
            Work = default;
        }
    }
}
