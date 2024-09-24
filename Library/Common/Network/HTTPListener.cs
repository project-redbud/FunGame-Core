using System.Net;
using System.Net.WebSockets;
using Milimoe.FunGame.Core.Api.Utility;
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
        public Dictionary<Guid, WebSocket> ClientSockets { get; } = [];

        private HTTPListener(HttpListener instance)
        {
            Instance = instance;
            Token = Guid.NewGuid();
        }

        public static HTTPListener StartListening(int port, bool ssl = false)
        {
            HttpListener? socket = HTTPManager.StartListening(port, ssl);
            if (socket != null)
            {
                HTTPListener instance = new(socket);
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
            bool closing = true;
            TaskUtility.NewTask(async () =>
            {
                // 关闭所有 WebSocket 连接
                foreach (WebSocket socket in ClientSockets.Values)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        // 安全关闭 WebSocket 连接
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "服务器正在关闭，断开连接！", CancellationToken.None);
                    }
                }
                ClientSockets.Clear();
                Instance?.Close();
                closing = true;
            });
            while (closing)
            {
                if (!closing) break;
                Thread.Sleep(100);
            }
        }
    }
}
