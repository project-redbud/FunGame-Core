using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Desktop.Library;

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
                    string roomid = new("-1");
                    if (args != null && args.Length > 0) roomid = (string)args[0];
                    if (Main.OnBeforeIntoRoomEvent(new RoomEventArgs(roomid)) == EventResult.Fail) return (T)result;
                    result = MainModel.IntoRoom(roomid);
                    break;

                case MainInvokeType.UpdateRoom:
                    result = MainModel.UpdateRoom();
                    break;
                    
                case MainInvokeType.QuitRoom:
                    roomid = new("-1");
                    if (args != null && args.Length > 0) roomid = (string)args[0];
                    if (Main.OnBeforeQuitRoomEvent(new RoomEventArgs(roomid)) == EventResult.Fail) return (T)result;
                    result = MainModel.QuitRoom(roomid);
                    break;
                    
                case MainInvokeType.CreateRoom:
                    if (Main.OnBeforeCreateRoomEvent(new RoomEventArgs()) == EventResult.Fail) return (T)result;
                    result = MainModel.CreateRoom();
                    break;
                    
                case MainInvokeType.Chat:
                    string msg = "";
                    if (args != null && args.Length > 0) msg = (string)args[0];
                    if (Main.OnBeforeSendTalkEvent(new SendTalkEventArgs(msg)) == EventResult.Fail) return (T)result;
                    if (msg.Trim() != "") result = MainModel.Chat(msg);
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

        public bool UpdateRoom()
        {
            return Do<bool>(MainInvokeType.UpdateRoom);
        }

        public bool IntoRoom(string roomid)
        {
            return Do<bool>(MainInvokeType.IntoRoom, roomid);
        }
        
        public bool QuitRoom(string roomid)
        {
            return Do<bool>(MainInvokeType.QuitRoom, roomid);
        }
        
        public bool CreateRoom()
        {
            return Do<bool>(MainInvokeType.CreateRoom);
        }

        public bool Chat(string msg)
        {
            return Do<bool>(MainInvokeType.Chat, msg);
        }
    }
}
