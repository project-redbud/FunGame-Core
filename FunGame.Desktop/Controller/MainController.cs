using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : IMain
    {
        public bool Connected => Do<bool>(MainInvokeType.Connected);

        private MainModel MainModel { get; }
        private Main Main { get; }

        public MainController(Main Main)
        {
            this.Main = Main;
            MainModel = new MainModel(Main);
        }

        /**
         * 从内部去调用Model的方法，并记录日志。
         */
        private T Do<T>(MainInvokeType DoType, params object[] args)
        {
            object result = new();
            switch(DoType)
            {
                case MainInvokeType.GetServerConnection:
                    result = MainModel.GetServerConnection();
                    break;

                case MainInvokeType.Connect:
                    result = MainModel.Connect();
                    if ((ConnectResult)result != ConnectResult.Success)
                    {
                        Main.OnFailedConnectEvent(new GeneralEventArgs());
                        Main.OnAfterConnectEvent(new GeneralEventArgs());
                    }
                    break;

                case MainInvokeType.Connected:
                    result = MainModel.Connected;
                    break;

                case MainInvokeType.Disconnect:
                    Main.OnBeforeDisconnectEvent(new GeneralEventArgs());
                    MainModel.Disconnect();
                    break;

                case MainInvokeType.Disconnected:
                    break;

                case MainInvokeType.WaitConnectAndSetYellow:
                    break;

                case MainInvokeType.WaitLoginAndSetYellow:
                    break;

                case MainInvokeType.SetGreenAndPing:
                    break;

                case MainInvokeType.SetGreen:
                    break;

                case MainInvokeType.SetYellow:
                    break;

                case MainInvokeType.SetRed:
                    break;

                case MainInvokeType.LogOut:
                    Main.OnBeforeLogoutEvent(new GeneralEventArgs());
                    result = MainModel.LogOut();
                    break;

                case MainInvokeType.Close:
                    result = MainModel.Close();
                    break;

                case MainInvokeType.IntoRoom:
                    Main.OnBeforeIntoRoomEvent(new GeneralEventArgs());
                    result = MainModel.IntoRoom();
                    break;
                    
                case MainInvokeType.Chat:
                    Main.OnBeforeSendTalkEvent(new GeneralEventArgs());
                    if (args != null && args.Length > 0)
                    result = MainModel.Chat((string)args[0]);
                    break;

                default:
                    break;
            }
            return (T)result;
        }

        public bool GetServerConnection()
        {
            return Do<bool>(MainInvokeType.GetServerConnection);
        }

        public ConnectResult Connect()
        {
            return Do<ConnectResult>(MainInvokeType.Connect);
        }

        public void Disconnect()
        {
            Do<object>(MainInvokeType.Disconnect);
        }

        public void Disconnected()
        {
            Do<object>(MainInvokeType.Disconnected);
        }

        public void SetWaitConnectAndSetYellow()
        {
            throw new NotImplementedException();
        }

        public void SetWaitLoginAndSetYellow()
        {
            throw new NotImplementedException();
        }

        public void SetGreenAndPing()
        {
            throw new NotImplementedException();
        }

        public void SetGreen()
        {
            throw new NotImplementedException();
        }

        public void SetYellow()
        {
            throw new NotImplementedException();
        }

        public void SetRed()
        {
            throw new NotImplementedException();
        }

        public bool LogOut()
        {
            return Do<bool>(MainInvokeType.LogOut);
        }

        public bool Close()
        {
            return Do<bool>(MainInvokeType.Close);
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
