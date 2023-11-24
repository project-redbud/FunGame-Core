using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    [Serializable]
    public readonly struct SocketObject
    {
        public SocketMessageType SocketType { get; } = SocketMessageType.Unknown;
        public Guid Token { get; } = Guid.Empty;
        public object[] Parameters { get; } = Array.Empty<object>();
        public int Length => Parameters.Length;

        // 从参数列表中获取指定索引的参数的Json字符串
        // -- 此索引器仅返回Json字符串，对象类型请使用反序列化方法GetParam<T>() --
        // -- 当然也可以自己反序列化 --
        // -- 基本类型可能有效，但仍建议使用反序列化方法 --
        public object? this[int index]
        {
            get
            {
                if (index >= Parameters.Length) throw new IndexOutOfArrayLengthException();
                object? obj = Parameters[index];
                return JsonManager.GetObject(obj.ToString() ?? "");
            }
        }

        [JsonConstructor]
        public SocketObject(SocketMessageType SocketType, Guid Token, params object[] Parameters)
        {
            this.SocketType = SocketType;
            this.Token = Token;
            if (Parameters != null && Parameters.Length > 0) this.Parameters = Parameters;
        }

        /// <summary>
        /// 从参数列表中获取指定类型和索引的参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">索引</param>
        /// <returns>类型的参数</returns>
        /// <exception cref="IndexOutOfArrayLengthException">索引超过数组上限</exception>
        public T? GetParam<T>(int index)
        {
            return JsonManager.GetObject<T>(this, index);
        }
    }
}
