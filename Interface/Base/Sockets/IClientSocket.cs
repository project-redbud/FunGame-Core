using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Sockets
{
    public interface IClientSocket : ISocket
    {
        public bool Receiving { get; }
        public void StartReceiving(Task t);
        public void StopReceiving();
        public SocketResult Send(SocketMessageType type, params object[] objs);
        public Library.Common.Network.SocketObject[] Receive();
        public void BindEvent(Delegate method, bool remove = false);
    }
}
