using System.Net.WebSockets;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class ClientWebSocket(HTTPListener listener, WebSocket instance, string clientIP, string clientName, Guid token) : IWebSocket, ISocketMessageProcessor
    {
        public HTTPListener Listener => listener;
        public WebSocket Instance => instance;
        public SocketRuntimeType Runtime => SocketRuntimeType.Server;
        public Guid Token => token;
        public string ClientIP => clientIP;
        public string ClientName => clientName;
        public Type InstanceType => typeof(ClientWebSocket);

        public void Close()
        {
            TaskUtility.NewTask(async () =>
            {
                if (Instance.State == WebSocketState.Open)
                {
                    // 安全关闭 WebSocket 连接
                    await Instance.CloseAsync(WebSocketCloseStatus.NormalClosure, "服务器正在关闭，断开连接！", CancellationToken.None);
                    Listener.ClientSockets.Remove(Token);
                }
            });
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            throw new AsyncSendException();
        }

        public async Task<SocketResult> SendAsync(SocketMessageType type, params object[] objs)
        {
            return await HTTPManager.Send(Instance, new(type, token, objs));
        }
    }
}
