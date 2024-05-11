using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IBaseSocket
    {
        public SocketRuntimeType Runtime { get; }
        public Guid Token { get; }
        public string ServerAddress { get; }
        public int ServerPort { get; }
        public string ServerName { get; }
        public string ServerNotice { get; }
        public void Close();
    }
}
