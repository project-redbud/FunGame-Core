using System.Collections;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    [Serializable]
    public class SocketMessage
    {
        public SocketMessageType SocketType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; } = Guid.Empty;
        public Hashtable Data { get; } = new Hashtable();
        public int Count { get; } = 0;

        [JsonConstructor]
        public SocketMessage(SocketMessageType SocketType, Guid Token, Hashtable Data)
        {
            this.SocketType = SocketType;
            this.Token = Token;
            this.Data = Data;
            this.Count = Data.Count;
        }
    }
}
