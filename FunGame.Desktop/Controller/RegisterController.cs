using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RegisterController
    {
        private readonly Register Register;
        private readonly RegisterModel RegModel;

        public RegisterController(Register reg)
        {
            Register = reg;
            RegModel = new RegisterModel(reg);
        }

        public void SocketHandler(SocketMessageType type, params object[]? objs)
        {
            RegModel.SocketHandler(type, objs);
        }

        public bool Reg(params object[]? objs)
        {
            if (Register.OnBeforeRegEvent(Register.EventArgs) == EventResult.Fail) return false;
            bool result = RegModel.Reg(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("注册失败！", "注册失败", 5);
                Register.OnFailedRegEvent(Register.EventArgs);
            }
            return result;
        }

        public bool CheckReg(params object[]? objs)
        {
            bool result = RegModel.CheckReg(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("注册失败！", "注册失败", 5);
                RunTime.Register?.OnFailedRegEvent(Register.EventArgs);
            }
            return result;
        }
    }
}
