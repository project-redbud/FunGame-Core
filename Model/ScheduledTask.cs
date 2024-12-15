namespace Milimoe.FunGame.Core.Model
{
    public class ScheduledTask(string name, TimeSpan timeSpan, Action action)
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
        /// 记录上一次运行时间
        /// </summary>
        public DateTime? LastRun { get; set; } = null;

        /// <summary>
        /// 当天是否已经运行
        /// </summary>
        public bool IsTodayRun => LastRun.HasValue && LastRun.Value.Date == DateTime.Today;

        /// <summary>
        /// 最后一次运行时发生的错误
        /// </summary>
        public Exception? Error { get; set; }
    }
}
