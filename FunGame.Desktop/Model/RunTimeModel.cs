using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    /// <summary>
    /// 与创建关闭Socket相关的方法，使用此类
    /// </summary>
    public class RunTimeModel
    {
        public bool Connected => Socket != null && Socket.Connected;

        private readonly Main Main;
        private Task? ReceivingTask;
        private Socket? Socket;
        private bool IsReceiving = false;

        public RunTimeModel(Main main)
        {
            Main = main;
        }

        #region 公开方法

        public void Disconnect()
        {
            try
            {
                Socket?.Send(SocketMessageType.Disconnect, "");
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                Main.OnFailedDisconnectEvent(new GeneralEventArgs());
                Main.OnAfterDisconnectEvent(new GeneralEventArgs());
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
                    throw new FindServerFailedException();
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return false;
        }

        public ConnectResult Connect()
        {
            if (Main.OnBeforeConnectEvent(new GeneralEventArgs()) == EventResult.Fail) return ConnectResult.ConnectFailed;
            if (Constant.Server_Address == "" || Constant.Server_Port <= 0)
            {
                ShowMessage.ErrorMessage("查找可用的服务器失败！");
                Main.OnFailedConnectEvent(new GeneralEventArgs());
                Main.OnAfterConnectEvent(new GeneralEventArgs());
                return ConnectResult.FindServerFailed;
            }
            try
            {
                if (Config.FunGame_isRetrying)
                {
                    Main.GetMessage("正在连接服务器，请耐心等待。");
                    Config.FunGame_isRetrying = false;
                    Main.OnFailedConnectEvent(new GeneralEventArgs());
                    Main.OnAfterConnectEvent(new GeneralEventArgs());
                    return ConnectResult.CanNotConnect;
                }
                if (!Config.FunGame_isConnected)
                {
                    Main.CurrentRetryTimes++;
                    if (Main.CurrentRetryTimes == 0) Main.GetMessage("开始连接服务器...", TimeType.General);
                    else Main.GetMessage("第" + Main.CurrentRetryTimes + "次重试连接服务器...");
                    // 超过重连次数上限
                    if (Main.CurrentRetryTimes + 1 > Main.MaxRetryTimes)
                    {
                        throw new CanNotConnectException();
                    }
                    // 与服务器建立连接
                    Socket?.Close();
                    Config.FunGame_isRetrying = true;
                    Socket = Socket.Connect(Constant.Server_Address, Constant.Server_Port);
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
                                    Main.GetMessage("连接服务器成功，请登录账号以体验FunGame。");
                                    Main.UpdateUI(MainInvokeType.Connected);
                                    StartReceiving();
                                    while (true)
                                    {
                                        if (IsReceiving)
                                        {
                                            Main.OnSucceedConnectEvent(new GeneralEventArgs());
                                            Main.OnAfterConnectEvent(new GeneralEventArgs());
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Config.FunGame_isRetrying = false;
                                    Socket.Close();
                                }
                            });
                            return ConnectResult.Success;
                        }
                        Socket?.Close();
                        Config.FunGame_isRetrying = false;
                        throw new CanNotConnectException();
                    }
                }
                else
                {
                    Main.GetMessage("已连接至服务器，请勿重复连接。");
                    return ConnectResult.CanNotConnect;
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                Main.UpdateUI(MainInvokeType.SetRed);
                Config.FunGame_isRetrying = false;
                Task.Factory.StartNew(() =>
                {
                    Main.OnFailedConnectEvent(new GeneralEventArgs());
                    Main.OnAfterConnectEvent(new GeneralEventArgs());
                });
                return ConnectResult.ConnectFailed;
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
                    IsReceiving = false;
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return false;
            }
            return true;
        }

        public void Error(Exception e)
        {
            Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            Main.UpdateUI(MainInvokeType.Disconnected);
            Main.OnFailedConnectEvent(new GeneralEventArgs());
            Close();
        }

        #endregion

        #region 私有方法

        private void StartReceiving()
        {
            ReceivingTask = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                IsReceiving = true;
                while (Socket != null && Socket.Connected)
                {
                    Receiving();
                }
            });
            Socket?.StartReceiving(ReceivingTask);
        }

        private SocketObject GetServerMessage()
        {
            if (Socket != null && Socket.Connected)
            {
                return Socket.Receive();
            }
            return new SocketObject();
        }

        private SocketMessageType Receiving()
        {
            if (Socket is null) return SocketMessageType.Unknown;
            SocketMessageType result = SocketMessageType.Unknown;
            try
            {
                SocketObject ServerMessage = GetServerMessage();
                SocketMessageType type = ServerMessage.SocketType;
                object[] objs = ServerMessage.Parameters;
                result = type;
                switch (type)
                {
                    case SocketMessageType.Connect:
                        if (!SocketHandler_Connect(ServerMessage)) return SocketMessageType.Unknown;
                        break;

                    case SocketMessageType.Disconnect:
                        SocketHandler_Disconnect(ServerMessage);
                        break;

                    case SocketMessageType.HeartBeat:
                        if (Socket.Connected && Usercfg.LoginUser != null)
                            Main.UpdateUI(MainInvokeType.SetGreenAndPing);
                        break;

                    case SocketMessageType.Unknown:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                // 报错中断服务器连接
                Error(e);
            }
            return result;
        }

        #endregion

        #region SocketHandler

        private bool SocketHandler_Connect(SocketObject ServerMessage)
        {
            string msg = "";
            Guid token = Guid.Empty;
            if (ServerMessage.Parameters.Length > 0) msg = ServerMessage.GetParam<string>(0)!;
            string[] strings = msg.Split(';');
            if (strings.Length != 2)
            {
                // 服务器拒绝连接
                msg = strings[0];
                Main.GetMessage(msg);
                ShowMessage.ErrorMessage(msg);
                return false;
            }
            string ServerName = strings[0];
            string ServerNotice = strings[1];
            Config.FunGame_ServerName = ServerName;
            Config.FunGame_Notice = ServerNotice;
            if (ServerMessage.Parameters.Length > 1) token = ServerMessage.GetParam<Guid>(1);
            Socket!.Token = token;
            Config.Guid_Socket = token;
            Main.GetMessage($"已连接服务器：{ServerName}。\n\n********** 服务器公告 **********\n\n{ServerNotice}\n\n");
            // 设置等待登录的黄灯
            Main.UpdateUI(MainInvokeType.WaitLoginAndSetYellow);
            return true;
        }

        private void SocketHandler_Disconnect(SocketObject ServerMessage)
        {
            string msg = "";
            if (ServerMessage.Parameters.Length > 0) msg = ServerMessage.GetParam<string>(0)!;
            Main.GetMessage(msg);
            Main.UpdateUI(MainInvokeType.Disconnect);
            Close();
            Main.OnSucceedDisconnectEvent(new GeneralEventArgs());
            Main.OnAfterDisconnectEvent(new GeneralEventArgs());
        }

        #endregion
    }
}