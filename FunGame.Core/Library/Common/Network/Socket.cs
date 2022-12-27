using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Service;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class Socket : ISocket, ISocketHeartBeat
    {
        public System.Net.Sockets.Socket Instance { get; }
        public int Runtime { get; } = (int)SocketRuntimeType.Client;
        public string Token { get; set; } = "";
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

        private Task? SendingHeartBeatTask;
        private Task? ReceivingTask;
        private Task? WaitHeartBeatReply;

        private Socket(System.Net.Sockets.Socket Instance, string ServerIP, int ServerPort)
        {
            this.Instance = Instance;
            this.ServerIP= ServerIP;
            this.ServerPort = ServerPort;
            this.StartSendingHeartBeat();
        }

        public static Socket Connect(string IP, int Port = 22222)
        {
            System.Net.Sockets.Socket? socket = SocketManager.Connect(IP, Port);
            if (socket != null) return new Socket(socket, IP, Port);
            else throw new System.Exception("连接到服务器失败。");
        }
        
        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                if (SocketManager.Send(type, Token, objs) == SocketResult.Success)
                {
                    return SocketResult.Success;
                }
                return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        public object[] Receive()
        {
            object[] result = SocketManager.Receive();
            if (result.Length != 2) throw new System.Exception("收到错误的返回信息。");
            if ((SocketMessageType)result[0] == SocketMessageType.HeartBeat)
            {
                if (WaitHeartBeatReply != null && !WaitHeartBeatReply.IsCompleted) WaitHeartBeatReply.Wait(1);
                HeartBeatFaileds = 0;
            }
            return result;
        }

        public void CheckHeartBeatFaileds()
        {
            if (HeartBeatFaileds >= 3) Close();
        }

        public void Close()
        {
            StopSendingHeartBeat();
            StopReceiving();
            Instance?.Close();
        }

        public void ResetHeartBeatFaileds()
        {
            HeartBeatFaileds = 0;
        }

        public void StartReceiving(Task t)
        {
            Receiving = true;
            ReceivingTask = t;
        }

        private void StartSendingHeartBeat()
        {
            SendingHeartBeat = true;
            SendingHeartBeatTask = Task.Factory.StartNew(SendHeartBeat);
        }

        private void StopReceiving()
        {
            Receiving = false;
            ReceivingTask?.Wait(1);
            ReceivingTask = null;
        }

        private void StopSendingHeartBeat()
        {
            SendingHeartBeat = false;
            SendingHeartBeatTask?.Wait(1);
            SendingHeartBeatTask = null;
        }

        private void SendHeartBeat()
        {
            Thread.Sleep(100);
            while (Connected)
            {
                if (!SendingHeartBeat) SendingHeartBeat= true;
                // 发送心跳包
                if (Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                {
                    WaitHeartBeatReply = Task.Run(() =>
                    {
                        Thread.Sleep(4000);
                        AddHeartBeatFaileds();
                    });
                }
                else AddHeartBeatFaileds();
                Thread.Sleep(20000);
            }
            SendingHeartBeat = false;
        }

        private void AddHeartBeatFaileds()
        {
            // 超过三次没回应心跳，服务器连接失败。
            if (HeartBeatFaileds++ >= 3)
                throw new System.Exception("ERROR：与服务器连接中断。");
        }

        public static string GetTypeString(SocketMessageType type)
        {
            return SocketManager.GetTypeString(type);
        }
    }
}
