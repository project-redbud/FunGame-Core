using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ClientSocket(ServerSocket listener, System.Net.Sockets.Socket instance, string clientIP, string clientName, Guid token) : IClientSocket, ISocketMessageProcessor
    {
        public ServerSocket Listener { get; } = listener;
        public System.Net.Sockets.Socket Instance { get; } = instance;
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = token;
        public string ClientIP { get; } = clientIP;
        public string ClientName => clientName;
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _receiving;
        public Type InstanceType => typeof(ClientSocket);

        private Task? _receivingTask;
        private bool _receiving;
        private readonly HashSet<Action<SocketObject>> _boundEvents = [];

        public void Close()
        {
            StopReceiving();
            Instance.Close();
        }
        
        public async Task CloseAsync()
        {
            StopReceiving();
            await Task.Run(() => Instance?.Close());
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

        public void AddSocketObjectHandler(Action<SocketObject> method)
        {
            if (_boundEvents.Add(method))
            {
                SocketManager.SocketReceive += new SocketManager.SocketReceiveHandler(method);
            }
        }

        public void RemoveSocketObjectHandler(Action<SocketObject> method)
        {
            _boundEvents.Remove(method);
            SocketManager.SocketReceive -= new SocketManager.SocketReceiveHandler(method);
        }

        public void StartReceiving(Task t)
        {
            _receiving = true;
            _receivingTask = t;
        }

        public void StopReceiving()
        {
            _receiving = false;
            _receivingTask?.Wait(1);
            _receivingTask = null;
        }
    }
}
