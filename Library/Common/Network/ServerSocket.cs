using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ServerSocket : ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public string ServerIP { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public bool Connected => Instance != null && Instance.Connected;
        public List<IServerModel> ClientList => OnlineClients.GetList();
        public List<IServerModel> UserList => OnlineUsers.GetList();
        public List<string> BannedList { get; } = new();
        public int ClientCount => OnlineClients.Count;
        public int UserCount => OnlineUsers.Count;
        public int BannedCount => BannedList.Count;

        private readonly ModelManager<IServerModel> OnlineClients;
        private readonly ModelManager<IServerModel> OnlineUsers;

        private ServerSocket(System.Net.Sockets.Socket Instance, int ServerPort, int MaxConnection = 0)
        {
            this.Instance = Instance;
            this.ServerPort = ServerPort;
            if (MaxConnection <= 0)
            {
                OnlineClients = new();
                OnlineUsers = new();
            }
            else
            {
                OnlineClients = new(MaxConnection);
                OnlineUsers = new(MaxConnection);
            }
        }

        public static ServerSocket StartListening(int Port = 22222, int MaxConnection = 0)
        {
            if (MaxConnection <= 0) MaxConnection = SocketSet.MaxConnection_2C2G;
            System.Net.Sockets.Socket? socket = SocketManager.StartListening(Port, MaxConnection);
            if (socket != null) return new ServerSocket(socket, Port);
            else throw new SocketCreateListenException();
        }

        public ClientSocket Accept(Guid Token)
        {
            object[] result = SocketManager.Accept();
            if (result != null && result.Length == 2)
            {
                string ClientIP = (string)result[0];
                System.Net.Sockets.Socket Client = (System.Net.Sockets.Socket)result[1];
                return new ClientSocket(Client, ServerPort, ClientIP, ClientIP, Token);
            }
            throw new SocketGetClientException();
        }

        public bool AddClient(string name, IServerModel t)
        {
            name = name.ToLower();
            return OnlineClients.Add(name, t);
        }

        public bool RemoveClient(string name)
        {
            name = name.ToLower();
            return OnlineClients.Remove(name);
        }

        public bool ContainsClient(string name)
        {
            name = name.ToLower();
            return OnlineClients.ContainsKey(name);
        }

        public IServerModel GetClient(string name)
        {
            name = name.ToLower();
            return OnlineClients[name];
        }

        public bool AddUser(string name, IServerModel t)
        {
            name = name.ToLower();
            return OnlineUsers.Add(name, t);
        }

        public bool RemoveUser(string name)
        {
            name = name.ToLower();
            return OnlineUsers.Remove(name);
        }

        public bool ContainsUser(string name)
        {
            name = name.ToLower();
            return OnlineUsers.ContainsKey(name);
        }

        public IServerModel GetUser(string name)
        {
            name = name.ToLower();
            return OnlineUsers[name];
        }

        public void Close()
        {
            Instance?.Close();
        }

        public static string GetTypeString(SocketMessageType type) => SocketSet.GetTypeString(type);
    }
}
