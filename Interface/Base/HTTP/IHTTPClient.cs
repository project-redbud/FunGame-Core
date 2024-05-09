using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.HTTP
{
    public interface IHTTPClient : IBaseSocket
    {
        public Task<SocketResult> Send(SocketMessageType type, params object[] objs);
        public SocketObject SocketObject_Handler(SocketObject objs);
    }
}
