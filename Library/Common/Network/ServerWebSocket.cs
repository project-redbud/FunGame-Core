using System.Net.WebSockets;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ServerWebSocket(ISocketListener<ServerWebSocket> listener, WebSocket instance, string clientIP, string clientName, Guid token) : IClientWebSocket, ISocketMessageProcessor
    {
        public ISocketListener<ServerWebSocket> Listener => listener;
        public WebSocket Instance => instance;
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token => token;
        public string ClientIP => clientIP;
        public string ClientName => clientName;
        public Type InstanceType => typeof(ServerWebSocket);
        public bool Receiving => _receiving;

        private Task? _receivingTask;
        private bool _receiving = false;

        public void Close()
        {
            throw new AsyncSendException();
        }

        public async Task CloseAsync()
        {
            if (Instance.State == WebSocketState.Open)
            {
                // 安全关闭 WebSocket 连接
                await Instance.CloseAsync(WebSocketCloseStatus.NormalClosure, "服务器正在关闭，断开连接！", CancellationToken.None);
            }
        }

        public SocketObject[] Receive()
        {
            throw new AsyncReadException();
        }

        public async Task<SocketObject[]> ReceiveAsync()
        {
            try
            {
                return await HTTPManager.Receive(Instance);
            }
            catch
            {
                throw new SocketWrongInfoException();
            }
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            throw new AsyncSendException();
        }

        public async Task<SocketResult> SendAsync(SocketMessageType type, params object[] objs)
        {
            return await HTTPManager.Send(Instance, new(type, token, objs));
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
