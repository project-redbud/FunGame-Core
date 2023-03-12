using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Server;
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
        public List<BaseModel> GetUsersList => OnlineUsers.GetList();
        public List<string> BannedList { get; } = new();
        public int UsersCount => OnlineUsers.Count;
        public int BannedCount => BannedList.Count;

        private readonly ThreadManager OnlineUsers;

        private ServerSocket(System.Net.Sockets.Socket Instance, int ServerPort, int MaxConnection = 0)
        {
            this.Instance = Instance;
            this.ServerPort = ServerPort;
            if (MaxConnection <= 0)
                OnlineUsers = new ThreadManager();
            else
                OnlineUsers = new ThreadManager(MaxConnection);
        }

        public static ServerSocket StartListening(int Port = 22222, int MaxConnection = 0)
        {
            if (MaxConnection <= 0) MaxConnection = SocketSet.MaxConnection_General;
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

        public bool AddUser(string UserName, BaseModel t)
        {
            return OnlineUsers.Add(UserName, t);
        }
        
        public bool RemoveUser(string UserName)
        {
            return OnlineUsers.Remove(UserName);
        }
        
        public bool ContainsUser(string UserName)
        {
            return OnlineUsers.ContainsKey(UserName);
        }
        
        public BaseModel GetUser(string UserName)
        {
            return OnlineUsers[UserName];
        }

        public void Close()
        {
            Instance?.Close();
        }

        public static string GetTypeString(SocketMessageType type)
        {
            return Socket.GetTypeString(type);
        }
    }
}
