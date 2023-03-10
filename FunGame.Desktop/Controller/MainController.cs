using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : BaseController
    {
        public bool Connected => Do<bool>(MainInvokeType.Connected);

        private MainModel MainModel { get; }
        private Main Main { get; }

        public MainController(Main Main)
        {
            this.Main = Main;
            MainModel = new MainModel(Main);
        }

        public override void Dispose()
        {
            MainModel.Dispose();
        }

        /**
         * 从内部去调用Model的方法，并记录日志。
         */
        private T Do<T>(MainInvokeType DoType, params object[] args)
        {
            object result = new();
            switch(DoType)
            {
                case MainInvokeType.LogOut:
                    if (Main.OnBeforeLogoutEvent(new GeneralEventArgs()) == EventResult.Fail) return (T)result;
                    result = MainModel.LogOut();
                    break;

                case MainInvokeType.IntoRoom:
                    if (Main.OnBeforeIntoRoomEvent(new GeneralEventArgs()) == EventResult.Fail) return (T)result;
                    result = MainModel.IntoRoom();
                    break;
                    
                case MainInvokeType.Chat:
                    if (Main.OnBeforeSendTalkEvent(new GeneralEventArgs()) == EventResult.Fail) return (T)result;
                    if (args != null && args.Length > 0)
                    result = MainModel.Chat((string)args[0]);
                    break;

                default:
                    break;
            }
            return (T)result;
        }

        public bool LogOut()
        {
            return Do<bool>(MainInvokeType.LogOut);
        }

        public bool IntoRoom()
        {
            return Do<bool>(MainInvokeType.IntoRoom);
        }

        public bool Chat(string msg)
        {
            return Do<bool>(MainInvokeType.Chat, msg);
        }
    }
}
