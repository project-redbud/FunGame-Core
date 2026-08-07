using System.Text.Json.Serialization;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 即时外发数据包（RoundRecordPayload，POST 到外部专用服务器的线协议模型）<para/>
    /// 序列化字段为紧凑格式：g（GamingQueue 的 Guid）、t（当前时间戳）、e（事件 id）、d（数据包）、s（签名）
    /// </summary>
    public class RoundRecordPayload
    {
        /// <summary>
        /// GamingQueue 的 Guid
        /// </summary>
        [JsonPropertyName("g")]
        public Guid G { get; set; } = Guid.Empty;

        /// <summary>
        /// 当前时间戳（Unix 毫秒）
        /// </summary>
        [JsonPropertyName("t")]
        public long T { get; set; } = 0;

        /// <summary>
        /// POST 事件（事件 id，如 "0"、"1"、"13"，见 <see cref="Api.RoundRecordSinkEventIds"/>）
        /// </summary>
        [JsonPropertyName("e")]
        public string E { get; set; } = "";

        /// <summary>
        /// 数据包，不同事件可能具有不同的格式
        /// </summary>
        [JsonPropertyName("d")]
        public object? D { get; set; } = null;

        /// <summary>
        /// 签名（握手成功前为空字符串）
        /// </summary>
        [JsonPropertyName("s")]
        public string S { get; set; } = "";
    }
}
