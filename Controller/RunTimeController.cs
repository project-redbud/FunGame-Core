using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// 此类实现服务器连接、断开连接、心跳检测、创建数据请求等功能
    /// -- 需要继承并实现部分方法 --
    /// </summary>
    public abstract class RunTimeController
    {
        /// <summary>
        /// 与服务器的连接套接字实例
        /// </summary>
        public Socket? Socket => _Socket;

        /// <summary>
        /// 套接字是否已经连接
        /// </summary>
        public bool Connected => _Socket != null && _Socket.Connected;

        /// <summary>
        /// 接收服务器信息的线程
        /// </summary>
        protected Task? _ReceivingTask;

        /// <summary>
        /// 用于类内赋值
        /// </summary>
        protected Socket? _Socket;

        /// <summary>
        /// 是否正在接收服务器信息
        /// </summary>
        protected bool _IsReceiving;

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            bool result = false;

            try
            {
                result = _Socket?.Send(SocketMessageType.Disconnect, "") == SocketResult.Success;
            }
            catch (Exception e)
            {
                WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public ConnectResult Connect(string ip, int port)
        {
            if (!BeforeConnect(ip, port))
            {
                return ConnectResult.ConnectFailed;
            }

            ConnectResult result = ConnectResult.Success;
            string msg = "";
            string servername = "";
            string notice = "";

            // 检查服务器IP地址和端口是否正确
            if (ip == "" || port <= 0)
            {
                result = ConnectResult.FindServerFailed;
            }
            if (result == ConnectResult.Success)
            {
                // 与服务器建立连接
                _Socket?.Close();
                _Socket = Socket.Connect(ip, port);
                if (_Socket != null && _Socket.Connected)
                {
                    if (_Socket.Send(SocketMessageType.Connect) == SocketResult.Success)
                    {
                        SocketObject[] objs = _Socket.ReceiveArray();
                        foreach (SocketObject obj in objs)
                        {
                            if (obj.SocketType == SocketMessageType.Connect)
                            {
                                bool success = obj.GetParam<bool>(0);
                                msg = obj.GetParam<string>(1) ?? "";
                                if (success)
                                {
                                    _Socket.Token = obj.GetParam<Guid>(2);
                                    servername = obj.GetParam<string>(3) ?? "";
                                    notice = obj.GetParam<string>(4) ?? "";
                                    StartReceiving();
                                    Task.Run(() =>
                                    {
                                        while (true)
                                        {
                                            if (_IsReceiving)
                                            {
                                                break;
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        result = ConnectResult.ConnectFailed;
                    }
                }
                else _Socket?.Close();
            }

            object[] ConnectArgs = new object[] { result, msg, servername, notice };
            AfterConnect(ConnectArgs);

            // 允许修改数组中的result，强行改变连接的结果
            return (ConnectResult)ConnectArgs[0];
        }

        /// <summary>
        /// 获取服务器地址
        /// </summary>
        /// <returns>string：IP地址；int：端口号</returns>
        /// <exception cref="FindServerFailedException"></exception>
        public (string, int) GetServerAddress()
        {
            try
            {
                string? ipaddress = (string?)Implement.GetFunGameImplValue(InterfaceType.IClient, InterfaceMethod.RemoteServerIP);
                if (ipaddress != null)
                {
                    string[] s = ipaddress.Split(':');
                    if (s != null && s.Length > 1)
                    {
                        return (s[0], Convert.ToInt32(s[1]));
                    }
                }
                throw new FindServerFailedException();
            }
            catch (FindServerFailedException e)
            {
                WritelnSystemInfo(e.GetErrorInfo());
                return ("", 0);
            }
        }

        /// <summary>
        /// 此方法将在连接服务器前触发<para/>
        /// 客户端可以重写此方法
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>false：中止连接</returns>
        public virtual bool BeforeConnect(string ip, int port)
        {
            return true;
        }
        
        /// <summary>
        /// 此方法将在连接服务器后触发（Connect结果返回前）<para/>
        /// 客户端可以重写此方法
        /// </summary>
        /// <param name="ConnectArgs">连接服务器后返回的一些数据，可以使用也可以修改它们</param>
        /// <returns></returns>
        public virtual void AfterConnect(object[] ConnectArgs)
        {

        }

        /// <summary>
        /// 客户端需要自行实现自动登录的事务
        /// </summary>
        public abstract void AutoLogin(string Username, string Password, string AutoKey);

        /// <summary>
        /// 关闭所有连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (_Socket != null)
                {
                    _Socket.Close();
                    _Socket = null;
                }
                if (_ReceivingTask != null && !_ReceivingTask.IsCompleted)
                {
                    _ReceivingTask.Wait(1);
                    _ReceivingTask = null;
                    _IsReceiving = false;
                }
            }
            catch (Exception e)
            {
                WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="msg"></param>
        public abstract void WritelnSystemInfo(string msg);

        /// <summary>
        /// 自定处理异常的方法
        /// -- 一般放在catch中 --
        /// </summary>
        /// <param name="e"></param>
        public abstract void Error(Exception e);

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        /// <param name="RequestType"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        public DataRequest NewDataRequest(DataRequestType RequestType)
        {
            if (_Socket != null)
            {
                DataRequest request = new(_Socket, RequestType);
                return request;
            }
            throw new ConnectFailedException();
        }
        
        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        /// <param name="RequestType"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        public DataRequest NewLongRunningDataRequest(DataRequestType RequestType)
        {
            if (_Socket != null)
            {
                DataRequest request = new(_Socket, RequestType, true);
                return request;
            }
            throw new ConnectFailedException();
        }

        /// <summary>
        /// 开始接收服务器信息
        /// </summary>
        protected void StartReceiving()
        {
            _ReceivingTask = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                _IsReceiving = true;
                while (Connected)
                {
                    Receiving();
                }
            });
            _Socket?.StartReceiving(_ReceivingTask);
        }

        /// <summary>
        /// 获取服务器已发送的信息为SocketObject数组
        /// </summary>
        /// <returns></returns>
        protected SocketObject[] GetServerMessage()
        {
            if (_Socket != null && _Socket.Connected)
            {
                return _Socket.ReceiveArray();
            }
            return Array.Empty<SocketObject>();
        }

        /// <summary>
        /// 具体接收服务器信息以及处理信息的方法
        /// </summary>
        /// <returns></returns>
        protected SocketMessageType Receiving()
        {
            if (_Socket is null) return SocketMessageType.Unknown;
            SocketMessageType result = SocketMessageType.Unknown;
            try
            {
                SocketObject[] ServerMessages = GetServerMessage();

                foreach (SocketObject ServerMessage in ServerMessages)
                {
                    SocketMessageType type = ServerMessage.SocketType;
                    object[] objs = ServerMessage.Parameters;
                    result = type;
                    switch (type)
                    {
                        case SocketMessageType.Disconnect:
                            Close();
                            SocketHandler_Disconnect(ServerMessage);
                            break;

                        case SocketMessageType.HeartBeat:
                            SocketHandler_HeartBeat(ServerMessage);
                            break;
                            
                        case SocketMessageType.ForceLogout:
                            SocketHandler_ForceLogout(ServerMessage);
                            break;
                            
                        case SocketMessageType.Chat:
                            SocketHandler_Chat(ServerMessage);
                            break;
                            
                        case SocketMessageType.UpdateRoomMaster:
                            SocketHandler_UpdateRoomMaster(ServerMessage);
                            break;
                            
                        case SocketMessageType.Unknown:
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // 报错中断服务器连接
                Error(e);
            }
            return result;
        }

        /// <summary>
        /// 客户端接收到服务器断开连接的通知后的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_Disconnect(SocketObject ServerMessage);

        /// <summary>
        /// 客户端接收到服务器心跳后的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_HeartBeat(SocketObject ServerMessage);

        /// <summary>
        /// 客户端接收到强制退出登录的通知后的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_ForceLogout(SocketObject ServerMessage);

        /// <summary>
        /// 客户端接收到聊天信息后的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_Chat(SocketObject ServerMessage);

        /// <summary>
        /// 客户端接收到更换房主后的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_UpdateRoomMaster(SocketObject ServerMessage);
    }
}
