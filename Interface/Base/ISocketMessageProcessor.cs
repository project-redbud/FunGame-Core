using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocketMessageProcessor
    {
        public Type InstanceType { get; }
        public Guid Token { get; }
        public string ClientIP { get; }
        public string ClientName { get; }

        public SocketObject[] Receive();
        public Task<SocketObject[]> ReceiveAsync();
        public SocketResult Send(SocketMessageType type, params object[] objs);
        public Task<SocketResult> SendAsync(SocketMessageType type, params object[] objs);
        public void Close();
        public Task CloseAsync();
    }
}
