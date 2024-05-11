using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ClientSocket(System.Net.Sockets.Socket Instance, int ServerPort, string ClientIP, string ClientName, Guid Token) : IClientSocket
    {
        public System.Net.Sockets.Socket Instance { get; } = Instance;
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Token;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = ServerPort;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public string ClientIP { get; } = ClientIP;
        public string ClientName => _ClientName;
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _Receiving;

        private Task? ReceivingTask;

        private bool _Receiving;
        private readonly string _ClientName = ClientName;

        public void Close()
        {
            StopReceiving();
            Instance?.Close();
        }

        public SocketObject[] Receive()
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

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                if (SocketManager.Send(Instance, new(type, Token, objs)) == SocketResult.Success)
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
