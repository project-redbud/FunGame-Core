using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Service;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class Socket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public string ServerIP { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public int HeartBeatFaileds { get; } = 0;
        public bool Connected
        {
            get
            {
                return Instance != null && Instance.Connected;
            }
        }
        
        private Socket(System.Net.Sockets.Socket Instance, string ServerIP, int ServerPort)
        {
            this.Instance = Instance;
            this.ServerIP= ServerIP;
            this.ServerPort = ServerPort;
        }

        public static Socket Connect(string IP, int Port = 22222)
        {
            System.Net.Sockets.Socket? socket = SocketManager.Connect(IP, Port);
            if (socket != null) return new Socket(socket, IP, Port);
            else throw new Milimoe.FunGame.Core.Library.Exception.SystemError("创建Socket失败。");
        }

        public SocketResult Send(SocketMessageType type, string msg = "")
        {
            if (Instance != null)
            {
                if (SocketManager.Send(type, msg) == SocketResult.Success)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        private string[] Receive()
        {
            return SocketManager.Receive();
        }

        public void Run()
        {
            Task HeartBeatStream = Task.Factory.StartNew(StartSendHeartBeatStream);
            Task StreamReader = Task.Factory.StartNew(StartReceive);
        }

        private void StartReceive()
        {
            Thread.Sleep(100);
            while (Connected)
            {
                Receive();
            }
        }

        private void StartSendHeartBeatStream()
        {
            Thread.Sleep(100);
            while (Connected)
            {
                Send(SocketMessageType.HeartBeat); // 发送心跳包
                Thread.Sleep(20000);
            }
        }
    }
}
