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

        public RegisterModel(Register reg) : base(RunTime.Socket)
        {
            Register = reg;
        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                SocketMessageType type = SocketObject.SocketType;
                object[] objs = SocketObject.Parameters;
                switch (SocketObject.SocketType)
                {
                    case SocketMessageType.Reg:
                        RegInvokeType invokeType = RegInvokeType.None;
                        if (objs != null && objs.Length > 0)
                        {
                            invokeType = SocketObject.GetParam<RegInvokeType>(0);
                            Register.UpdateUI(invokeType);
                        }
                        break;
                    case SocketMessageType.CheckReg:
                        SocketHandler_CheckReg(SocketObject);
                        break;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        private void SocketHandler_CheckReg(SocketObject SocketObject)
        {
            if (SocketObject.Parameters != null && SocketObject.Parameters.Length > 1)
            {
                bool successful = SocketObject.GetParam<bool>(0)!;
                string msg = SocketObject.GetParam<string>(1)!;
                ShowMessage.Message(msg, "注册结果");
                if (successful)
                {
                    Register.OnSucceedRegEvent(Register.EventArgs);
                    Register.OnAfterRegEvent(Register.EventArgs);
                }
            }
            Register.OnFailedRegEvent(Register.EventArgs);
            Register.OnAfterRegEvent(Register.EventArgs);
            Register.UpdateUI(RegInvokeType.InputVerifyCode);
        }

        public bool Reg(params object[]? objs)
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
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            return false;
        }

        public bool CheckReg(params object[]? objs)
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
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            return false;
        }
    }
}
