using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class Socket : IClientSocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Client;
        public Guid Token { get; set; } = Guid.Empty;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _receiving;
        private HeartBeat HeartBeat { get; }

        public event Action<System.Exception>? ConnectionLost;
        public event Action? Closed;

        private Task? _receivingTask;
        private bool _receiving = false;
        private readonly HashSet<Action<SocketObject>> _boundEvents = [];

        private Socket(System.Net.Sockets.Socket instance, string serverAddress, int serverPort)
        {
            this.Instance = instance;
            this.ServerAddress = serverAddress;
            this.ServerPort = serverPort;
            HeartBeat = new(this);
            HeartBeat.StartSendingHeartBeat();
        }

        public static Socket Connect(string address, int port = 22222)
        {
            System.Net.Sockets.Socket? socket = SocketManager.Connect(address, port);
            if (socket != null) return new Socket(socket, address, port);
            else throw new ConnectFailedException();
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                if (SocketManager.Send(new(type, Token, objs)) == SocketResult.Success)
                {
                    return SocketResult.Success;
                }
                return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        public SocketObject[] Receive()
        {
            try
            {
                SocketObject[] result = SocketManager.Receive();
                return result;
            }
            catch (System.Exception e)
            {
                OnConnectionLost(e);
                Close();
                Api.Utility.TXTHelper.AppendErrorLog(e.GetErrorInfo());
                throw new SocketWrongInfoException();
            }
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

        public void OnConnectionLost(System.Exception e)
        {
            ConnectionLost?.Invoke(e);
        }

        public void Close()
        {
            HeartBeat.StopSendingHeartBeat();
            StopReceiving();
            Instance?.Close();
            foreach (Action<SocketObject> method in _boundEvents.ToList())
            {
                RemoveSocketObjectHandler(method);
            }
            Closed?.Invoke();
            ConnectionLost = null;
            Closed = null;
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
