using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Interface.Base;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Api.Utility;

namespace Milimoe.FunGame.Core.Service
{
    internal class SocketManager
    {
        internal static Socket? Socket { get; private set; } = null;

        internal static Socket? Connect(string IP, int Port = 22222)
        {
            Socket? socket = null;
            EndPoint ServerEndPoint;
            try
            {
                socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
                if (ServerEndPoint != null)
                {
                    while (true)
                    {
                        if (!socket.Connected)
                        {
                            socket.Connect(ServerEndPoint);
                            if (socket.Connected)
                            {
                                SocketManager.Socket = socket;
                                return socket;
                            }
                        }
                    }
                }
            }
            catch
            {
                socket?.Close();
            }
            return null;
        }

        internal static SocketResult Send(SocketMessageType type, string msg)
        {
            if (Socket != null)
            {
                if (Socket.Send(Core.Library.Constant.General.DEFAULT_ENCODING.GetBytes(MakeMessage(type, msg))) > 0)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        internal static string[] Receive()
        {
            string[] result = new string[2];
            if (Socket != null)
            {
                // 从服务器接收消息
                byte[] buffer = new byte[2048];
                int length = Socket.Receive(buffer);
                if (length > 0)
                {
                    string msg = Core.Library.Constant.General.DEFAULT_ENCODING.GetString(buffer, 0, length);
                    result[0] = GetTypeString(GetType(msg));
                    result[1] = GetMessage(msg);
                    return result;
                }
            }
            return result;
        }

        private static int GetType(string msg)
        {
            int index = msg.IndexOf(';') - 1;
            if (index > 0)
                return Convert.ToInt32(msg[..index]);
            else
                return Convert.ToInt32(msg[..1]);
        }

        private static string GetMessage(string msg)
        {
            int index = msg.IndexOf(';') + 1;
            return msg[index..];
        }

        private static string MakeMessage(SocketMessageType type, string msg)
        {
            return (int)type + ";" + msg;
        }

        private static string GetTypeString(SocketMessageType type)
        {
            return type switch
            {
                SocketMessageType.GetNotice => SocketSet.GetNotice,
                SocketMessageType.Login => SocketSet.Login,
                SocketMessageType.CheckLogin => SocketSet.CheckLogin,
                SocketMessageType.Logout => SocketSet.Logout,
                SocketMessageType.Disconnect => SocketSet.Disconnect,
                SocketMessageType.HeartBeat => SocketSet.HeartBeat,
                _ => SocketSet.Unknown,
            };
        }

        private static string GetTypeString(int type)
        {
            return GetTypeString((SocketMessageType)type);
        }
    }
}
