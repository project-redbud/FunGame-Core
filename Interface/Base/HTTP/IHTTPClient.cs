using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.HTTP
{
    public interface IHTTPClient : IBaseSocket
    {
        public bool Receiving { get; }
        public Task Receive();
        public Task<SocketResult> Send(SocketMessageType type, params object[] objs);
        public void AddSocketObjectHandler(Action<SocketObject> method);
        public void RemoveSocketObjectHandler(Action<SocketObject> method);
    }
}
