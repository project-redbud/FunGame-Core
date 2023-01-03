using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.Others;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController
    {
        private MainModel MainModel { get; }

        public MainController(Main Main)
        {
            MainModel = new MainModel(Main);
        }

        public T Do<T>(string DoType)
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
                    if (MainModel.Socket is null) result = false;
                    else result = MainModel.Socket.Connected;
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
                case MainControllerSet.LogIn:
                    result = MainModel.Login();
                    break;
                case MainControllerSet.Close:
                    result = MainModel.Close();
                    break;
                default:
                    break;
            }
            return (T)result;
        }
    }
}
