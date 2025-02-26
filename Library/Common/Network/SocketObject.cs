using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    /// <summary>
    /// 唯一指定的通信数据结构
    /// </summary>
    public readonly struct SocketObject
    {
        /// <summary>
        /// 通信类型
        /// </summary>
        public SocketMessageType SocketType { get; } = SocketMessageType.Unknown;

        /// <summary>
        /// 通信令牌
        /// </summary>
        public Guid Token { get; } = Guid.Empty;

        /// <summary>
        /// 参数列表
        /// </summary>
        public object[] Parameters { get; } = [];

        /// <summary>
        /// 参数数量
        /// </summary>
        [JsonIgnore]
        public int Length => Parameters.Length;

        /// <summary>
        /// 从参数列表中获取指定索引的参数的Json对象<para/>
        /// -- 此索引器仅返回Json对象，获取实例请使用反序列化方法GetParam[T]() --<para/>
        /// -- 当然也可以自己反序列化 --<para/>
        /// -- 基本类型可能有效，但仍建议使用反序列化方法 --
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfArrayLengthException">索引超过数组上限</exception>
        public object? this[int index]
        {
            get
            {
                if (index >= Parameters.Length) throw new IndexOutOfArrayLengthException();
                object? obj = Parameters[index];
                return JsonManager.GetObject(obj.ToString() ?? "");
            }
        }

        /// <summary>
        /// 构建通信数据对象
        /// </summary>
        /// <param name="socketType"></param>
        /// <param name="token"></param>
        /// <param name="parameters"></param>
        [JsonConstructor]
        public SocketObject(SocketMessageType socketType, Guid token, params object[] parameters)
        {
            SocketType = socketType;
            Token = token;
            if (parameters != null && parameters.Length > 0) Parameters = parameters;
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
