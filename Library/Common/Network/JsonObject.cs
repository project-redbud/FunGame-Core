using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    [Serializable]
    public struct JsonObject
    {
        public SocketMessageType MessageType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; }
        public object[] Parameters { get; }
        public string JsonString { get; }

        private JArray? JArray;

        public JsonObject(SocketMessageType MessageType, Guid Token, params object[] Parameters)
        {
            this.MessageType = MessageType;
            this.Token = Token;
            this.Parameters = Parameters;
            this.JsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public T? GetObject<T>(int i)
        {
            if (i >= Parameters.Length) throw new IndexOutOfArrayLengthException();
            JArray ??= JArray.FromObject(Parameters);
            return JArray[i].ToObject<T>();
        }

        public static string GetString(SocketMessageType MessageType, Guid Token, params object[] Parameters)
        {
            return new JsonObject(MessageType, Token, Parameters).JsonString;
        }

        public static JsonObject GetObject(string JsonString)
        {
            return JsonConvert.DeserializeObject<JsonObject>(JsonString);
        }
    }
    
}
