using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class RegisterModel
    {
        private readonly Register Register;

        public RegisterModel(Register reg)
        {
            Register = reg;
        }

        public void SocketHandler(SocketMessageType type, params object[]? objs)
        {
            try
            {
                switch (type)
                {
                    case SocketMessageType.Reg:
                        RegInvokeType invokeType = RegInvokeType.None;
                        if (objs != null && objs.Length > 0)
                        {
                            invokeType = NetworkUtility.ConvertJsonObject<RegInvokeType>(objs[0]);
                            Register.UpdateUI(invokeType);
                        }
                        break;
                    case SocketMessageType.CheckReg:
                        SocketHandler_CheckReg(objs);
                        break;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        private void SocketHandler_CheckReg(params object[]? objs)
        {
            if (objs != null && objs.Length > 1)
            {
                bool successful = NetworkUtility.ConvertJsonObject<bool>(objs[0])!;
                string msg = NetworkUtility.ConvertJsonObject<string>(objs[1])!;
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
                e.GetErrorInfo();
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
                e.GetErrorInfo();
            }
            return false;
        }
    }
}
