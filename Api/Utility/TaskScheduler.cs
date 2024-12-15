using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class TaskScheduler
    {
        /// <summary>
        /// 任务计划管理器实例，可以直接使用
        /// </summary>
        public static TaskScheduler Shared { get; } = new();

        private readonly List<ScheduledTask> _tasks = [];
        private readonly List<RecurringTask> _recurringTasks = [];
        private readonly Timer _timer;
        private readonly Lock _lock = new();

        /// <summary>
        /// 创建一个轻量级的任务计划管理器
        /// </summary>
        public TaskScheduler()
        {
            _timer = new Timer(CheckAndRunTasks, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 添加一个任务计划
        /// </summary>
        /// <param name="name"></param>
        /// <param name="timeOfDay"></param>
        /// <param name="action"></param>
        public void AddTask(string name, TimeSpan timeOfDay, Action action)
        {
            lock (_lock)
            {
                _tasks.Add(new ScheduledTask(name, timeOfDay, action));
            }
        }

        /// <summary>
        /// 添加一个循环任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        /// <param name="startNow"></param>
        public void AddRecurringTask(string name, TimeSpan interval, Action action, bool startNow = false)
        {
            lock (_lock)
            {
                DateTime now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0);
                DateTime nextRun = startNow ? now : now.Add(interval);
                RecurringTask recurringTask = new(name, interval, action)
                {
                    NextRun = nextRun
                };
                _recurringTasks.Add(recurringTask);
            }
        }

        /// <summary>
        /// 移除任务计划
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTask(string name)
        {
            lock (_lock)
            {
                int removeTasks = _tasks.RemoveAll(t => t.Name == name);
                int removeRecurringTasks = _recurringTasks.RemoveAll(t => t.Name == name);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="state"></param>
        private void CheckAndRunTasks(object? state)
        {
            lock (_lock)
            {
                DateTime now = DateTime.Now;

                foreach (ScheduledTask task in _tasks)
                {
                    if (!task.IsTodayRun)
                    {
                        if (now.TimeOfDay >= task.TimeOfDay && now.TimeOfDay < task.TimeOfDay.Add(TimeSpan.FromSeconds(10)))
                        {
                            task.LastRun = now;
                            ThreadPool.QueueUserWorkItem(_ =>
                            {
                                try
                                {
                                    task.Action();
                                }
                                catch (Exception ex)
                                {
                                    task.Error = ex;
                                }
                            });
                        }
                    }
                }

                foreach (RecurringTask recurringTask in _recurringTasks)
                {
                    if (now >= recurringTask.NextRun)
                    {
                        recurringTask.LastRun = now;
                        recurringTask.NextRun = recurringTask.NextRun.Add(recurringTask.Interval);
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try
                            {
                                recurringTask.Action();
                            }
                            catch (Exception ex)
                            {
                                recurringTask.Error = ex;
                            }
                        });
                    }
                }
            }
        }
    }
}
