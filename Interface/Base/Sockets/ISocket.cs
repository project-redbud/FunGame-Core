using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Interface.Sockets
{
    public interface ISocket : IBaseSocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public bool Connected => Instance != null && Instance.Connected;
    }
}
