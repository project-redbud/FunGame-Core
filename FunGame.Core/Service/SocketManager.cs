using System.Net;
using System.Net.Sockets;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Service
{
    internal class SocketManager
    {
        /// <summary>
        /// 客户端专用Socket
        /// </summary>
        internal static Socket? Socket => _Socket;

        /// <summary>
        /// 服务器端专用Socket
        /// </summary>
        internal static Socket? ServerSocket => _ServerSocket;

        private static Socket? _Socket = null;
        private static Socket? _ServerSocket = null;

        /// <summary>
        /// 异步监听事件
        /// </summary>
        /// <typeparam name="T">结果类</typeparam>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>结果</returns>
        internal delegate Task<T> SocketHandler<T>(SocketMessageType type, params object[] objs);
        
        /// <summary>
        /// 异步监听事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>线程</returns>
        internal delegate Task SocketHandler(SocketMessageType type, params object[] objs);

        /// <summary>
        /// 监听返回值为bool的事件
        /// </summary>
        internal event SocketHandler<bool>? SocketReceiveBoolAsync;
        
        /// <summary>
        /// 监听返回值为String的事件
        /// </summary>
        internal event SocketHandler<string>? SocketReceiveStringAsync;
        
        /// <summary>
        /// 监听返回值为object的事件
        /// </summary>
        internal event SocketHandler<object>? SocketReceiveObjectAsync;
        
        /// <summary>
        /// 监听返回值为int的事件
        /// </summary>
        internal event SocketHandler<int>? SocketReceiveIntAsync;
        
        /// <summary>
        /// 监听返回值为decimal的事件
        /// </summary>
        internal event SocketHandler<decimal>? SocketReceiveDecimalAsync;
        
        /// <summary>
        /// 监听没有返回值的事件
        /// </summary>
        internal event SocketHandler? SocketReceiveAsync;

        /// <summary>
        /// 创建服务器监听Socket
        /// </summary>
        /// <param name="Port">监听端口号</param>
        /// <param name="MaxConnection">最大连接数量</param>
        /// <returns>服务器端专用Socket</returns>
        internal static Socket? StartListening(int Port = 22222, int MaxConnection = 0)
        {
            if (MaxConnection <= 0) MaxConnection = SocketSet.MaxConnection_General;
            try
            {
                _ServerSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ServerEndPoint = new(IPAddress.Any, Port);
                _ServerSocket.Bind(ServerEndPoint);
                _ServerSocket.Listen(MaxConnection);
                return _ServerSocket;
            }
            catch
            {
                ServerSocket?.Close();
            }
            return null;
        }

        /// <summary>
        /// 创建一个监听到客户端Socket
        /// </summary>
        /// <returns>客户端IP地址[0]和客户端Socket[1]</returns>
        internal static object[] Accept()
        {
            if (ServerSocket is null) return Array.Empty<object>();
            Socket Client;
            string ClientIP;
            try
            {
                Client = ServerSocket.Accept();
                IPEndPoint? ClientIPEndPoint = (IPEndPoint?)Client.RemoteEndPoint;
                ClientIP = (ClientIPEndPoint != null) ? ClientIPEndPoint.ToString() : "Unknown";
                return new object[] { ClientIP, Client };
            }
            catch
            {
                ServerSocket?.Close();
            }
            return Array.Empty<object>();
        }

        /// <summary>
        /// 创建客户端Socket
        /// </summary>
        /// <param name="IP">服务器IP地址</param>
        /// <param name="Port">服务器监听端口</param>
        /// <returns>客户端专用Socket</returns>
        internal static Socket? Connect(string IP, int Port = 22222)
        {
            Socket? ClientSocket;
            EndPoint ServerEndPoint;
            try
            {
                ClientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
                if (ServerEndPoint != null)
                {
                    while (true)
                    {
                        if (!ClientSocket.Connected)
                        {
                            ClientSocket.Connect(ServerEndPoint);
                            if (ClientSocket.Connected)
                            {
                                _Socket = ClientSocket;
                                return _Socket;
                            }
                        }
                    }
                }
            }
            catch
            {
                _Socket?.Close();
            }
            return null;
        }
        
        /// <summary>
        /// 用于服务器端向客户端Socket发送信息
        /// </summary>
        /// <param name="ClientSocket">客户端Socket</param>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>通信结果</returns>
        internal static SocketResult Send(Socket ClientSocket, SocketMessageType type, Guid token, params object[] objs)
        {
            if (ClientSocket != null && objs != null && objs.Length > 0)
            {
                if (ClientSocket.Send(General.DefaultEncoding.GetBytes(Library.Common.Network.JsonObject.GetString(type, token, objs))) > 0)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        /// <summary>
        /// 用于客户端向服务器Socket发送信息
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>通信结果</returns>
        internal static SocketResult Send(SocketMessageType type, Guid token, params object[] objs)
        {
            if (objs is null || objs.Length <= 0)
            {
                objs = new object[] { "" };
            }
            if (Socket != null)
            {
                if (Socket.Send(General.DefaultEncoding.GetBytes(Library.Common.Network.JsonObject.GetString(type, token, objs))) > 0)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        /// <summary>
        /// 用于客户端接收服务器信息
        /// </summary>
        /// <returns>通信类型[0]和参数[1]</returns>
        internal static object[] Receive()
        {
            object[] result = Array.Empty<object>();
            if (Socket != null)
            {
                // 从服务器接收消息
                byte[] buffer = new byte[2048];
                int length = Socket.Receive(buffer);
                if (length > 0)
                {
                    string msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    Library.Common.Network.JsonObject? json = Library.Common.Network.JsonObject.GetObject(msg);
                    if (json != null)
                    {
                        result = new object[] { json.MessageType, json.Parameters };
                    }
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 用于服务器接收客户端信息
        /// </summary>
        /// <param name="ClientSocket">客户端Socket</param>
        /// <returns>通信类型[0]、Token[1]和参数[2]</returns>
        internal static object[] Receive(Socket ClientSocket)
        {
            object[] result = Array.Empty<object>();
            if (ClientSocket != null)
            {
                // 从客户端接收消息
                byte[] buffer = new byte[2048];
                int length = ClientSocket.Receive(buffer);
                if (length > 0)
                {
                    string msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    Library.Common.Network.JsonObject? json = Library.Common.Network.JsonObject.GetObject(msg);
                    if (json != null)
                    {
                        result = new object[] { json.MessageType, json.Token, json.Parameters };
                    }
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 将通信类型的枚举转换为字符串
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <returns>等效字符串</returns>
        internal static string GetTypeString(SocketMessageType type)
        {
            return type switch
            {
                SocketMessageType.Connect => SocketSet.Connect,
                SocketMessageType.GetNotice => SocketSet.GetNotice,
                SocketMessageType.Login => SocketSet.Login,
                SocketMessageType.CheckLogin => SocketSet.CheckLogin,
                SocketMessageType.Logout => SocketSet.Logout,
                SocketMessageType.Disconnect => SocketSet.Disconnect,
                SocketMessageType.HeartBeat => SocketSet.HeartBeat,
                SocketMessageType.IntoRoom => SocketSet.IntoRoom,
                SocketMessageType.QuitRoom => SocketSet.QuitRoom,
                SocketMessageType.Chat => SocketSet.Chat,
                SocketMessageType.Reg => SocketSet.Reg,
                SocketMessageType.CheckReg => SocketSet.CheckReg,
                _ => SocketSet.Unknown,
            };
        }

        /// <summary>
        /// 触发异步返回bool事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>bool结果</returns>
        internal async Task<bool> OnSocketReceiveBoolAsync(SocketMessageType type, params object[] objs)
        {
            if (SocketReceiveBoolAsync != null)
            {
                return await SocketReceiveBoolAsync.Invoke(type, objs);
            }
            return false;
        }

        /// <summary>
        /// 触发异步返回string事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>string结果</returns>
        internal async Task<string> OnSocketReceiveStringAsync(SocketMessageType type, params object[] objs)
        {
            if (SocketReceiveStringAsync != null)
            {
                return await SocketReceiveStringAsync.Invoke(type, objs);
            }
            return "";
        }

        /// <summary>
        /// 触发异步返回object事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>object结果</returns>
        internal async Task<object> OnSocketReceiveObjectAsync(SocketMessageType type, params object[] objs)
        {
            if (SocketReceiveObjectAsync != null)
            {
                return await SocketReceiveObjectAsync.Invoke(type, objs);
            }
            return General.EntityInstance;
        }

        /// <summary>
        /// 触发异步返回int事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>int结果</returns>
        internal async Task<int> OnSocketReceiveIntAsync(SocketMessageType type, params object[] objs)
        {
            if (SocketReceiveIntAsync != null)
            {
                return await SocketReceiveIntAsync.Invoke(type, objs);
            }
            return -1;
        }

        /// <summary>
        /// 触发异步返回decimal事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        /// <returns>decimal结果</returns>
        internal async Task<decimal> OnSocketReceiveDecimalAsync(SocketMessageType type, params object[] objs)
        {
            if (SocketReceiveDecimalAsync != null)
            {
                return await SocketReceiveDecimalAsync.Invoke(type, objs);
            }
            return -1;
        }

        /// <summary>
        /// 触发异步无返回值事件
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <param name="objs">参数</param>
        internal async Task OnSocketReceiveAsync(SocketMessageType type, params object[] objs)
        {
            if (SocketReceiveAsync != null)
            {
                await SocketReceiveAsync.Invoke(type, objs);
            }
        }
    }
}
