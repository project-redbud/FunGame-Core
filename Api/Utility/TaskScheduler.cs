using Milimoe.FunGame.Core.Interface.Base;
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
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    CheckAndRunTasks();
                    await Task.Delay(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 添加一个任务计划
        /// </summary>
        /// <param name="name"></param>
        /// <param name="timeOfDay"></param>
        /// <param name="action"></param>
        /// <param name="error"></param>
        public void AddTask(string name, TimeSpan timeOfDay, Action action, Action<Exception>? error = null)
        {
            using Lock.Scope scope = _lock.EnterScope();
            ScheduledTask task = new(name, timeOfDay, action, error);
            if (DateTime.Now > DateTime.Today.Add(timeOfDay))
            {
                task.LastRun = DateTime.Today.Add(timeOfDay);
            }
            _tasks.Add(task);
        }

        /// <summary>
        /// 添加一个循环任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        /// <param name="startNow"></param>
        /// <param name="error"></param>
        public void AddRecurringTask(string name, TimeSpan interval, Action action, bool startNow = false, Action<Exception>? error = null)
        {
            using Lock.Scope scope = _lock.EnterScope();
            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0);
            DateTime nextRun = startNow ? now : now.Add(interval);
            RecurringTask recurringTask = new(name, interval, action, error)
            {
                NextRun = nextRun
            };
            _recurringTasks.Add(recurringTask);
        }

        /// <summary>
        /// 移除任务计划
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTask(string name)
        {
            using Lock.Scope scope = _lock.EnterScope();
            int removeTasks = _tasks.RemoveAll(t => t.Name == name);
            int removeRecurringTasks = _recurringTasks.RemoveAll(t => t.Name == name);
        }

        /// <summary>
        /// 获取任务计划
        /// </summary>
        /// <param name="name"></param>
        /// <param name="recurring"></param>
        /// <returns></returns>
        public IScheduledTask? GetTask(string name, bool recurring = false)
        {
            if (!recurring)
            {
                if (_tasks.FirstOrDefault(t => t.Name == name) is ScheduledTask task)
                {
                    return task;
                }
                else if (_recurringTasks.FirstOrDefault(t => t.Name == name) is RecurringTask recurringTask)
                {
                    return recurringTask;
                }
            }
            else if (_recurringTasks.FirstOrDefault(t => t.Name == name) is RecurringTask recurringTask)
            {
                return recurringTask;
            }
            return null;
        }

        /// <summary>
        /// 获取任务计划上一次执行时间
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static DateTime GetLastTime(IScheduledTask task) => task.LastRun.HasValue ? task.LastRun.Value : DateTime.MinValue;

        /// <summary>
        /// 获取任务计划下一次执行时间
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static DateTime GetNextTime(IScheduledTask task) => task.NextRun;

        /// <summary>
        /// 格式化任务计划执行信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetRunTimeInfo(string name)
        {
            string msg;
            IScheduledTask? task = GetTask(name);
            if (task is null)
            {
                msg = $"任务计划 {name} 不存在！";
            }
            else
            {
                msg = $"任务计划：{name}\r\n";
                DateTime last = GetLastTime(task);
                DateTime next = GetNextTime(task);
                if (last != DateTime.MinValue)
                {
                    msg += $"上次执行时间：{last.ToString(General.GeneralDateTimeFormat)}\r\n";
                }
                if (next != DateTime.MaxValue)
                {
                    msg += $"下次执行时间：{next.ToString(General.GeneralDateTimeFormat)}\r\n";
                }
                if (task.ExecutedTimeSpan.TotalSeconds > 0)
                {
                    msg += $"任务执行时长：{task.ExecutedTimeSpan.TotalSeconds:0.###} 秒\r\n";
                }
            }
            return msg.Trim();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void CheckAndRunTasks()
        {
            using Lock.Scope scope = _lock.EnterScope();
            DateTime now = DateTime.Now;

            foreach (ScheduledTask task in _tasks)
            {
                if (!task.IsTodayRun)
                {
                    now = DateTime.Now;
                    if (now.TimeOfDay >= task.TimeOfDay && now.TimeOfDay < task.TimeOfDay.Add(TimeSpan.FromSeconds(10)))
                    {
                        task.LastRun = now;
                        Task.Run(() =>
                        {
                            try
                            {
                                task.Action();
                                task.ExecutedTimeSpan = DateTime.Now - now;
                            }
                            catch (Exception ex)
                            {
                                task.Error = ex;
                                TXTHelper.AppendErrorLog(ex.ToString());
                                task.ErrorHandler?.Invoke(ex);
                            }
                        });
                    }
                }
            }

            foreach (RecurringTask recurringTask in _recurringTasks)
            {
                now = DateTime.Now;
                if (now >= recurringTask.NextRun)
                {
                    recurringTask.LastRun = now;
                    recurringTask.NextRun = recurringTask.NextRun.Add(recurringTask.Interval);
                    Task.Run(() =>
                    {
                        try
                        {
                            recurringTask.Action();
                            recurringTask.ExecutedTimeSpan = DateTime.Now - now;
                        }
                        catch (Exception ex)
                        {
                            recurringTask.Error = ex;
                            TXTHelper.AppendErrorLog(ex.ToString());
                            recurringTask.ErrorHandler?.Invoke(ex);
                        }
                    });
                }
            }
        }
    }
}
