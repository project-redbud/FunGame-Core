using System.Collections.Concurrent;
using Milimoe.FunGame.Core.Library.Server;

namespace Milimoe.FunGame.Core.Service
{
    internal class ThreadManager
    {
        /// <summary>
        /// 目前的线程数量
        /// </summary>
        internal int Count => Threads.Count;

        /// <summary>
        /// 最大接受的线程数量
        /// </summary>
        private int MaxThread { get; }

        /// <summary>
        /// 可参与高并发的字典，但添加效率较低
        /// </summary>
        private ConcurrentDictionary<string, BaseModel> Threads { get; } = new();

        /// <summary>
        /// Init ThreadManager
        /// </summary>
        /// <param name="MaxThread">MaxThread</param>
        internal ThreadManager(int MaxThread = 0)
        {
            if (MaxThread <= 0)
                this.MaxThread = Library.Constant.General.MaxThread_General;
            else
            {
                this.MaxThread = MaxThread;
            }
        }

        /// <summary>
        /// 获取Thread对象
        /// </summary>
        /// <param name="name">Thread的Key</param>
        /// <returns>Thread对象</returns>
        internal BaseModel this[string name] => Threads[name];

        /// <summary>
        /// 向线程管理器中添加Thread
        /// </summary>
        /// <param name="name">Thread的Key</param>
        /// <param name="t">Thread对象</param>
        /// <returns>True：操作成功</returns>
        internal bool Add(string name, BaseModel t)
        {
            if (Threads.Count + 1 > MaxThread) return false;
            return Threads.TryAdd(name, t);
        }

        /// <summary>
        /// 从线程管理器中移除Thread
        /// </summary>
        /// <param name="name">Thread的Key</param>
        /// <returns>True：操作成功</returns>
        internal bool Remove(string name)
        {
            return Threads.TryRemove(name, out _);
        }

        /// <summary>
        /// 将Thread移除，并取得这个Thread
        /// </summary>
        /// <param name="name">Thread的Key</param>
        /// <param name="t">Thread对象</param>
        /// <returns>被移除的Thread</returns>
        internal bool Remove(string name, ref BaseModel? t)
        {
            return Threads.TryRemove(name, out t);
        }
        
        /// <summary>
        /// 将Thread移除，并取得这个Thread
        /// </summary>
        /// <param name="name">Thread的Key</param>
        /// <returns>被移除的Thread</returns>
        internal BaseModel? RemoveAndGet(string name)
        {
            Threads.TryRemove(name, out BaseModel? result);
            return result;
        }

        internal bool ContainsKey(string name)
        {
            return Threads.ContainsKey(name);
        }

        /// <summary>
        /// 清空线程管理器
        /// </summary>
        internal void Clear()
        {
            Threads.Clear();
        }
        

        /// <summary>
        /// 获取线程对象的列表
        /// </summary>
        internal List<BaseModel> GetList()
        {
            return Threads.Values.ToList();
        }
    }
}
