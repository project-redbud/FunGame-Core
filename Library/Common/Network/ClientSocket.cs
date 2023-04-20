using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ClientSocket : IClientSocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public string ServerIP { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public string ClientIP { get; } = "";
        public string ClientName => _ClientName;
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _Receiving;

        private Task? ReceivingTask;

        private bool _Receiving;
        private string _ClientName;

        public ClientSocket(System.Net.Sockets.Socket Instance, int ServerPort, string ClientIP, string ClientName, Guid Token)
        {
            this.Instance= Instance;
            this.ServerPort = ServerPort;
            this.ClientIP = ClientIP;
            this._ClientName = ClientName;
            this.Token = Token;
        }

        public void Close()
        {
            StopReceiving();
            Instance?.Close();
        }

        public SocketObject Receive()
        {
            try
            {
                return SocketManager.Receive(Instance);
            }
            catch
            {
                throw new SocketWrongInfoException();
            }
        }
        
        public SocketObject[] ReceiveArray()
        {
            try
            {
                return SocketManager.ReceiveArray(Instance);
            }
            catch
            {
                throw new SocketWrongInfoException();
            }
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

        public void BindEvent(Delegate Method, bool Remove = false)
        {
            if (!Remove)
            {
                SocketManager.SocketReceive += (SocketManager.SocketReceiveHandler)Method;
            }
            else
            {
                SocketManager.SocketReceive -= (SocketManager.SocketReceiveHandler)Method;
            }
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
