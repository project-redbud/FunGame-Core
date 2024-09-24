using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocketMessageProcessor
    {
        public Type InstanceType { get; }
        public string ClientIP { get; }
        public string ClientName { get; }

        public SocketResult Send(SocketMessageType type, params object[] objs);
        public Task<SocketResult> SendAsync(SocketMessageType type, params object[] objs);
        public void Close();
    }
}
