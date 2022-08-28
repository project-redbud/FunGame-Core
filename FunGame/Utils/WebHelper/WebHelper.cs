using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FunGame.Models.Config;
using FunGame.Models.Component;
using FunGame.Models.Entity;
using System.ComponentModel.DataAnnotations;
using FunGame.Models.Enum;

namespace FunGame.Utils.WebHelper
{
    public class WebHelper
    {
        private Socket? client;
        private EndPoint? server;
        Main Main;

        Action<Main, Socket>? WebHelper_Action = null;

        Task? WaitHeartBeat = null;

        public WebHelper(Main main)
        {
            Main = main;
        }

        /// <summary>
        /// 选择WebHelp分支方法
        /// </summary>
        /// <param name="i">分支方法ID</param>
        public void WebHelpMethod(int i)
        {
            switch (i)
            {
                case (int)CommonEnums.WebHelperMethod.CreateSocket:
                    CreateSocket();
                    break;
                case (int)CommonEnums.WebHelperMethod.CloseSocket:
                    Close();
                    break;
                case (int)CommonEnums.WebHelperMethod.StartWebHelper:
                    StartWebHelper();
                    break;
            }
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
                            Main.GetMessage(this, Config.WebHelper_SetGreen);
                            break;
                        }
                    }
                }
                WebHelper_Action = (main, socket) =>
                {
                    object? obj = main.GetMessage(this, Config.WebHelper_GetUser);
                    object[] objs;
                    if (obj != null)
                        objs = new object[] { main, socket, obj };
                    else
                        objs = new object[] { main, socket };
                    Task.Factory.StartNew(() =>
                    {

                    });
                    if (Send((int)SocketEnums.Type.CheckLogin, objs)) // 确认连接的玩家
                        StartWebHelper(); // 开始创建TCP流
                };
                Task t = Task.Factory.StartNew(() =>
                {
                    if (Main.InvokeRequired)
                    {
                        Main.Invoke(WebHelper_Action, Main, client);
                    }
                    else
                    {
                        WebHelper_Action(Main, client);
                    }
                });
            }
            catch (Exception e)
            {
                Main.GetMessage(this, Config.WebHelper_Disconnected);
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
                        string typestring = GetTypeString(type);
                        string read = GetMessage(msg);
                        main.GetMessage(this, read);
                        switch (type)
                        {
                            case (int)SocketEnums.Type.GetNotice:
                                break;
                            case (int)SocketEnums.Type.Login:
                                break;
                            case (int)SocketEnums.Type.CheckLogin:
                                return true;
                            case (int)SocketEnums.Type.Logout:
                                break;
                            case (int)SocketEnums.Type.HeartBeat:
                                if (WaitHeartBeat != null && !WaitHeartBeat.IsCompleted) WaitHeartBeat.Wait(1);
                                Config.WebHelper_HeartBeatFaileds = 0;
                                return true;
                        }
                    }
                    else
                        throw new Exception("ERROR：未收到任何来自服务器的信息，与服务器连接可能丢失。");
                }
                else
                {
                    main.GetMessage(this, Config.WebHelper_Disconnected);
                    throw new Exception("ERROR：服务器连接失败。");
                }
            }
            catch (Exception e)
            {
                main.GetMessage(this, Config.WebHelper_Disconnected);
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
                    // 发送消息给服务器端
                    switch (i)
                    {
                        case (int)SocketEnums.Type.GetNotice:
                            break;
                        case (int)SocketEnums.Type.Login:
                            break;
                        case (int)SocketEnums.Type.CheckLogin:
                            User user;
                            string msg;
                            if (objs != null && objs.Length > 2)
                            {
                                user = (User)objs[2];
                                msg = user.Userame;
                            }
                            else
                            {
                                Usercfg.FunGame_isAutoRetry = false;
                                throw new Exception("SUCCESS：服务器连接成功，检测到未登录账号已自动断开，请登录后手动发起服务器连接。");
                            }
                            byte[] buffer = new byte[2048];
                            buffer = Config.DEFAULT_ENCODING.GetBytes(MakeMessage((int)SocketEnums.Type.CheckLogin, msg));
                            int l = socket.Send(buffer);
                            if (l > 0)
                            {
                                return Read(objs);
                            }
                            else
                                throw new Exception("ERROR：消息未送达服务器，与服务器连接可能丢失。");
                        case (int)SocketEnums.Type.Logout:
                            break;
                        case (int)SocketEnums.Type.HeartBeat:
                            buffer = new byte[2048];
                            buffer = Config.DEFAULT_ENCODING.GetBytes(Convert.ToString(MakeMessage((int)SocketEnums.Type.HeartBeat, ">> 心跳检测")));
                            if (socket.Send(buffer) > 0)
                            {
                                WaitHeartBeat = Task.Run(() =>
                                {
                                    Thread.Sleep(4000);
                                    AddHeartBeatFaileds(main);
                                });
                                return true;
                            }
                            AddHeartBeatFaileds(main);
                            break;
                    }
                }
                else
                {
                    main.GetMessage(this, Config.WebHelper_Disconnected);
                    throw new Exception("ERROR：服务器连接失败。");
                }
            }
            catch (Exception e)
            {
                CatchException(main, e, false);
            }
            return false;
        }

        private void CatchException(Main main, Exception e, bool isDisconnected)
        {
            if (isDisconnected)
                main.GetMessage(this, Config.WebHelper_Disconnected);
            else
                main.GetMessage(this, Config.WebHelper_SetRed);
            main.GetMessage(this, e.Message != null ? e.Message + "\n" + e.StackTrace : "" + e.StackTrace);
            Close();
        }

        private void AddHeartBeatFaileds(Main main)
        {
            // 超过三次没回应心跳，服务器连接失败。
            try
            {
                Config.WebHelper_HeartBeatFaileds++;
                if (Config.WebHelper_HeartBeatFaileds >= 3)
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

        private string GetTypeString(int type)
        {
            switch (type)
            {
                case (int)SocketEnums.Type.GetNotice:
                    return SocketEnums.TYPE_GetNotice;
                case (int)SocketEnums.Type.Login:
                    return SocketEnums.TYPE_Login;
                case (int)SocketEnums.Type.CheckLogin:
                    return SocketEnums.TYPE_CheckLogin;
                case (int)SocketEnums.Type.Logout:
                    return SocketEnums.TYPE_Logout;
                case (int)SocketEnums.Type.HeartBeat:
                    return SocketEnums.TYPE_HeartBeat;
                default:
                    return SocketEnums.TYPE_UNKNOWN;
            }
        }

        private string GetMessage(string msg)
        {
            int index = msg.IndexOf(';') + 1;
            return msg[index..];
        }

        private string MakeMessage(int type, string msg)
        {
            return type + ";" + msg;
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

        private void StartWebHelper()
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
            Thread.Sleep(1000);
            Main.GetMessage(this, "Creating: SendHeartBeatStream...OK");
            while (IsConnected())
            {
                Send((int)SocketEnums.Type.HeartBeat); // 发送心跳包
                Thread.Sleep(20000);
            }
        }

        private void CreateStreamReader()
        {
            Thread.Sleep(1000);
            Main.GetMessage(this, "Creating: StreamReader...OK");
            while (IsConnected())
            {
                Read();
            }
        }
    }
}
