using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ServerSocket : ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public int Runtime { get; } = (int)SocketRuntimeType.Server;
        public string ServerIP { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public int HeartBeatFaileds { get; private set; } = 0;
        public bool Connected
        {
            get
            {
                return Instance != null && Instance.Connected;
            }
        }
        public bool Receiving { get; private set; } = false;
        public bool SendingHeartBeat { get; private set; } = false;

        private ServerSocket(System.Net.Sockets.Socket Instance, int ServerPort)
        {
            this.Instance = Instance;
            this.ServerPort = ServerPort;
        }

        public ServerSocket StartListening(int Port = 22222, int MaxConnection = 20)
        {
            System.Net.Sockets.Socket? socket = SocketManager.StartListening(Port, MaxConnection);
            if (socket != null) return new ServerSocket(socket, Port);
            else throw new System.Exception("无法创建监听，请重新启动服务器再试。");
        }

        public ClientSocket Accept()
        {
            object[] result = SocketManager.Accept();
            if (result != null && result.Length == 2)
            {
                string ClientIP = (string)result[0];
                System.Net.Sockets.Socket Client = (System.Net.Sockets.Socket)result[1];
                return new ClientSocket(Client, ServerPort, ClientIP, ClientIP);
            }
            throw new System.Exception("无法获取客户端信息。");
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            throw new System.Exception("监听Socket不能用于发送信息。");
        }

        public object[] Receive()
        {
            throw new System.Exception("监听Socket不能用于接收信息。");
        }

        public void Close()
        {
            Instance?.Close();
        }

        public void StartReceiving(Task t)
        {
            throw new System.Exception("监听Socket不能用于接收信息。");
        }
    }
}
