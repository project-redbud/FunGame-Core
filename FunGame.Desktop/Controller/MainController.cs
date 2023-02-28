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
        public bool Connected => Do<bool>(MainSet.Connected);

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
        private T Do<T>(string DoType, params object[] args)
        {
            object result = new();
            switch(DoType)
            {
                case MainSet.GetServerConnection:
                    result = MainModel.GetServerConnection();
                    break;

                case MainSet.Connect:
                    result = MainModel.Connect();
                    if ((ConnectResult)result != ConnectResult.Success)
                    {
                        Main.OnFailedConnectEvent(new GeneralEventArgs());
                        Main.OnAfterConnectEvent(new GeneralEventArgs());
                    }
                    break;

                case MainSet.Connected:
                    result = MainModel.Connected;
                    break;

                case MainSet.Disconnect:
                    Main.OnBeforeDisconnectEvent(new GeneralEventArgs());
                    MainModel.Disconnect();
                    break;

                case MainSet.Disconnected:
                    break;

                case MainSet.WaitConnectAndSetYellow:
                    break;

                case MainSet.WaitLoginAndSetYellow:
                    break;

                case MainSet.SetGreenAndPing:
                    break;

                case MainSet.SetGreen:
                    break;

                case MainSet.SetYellow:
                    break;

                case MainSet.SetRed:
                    break;

                case MainSet.LogOut:
                    Main.OnBeforeLogoutEvent(new GeneralEventArgs());
                    result = MainModel.LogOut();
                    break;

                case MainSet.Close:
                    result = MainModel.Close();
                    break;

                case MainSet.IntoRoom:
                    Main.OnBeforeIntoRoomEvent(new GeneralEventArgs());
                    result = MainModel.IntoRoom();
                    break;
                    
                case MainSet.Chat:
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
            return Do<bool>(MainSet.GetServerConnection);
        }

        public ConnectResult Connect()
        {
            return Do<ConnectResult>(MainSet.Connect);
        }

        public void Disconnect()
        {
            Do<object>(MainSet.Disconnect);
        }

        public void Disconnected()
        {
            Do<object>(MainSet.Disconnected);
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
            return Do<bool>(MainSet.LogOut);
        }

        public bool Close()
        {
            return Do<bool>(MainSet.Close);
        }

        public bool IntoRoom()
        {
            return Do<bool>(MainSet.IntoRoom);
        }

        public bool Chat(string msg)
        {
            return Do<bool>(MainSet.Chat, msg);
        }
    }
}
