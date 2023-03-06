using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Library.Interface
{
    public interface ISocketCallBack
    {
        public void SocketHandler(SocketMessageType type, params object[]? objs);
    }
}
