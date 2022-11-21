using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    [Serializable]
    internal class JsonObject
    {
        internal SocketMessageType MessageType { get; } = SocketMessageType.Unknown;
        internal object[] Parameters { get; }
        internal string JsonString { get; }

        internal JsonObject(SocketMessageType MessageType, object[] Parameters)
        {
            this.MessageType = MessageType;
            this.Parameters = Parameters;
            this.JsonString = JsonSerializer.Serialize(this);
        }

        internal static string GetString(SocketMessageType MessageType, object[] Parameters)
        {
            return new JsonObject(MessageType, Parameters).JsonString;
        }

        internal static JsonObject? GetObject(string JsonString)
        {
            return JsonSerializer.Deserialize<JsonObject>(JsonString);
        }
    }
}
