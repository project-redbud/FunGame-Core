using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime { get; }
        public Guid Token { get; }
        public string ServerAddress { get; }
        public int ServerPort { get; }
        public string ServerName { get; }
        public string ServerNotice { get; }
        public bool Connected => Instance != null && Instance.Connected;
        public void Close();
    }
}
