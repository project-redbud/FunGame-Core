using Milimoe.FunGame.Core.Library.Constant;
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
        private readonly Lock _lock = new();

        /// <summary>
        /// 创建一个轻量级的任务计划管理器
        /// </summary>
        public TaskScheduler()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Run(CheckAndRunTasks);
                    await Task.Delay(1000);
                }
            });
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
                ScheduledTask task = new(name, timeOfDay, action);
                if (DateTime.Now > DateTime.Today.Add(timeOfDay))
                {
                    task.LastRun = DateTime.Today.Add(timeOfDay);
                }
                _tasks.Add(task);
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
        /// 获取任务计划上一次执行时间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="recurring"></param>
        /// <returns></returns>
        public DateTime GetLastTime(string name, bool recurring = false)
        {
            if (!recurring)
            {
                if (_tasks.FirstOrDefault(t => t.Name == name) is ScheduledTask task && task.LastRun.HasValue)
                {
                    return task.LastRun.Value;
                }
                else if (_recurringTasks.FirstOrDefault(t => t.Name == name) is RecurringTask recurringTask && recurringTask.LastRun.HasValue)
                {
                    return recurringTask.LastRun.Value;
                }
            }
            else if (_recurringTasks.FirstOrDefault(t => t.Name == name) is RecurringTask recurringTask && recurringTask.LastRun.HasValue)
            {
                return recurringTask.LastRun.Value;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// 获取任务计划下一次执行时间
        /// </summary>
        /// <param name="name"></param>
        /// <param name="recurring"></param>
        /// <returns></returns>
        public DateTime GetNextTime(string name, bool recurring = false)
        {
            if (!recurring)
            {
                if (_tasks.FirstOrDefault(t => t.Name == name) is ScheduledTask task)
                {
                    DateTime today = DateTime.Today.Add(task.TimeOfDay);
                    return task.IsTodayRun ? today.AddDays(1) : today;
                }
                else if (_recurringTasks.FirstOrDefault(t => t.Name == name) is RecurringTask recurringTask)
                {
                    return recurringTask.NextRun;
                }
            }
            else if (_recurringTasks.FirstOrDefault(t => t.Name == name) is RecurringTask recurringTask)
            {
                return recurringTask.NextRun;
            }
            return DateTime.MaxValue;
        }

        public string GetRunTimeInfo(string name)
        {
            DateTime last = GetLastTime(name);
            DateTime next = GetNextTime(name);
            string msg = "";
            if (last != DateTime.MinValue)
            {
                msg += $"上次运行时间：{last.ToString(General.GeneralDateTimeFormat)}\r\n";
            }
            if (next != DateTime.MaxValue)
            {
                msg += $"下次运行时间：{next.ToString(General.GeneralDateTimeFormat)}\r\n";
            }
            if (msg != "")
            {
                msg = $"任务计划：{name}\r\n{msg}";
            }
            else
            {
                msg = $"任务计划 {name} 不存在！";
            }
            return msg.Trim();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void CheckAndRunTasks()
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
                            Task.Run(() =>
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
                        Task.Run(() =>
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
