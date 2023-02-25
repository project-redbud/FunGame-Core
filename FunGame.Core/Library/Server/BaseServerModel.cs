using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Server
{
    public abstract class BaseModel
    {
        public bool Running = false;
        public ClientSocket? Socket = null;
        public Task? Task = null;
        public string ClientName = "";

        public abstract bool Read(ClientSocket socket);

        public abstract bool Send(ClientSocket socket, SocketMessageType type, params object[] objs);

        public abstract void Start();
    }
}
