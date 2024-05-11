using System.Net;
using System.Net.WebSockets;
using Milimoe.FunGame.Core.Interface.HTTP;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class HTTPListener : IHTTPListener
    {
        public HttpListener Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token { get; } = Guid.Empty;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public Dictionary<Guid, WebSocket> ClientSockets { get; } = [];

        private HTTPListener(HttpListener Instance, int ServerPort)
        {
            this.Instance = Instance;
            this.ServerPort = ServerPort;
        }

        public static HTTPListener StartListening(int Port, bool SSL = false)
        {
            HttpListener? socket = HTTPManager.StartListening(Port, SSL);
            if (socket != null)
            {
                HTTPListener instance = new(socket, Port);
                Task t = Task.Run(async () => await HTTPManager.Receiving(instance));
                return instance;
            }
            else throw new SocketCreateListenException();
        }

        public async Task SendMessage(Guid token, SocketObject obj)
        {
            if (ClientSockets.TryGetValue(token, out WebSocket? socket) && socket != null)
            {
                await HTTPManager.Send(socket, obj);
            }
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
