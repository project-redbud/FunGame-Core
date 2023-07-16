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
                result = _Socket?.Send(SocketMessageType.RunTime_Disconnect, "") == SocketResult.Success;
            }
            catch (Exception e)
            {
                WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
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
        /// 客户端需要自行实现连接服务器的事务
        /// </summary>
        /// <returns>连接结果</returns>
        public abstract ConnectResult Connect();
        
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
                        case SocketMessageType.RunTime_Connect:
                            if (!SocketHandler_Connect(ServerMessage)) return SocketMessageType.Unknown;
                            break;

                        case SocketMessageType.RunTime_Disconnect:
                            SocketHandler_Disconnect(ServerMessage);
                            break;

                        case SocketMessageType.RunTime_HeartBeat:
                            if (_Socket != null && _Socket.Connected)
                            {
                                SocketHandler_HeartBeat(ServerMessage);
                            }
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
        /// 连接服务器的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        /// <returns></returns>
        protected abstract bool SocketHandler_Connect(SocketObject ServerMessage);

        /// <summary>
        /// 与服务器断开连接的处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_Disconnect(SocketObject ServerMessage);

        /// <summary>
        /// 心跳检测处理方法
        /// </summary>
        /// <param name="ServerMessage"></param>
        protected abstract void SocketHandler_HeartBeat(SocketObject ServerMessage);
    }
}
