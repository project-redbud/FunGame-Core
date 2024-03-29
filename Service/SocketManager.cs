﻿using System.Net;
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
            if (ServerSocket is null) return [];
            Socket Client;
            string ClientIP;
            try
            {
                Client = ServerSocket.Accept();
                Client.NoDelay = true;
                IPEndPoint? ClientIPEndPoint = (IPEndPoint?)Client.RemoteEndPoint;
                ClientIP = (ClientIPEndPoint != null) ? ClientIPEndPoint.ToString() : "Unknown";
                return [ClientIP, Client];
            }
            catch
            {
                ServerSocket?.Close();
            }
            return [];
        }

        /// <summary>
        /// 创建客户端Socket
        /// </summary>
        /// <param name="Address">服务器IP地址</param>
        /// <param name="Port">服务器监听端口</param>
        /// <returns>客户端专用Socket</returns>
        internal static Socket? Connect(string Address, int Port = 22222)
        {
            Socket? ClientSocket;
            EndPoint ServerEndPoint;
            try
            {
                string IP = Api.Utility.NetworkUtility.GetIPAddress(Address);
                ServerEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
                if (ServerEndPoint != null)
                {
                    ClientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    bool Connecting = true;
                    Task t = Task.Run(() =>
                    {
                        while (Connecting)
                        {
                            if (!ClientSocket.Connected && Connecting)
                            {
                                ClientSocket.Connect(ServerEndPoint);
                                if (ClientSocket.Connected)
                                {
                                    ClientSocket.NoDelay = true;
                                    _Socket = ClientSocket;
                                    break;
                                }
                            }
                        }
                    });
                    if (t.Wait(10 * 1000) && (_Socket?.Connected ?? false))
                    {
                        return _Socket;
                    }
                    else
                    {
                        Connecting = false;
                        throw new ConnectFailedException();
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
        /// <param name="SocketObject">Socket信息容器</param>
        /// <returns>通信结果</returns>
        internal static SocketResult Send(Socket ClientSocket, Library.Common.Network.SocketObject SocketObject)
        {
            if (ClientSocket != null)
            {
                if (ClientSocket.Send(General.DefaultEncoding.GetBytes(JsonManager.GetString(SocketObject))) > 0)
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
        /// <param name="SocketObject">Socket信息容器</param>
        /// <returns>通信结果</returns>
        internal static SocketResult Send(Library.Common.Network.SocketObject SocketObject)
        {
            if (Socket != null)
            {
                if (Socket.Send(General.DefaultEncoding.GetBytes(JsonManager.GetString(SocketObject))) > 0)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        /// <summary>
        /// 接收数据流中的信息
        /// <para/>如果是服务器接收信息需要传入客户端Socket <paramref name="ClientSocket"/>
        /// </summary>
        /// <param name="ClientSocket">如果是服务器接收信息需要传入客户端Socket</param>
        /// <returns>SocketObjects</returns>
        internal static Library.Common.Network.SocketObject[] Receive(Socket? ClientSocket = null)
        {
            List<Library.Common.Network.SocketObject> result = [];
            Socket? tempSocket = ClientSocket is null ? Socket : ClientSocket;
            if (tempSocket != null)
            {
                // 从服务器接收消息
                byte[] buffer = new byte[General.SocketByteSize];
                int length = tempSocket.Receive(buffer, buffer.Length, SocketFlags.None);
                string msg = "";
                if (length > 0)
                {
                    msg = General.DefaultEncoding.GetString(buffer, 0, length);
                    if (JsonManager.IsCompleteJson<Library.Common.Network.SocketObject>(msg))
                    {
                        foreach (Library.Common.Network.SocketObject obj in JsonManager.GetObjects<Library.Common.Network.SocketObject>(msg))
                        {
                            result.Add(obj);
                            // 客户端接收消息，广播ScoketObject到每个UIModel
                            if (ClientSocket is null) OnSocketReceive(obj);
                        }
                        return [.. result];
                    }
                    else
                    {
                        Thread.Sleep(20);
                        while (true)
                        {
                            if (tempSocket.Available > 0)
                            {
                                length = tempSocket.Receive(buffer, buffer.Length, SocketFlags.None);
                                msg += General.DefaultEncoding.GetString(buffer, 0, length);
                                if (JsonManager.IsCompleteJson<Library.Common.Network.SocketObject>(msg))
                                {
                                    break;
                                }
                                Thread.Sleep(20);
                            }
                            else break;
                        }
                    }
                }
                foreach (Library.Common.Network.SocketObject obj in JsonManager.GetObjects<Library.Common.Network.SocketObject>(msg))
                {
                    result.Add(obj);
                    // 客户端接收消息，广播ScoketObject到每个UIModel
                    if (ClientSocket is null) OnSocketReceive(obj);
                }
            }
            return [.. result];
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
