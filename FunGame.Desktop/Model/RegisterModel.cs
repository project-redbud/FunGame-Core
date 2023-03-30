using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class RegisterModel : BaseModel
    {
        private readonly Register Register;
        private SocketObject Work;
        private bool Working = false;

        public RegisterModel(Register reg) : base(RunTime.Socket)
        {
            Register = reg;
        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                if (SocketObject.SocketType == SocketMessageType.Reg || SocketObject.SocketType == SocketMessageType.CheckReg)
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

        public async Task<bool> Reg(params object[]? objs)
        {
            try
            {
                Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string password = "";
                    string email = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) password = (string)objs[1];
                    password = password.Encrypt(username);
                    if (objs.Length > 2) email = (string)objs[2];
                    SetWorking();
                    if (Socket.Send(SocketMessageType.Reg, username, email) == SocketResult.Success)
                    {
                        RegInvokeType InvokeType = await Task.Factory.StartNew(GetRegInvokeType);
                        while (true)
                        {
                            switch (InvokeType)
                            {
                                case RegInvokeType.InputVerifyCode:
                                    string verifycode = ShowMessage.InputMessageCancel("请输入注册邮件中的6位数字验证码", "注册验证码", out MessageResult cancel);
                                    if (cancel != MessageResult.Cancel)
                                    {
                                        SetWorking();
                                        if (Socket.Send(SocketMessageType.CheckReg, username, password, email, verifycode) == SocketResult.Success)
                                        {
                                            bool success = false;
                                            string msg = "";
                                            (success, msg) = await Task.Factory.StartNew(GetRegResult);
                                            ShowMessage.Message(msg, "注册结果");
                                            if (success) return success;
                                        }
                                        break;
                                    }
                                    else return false;
                                case RegInvokeType.DuplicateUserName:
                                    ShowMessage.WarningMessage("此账号名已被注册，请使用其他账号名。");
                                    return false;
                                case RegInvokeType.DuplicateEmail:
                                    ShowMessage.WarningMessage("此邮箱已被使用，请使用其他邮箱注册。");
                                    return false;
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

        private RegInvokeType GetRegInvokeType()
        {
            RegInvokeType type = RegInvokeType.None;
            try
            {
                while (true)
                {
                    if (!Working) break;
                }
                if (Work.Length > 0) type = Work.GetParam<RegInvokeType>(0);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            return type;
        }

        private (bool, string) GetRegResult()
        {
            bool success = false;
            string? msg = "";
            try
            {
                while (true)
                {
                    if (!Working) break;
                }
                if (Work.Length > 0) success = Work.GetParam<bool>(0);
                if (Work.Length > 1) msg = Work.GetParam<string>(1) ?? "";
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            return (success, msg);
        }

        private void SetWorking()
        {
            Working = true;
            Work = default;
        }
    }
}
