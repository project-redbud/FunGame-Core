using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    /// <summary>
    /// 任务的等待器，可以设置在任务完成后需要的操作
    /// </summary>
    public struct TaskAwaiter
    {
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public readonly bool IsCompleted => awaiter.IsCompleted;

        /// <summary>
        /// 捕获到的异常
        /// </summary>
        public readonly System.Exception Exception => awaiter.Exception;

        /// <summary>
        /// <para>内部实现类</para>
        /// 实现参见 <see cref="Service.TaskManager.TaskAwaiter"/>
        /// </summary>
        private ITaskAwaiter awaiter;

        /// <summary>
        /// 构造一个等待器
        /// </summary>
        /// <param name="awaiter"></param>
        internal TaskAwaiter(ITaskAwaiter awaiter)
        {
            this.awaiter = awaiter;
        }

        /// <summary>
        /// 返回 TaskAwaiter 可以进一步等待并执行方法<para/>
        /// 注意事项：async () 委托的后续 OnCompleted 方法将不会进一步等待，而是直接执行，因为它是异步的<para/>
        /// <example>
        /// 这意味着你不能这样操作：
        /// <code>
        /// TaskUtility.NewTask(() =>
        /// {
        ///     Console.WriteLine(0);
        /// }).OnCompleted(async () =>
        /// {
        ///     await Task.Delay(3000);
        ///     Console.WriteLine(1);
        /// }).OnCompleted(() =>
        /// {
        ///     Console.WriteLine(2);
        /// });
        /// </code>
        /// 上述代码将导致：任务输出 0 之后，2 不会等待 1 的完成，因此会直接输出 2，再输出 1.
        /// </example>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TaskAwaiter OnCompleted(Action action)
        {
            awaiter = awaiter.OnCompleted(action);
            return this;
        }

        /// <summary>
        /// 在捕获到异常时，将触发Error事件
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TaskAwaiter OnError(Action<System.Exception> action)
        {
            awaiter = awaiter.OnError(action);
            return this;
        }
    }
}
