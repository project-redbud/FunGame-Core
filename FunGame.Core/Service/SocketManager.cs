using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Interface.Base;
using System.Collections;
using System.Net.Sockets;
using System.Net;

namespace Milimoe.FunGame.Core.Service
{
    internal class SocketManager
    {
        internal static SocketManager? Instance { get; private set; }

        internal Socket? Socket { get; } = null;

        private SocketManager(Socket socket)
        {
            Socket = socket;
        }

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
                                Instance = new SocketManager(socket);
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
    }
}
