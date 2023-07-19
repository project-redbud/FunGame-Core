using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Service
{
    internal class TaskManager
    {
        /// <summary>
        /// 开启一个任务：调用返回对象的OnCompleted()方法可以执行后续操作，支持异步
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal static ITaskAwaiter StartAndAwaitTask(Action action) => new TaskAwaiter(action);

        /// <summary>
        /// 开启一个任务：调用返回对象的OnCompleted()方法可以执行后续操作，支持异步
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        internal static ITaskAwaiter StartAndAwaitTask(Func<Task> function) => new TaskAwaiter(function);

        /// <summary>
        /// 内部实现类
        /// </summary>
        private class TaskAwaiter : ITaskAwaiter
        {
            public bool IsCompleted => _IsCompleted;

            private delegate void CompletedEvent();
            private event CompletedEvent? Completed;
            private bool _IsCompleted = false;

            internal TaskAwaiter(Action action) => _ = Worker(action);

            internal TaskAwaiter(Func<Task> function) => _ = Worker(function);

            /// <summary>
            /// <para>返回ITaskAwaiter可以进一步调用方法</para>
            /// <para>但是意义不大，前一个OnCompleted方法并不会等待下一个方法</para>
            /// <para>可以理解为并行广播</para>
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public ITaskAwaiter OnCompleted(Action action)
            {
                if (IsCompleted) action();
                else Completed += new CompletedEvent(action);
                return this;
            }

            private async Task Worker(Action action)
            {
                await Task.Run(action);
                _IsCompleted = true;
                Completed?.Invoke();
            }

            private async Task Worker(Func<Task> function)
            {
                await function();
                _IsCompleted = true;
                Completed?.Invoke();
            }
        }
    }
}
