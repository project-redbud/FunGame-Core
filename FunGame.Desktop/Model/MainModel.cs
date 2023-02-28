using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
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
        private bool IsReceiving = false;

        public MainModel(Main main)
        {
            Main = main;
        }

        #region 公开方法

        public bool LogOut()
        {
            try
            {
                // 需要当时登录给的Key发回去，确定是账号本人在操作才允许登出
                if (Config.Guid_LoginKey != Guid.Empty)
                {
                    if (Socket?.Send(SocketMessageType.Logout, Config.Guid_LoginKey) == SocketResult.Success)
                    {
                        return true;
                    }
                }
                else throw new CanNotLogOutException();
            }
            catch (Exception e)
            {
                ShowMessage.ErrorMessage("无法登出您的账号，请联系服务器管理员。", "登出失败", 5);
                Main.GetMessage(e.GetErrorInfo());
                Main.OnFailedLogoutEvent(new GeneralEventArgs());
                Main.OnAfterLogoutEvent(new GeneralEventArgs());
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
            Main.OnBeforeConnectEvent(new GeneralEventArgs());
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
                                    Main.GetMessage("连接服务器成功，请登录账号以体验FunGame。");
                                    Main.UpdateUI(MainSet.Connected);
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
                Main.UpdateUI(MainSet.SetRed);
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

        public bool IntoRoom()
        {
            try
            {
                if (Socket?.Send(SocketMessageType.IntoRoom, Config.FunGame_Roomid) == SocketResult.Success)
                    return true;
                else throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                Main.OnFailedIntoRoomEvent(new GeneralEventArgs());
                Main.OnAfterIntoRoomEvent(new GeneralEventArgs());
                return false;
            }
        }
        
        public bool Chat(string msg)
        {
            try
            {
                if (Socket?.Send(SocketMessageType.Chat, msg) == SocketResult.Success)
                    return true;
                else throw new CanNotSendTalkException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                Main.OnFailedSendTalkEvent(new GeneralEventArgs());
                Main.OnAfterSendTalkEvent(new GeneralEventArgs());
                return false;
            }
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
                        SocketHandler_Connect(objs);
                        break;

                    case SocketMessageType.GetNotice:
                        SocketHandler_GetNotice(objs);
                        break;

                    case SocketMessageType.Login:
                        SocketHandler_Login(objs);
                        break;

                    case SocketMessageType.CheckLogin:
                        SocketHandler_CheckLogin(objs);
                        RunTime.Login?.OnAfterLoginEvent(new GeneralEventArgs());
                        break;

                    case SocketMessageType.Logout:
                        SocketHandler_LogOut(objs);
                        break;

                    case SocketMessageType.Disconnect:
                        SocketHandler_Disconnect(objs);
                        break;

                    case SocketMessageType.HeartBeat:
                        if (Socket.Connected && Usercfg.LoginUser != null)
                            Main.UpdateUI(MainSet.SetGreenAndPing);
                        break;

                    case SocketMessageType.IntoRoom:
                        SocketHandler_IntoRoom(objs);
                        break;
                        
                    case SocketMessageType.Chat:
                        SocketHandler_Chat(objs);
                        break;

                    case SocketMessageType.Unknown:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                // 报错中断服务器连接
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                Main.UpdateUI(MainSet.Disconnected);
                Main.OnFailedConnectEvent(new GeneralEventArgs());
                Close();
            }
            return result;
        }

        #endregion

        #region SocketHandler

        private void SocketHandler_Connect(object[] objs)
        {
            string msg = "";
            Guid token = Guid.Empty;
            if (objs.Length > 0) msg = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
            string[] strings = msg.Split(';');
            string ServerName = strings[0];
            string ServerNotice = strings[1];
            Config.FunGame_ServerName = ServerName;
            Config.FunGame_Notice = ServerNotice;
            if (objs.Length > 1) token = NetworkUtility.ConvertJsonObject<Guid>(objs[1]);
            Socket!.Token = token;
            Config.Guid_Socket = token;
            Main.GetMessage($"已连接服务器：{ServerName}。\n\n********** 服务器公告 **********\n\n{ServerNotice}\n\n");
            // 设置等待登录的黄灯
            Main.UpdateUI(MainSet.WaitLoginAndSetYellow);
        }

        private void SocketHandler_GetNotice(object[] objs)
        {
            if (objs.Length > 0) Config.FunGame_Notice = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
        }

        private void SocketHandler_Login(object[] objs)
        {
            Guid key = Guid.Empty;
            string? msg = "";
            // 返回一个Key，再发回去给服务器就行了
            if (objs.Length > 0) key = NetworkUtility.ConvertJsonObject<Guid>(objs[0]);
            if (objs.Length > 1) msg = NetworkUtility.ConvertJsonObject<string>(objs[1]);
            // 如果返回了msg，说明验证错误。
            if (msg != null && msg.Trim() != "")
            {
                ShowMessage.ErrorMessage(msg, "登录失败");
                RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
                RunTime.Login?.OnAfterLoginEvent(new GeneralEventArgs());
            }
            else
            {
                if (key != Guid.Empty)
                {
                    Config.Guid_LoginKey = key;
                    LoginController.CheckLogin(key);
                }
                else
                {
                    ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
                    RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
                    RunTime.Login?.OnAfterLoginEvent(new GeneralEventArgs());
                }
            }
        }
        
        private void SocketHandler_LogOut(object[] objs)
        {
            Guid key = Guid.Empty;
            string? msg = "";
            // 返回一个Key，如果这个Key是空的，登出失败
            if (objs != null && objs.Length > 0) key = NetworkUtility.ConvertJsonObject<Guid>(objs[0]);
            if (objs != null && objs.Length > 1) msg = NetworkUtility.ConvertJsonObject<string>(objs[1]);
            if (key != Guid.Empty)
            {
                Config.Guid_LoginKey = Guid.Empty;
                Main.UpdateUI(MainSet.LogOut, msg ?? "");
                Main.OnSucceedLogoutEvent(new GeneralEventArgs());
            }
            else
            {
                ShowMessage.ErrorMessage("无法登出您的账号，请联系服务器管理员。", "登出失败", 5);
                Main.OnFailedLogoutEvent(new GeneralEventArgs());
            }
            Main.OnAfterLogoutEvent(new GeneralEventArgs());
        }
        
        private void SocketHandler_CheckLogin(object[] objs)
        {
            // 返回的objs是该Login的User对象的各个属性
            if (objs != null && objs.Length > 0)
            {
                // 创建User对象并返回到Main
                Main.UpdateUI(MainSet.SetUser, new object[] { Factory.New<User>(objs) });
                RunTime.Login?.OnSucceedLoginEvent(new GeneralEventArgs());
                return;
            }
            ShowMessage.ErrorMessage("登录失败！！", "登录失败", 5);
            RunTime.Login?.OnFailedLoginEvent(new GeneralEventArgs());
        }

        private void SocketHandler_Disconnect(object[] objs)
        {
            string msg = "";
            if (objs.Length > 0) msg = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
            Main.GetMessage(msg);
            Main.UpdateUI(MainSet.Disconnect);
            Close();
            Main.OnSucceedDisconnectEvent(new GeneralEventArgs());
            Main.OnAfterDisconnectEvent(new GeneralEventArgs());
        }

        private void SocketHandler_IntoRoom(object[] objs)
        {
            string roomid = "";
            if (objs.Length > 0) roomid = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
            if (roomid.Trim() != "" && roomid == "-1")
            {
                Main.GetMessage($"已连接至公共聊天室。");
            }
            else
            {
                Config.FunGame_Roomid = roomid;
            }
            Main.OnSucceedIntoRoomEvent(new GeneralEventArgs());
            Main.OnAfterIntoRoomEvent(new GeneralEventArgs());
        }
        
        private void SocketHandler_Chat(object[] objs)
        {
            if (objs != null && objs.Length > 1)
            {
                string user = NetworkUtility.ConvertJsonObject<string>(objs[0])!;
                string msg = NetworkUtility.ConvertJsonObject<string>(objs[1])!;
                if (user != Usercfg.LoginUserName)
                {
                    Main.GetMessage(msg, TimeType.None);
                }
                Main.OnSucceedSendTalkEvent(new GeneralEventArgs());
                Main.OnAfterSendTalkEvent(new GeneralEventArgs());
                return;
            }
            Main.OnFailedSendTalkEvent(new GeneralEventArgs());
            Main.OnAfterSendTalkEvent(new GeneralEventArgs());
        }

        #endregion
    }
}
