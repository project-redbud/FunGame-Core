using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Library.Interface;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class MainModel : IMain
    {
        public bool Connected => Socket != null && Socket.Connected;

        private readonly Main Main;
        private Task? ReceivingTask;
        private Core.Library.Common.Network.Socket? Socket;

        public MainModel(Main main)
        {
            Main = main;
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

        public void Disconnected()
        {
            Disconnect();
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
                        Constant.Server_Address = s[0];
                        Constant.Server_Port = Convert.ToInt32(s[1]);
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
            if (Constant.Server_Address == "" || Constant.Server_Port <= 0)
            {
                ShowMessage.ErrorMessage("查找可用的服务器失败！");
                return ConnectResult.FindServerFailed;
            }
            try
            {
                if (Config.FunGame_isRetrying)
                {
                    Main?.GetMessage("正在连接服务器，请耐心等待。");
                    Config.FunGame_isRetrying = false;
                    return ConnectResult.CanNotConnect;
                }
                if (!Config.FunGame_isConnected)
                {
                    Main!.CurrentRetryTimes++;
                    if (Main!.CurrentRetryTimes == 0) Main!.GetMessage("开始连接服务器...", true, TimeType.General);
                    else Main!.GetMessage("第" + Main!.CurrentRetryTimes + "次重试连接服务器...");
                    // 超过重连次数上限
                    if (Main!.CurrentRetryTimes + 1 > Main!.MaxRetryTimes)
                    {
                        throw new Exception("无法连接至服务器，请检查网络并重启游戏再试。");
                    }
                    // 与服务器建立连接
                    Socket?.Close();
                    Config.FunGame_isRetrying = true;
                    Socket = Core.Library.Common.Network.Socket.Connect(Constant.Server_Address, Constant.Server_Port);
                    if (Socket != null && Socket.Connected)
                    {
                        // 设置可复用Socket
                        RunTime.Socket = Socket;
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
                                }
                            });
                            return ConnectResult.Success;
                        }
                        Socket?.Close();
                        Config.FunGame_isRetrying = false;
                        throw new Exception("无法连接至服务器，请检查网络并重启游戏再试。");
                    }
                }
                else
                {
                    Main?.GetMessage("已连接至服务器，请勿重复连接。");
                    return ConnectResult.CanNotConnect;
                }
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetErrorInfo(), false);
                Config.FunGame_isRetrying = false;
                if (Config.FunGame_isAutoRetry && Main!.CurrentRetryTimes <= Main!.MaxRetryTimes)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(5000);
                        if (Config.FunGame_isAutoRetry) Connect(); // 再次判断是否开启自动重连
                    });
                    Main?.GetMessage("连接服务器失败，5秒后自动尝试重连。");
                }
                else return ConnectResult.ConnectFailed;
            }
            return ConnectResult.CanNotConnect;
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
            throw new NotImplementedException();
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
            if (objs.Length > 0) msg = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
            string[] strings = msg.Split(';');
            string ServerName = strings[0];
            string ServerNotice = strings[1];
            Config.FunGame_ServerName = ServerName;
            Config.FunGame_Notice = ServerNotice;
            if (objs.Length > 1) msg = NetworkUtility.ConvertJsonObject<string>(objs[1])!;
            Socket!.Token = msg;
            Main?.GetMessage($"已连接服务器：{ServerName}。\n\n********** 服务器公告 **********\n\n{ServerNotice}\n\n");
            // 设置等待登录的黄灯
            Main?.UpdateUI(MainControllerSet.WaitLoginAndSetYellow);
        }

        private void SocketHandle_GetNotice(object[] objs)
        {
            if (objs.Length > 0) Config.FunGame_Notice = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
        }

        private void SocketHandle_CheckLogin(object[] objs)
        {
            string msg = "";
            // 返回的objs是该Login的User对象的各个属性
            if (objs.Length > 0) msg = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
            Main?.GetMessage(msg);
            Main?.UpdateUI(MainControllerSet.SetUser, new object[] { Factory.New<User>(msg) });
        }

        private void SocketHandle_Disconnect(object[] objs)
        {
            string msg = "";
            if (objs.Length > 0) msg = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
            Main?.GetMessage(msg);
            Main?.UpdateUI(MainControllerSet.Disconnect);
            Close();
        }
    }
}
