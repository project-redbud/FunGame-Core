using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Sockets
{
    public interface IClientWebSocket : IBaseSocket
    {
        public System.Net.WebSockets.WebSocket Instance { get; }
        public bool Connected => Instance != null && Instance.State == System.Net.WebSockets.WebSocketState.Open;
        public bool Receiving { get; }
        public void StartReceiving(Task t);
        public void StopReceiving();
        public Task<Library.Common.Network.SocketObject[]> ReceiveAsync();
        public Task<SocketResult> SendAsync(SocketMessageType type, params object[] objs);
    }
}
