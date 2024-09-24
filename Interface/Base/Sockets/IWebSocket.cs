using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Interface.Sockets
{
    public interface IWebSocket : IBaseSocket
    {
        public System.Net.WebSockets.WebSocket Instance { get; }
        public bool Connected => Instance != null && Instance.State == System.Net.WebSockets.WebSocketState.Open;
    }
}
