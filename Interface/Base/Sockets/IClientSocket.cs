using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Sockets
{
    public interface IClientSocket : ISocket
    {
        public bool Receiving { get; }
        public void StartReceiving(Task t);
        public void StopReceiving();
        public SocketResult Send(SocketMessageType type, params object[] objs);
        public SocketObject[] Receive();
        public void AddSocketObjectHandler(Action<SocketObject> method);
        public void RemoveSocketObjectHandler(Action<SocketObject> method);
    }
}
