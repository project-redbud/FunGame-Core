using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Model
{
    public class RecurringTask(string name, TimeSpan interval, Action action, Action<Exception>? error = null) : IScheduledTask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// 循环执行间隔
        /// </summary>
        public TimeSpan Interval { get; set; } = interval;

        /// <summary>
        /// 任务执行逻辑
        /// </summary>
        public Action Action { get; set; } = action;

        /// <summary>
        /// 记录上一次执行时间
        /// </summary>
        public DateTime? LastRun { get; set; } = null;

        /// <summary>
        /// 记录下一次执行时间
        /// </summary>
        public DateTime NextRun { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// 任务执行时长
        /// </summary>
        public TimeSpan ExecutedTimeSpan { get; set; } = new();

        /// <summary>
        /// 最后一次执行时发生的错误
        /// </summary>
        public Exception? Error { get; set; }

        /// <summary>
        /// 捕获异常后，触发的回调函数
        /// </summary>
        public Action<Exception>? ErrorHandler { get; set; } = error;
    }
}
