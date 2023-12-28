using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IClientSocket : ISocket
    {
        public bool Receiving { get; }
        public void StartReceiving(Task t);
        public SocketResult Send(SocketMessageType type, params object[] objs);
        public Library.Common.Network.SocketObject[] Receive();
        public void BindEvent(Delegate Method, bool Remove = false);
    }
}
