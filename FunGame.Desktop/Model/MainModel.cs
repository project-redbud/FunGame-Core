﻿using Milimoe.FunGame.Core.Api.Utility;
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
        public Core.Library.Common.Event.ConnectEvent ConnectEvent { get; } = new ConnectEvent();
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
                Main?.GetMessage(e.GetStackTrace());
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
                Main?.GetMessage(e.GetStackTrace());
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
                Main?.GetMessage(e.GetStackTrace());
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
                    throw new Exception("查找可用的服务器失败，请重启FunGame。");
                }
            }
            catch (Exception e)
            {
                Main?.GetMessage(e.GetStackTrace(), false);
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
                Main?.GetMessage(e.GetStackTrace(), false);
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
                Main.GetMessage(e.GetStackTrace(), false);
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
            return new object[2] { SocketSet.Unknown, Array.Empty<object>() };
        }

        private SocketMessageType Receiving()
        {
            if (Socket is null) return SocketMessageType.Unknown;
            SocketMessageType result = SocketMessageType.Unknown;
            try
            {
                object[] ServerMessage = GetServerMessage();
                string type = (string)ServerMessage[0];
                object[] objs = (object[])ServerMessage[1];
                string msg = "";
                switch (type)
                {
                    case SocketSet.GetNotice:
                        result = SocketMessageType.GetNotice;
                        if (objs.Length > 0) msg = (string)objs[0];
                        Config.FunGame_Notice = msg;
                        break;

                    case SocketSet.Connect:
                        result = SocketMessageType.Connect;
                        if (objs.Length > 0) msg = (string)objs[0];
                        string[] strings = msg.Split(';');
                        string ServerName = strings[0];
                        string ServerNotice = strings[1];
                        Config.FunGame_ServerName = ServerName;
                        Config.FunGame_Notice = ServerNotice;
                        Main?.GetMessage($"已连接服务器：{ServerName}。\n\n********** 服务器公告 **********\n\n{ServerNotice}\n\n");
                        // 设置等待登录的黄灯
                        Main?.UpdateUI(MainControllerSet.WaitLoginAndSetYellow);
                        break;

                    case SocketSet.Login:
                        result = SocketMessageType.Login;
                        break;

                    case SocketSet.CheckLogin:
                        result = SocketMessageType.CheckLogin;
                        if (objs.Length > 0) msg = (string)objs[0];
                        Main?.GetMessage(msg);
                        Main?.UpdateUI(MainControllerSet.SetUser, true, TimeType.TimeOnly, new object[] { Factory.New<User>(msg) });
                        break;

                    case SocketSet.Logout:
                        break;

                    case SocketSet.Disconnect:
                        result = SocketMessageType.Disconnect;
                        if (objs.Length > 0) msg = (string)objs[0];
                        Main?.GetMessage(msg);
                        Main?.UpdateUI(MainControllerSet.Disconnected);
                        Close();
                        break;

                    case SocketSet.HeartBeat:
                        result = SocketMessageType.HeartBeat;
                        if (Usercfg.LoginUser != null)
                            Main?.UpdateUI(MainControllerSet.SetGreenAndPing);
                        break;

                    case SocketSet.Unknown:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                // 报错中断服务器连接
                Main?.GetMessage(e.GetStackTrace(), false);
                Main?.UpdateUI(MainControllerSet.Disconnected);
                Close();
            }
            return result;
        }
    }
}