using System.Data;
using System.Windows.Forms;
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

        public LoginModel() : base(RunTime.Socket)
        {

        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                if (SocketObject.SocketType == SocketMessageType.Login || SocketObject.SocketType == SocketMessageType.CheckLogin)
                {
                    Work = SocketObject;
                    Working = false;
                }
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
                    SetWorking();
                    if (Socket.Send(SocketMessageType.Login, username, password, autokey) == SocketResult.Success)
                    {
                        string ErrorMsg = "";
                        Guid CheckLoginKey = Guid.Empty;
                        (CheckLoginKey, ErrorMsg) = await Task.Factory.StartNew(GetCheckLoginKeyAsync);
                        if (ErrorMsg != null && ErrorMsg.Trim() != "")
                        {
                            ShowMessage.ErrorMessage(ErrorMsg, "登录失败");
                            return false;
                        }
                        SetWorking();
                        if (Socket.Send(SocketMessageType.CheckLogin, CheckLoginKey) == SocketResult.Success)
                        {
                            DataSet ds = await Task.Factory.StartNew(GetLoginUserAsync);
                            if (ds != null)
                            {
                                // 创建User对象并返回到Main
                                RunTime.Main?.UpdateUI(MainInvokeType.SetUser, new object[] { Factory.GetInstance<User>(ds) });
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return false;
        }

        public static (Guid, string) GetCheckLoginKeyAsync()
        {
            Guid key = Guid.Empty;
            string? msg = "";
            try
            {
                while (true)
                {
                    if (!Working) break;
                }
                // 返回一个确认登录的Key
                if (Work.Length > 0) key = Work.GetParam<Guid>(0);
                if (Work.Length > 1) msg = Work.GetParam<string>(1);
                if (key != Guid.Empty)
                {
                    Config.Guid_LoginKey = key;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            msg ??= "";
            return (key, msg);
        }

        private static DataSet GetLoginUserAsync()
        {
            DataSet? ds = new();
            try
            {
                while (true)
                {
                    if (!Working) break;
                }
                // 返回构造User对象的DataSet
                if (Work.Length > 0) ds = Work.GetParam<DataSet>(0);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            ds ??= new();
            return ds;
        }

        private static void SetWorking()
        {
            Working = true;
            Work = default;
        }
    }
}
