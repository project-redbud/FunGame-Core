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
            RunTime.Register?.OnBeforeRegEvent(new GeneralEventArgs());
            bool result = RegisterModel.Reg(objs);
            if (!result)
            {
                ShowMessage.ErrorMessage("注册失败！！", "注册失败", 5);
                RunTime.Register?.OnFailedRegEvent(new GeneralEventArgs());
            }
            return result;
        }
    }
}
