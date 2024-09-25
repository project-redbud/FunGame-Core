using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ClientSocket(System.Net.Sockets.Socket instance, string clientIP, string clientName, Guid token) : IClientSocket, ISocketMessageProcessor
    {
        public System.Net.Sockets.Socket Instance { get; } = instance;
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = token;
        public string ClientIP { get; } = clientIP;
        public string ClientName => _ClientName;
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _Receiving;
        public Type InstanceType => typeof(ClientSocket);

        private Task? ReceivingTask;

        private bool _Receiving;
        private readonly string _ClientName = clientName;

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

        public async Task<SocketResult> SendAsync(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                if (await SocketManager.SendAsync(Instance, new(type, Token, objs)) == SocketResult.Success)
                {
                    return SocketResult.Success;
                }
                else return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        public void BindEvent(Delegate method, bool remove = false)
        {
            if (!remove)
            {
                SocketManager.SocketReceive += (SocketManager.SocketReceiveHandler)method;
            }
            else
            {
                SocketManager.SocketReceive -= (SocketManager.SocketReceiveHandler)method;
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
