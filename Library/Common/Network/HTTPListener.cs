using System.Net;
using System.Net.WebSockets;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.HTTP;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class HTTPListener : IHTTPListener, ISocketListener<ServerWebSocket>
    {
        public HttpListener Instance { get; }
        public string Name => "HTTPListener";
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public ConcurrentModelList<IServerModel> ClientList { get; } = [];
        public ConcurrentModelList<IServerModel> UserList { get; } = [];
        public List<string> BannedList { get; } = [];

        private HTTPListener(HttpListener instance)
        {
            Instance = instance;
            Token = Guid.NewGuid();
        }

        public static HTTPListener StartListening(string address = "*", int port = 22223, string subUrl = "ws", bool ssl = false)
        {
            HttpListener? socket = HTTPManager.StartListening(address, port, subUrl, ssl);
            if (socket != null)
            {
                HTTPListener instance = new(socket);
                return instance;
            }
            else throw new SocketCreateListenException();
        }

        public async Task<ServerWebSocket> Accept(Guid token)
        {
            object[] result = await HTTPManager.Accept();
            if (result != null && result.Length == 2)
            {
                string clientIP = (string)result[0];
                WebSocket client = (WebSocket)result[1];
                return new ServerWebSocket(this, client, clientIP, clientIP, token);
            }
            throw new SocketGetClientException();
        }

        public virtual bool CheckClientConnection(SocketObject objs)
        {
            return true;
        }

        public virtual SocketObject SocketObject_Handler(SocketObject objs)
        {
            return new(SocketMessageType.Unknown, Guid.Empty);
        }

        public void Close()
        {
            Instance?.Close();
        }
    }
}
