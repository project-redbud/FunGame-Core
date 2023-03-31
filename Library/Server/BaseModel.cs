using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Server
{
    public abstract class BaseModel
    {
        public abstract bool Running { get; }
        public abstract ClientSocket? Socket { get; }
        public abstract Task? Task { get; }
        public abstract string ClientName { get; }
        public abstract User? User { get; }

        public abstract bool Read(ClientSocket socket);
        public abstract bool Send(ClientSocket socket, SocketMessageType type, params object[] objs);
        public abstract void Start();
    }
}
