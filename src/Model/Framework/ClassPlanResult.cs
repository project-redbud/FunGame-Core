namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业规划操作结果
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    public class ClassPlanResult(bool success, string message, Dictionary<string, object>? data = null)
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; } = success;

        /// <summary>
        /// 结果消息（失败原因 / 描述）
        /// </summary>
        public string Message { get; } = message;

        /// <summary>
        /// 附加数据（供上层 / 事件消费）
        /// </summary>
        public Dictionary<string, object>? Data { get; } = data;

        public static ClassPlanResult Ok(string message = "") => new(true, message);

        public static ClassPlanResult Fail(string message) => new(false, message);

        public static implicit operator bool(ClassPlanResult result) => result.Success;
    }
}
