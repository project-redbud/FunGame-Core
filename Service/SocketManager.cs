using System.Net;
using System.Net.Sockets;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Service
{
    internal class SocketManager
    {
        #region 属性

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

        #endregion

        #region 实现

        /// <summary>
        /// 创建服务器监听Socket
        /// </summary>
        /// <param name="Port">监听端口号</param>
        /// <param name="MaxConnection">最大连接数量</param>
        /// <returns>服务器端专用Socket</returns>
        internal static Socket? StartListening(int Port = 22222, int MaxConnection = 0)
        {
            if (MaxConnection <= 0) MaxConnection = SocketSet.MaxConnection_2C2G;
            try
            {
                _ServerSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ServerEndPoint = new(IPAddress.Any, Port);
                _ServerSocket.Bind(ServerEndPoint);
                _ServerSocket.Listen(MaxConnection);
                _ServerSocket.NoDelay = true;
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
                Client.NoDelay = true;
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
                                ClientSocket.NoDelay = true;
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
        /// <returns>SocketObject</returns>
        internal static Library.Common.Network.SocketObject Receive()
        {
            Library.Common.Network.SocketObject result = default;
            if (Socket != null)
            {
                // 从服务器接收消息
                byte[] buffer = new byte[General.SocketByteSize];
                int length = Socket.Receive(buffer);
                if (length > 0)
                {
                    string msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    Library.Common.Network.JsonObject json = Library.Common.Network.JsonObject.GetObject(msg);
                    result = new Library.Common.Network.SocketObject(json);
                    // 客户端接收消息，广播ScoketObject到每个UIModel
                    OnSocketReceive(result);
                    return result;
                }
            }
            return result;
        }
        
        /// <summary>
        /// 用于服务器接收客户端信息
        /// </summary>
        /// <param name="ClientSocket">客户端Socket</param>
        /// <returns>SocketObject</returns>
        internal static Library.Common.Network.SocketObject Receive(Socket ClientSocket)
        {
            Library.Common.Network.SocketObject result = default;
            if (ClientSocket != null)
            {
                // 从客户端接收消息
                byte[] buffer = new byte[General.SocketByteSize];
                int length = ClientSocket.Receive(buffer);
                if (length > 0)
                {
                    string msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    Library.Common.Network.JsonObject json = Library.Common.Network.JsonObject.GetObject(msg);
                    result = new Library.Common.Network.SocketObject(json);
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 用于客户端接收服务器信息（数组版）
        /// </summary>
        /// <returns>SocketObjects</returns>
        internal static Library.Common.Network.SocketObject[] ReceiveArray()
        {
            List<Library.Common.Network.SocketObject> result = new();
            if (Socket != null)
            {
                // 从服务器接收消息
                byte[] buffer = new byte[General.SocketByteSize];
                int length = Socket.Receive(buffer);
                if (length > 0)
                {
                    string msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    Library.Common.Network.JsonObject[] jsons = Library.Common.Network.JsonObject.GetObjects(msg);
                    foreach (Library.Common.Network.JsonObject json in jsons)
                    {
                        // 客户端接收消息，广播ScoketObject到每个UIModel
                        Library.Common.Network.SocketObject obj = new(json);
                        result.Add(obj);
                        OnSocketReceive(obj);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 用于服务器接收客户端信息（数组版）
        /// </summary>
        /// <param name="ClientSocket">客户端Socket</param>
        /// <returns>SocketObjects</returns>
        internal static Library.Common.Network.SocketObject[] ReceiveArray(Socket ClientSocket)
        {
            List<Library.Common.Network.SocketObject> result = new();
            if (ClientSocket != null)
            {
                // 从客户端接收消息
                byte[] buffer = new byte[General.SocketByteSize];
                int length = ClientSocket.Receive(buffer);
                if (length > 0)
                {
                    string msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    Library.Common.Network.JsonObject[] jsons = Library.Common.Network.JsonObject.GetObjects(msg);
                    foreach (Library.Common.Network.JsonObject json in jsons)
                    {
                        Library.Common.Network.SocketObject so = new(json);
                        result.Add(so);
                    }
                }
            }
            return result.ToArray();
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
                SocketMessageType.CreateRoom => SocketSet.CreateRoom,
                SocketMessageType.UpdateRoom => SocketSet.UpdateRoom,
                SocketMessageType.ChangeRoomSetting => SocketSet.ChangeRoomSetting,
                SocketMessageType.MatchRoom => SocketSet.MatchRoom,
                SocketMessageType.UpdateRoomMaster => SocketSet.UpdateRoomMaster,
                SocketMessageType.GetRoomPlayerCount => SocketSet.GetRoomPlayerCount,
                _ => SocketSet.Unknown,
            };
        }

        #endregion

        #region 事件

        /// <summary>
        /// 监听事件的委托
        /// </summary>
        /// <param name="SocketObject">SocketObject</param>
        internal delegate void SocketReceiveHandler(Library.Common.Network.SocketObject SocketObject);

        /// <summary>
        /// 监听事件
        /// </summary>
        internal static event SocketReceiveHandler? SocketReceive;

        /// <summary>
        /// 触发异步监听事件
        /// </summary>
        /// <param name="SocketObject">SocketObject</param>
        internal static void OnSocketReceive(Library.Common.Network.SocketObject SocketObject)
        {
            SocketReceive?.Invoke(SocketObject);
        }

        #endregion
    }
}
