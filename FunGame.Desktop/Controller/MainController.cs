using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : IMain
    {
        private MainModel MainModel { get; }

        public bool Connected => Do<bool>(MainControllerSet.Connected);

        public MainController(Main Main)
        {
            MainModel = new MainModel(Main);
        }

        /**
         * 从内部去调用Model的方法，并记录日志。
         */
        private T Do<T>(string DoType)
        {
            object result = new();
            switch(DoType)
            {
                case MainControllerSet.GetServerConnection:
                    result = MainModel.GetServerConnection();
                    break;
                case MainControllerSet.Connect:
                    result = MainModel.Connect();
                    break;
                case MainControllerSet.Connected:
                    result = MainModel.Connected;
                    break;
                case MainControllerSet.Disconnect:
                    MainModel.Disconnect();
                    break;
                case MainControllerSet.Disconnected:
                    MainModel.Disconnect();
                    break;
                case MainControllerSet.WaitConnectAndSetYellow:
                    break;
                case MainControllerSet.WaitLoginAndSetYellow:
                    break;
                case MainControllerSet.SetGreenAndPing:
                    break;
                case MainControllerSet.SetGreen:
                    break;
                case MainControllerSet.SetYellow:
                    break;
                case MainControllerSet.SetRed:
                    break;
                case MainControllerSet.SetUser:
                    break;
                case MainControllerSet.LogOut:
                    result = MainModel.Logout();
                    break;
                case MainControllerSet.Close:
                    result = MainModel.Close();
                    break;
                default:
                    break;
            }
            return (T)result;
        }

        public bool GetServerConnection()
        {
            return Do<bool>(MainControllerSet.GetServerConnection);
        }

        public ConnectResult Connect()
        {
            return Do<ConnectResult>(MainControllerSet.Connect);
        }

        public void Disconnect()
        {
            Do<object>(MainControllerSet.Disconnect);
        }

        public void Disconnected()
        {
            Do<object>(MainControllerSet.Disconnected);
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

        public void SetUser()
        {
            throw new NotImplementedException();
        }

        public bool LogOut()
        {
            return Do<bool>(MainControllerSet.LogOut);
        }

        public bool Close()
        {
            return Do<bool>(MainControllerSet.Close);
        }
    }
}
