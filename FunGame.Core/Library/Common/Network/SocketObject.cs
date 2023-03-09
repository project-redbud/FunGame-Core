using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public readonly struct SocketObject
    {
        public SocketMessageType SocketType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; } = Guid.Empty;
        public object[] Parameters { get; } = Array.Empty<object>();

        public SocketObject(SocketMessageType type, Guid token, params object[] parameters)
        {
            SocketType = type;
            Token = token;
            Parameters = parameters;
        }
    }
}
