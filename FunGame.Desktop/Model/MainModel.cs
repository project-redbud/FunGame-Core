using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Others;
using Milimoe.FunGame.Desktop.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Desktop.Model
{
    public class MainModel
    {
        public Core.Library.Common.Network.Socket? Socket { get; private set; }
        public Main Main { get; }

        private int CurrentRetryTimes = -1;
        private int MaxRetryTimes { get; } = SocketSet.MaxRetryTimes;
        private Task? ReceivingTask;

        public MainModel(Main main)
        {
            Main = main;
        }

        public bool Login()
        {
            try
            {
                if (Socket != null && Socket.Send(SocketMessageType.Login, "Mili", "OK") == SocketResult.Success)
                    return true;
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetErrorInfo());
            }
            return false;
        }

        public bool Logout()
        {
            try
            {
                //Socket?.Send(SocketMessageType.Logout, "");
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetErrorInfo());
            }
            return false;
        }

        public void Disconnect()
        {
            try
            {
                Socket?.Send(SocketMessageType.Disconnect, "");
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetErrorInfo());
            }
        }

        public bool GetServerConnection()
        {
            try
            {
                // 获取服务器IP
                string? ipaddress = (string?)Implement.GetFunGameImplValue(InterfaceType.IClient, InterfaceMethod.RemoteServerIP);
                if (ipaddress != null)
                {
                    string[] s = ipaddress.Split(':');
                    if (s != null && s.Length > 1)
                    {
                        Others.Constant.SERVER_IPADRESS = s[0];
                        Others.Constant.SERVER_PORT = Convert.ToInt32(s[1]);
                        if (Connect() == ConnectResult.Success) return true; // 连接服务器
                    }
                }
                else
                {
                    ShowMessage.ErrorMessage("查找可用的服务器失败！");
                    Config.FunGame_isRetrying = false;
                    throw new Exception("查找可用的服务器失败，请重启FunGame。");
                }
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetErrorInfo(), false);
            }

            return false;
        }

        public ConnectResult Connect()
        {
            try
            {
                if (Others.Constant.SERVER_IPADRESS == "" || Others.Constant.SERVER_PORT <= 0)
                {
                    ShowMessage.ErrorMessage("查找可用的服务器失败！");
                    return ConnectResult.FindServerFailed;
                }
                while (true)
                {
                    if (Others.Config.FunGame_isRetrying)
                    {
                        Main?.GetMessage("正在连接服务器，请耐心等待。");
                        Config.FunGame_isRetrying= false;
                        return ConnectResult.CanNotConnect;
                    }
                    if (!Others.Config.FunGame_isConnected)
                    {
                        CurrentRetryTimes++;
                        if (CurrentRetryTimes == 0) Main?.GetMessage("开始连接服务器...", true, TimeType.General);
                        else Main?.GetMessage("第" + CurrentRetryTimes + "次重试连接服务器...");
                        // 超过重连次数上限
                        if (CurrentRetryTimes + 1 > MaxRetryTimes)
                        {
                            throw new Exception("无法连接至服务器，请检查网络并重启游戏再试。");
                        }
                        // 与服务器建立连接
                        Socket?.Close();
                        Others.Config.FunGame_isRetrying = true;
                        Socket = Core.Library.Common.Network.Socket.Connect(Others.Constant.SERVER_IPADRESS, Others.Constant.SERVER_PORT);
                        if (Socket != null && Socket.Connected)
                        {
                            // 发送连接请求
                            if (Socket.Send(SocketMessageType.Connect) == SocketResult.Success)
                            {
                                Task t = Task.Factory.StartNew(() =>
                                {
                                    if (Receiving() == SocketMessageType.Connect)
                                    {
                                        Main?.GetMessage("连接服务器成功，请登录账号以体验FunGame。");
                                        Main?.UpdateUI(MainControllerSet.Connected);
                                        StartReceiving();
                                        return ConnectResult.Success;
                                    }
                                    return ConnectResult.ConnectFailed;
                                });
                                t.Wait(5000);
                                Main?.GetMessage("ERROR: 连接超时，远程服务器没有回应。", false);
                            }
                            Socket?.Close();
                            Config.FunGame_isRetrying = false;
                            return ConnectResult.ConnectFailed;
                        }
                    }
                    else
                    {
                        Main?.GetMessage("已连接至服务器，请勿重复连接。");
                        return ConnectResult.CanNotConnect;
                    }
                }
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetErrorInfo(), false);
                Config.FunGame_isRetrying = false;
            }

            return ConnectResult.ConnectFailed;
        }

        public bool Close()
        {
            try
            {
                if (Socket != null)
                {
                    Socket.Close();
                    Socket = null;
                }
                if (ReceivingTask != null && !ReceivingTask.IsCompleted)
                {
                    ReceivingTask.Wait(1);
                    ReceivingTask = null;
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), false);
                return false;
            }
            return true;
        }

        private void StartReceiving()
        {
            ReceivingTask = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                while (Socket != null && Socket.Connected)
                {
                    Receiving();
                }
            });
            Socket?.StartReceiving(ReceivingTask);
        }

        private object[] GetServerMessage()
        {
            if (Socket != null && Socket.Connected)
            {
                return Socket.Receive();
            }
            return new object[2] { SocketMessageType.Unknown, Array.Empty<object>() };
        }

        private SocketMessageType Receiving()
        {
            if (Socket is null) return SocketMessageType.Unknown;
            SocketMessageType result = SocketMessageType.Unknown;
            try
            {
                object[] ServerMessage = GetServerMessage();
                SocketMessageType type = (SocketMessageType)ServerMessage[0];
                object[] objs = (object[])ServerMessage[1];

                result = type;
                switch (type)
                {
                    case SocketMessageType.Connect:
                        SocketHandle_Connect(objs);
                        break;

                    case SocketMessageType.GetNotice:
                        SocketHandle_GetNotice(objs);
                        break;

                    case SocketMessageType.Login:
                        break;

                    case SocketMessageType.CheckLogin:
                        SocketHandle_CheckLogin(objs);
                        break;

                    case SocketMessageType.Logout:
                        break;

                    case SocketMessageType.Disconnect:
                        SocketHandle_Disconnect(objs);
                        break;

                    case SocketMessageType.HeartBeat:
                        if (Socket.Connected && Usercfg.LoginUser != null)
                            Main?.UpdateUI(MainControllerSet.SetGreenAndPing);
                        break;

                    case SocketMessageType.Unknown:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                // 报错中断服务器连接
                Main?.GetMessage(e.GetErrorInfo(), false);
                Main?.UpdateUI(MainControllerSet.Disconnected);
                Close();
            }

            return result;
        }

        private void SocketHandle_Connect(object[] objs)
        {
            string msg = "";
            if (objs.Length > 0) msg = (string)objs[0];
            string[] strings = msg.Split(';');
            string ServerName = strings[0];
            string ServerNotice = strings[1];
            Config.FunGame_ServerName = ServerName;
            Config.FunGame_Notice = ServerNotice;
            if (objs.Length > 1) msg = (string)objs[1];
            Socket!.Token = msg;
            Main?.GetMessage($"已连接服务器：{ServerName}。\n\n********** 服务器公告 **********\n\n{ServerNotice}\n\n");
            // 设置等待登录的黄灯
            Main?.UpdateUI(MainControllerSet.WaitLoginAndSetYellow);
        }

        private void SocketHandle_GetNotice(object[] objs)
        {
            if (objs.Length > 0) Config.FunGame_Notice = (string)objs[0];
        }
        
        private void SocketHandle_CheckLogin(object[] objs)
        {
            string msg = "";
            // 返回的objs是该Login的User对象的各个属性
            if (objs.Length > 0) msg = (string)objs[0];
            Main?.GetMessage(msg);
            Main?.UpdateUI(MainControllerSet.SetUser, new object[] { Factory.New<User>(msg) });
        }
        
        private void SocketHandle_Disconnect(object[] objs)
        {
            string msg = "";
            if (objs.Length > 0) msg = (string)objs[0];
            Main?.GetMessage(msg);
            Main?.UpdateUI(MainControllerSet.Disconnected);
            Close();
        }

    }
}
