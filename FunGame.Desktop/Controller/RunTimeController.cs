using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RunTimeController
    {
        public bool Connected => Do<bool>(MainInvokeType.Connected);

        private RunTimeModel RunTimeModel { get; }
        private Main Main { get; }

        public RunTimeController(Main Main)
        {
            this.Main = Main;
            RunTimeModel = new RunTimeModel(Main);
        }

        /**
         * 从内部去调用Model的方法，并记录日志。
         */
        private T Do<T>(MainInvokeType DoType, params object[] args)
        {
            object result = new();
            switch (DoType)
            {
                case MainInvokeType.GetServerConnection:
                    result = RunTimeModel.GetServerConnection();
                    break;

                case MainInvokeType.Connect:
                    result = RunTimeModel.Connect();
                    if ((ConnectResult)result != ConnectResult.Success)
                    {
                        Main.OnFailedConnectEvent(new GeneralEventArgs());
                        Main.OnAfterConnectEvent(new GeneralEventArgs());
                    }
                    break;

                case MainInvokeType.Connected:
                    result = RunTimeModel.Connected;
                    break;

                case MainInvokeType.Disconnect:
                    if (Main.OnBeforeDisconnectEvent(new GeneralEventArgs()) == EventResult.Fail) return (T)result;
                    RunTimeModel.Disconnect();
                    break;

                case MainInvokeType.Disconnected:
                    break;
                    
                case MainInvokeType.Close:
                    if (args != null && args.Length > 0)
                    {
                        RunTimeModel.Error((Exception)args[0]);
                        result = true;
                    }
                    else
                        result = RunTimeModel.Close();
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

        public bool Close()
        {
            return Do<bool>(MainInvokeType.Close);
        }

        public bool Error(Exception e)
        {
            return Do<bool>(MainInvokeType.Close, e);
        }
    }
}
