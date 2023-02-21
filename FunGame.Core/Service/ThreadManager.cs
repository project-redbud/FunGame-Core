using System.Collections.Concurrent;

namespace Milimoe.FunGame.Core.Service
{
    internal class ThreadManager
    {
        /// <summary>
        /// 最大接受的线程数量
        /// </summary>
        private int MaxTask { get; }

        /// <summary>
        /// 可参与高并发的字典，但添加效率较低
        /// </summary>
        private ConcurrentDictionary<string, Task> Threads { get; } = new();

        /// <summary>
        /// Init ThreadManager
        /// </summary>
        /// <param name="MaxTask">MaxTask</param>
        internal ThreadManager(int MaxTask = 0)
        {
            if (MaxTask <= 0)
                this.MaxTask = Library.Constant.General.MaxTask_General;
            else
            {
                this.MaxTask = MaxTask;
            }
        }

        /// <summary>
        /// 获取Task对象
        /// </summary>
        /// <param name="name">Task的Key</param>
        /// <returns>Task对象</returns>
        internal Task this[string name]
        {
            get
            {
                return Threads[name];
            }
        }

        /// <summary>
        /// 向线程管理器中添加Task
        /// </summary>
        /// <param name="name">Task的Key</param>
        /// <param name="t">Task对象</param>
        /// <returns>True：操作成功</returns>
        internal bool Add(string name, Task t)
        {
            if (Threads.Count + 1 > MaxTask) return false;
            return Threads.TryAdd(name, t);
        }

        /// <summary>
        /// 从线程管理器中移除Task
        /// </summary>
        /// <param name="name">Task的Key</param>
        /// <returns>True：操作成功</returns>
        internal bool Remove(string name)
        {
            return Threads.TryRemove(name, out _);
        }

        /// <summary>
        /// 将Task移除，并取得这个Task
        /// </summary>
        /// <param name="name">Task的Key</param>
        /// <param name="t">Task对象</param>
        /// <returns>被移除的Task</returns>
        internal bool Remove(string name, ref Task? t)
        {
            return Threads.TryRemove(name, out t);
        }
        
        /// <summary>
        /// 将Task移除，并取得这个Task
        /// </summary>
        /// <param name="name">Task的Key</param>
        /// <returns>被移除的Task</returns>
        internal Task? RemoveAndGet(string name)
        {
            Threads.TryRemove(name, out Task? result);
            return result;
        }

        /// <summary>
        /// 清空线程管理器
        /// </summary>
        internal void Clear()
        {
            Threads.Clear();
        }
        
    }
}
