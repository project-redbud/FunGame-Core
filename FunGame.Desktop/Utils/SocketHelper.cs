using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FunGame.Desktop.Models.Component;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using Milimoe.FunGame.Core.Entity.General;
using Milimoe.FunGame.Core.Entity.Enum;
using Milimoe.FunGame.Desktop.Others;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Api.Factory;

namespace Milimoe.FunGame.Desktop.Utils
{
    public class SocketHelper
    {
        private Socket? client;
        private EndPoint? server;
        Main Main;

        Action<Main, Socket>? SocketHelper_Action = null;

        Task? WaitHeartBeat = null;

        public SocketHelper(Main main)
        {
            Main = main;
        }

        /// <summary>
        /// 选择WebHelp分支方法
        /// </summary>
        /// <param name="i">分支方法ID</param>
        public bool WebHelpMethod(int i)
        {
            switch (i)
            {
                case (int)SocketHelperMethod.CreateSocket:
                    CreateSocket();
                    break;
                case (int)SocketHelperMethod.CloseSocket:
                    Close();
                    break;
                case (int)SocketHelperMethod.StartSocketHelper:
                    StartSocketHelper();
                    break;
                case (int)SocketHelperMethod.Login:
                    if (client != null)
                    {
                        Send((int)SocketMessageType.CheckLogin, new object[] { Main, client, UserFactory.GetInstance("Mili") });
                        return true;
                    }
                    return false;
                case (int)SocketHelperMethod.Logout:
                    if (client != null && Usercfg.LoginUser != null)
                    {
                        Send((int)SocketMessageType.Logout, new object[] { Main, client, Usercfg.LoginUser });
                        return true;
                    }
                    return false;
                case (int)SocketHelperMethod.Disconnect:
                    if (client != null)
                    {
                        Send((int)SocketMessageType.Disconnect, new object[] { Main, client });
                        return true;
                    }
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 创建客户端专属Socket
        /// </summary>
        private void CreateSocket()
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server = new IPEndPoint(IPAddress.Parse(Config.SERVER_IPADRESS), Config.SERVER_PORT);
                while (true)
                {
                    if (!IsConnected())
                    {
                        client.Connect(server);
                        if (IsConnected())
                        {
                            Main.GetMessage(this, Config.SocketHelper_WaitLoginAndSetYellow);
                            break;
                        }
                    }
                }
                SocketHelper_Action = (main, socket) =>
                {
                    object? obj = main.GetMessage(this, Config.SocketHelper_GetUser);
                    object[] objs;
                    if (obj != null)
                        objs = new object[] { main, socket, obj };
                    else
                        objs = new object[] { main, socket };
                    if (Send((int)SocketMessageType.GetNotice, objs)) // 接触服务器并获取公告
                    {
                        main.GetMessage(this, " >> 连接服务器成功，请登录账号以体验FunGame。", true);
                        main.GetMessage(this, Config.SocketHelper_SetNotice);
                    }
                };
                Task t = Task.Factory.StartNew(() =>
                {
                    if (Main.InvokeRequired)
                    {
                        Main.Invoke(SocketHelper_Action, Main, client);
                    }
                    else
                    {
                        SocketHelper_Action(Main, client);
                    }
                });
            }
            catch (Exception e)
            {
                Main.GetMessage(this, Config.SocketHelper_Disconnected);
                Main.GetMessage(this, e.StackTrace);
                Close();
            }
        }

        /// <summary>
        /// 判断是否连接成功
        /// </summary>
        /// <returns>连接状态</returns>
        public bool IsConnected()
        {
            if (client != null)
                return client.Connected;
            return false;
        }

        private bool Read(object[]? objs = null)
        {
            Main main = Main;
            Socket? socket = null;
            try
            {
                if (objs != null)
                {
                    if (objs.Length > 0) main = (Main)objs[0];
                    if (objs.Length > 1) socket = (Socket)objs[1];
                }
                else
                {
                    main = Main;
                    socket = client;
                }
                if (socket != null)
                {
                    // 从服务器接收消息
                    byte[] buffer = new byte[2048];
                    int length = socket.Receive(buffer);
                    if (length > 0)
                    {
                        string msg = Config.DEFAULT_ENCODING.GetString(buffer, 0, length);
                        int type = GetType(msg);
                        string typestring = EnumHelper.GetSocketTypeName(type);
                        string read = GetMessage(msg);
                        switch (type)
                        {
                            case (int)SocketMessageType.GetNotice:
                                string[] reads = read.Split(';');
                                Config.FunGame_Notice = reads[1];
                                main.GetMessage(this, " >> 已连接至服务器: " + reads[0] + "。\n\n********** 服务器公告 **********\n\n" + Config.FunGame_Notice + "\n\n", true);
                                return true;
                            case (int)SocketMessageType.Login:
                                break;
                            case (int)SocketMessageType.CheckLogin:
                                Main.GetMessage(this, Config.SocketHelper_SetUser, false, objs);
                                Main.GetMessage(this, read, true);
                                StartSocketHelper(); // 开始创建TCP流
                                return true;
                            case (int)SocketMessageType.Logout:
                                Main.GetMessage(this, Config.SocketHelper_SetUser, false, objs);
                                Main.GetMessage(this, read, true);
                                Main.GetMessage(this, Config.SocketHelper_LogOut);
                                Close();
                                return true;
                            case (int)SocketMessageType.Disconnect:
                                Main.GetMessage(this, read, true);
                                Main.GetMessage(this, Config.SocketHelper_Disconnect);
                                Close();
                                return true;
                            case (int)SocketMessageType.HeartBeat:
                                if (WaitHeartBeat != null && !WaitHeartBeat.IsCompleted) WaitHeartBeat.Wait(1);
                                Config.SocketHelper_HeartBeatFaileds = 0;
                                main.GetMessage(this, Config.SocketHelper_SetGreenAndPing);
                                return true;
                        }
                        main.GetMessage(this, read);
                        return true;
                    }
                    else
                        throw new Exception("ERROR：未收到任何来自服务器的信息，与服务器连接可能丢失。");
                }
                else
                {
                    main.GetMessage(this, Config.SocketHelper_Disconnected);
                    throw new Exception("ERROR：服务器连接失败。");
                }
            }
            catch (Exception e)
            {
                main.GetMessage(this, Config.SocketHelper_Disconnected);
                main.GetMessage(this, e.Message != null ? e.Message + "\n" + e.StackTrace : "" + e.StackTrace);
                Close();
            }
            return false;
        }

        private bool Send(int i, object[]? objs = null)
        {
            Main main = Main;
            Socket? socket = null;
            try
            {
                if (objs != null)
                {
                    if (objs.Length > 0) main = (Main)objs[0];
                    if (objs.Length > 1) socket = (Socket)objs[1];
                }
                else
                {
                    main = Main;
                    socket = client;
                }
                if (socket != null)
                {
                    string msg = "";
                    SocketMessageType type = (SocketMessageType)i;
                    // 发送消息给服务器端
                    switch (type)
                    {
                        case SocketMessageType.GetNotice:
                            msg = MakeMessage(type, "获取公告");
                            if (Send(msg, socket) > 0)
                            {
                                return Read(objs);
                            }
                            else
                                throw new Exception("ERROR：消息未送达服务器，与服务器连接可能丢失。");
                        case SocketMessageType.Login:
                            break;
                        case SocketMessageType.CheckLogin:
                            User user;
                            if (objs != null && objs.Length > 2)
                            {
                                user = (User)objs[2];
                                msg = MakeMessage(type, user.Userame);
                            }
                            else
                            {
                                Config.FunGame_isAutoRetry = false;
                                throw new Exception("ERROR: 请登录账号。");
                            }
                            break;
                        case SocketMessageType.Logout:
                            if (objs != null && objs.Length > 2)
                            {
                                user = (User)objs[2];
                                msg = MakeMessage(type, user.Userame);
                                if (Send(msg, socket) > 0)
                                    return true;
                            }
                            return false;
                        case SocketMessageType.Disconnect:
                            msg = MakeMessage(type, "断开连接");
                            if (Send(msg, socket) > 0)
                                return true;
                            return false;
                        case SocketMessageType.HeartBeat:
                            msg = MakeMessage(type, "心跳检测");
                            if (Send(msg, socket) > 0)
                            {
                                WaitHeartBeat = Task.Run(() =>
                                {
                                    Thread.Sleep(4000);
                                    AddHeartBeatFaileds(main);
                                });
                                return true;
                            }
                            AddHeartBeatFaileds(main);
                            return false;
                        default:
                            return false;
                    }
                    if (Send(msg, socket) > 0)
                    {
                        return Read(objs);
                    }
                    else
                        throw new Exception("ERROR：消息未送达服务器，与服务器连接可能丢失。");
                }
                else
                {
                    main.GetMessage(this, Config.SocketHelper_Disconnected);
                    throw new Exception("ERROR：服务器连接失败。");
                }
            }
            catch (Exception e)
            {
                CatchException(main, e, false);
            }
            return false;
        }

        private int Send(string msg, Socket socket)
        {
            byte[] buffer = Config.DEFAULT_ENCODING.GetBytes(msg);
            int length = socket.Send(buffer);
            return length;
        }

        private void CatchException(Main main, Exception e, bool isDisconnected)
        {
            if (isDisconnected)
                main.GetMessage(this, Config.SocketHelper_Disconnected);
            else
                main.GetMessage(this, Config.SocketHelper_SetRed);
            main.GetMessage(this, e.Message != null ? e.Message + "\n" + e.StackTrace : "" + e.StackTrace);
            Close();
        }

        private void AddHeartBeatFaileds(Main main)
        {
            // 超过三次没回应心跳，服务器连接失败。
            try
            {
                Config.SocketHelper_HeartBeatFaileds++;
                if (Config.SocketHelper_HeartBeatFaileds >= 3)
                    throw new Exception("ERROR：服务器连接失败。");
            }
            catch (Exception e)
            {
                CatchException(main, e, true);
            }
        }

        private int GetType(string msg)
        {
            int index = msg.IndexOf(';') - 1;
            if (index > 0)
                return Convert.ToInt32(msg[..index]);
            else
                return Convert.ToInt32(msg[..1]);
        }

        private string GetMessage(string msg)
        {
            int index = msg.IndexOf(';') + 1;
            return msg[index..];
        }

        private string MakeMessage(SocketMessageType type, string msg)
        {
            return (int)type + ";" + msg;
        }

        private void Close()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
            if (server != null)
            {
                server = null;
            }
        }

        private void StartSocketHelper()
        {
            Task HeartBeatStream = Task.Factory.StartNew(() =>
            {
                CreateSendHeartBeatStream();
            });
            Task StreamReader = Task.Factory.StartNew(() =>
            {
                CreateStreamReader();
            });
        }

        private void CreateSendHeartBeatStream()
        {
            Thread.Sleep(100);
            Main.GetMessage(this, "Creating: SendHeartBeatStream...OK");
            while (IsConnected())
            {
                Send((int)SocketMessageType.HeartBeat); // 发送心跳包
                Thread.Sleep(20000);
            }
        }

        private void CreateStreamReader()
        {
            Thread.Sleep(100);
            Main.GetMessage(this, "Creating: StreamReader...OK");
            while (IsConnected())
            {
                Read();
            }
        }
    }
}
