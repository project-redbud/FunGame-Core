using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IBaseSocket
    {
        public SocketRuntimeType Runtime { get; }
        public Guid Token { get; }
        public void Close();
    }
}
