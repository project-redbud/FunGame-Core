using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public int Runtime { get; }
        public Guid Token { get; }
        public string ServerIP { get; }
        public int ServerPort { get; }
        public string ServerName { get; }
        public string ServerNotice { get; }
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving { get; }
        public SocketResult Send(SocketMessageType type, params object[] objs);
        public Library.Common.Network.SocketObject Receive();
        public void Close();
        public void StartReceiving(Task t);
    }
}
