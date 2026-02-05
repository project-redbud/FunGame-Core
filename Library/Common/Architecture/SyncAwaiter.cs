namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    /// <summary>
    /// 该类的工具方法允许在同步方法中安全等待异步任务完成
    /// </summary>
    public class SyncAwaiter
    {
        /// <summary>
        /// 在同步方法中安全等待一个 Task 完成并获取结果
        /// 内部使用 ManualResetEventSlim，避免死锁
        /// </summary>
        public static T WaitResult<T>(Task<T> task)
        {
            if (task.IsCompleted)
                return task.Result;

            ManualResetEventSlim mres = new(false);

            // 当 task 完成时，设置事件信号
            task.ContinueWith(_ =>
            {
                mres.Set();
            }, TaskScheduler.Default);

            // 阻塞当前线程直到 task 完成
            // 注意：这会阻塞调用线程！
            mres.Wait();

            // 现在可以安全取 Result（不会抛死锁）
            return task.Result;
        }

        /// <summary>
        /// 无返回值版本
        /// </summary>
        public static void Wait(Task task)
        {
            if (task.IsCompleted) return;

            ManualResetEventSlim mres = new(false);
            task.ContinueWith(_ => mres.Set(), TaskScheduler.Default);
            mres.Wait();
        }
    }
}
