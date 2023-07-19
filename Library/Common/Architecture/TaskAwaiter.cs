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
        /// <para>内部实现类</para>
        /// <see cref="Service.TaskManager.TaskAwaiter"/>
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
        /// <para>返回TaskAwaiter可以连续的调用方法</para>
        /// <para>但是意义不大，前一个OnCompleted方法并不会等待下一个方法</para>
        /// <para>可以理解为并行广播</para>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TaskAwaiter OnCompleted(Action action)
        {
            awaiter = awaiter.OnCompleted(action);
            return this;
        }
    }
}
