using Milimoe.FunGame.Core.Library.Constant;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    [Serializable]
    public class JsonObject
    {
        public SocketMessageType MessageType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; }
        public object[] Parameters { get; }
        public string JsonString { get; }

        [JsonConstructor]
        public JsonObject(SocketMessageType MessageType, Guid Token, params object[] Parameters)
        {
            this.MessageType = MessageType;
            this.Token = Token;
            this.Parameters = Parameters;
            this.JsonString = JsonSerializer.Serialize(this);
        }

        public static string GetString(SocketMessageType MessageType, Guid Token, params object[] Parameters)
        {
            return new JsonObject(MessageType, Token, Parameters).JsonString;
        }

        public static JsonObject? GetObject(string JsonString)
        {
            return JsonSerializer.Deserialize<JsonObject>(JsonString);
        }
    }
}
