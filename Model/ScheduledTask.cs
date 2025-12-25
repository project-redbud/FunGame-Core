using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Model
{
    public class ScheduledTask(string name, TimeSpan timeSpan, Action action, Action<Exception>? error = null) : IScheduledTask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// 每天的目标时间
        /// </summary>
        public TimeSpan TimeOfDay { get; set; } = timeSpan;

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
        public DateTime NextRun => IsTodayRun ? DateTime.Today.AddDays(1).Add(TimeOfDay) : DateTime.Today.Add(TimeOfDay);

        /// <summary>
        /// 当天是否已经执行
        /// </summary>
        public bool IsTodayRun => LastRun.HasValue && LastRun.Value.Date == DateTime.Today;

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
