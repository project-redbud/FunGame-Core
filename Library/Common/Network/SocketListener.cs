using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class SocketListener : ISocket, ISocketListener<ServerSocket>
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public bool Connected => Instance != null && Instance.Connected;
        public ConcurrentModelList<IServerModel> ClientList { get; } = [];
        public ConcurrentModelList<IServerModel> UserList { get; } = [];
        public List<string> BannedList { get; } = [];

        private SocketListener(System.Net.Sockets.Socket instance)
        {
            Token = Guid.NewGuid();
            Instance = instance;
        }

        public static SocketListener StartListening(int port = 22222, int maxConnection = 0)
        {
            if (maxConnection <= 0) maxConnection = SocketSet.MaxConnection_2C2G;
            System.Net.Sockets.Socket? socket = SocketManager.StartListening(port, maxConnection);
            if (socket != null) return new SocketListener(socket);
            else throw new SocketCreateListenException();
        }

        public ServerSocket Accept(Guid token)
        {
            object[] result = SocketManager.Accept();
            if (result != null && result.Length == 2)
            {
                string clientIP = (string)result[0];
                System.Net.Sockets.Socket client = (System.Net.Sockets.Socket)result[1];
                return new ServerSocket(this, client, clientIP, clientIP, token);
            }
            throw new SocketGetClientException();
        }

        public void Close()
        {
            Instance?.Close();
        }
    }
}
