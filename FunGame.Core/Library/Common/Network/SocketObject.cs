using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public readonly struct SocketObject
    {
        public SocketMessageType SocketType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; } = Guid.Empty;
        public object[] Parameters { get; } = Array.Empty<object>();
        public int Length { get; } = 0;
        private JsonObject Json { get; }

        public SocketObject(JsonObject json)
        {
            Json = json;
            SocketType = Json.MessageType;
            Token = Json.Token;
            Parameters = Json.Parameters;
            Length = Parameters.Length;
        }

        public SocketObject()
        {
            Json = new JsonObject(SocketMessageType.Unknown, Guid.Empty, Array.Empty<object>());
            SocketType = Json.MessageType;
            Token = Json.Token;
            Parameters = Json.Parameters;
            Length = Parameters.Length;
        }

        /// <summary>
        /// 从参数列表中获取指定类型的参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">索引</param>
        /// <returns>类型的参数</returns>
        /// <exception cref="IndexOutOfArrayLengthException">索引超过数组上限</exception>
        public T? GetParam<T>(int index) => Json.GetObject<T>(index);
    }
}
