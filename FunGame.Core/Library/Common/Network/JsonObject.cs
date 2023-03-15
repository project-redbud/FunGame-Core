using Newtonsoft.Json;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    [Serializable]
    public class JsonObject
    {
        public SocketMessageType MessageType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; }
        public object[] Parameters { get; }
        public string JsonString { get; }

        public JsonObject(SocketMessageType MessageType, Guid Token, params object[] Parameters)
        {
            this.MessageType = MessageType;
            this.Token = Token;
            this.Parameters = Parameters;
            this.JsonString = JsonConvert.SerializeObject(this);
        }

        public static string GetString(SocketMessageType MessageType, Guid Token, params object[] Parameters)
        {
            return new JsonObject(MessageType, Token, Parameters).JsonString;
        }

        public static JsonObject? GetObject(string JsonString)
        {
            return JsonConvert.DeserializeObject<JsonObject>(JsonString);
        }
    }
}
