using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ClientSocket : ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public int Runtime { get; } = (int)SocketRuntimeType.Server;
        public string Token { get; } = "";
        public string ServerIP { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public string ClientIP { get; } = "";
        public string ClientName
        {
            get
            {
                return _ClientName;
            }
        }
        public bool Connected
        {
            get
            {
                return Instance != null && Instance.Connected;
            }
        }
        public bool Receiving
        {
            get
            {
                return _Receiving;
            }
        }

        private Task? ReceivingTask;

        private bool _Receiving;
        private string _ClientName;

        public ClientSocket(System.Net.Sockets.Socket Instance, int ServerPort, string ClientIP, string ClientName)
        {
            this.Instance= Instance;
            this.ServerPort = ServerPort;
            this.ClientIP = ClientIP;
            this._ClientName = ClientName;
        }

        public void Close()
        {
            StopReceiving();
            Instance?.Close();
        }

        public object[] Receive()
        {
            object[] result = SocketManager.Receive(Instance);
            if (result.Length != 2) throw new System.Exception("收到错误的返回信息。");
            return result;
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                if (SocketManager.Send(Instance, type, Token, objs) == SocketResult.Success)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        public void StartReceiving(Task t)
        {
            _Receiving = true;
            ReceivingTask = t;
        }

        public void StopReceiving()
        {
            _Receiving = false;
            ReceivingTask?.Wait(1);
            ReceivingTask = null;
        }
    }
}
