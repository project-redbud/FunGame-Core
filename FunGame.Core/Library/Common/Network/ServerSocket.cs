using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Server;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ServerSocket : ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public int Runtime { get; } = (int)SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public string ServerIP { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _Receiving;

        private readonly ThreadManager PlayerThreads;
        private bool _Receiving = false;

        private ServerSocket(System.Net.Sockets.Socket Instance, int ServerPort, int MaxConnection = 0)
        {
            this.Instance = Instance;
            this.ServerPort = ServerPort;
            if (MaxConnection <= 0)
                PlayerThreads = new ThreadManager();
            else
                PlayerThreads = new ThreadManager(MaxConnection);
        }

        public static ServerSocket StartListening(int Port = 22222, int MaxConnection = 0)
        {
            if (MaxConnection <= 0) MaxConnection = SocketSet.MaxConnection_General;
            System.Net.Sockets.Socket? socket = SocketManager.StartListening(Port, MaxConnection);
            if (socket != null) return new ServerSocket(socket, Port);
            else throw new SocketCreateListenException();
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
            throw new SocketGetClientException();
        }

        public bool AddClient(string ClientName, BaseModel t)
        {
            return PlayerThreads.Add(ClientName, t);
        }
        
        public bool RemoveClient(string ClientName)
        {
            return PlayerThreads.Remove(ClientName);
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            throw new ListeningSocketCanNotSendException();
        }

        public object[] Receive()
        {
            throw new ListeningSocketCanNotSendException();
        }

        public void Close()
        {
            Instance?.Close();
        }

        public void StartReceiving(Task t)
        {
            throw new ListeningSocketCanNotSendException();
        }

        public static string GetTypeString(SocketMessageType type)
        {
            return Socket.GetTypeString(type);
        }
    }
}
