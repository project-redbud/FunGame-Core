using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RegisterController
    {
        public static bool Reg(params object[]? objs)
        {

            if (RunTime.Register != null) RunTime.Register.OnBeforeRegEvent(RunTime.Register.EventArgs);
            bool result = RegisterModel.Reg(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("注册失败！！", "注册失败", 5);
                if (RunTime.Register != null) RunTime.Register.OnFailedRegEvent(RunTime.Register.EventArgs);
            }
            return result;
        }

        public static bool CheckReg(params object[]? objs)
        {
            bool result = RegisterModel.CheckReg(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("注册失败！！", "注册失败", 5);
                if (RunTime.Register != null) RunTime.Register.OnFailedRegEvent(RunTime.Register.EventArgs);
            }
            return result;
        }
    }
}
