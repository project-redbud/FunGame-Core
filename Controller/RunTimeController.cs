using System.Collections;
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
        /// 与服务器的连接套接字实例（WebSocket）
        /// </summary>
        public HTTPClient? HTTPClient => _HTTPClient;

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
        /// 用于类内赋值
        /// </summary>
        protected HTTPClient? _HTTPClient;

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
        /// 发送结束游戏反馈
        /// </summary>
        /// <returns></returns>
        public bool EndGame()
        {
            bool result = false;

            try
            {
                result = _Socket?.Send(SocketMessageType.EndGame, "") == SocketResult.Success;
            }
            catch (Exception e)
            {
                WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }

        /// <summary>
        /// 连接服务器 [ 可选参数需要根据连接方式提供 ]
        /// 建议使用异步版，此方法为兼容性处理
        /// </summary>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="ssl"></param>
        /// <param name="subUrl"></param>
        /// <returns></returns>
        public ConnectResult Connect(TransmittalType type, string address, int port, bool ssl = false, string subUrl = "")
        {
            return Task.Run(() => ConnectAsync(type, address, port, ssl, subUrl)).Result;
        }

        /// <summary>
        /// 连接服务器 [ 异步版，可选参数需要根据连接方式提供 ]
        /// </summary>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="ssl"></param>
        /// <param name="subUrl"></param>
        /// <returns></returns>
        public async Task<ConnectResult> ConnectAsync(TransmittalType type, string address, int port = 0, bool ssl = false, string subUrl = "")
        {
            ArrayList connectArgs = [];
            if (!BeforeConnect(ref address, ref port, connectArgs))
            {
                return ConnectResult.ConnectFailed;
            }

            ConnectResult result = ConnectResult.Success;
            string msg;
            string serverName = "";
            string notice = "";

            // 检查服务器地址和端口是否正确
            if (address == "" || (type == TransmittalType.Socket && port <= 0) || (type == TransmittalType.WebSocket && port < 0))
            {
                result = ConnectResult.FindServerFailed;
            }
            if (result == ConnectResult.Success)
            {
                // 与服务器建立连接
                if (type == TransmittalType.Socket)
                {
                    connectArgs = await Connect_Socket(connectArgs, address, port);
                }
                else if (type == TransmittalType.WebSocket)
                {
                    connectArgs = await Connect_WebSocket(connectArgs, address, ssl, port, subUrl);
                }
                else
                {
                    result = ConnectResult.FindServerFailed;
                    msg = "连接服务器失败，未指定连接方式。";
                    connectArgs = [result, msg, serverName, notice];
                }
            }

            AfterConnect(connectArgs);

            // 允许修改数组中的result，强行改变连接的结果
            if (connectArgs.Count > 0)
            {
                result = (ConnectResult?)connectArgs[0] ?? result;
            }

            return result;
        }

        /// <summary>
        /// 使用 Socket 方式连接服务器
        /// </summary>
        /// <param name="connectArgs"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private async Task<ArrayList> Connect_Socket(ArrayList connectArgs, string address, int port)
        {
            ConnectResult result = ConnectResult.Success;
            string msg = "";
            string serverName = "";
            string notice = "";

            _Socket?.Close();
            _Socket = Socket.Connect(address, port);
            if (_Socket != null && _Socket.Connected)
            {
                if (_Socket.Send(SocketMessageType.Connect, connectArgs.Cast<object>().ToArray()) == SocketResult.Success)
                {
                    SocketObject[] objs = _Socket.Receive();
                    foreach (SocketObject obj in objs)
                    {
                        if (obj.SocketType == SocketMessageType.Connect)
                        {
                            bool success = obj.GetParam<bool>(0);
                            msg = obj.GetParam<string>(1) ?? "";
                            result = success ? ConnectResult.Success : ConnectResult.ConnectFailed;
                            if (success)
                            {
                                _Socket.Token = obj.GetParam<Guid>(2);
                                serverName = obj.GetParam<string>(3) ?? "";
                                notice = obj.GetParam<string>(4) ?? "";
                                StartReceiving();
                                await Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        if (_IsReceiving)
                                        {
                                            break;
                                        }
                                    }
                                });
                                _Socket.ConnectionLost += Error;
                            }
                        }
                    }
                }
                else result = ConnectResult.ConnectFailed;
            }
            else _Socket?.Close();

            return [result, msg, serverName, notice];
        }

        /// <summary>
        /// 使用 WebSocket 方式连接服务器
        /// </summary>
        /// <param name="connectArgs"></param>
        /// <param name="address"></param>
        /// <param name="ssl"></param>
        /// <param name="port"></param>
        /// <param name="subUrl"></param>
        /// <returns></returns>
        private async Task<ArrayList> Connect_WebSocket(ArrayList connectArgs, string address, bool ssl, int port, string subUrl)
        {
            ConnectResult result = ConnectResult.Success;
            string msg = "";
            string serverName = "";
            string notice = "";

            _HTTPClient?.Close();
            _HTTPClient = await HTTPClient.Connect(address, ssl, port, subUrl, connectArgs.Cast<object>().ToArray());
            if (_HTTPClient.Connected)
            {
                bool webSocketConnected = false;
                _HTTPClient.AddSocketObjectHandler(obj =>
                {
                    try
                    {
                        if (obj.SocketType == SocketMessageType.Connect)
                        {
                            bool success = obj.GetParam<bool>(0);
                            msg = obj.GetParam<string>(1) ?? "";
                            result = success ? ConnectResult.Success : ConnectResult.ConnectFailed;
                            if (success)
                            {
                                _HTTPClient.Token = obj.GetParam<Guid>(2);
                                serverName = obj.GetParam<string>(3) ?? "";
                                notice = obj.GetParam<string>(4) ?? "";
                            }
                            webSocketConnected = true;
                            return;
                        }
                        HandleSocketMessage(TransmittalType.WebSocket, obj);
                    }
                    catch (Exception e)
                    {
                        Error(e);
                    }
                });
                while (!webSocketConnected)
                {
                    await Task.Delay(100);
                }
                _HTTPClient.ConnectionLost += Error;
            }
            else
            {
                _HTTPClient?.Close();
                result = ConnectResult.ConnectFailed;
            }

            return [result, msg, serverName, notice];
        }

        /// <summary>
        /// 获取服务器地址
        /// </summary>
        /// <returns>string：服务器地址；int：端口号</returns>
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
        /// <param name="address">服务器地址</param>
        /// <param name="port">服务器端口</param>
        /// <param name="args">重写时可以提供额外的连接参数</param>
        /// <returns>false：中止连接</returns>
        public virtual bool BeforeConnect(ref string address, ref int port, ArrayList args)
        {
            return true;
        }

        /// <summary>
        /// 此方法将在连接服务器后触发（Connect结果返回前）<para/>
        /// 客户端可以重写此方法
        /// </summary>
        /// <param name="ConnectArgs">连接服务器后返回的一些数据，可以使用也可以修改它们</param>
        /// <returns></returns>
        public virtual void AfterConnect(ArrayList ConnectArgs)
        {

        }

        /// <summary>
        /// 客户端需要自行实现自动登录的事务
        /// </summary>
        public virtual void AutoLogin(string Username, string Password, string AutoKey)
        {

        }

        /// <summary>
        /// 关闭 Socket 连接
        /// </summary>
        /// <returns></returns>
        public bool Close_Socket()
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
        /// 关闭 WebSocket 连接
        /// </summary>
        /// <returns></returns>
        public bool Close_WebSocket()
        {
            try
            {
                if (_HTTPClient != null)
                {
                    _HTTPClient.Close();
                    _HTTPClient = null;
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
        /// <param name="level"></param>
        /// <param name="useLevel"></param>
        public abstract void WritelnSystemInfo(string msg, LogLevel level = LogLevel.Info, bool useLevel = true);

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
            else if (_HTTPClient != null)
            {
                DataRequest request = new(_HTTPClient, RequestType);
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
            else if (_HTTPClient != null)
            {
                DataRequest request = new(_HTTPClient, RequestType, true);
                return request;
            }
            throw new ConnectFailedException();
        }

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求<para/>
        /// 加载项专用（<see cref="Library.Common.Addon.Plugin"/> / <see cref="Library.Common.Addon.GameModule"/>）
        /// </summary>
        /// <param name="RequestType"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        public DataRequest NewDataRequestForAddon(DataRequestType RequestType)
        {
            if (_Socket != null)
            {
                DataRequest request = new(_Socket, RequestType, false, SocketRuntimeType.Addon);
                return request;
            }
            else if (_HTTPClient != null)
            {
                DataRequest request = new(_HTTPClient, RequestType, false, SocketRuntimeType.Addon);
                return request;
            }
            throw new ConnectFailedException();
        }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// 加载项专用（<see cref="Library.Common.Addon.Plugin"/> / <see cref="Library.Common.Addon.GameModule"/>）
        /// </summary>
        /// <param name="RequestType"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        public DataRequest NewLongRunningDataRequestForAddon(DataRequestType RequestType)
        {
            if (_Socket != null)
            {
                DataRequest request = new(_Socket, RequestType, true, SocketRuntimeType.Addon);
                return request;
            }
            else if (_HTTPClient != null)
            {
                DataRequest request = new(_HTTPClient, RequestType, true, SocketRuntimeType.Addon);
                return request;
            }
            throw new ConnectFailedException();
        }

        /// <summary>
        /// 基于本地已连接的Socket创建新的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 加载项专用：此方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的
        /// </summary>
        /// <param name="GamingType"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        public DataRequest NewDataRequestForAddon(GamingType GamingType)
        {
            if (_Socket != null)
            {
                DataRequest request = new(_Socket, GamingType, false, SocketRuntimeType.Addon);
                return request;
            }
            else if (_HTTPClient != null)
            {
                DataRequest request = new(_HTTPClient, GamingType, false, SocketRuntimeType.Addon);
                return request;
            }
            throw new ConnectFailedException();
        }

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的局内（<see cref="Model.Gaming"/>）数据请求<para/>
        /// 加载项专用：此方法是给 <see cref="Library.Common.Addon.GameModule"/> 提供的
        /// </summary>
        /// <param name="GamingType"></param>
        /// <returns></returns>
        /// <exception cref="ConnectFailedException"></exception>
        public DataRequest NewLongRunningDataRequestForAddon(GamingType GamingType)
        {
            if (_Socket != null)
            {
                DataRequest request = new(_Socket, GamingType, true, SocketRuntimeType.Addon);
                return request;
            }
            else if (_HTTPClient != null)
            {
                DataRequest request = new(_HTTPClient, GamingType, true, SocketRuntimeType.Addon);
                return request;
            }
            throw new ConnectFailedException();
        }

        /// <summary>
        /// 开始接收服务器信息 [ Socket Only ]
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
        /// 获取服务器已发送的信息为SocketObject数组 [ Socket Only ]
        /// </summary>
        /// <returns></returns>
        protected SocketObject[] GetServerMessages()
        {
            if (_Socket != null && _Socket.Connected)
            {
                return _Socket.Receive();
            }
            return [];
        }

        /// <summary>
        /// 具体接收服务器信息以及处理信息的方法 [ Socket Only ]
        /// </summary>
        /// <returns></returns>
        protected SocketMessageType Receiving()
        {
            if (_Socket is null) return SocketMessageType.Unknown;
            SocketMessageType result = SocketMessageType.Unknown;
            try
            {
                SocketObject[] messages = GetServerMessages();

                foreach (SocketObject obj in messages)
                {
                    result = HandleSocketMessage(TransmittalType.Socket, obj);
                }
            }
            catch (Exception e)
            {
                _Socket?.OnConnectionLost(e);
                Close_Socket();
            }
            return result;
        }

        /// <summary>
        /// 处理接收到的信息
        /// </summary>
        /// <param name="transmittalType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected SocketMessageType HandleSocketMessage(TransmittalType transmittalType, SocketObject obj)
        {
            SocketMessageType type = obj.SocketType;
            SocketMessageType result = type;
            switch (type)
            {
                case SocketMessageType.Disconnect:
                    if (transmittalType == TransmittalType.Socket)
                    {
                        Close_Socket();
                    }
                    else if (transmittalType == TransmittalType.WebSocket)
                    {
                        Close_WebSocket();
                    }
                    SocketHandler_Disconnect(obj);
                    break;

                case SocketMessageType.System:
                    SocketHandler_System(obj);
                    break;

                case SocketMessageType.HeartBeat:
                    SocketHandler_HeartBeat(obj);
                    break;

                case SocketMessageType.ForceLogout:
                    SocketHandler_ForceLogout(obj);
                    break;

                case SocketMessageType.Chat:
                    SocketHandler_Chat(obj);
                    break;

                case SocketMessageType.UpdateRoomMaster:
                    SocketHandler_UpdateRoomMaster(obj);
                    break;

                case SocketMessageType.MatchRoom:
                    SocketHandler_MatchRoom(obj);
                    break;

                case SocketMessageType.StartGame:
                    SocketHandler_StartGame(obj);
                    break;

                case SocketMessageType.EndGame:
                    SocketHandler_EndGame(obj);
                    break;

                case SocketMessageType.Gaming:
                    SocketHandler_Gaming(obj);
                    break;

                case SocketMessageType.AnonymousGameServer:
                    SocketHandler_AnonymousGameServer(obj);
                    break;

                case SocketMessageType.Unknown:
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 客户端接收服务器断开连接的通知
        /// </summary>
        /// <param name="obj"></param>
        protected abstract void SocketHandler_Disconnect(SocketObject obj);

        /// <summary>
        /// 客户端接收并处理服务器系统消息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_System(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理服务器心跳
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_HeartBeat(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收强制退出登录的通知
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_ForceLogout(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理聊天信息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_Chat(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理更换房主信息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_UpdateRoomMaster(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理匹配房间成功信息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_MatchRoom(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理开始游戏信息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_StartGame(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理游戏结束信息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_EndGame(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理局内消息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_Gaming(SocketObject obj)
        {

        }

        /// <summary>
        /// 客户端接收并处理匿名服务器的消息
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SocketHandler_AnonymousGameServer(SocketObject obj)
        {

        }
    }
}
