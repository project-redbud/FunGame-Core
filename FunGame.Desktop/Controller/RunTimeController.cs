using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RunTimeController
    {
        public bool Connected => Do<bool>(RunTimeInvokeType.Connected);

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
        private T Do<T>(RunTimeInvokeType DoType, params object[] args)
        {
            object result = new();
            switch (DoType)
            {
                case RunTimeInvokeType.GetServerConnection:
                    result = RunTimeModel.GetServerConnection();
                    break;

                case RunTimeInvokeType.Connect:
                    result = RunTimeModel.Connect();
                    if ((ConnectResult)result != ConnectResult.Success)
                    {
                        Main.OnFailedConnectEvent(new GeneralEventArgs());
                        Main.OnAfterConnectEvent(new GeneralEventArgs());
                    }
                    break;

                case RunTimeInvokeType.Connected:
                    result = RunTimeModel.Connected;
                    break;

                case RunTimeInvokeType.Disconnect:
                    if (Main.OnBeforeDisconnectEvent(new GeneralEventArgs()) == EventResult.Fail) return (T)result;
                    RunTimeModel.Disconnect();
                    break;

                case RunTimeInvokeType.Disconnected:
                    break;

                case RunTimeInvokeType.AutoLogin:
                    break;
                    
                case RunTimeInvokeType.Close:
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
            return Do<bool>(RunTimeInvokeType.GetServerConnection);
        }

        public ConnectResult Connect()
        {
            return Do<ConnectResult>(RunTimeInvokeType.Connect);
        }

        public void Disconnect()
        {
            Do<object>(RunTimeInvokeType.Disconnect);
        }

        public void Disconnected()
        {
            Do<object>(RunTimeInvokeType.Disconnected);
        }

        public bool Close()
        {
            return Do<bool>(RunTimeInvokeType.Close);
        }

        public bool Error(Exception e)
        {
            return Do<bool>(RunTimeInvokeType.Close, e);
        }

        public async Task AutoLogin(params object[] objs)
        {
            try
            {
                Do<object>(RunTimeInvokeType.AutoLogin);
                LoginController LoginController = new();
                await LoginController.LoginAccount(objs);
                LoginController.Dispose();
            }
            catch (Exception e)
            {
                RunTime.WriteGameInfo(e.GetErrorInfo());
            }
        }
    }
}
