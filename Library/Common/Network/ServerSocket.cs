using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ServerSocket : ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public bool Connected => Instance != null && Instance.Connected;
        public List<IServerModel> ClientList => OnlineClients.GetList();
        public List<IServerModel> UserList => OnlineUsers.GetList();
        public List<string> BannedList { get; } = [];
        public int ClientCount => OnlineClients.Count;
        public int UserCount => OnlineUsers.Count;
        public int BannedCount => BannedList.Count;

        private readonly ModelManager<IServerModel> OnlineClients;
        private readonly ModelManager<IServerModel> OnlineUsers;

        private ServerSocket(System.Net.Sockets.Socket instance, int maxConnection = 0)
        {
            Token = Guid.NewGuid();
            Instance = instance;
            if (maxConnection <= 0)
            {
                OnlineClients = [];
                OnlineUsers = [];
            }
            else
            {
                OnlineClients = new(maxConnection);
                OnlineUsers = new(maxConnection);
            }
        }

        public static ServerSocket StartListening(int port = 22222, int maxConnection = 0)
        {
            if (maxConnection <= 0) maxConnection = SocketSet.MaxConnection_2C2G;
            System.Net.Sockets.Socket? socket = SocketManager.StartListening(port, maxConnection);
            if (socket != null) return new ServerSocket(socket, port);
            else throw new SocketCreateListenException();
        }

        public static ClientSocket Accept(Guid token)
        {
            object[] result = SocketManager.Accept();
            if (result != null && result.Length == 2)
            {
                string clientIP = (string)result[0];
                System.Net.Sockets.Socket client = (System.Net.Sockets.Socket)result[1];
                return new ClientSocket(client, clientIP, clientIP, token);
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
    }
}
