namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IScheduledTask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 任务执行逻辑
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// 记录上一次执行时间
        /// </summary>
        public DateTime? LastRun { get; }

        /// <summary>
        /// 记录下一次执行时间
        /// </summary>
        public DateTime NextRun { get; }

        /// <summary>
        /// 任务执行时长
        /// </summary>
        public TimeSpan ExecutedTimeSpan { get; }

        /// <summary>
        /// 最后一次执行时发生的错误
        /// </summary>
        public Exception? Error { get; }

        /// <summary>
        /// 捕获异常后，触发的回调函数
        /// </summary>
        public Action<Exception>? ErrorHandler { get; }
    }
}
