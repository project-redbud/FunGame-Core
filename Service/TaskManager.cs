using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Service
{
    internal class TaskManager
    {
        /// <summary>
        /// 开启一个任务：调用返回对象的 OnCompleted() 方法可以执行后续操作，支持异步
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal static ITaskAwaiter NewTask(Action action) => new TaskAwaiter(action);

        /// <summary>
        /// 开启一个任务：调用返回对象的 OnCompleted() 方法可以执行后续操作，支持异步
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        internal static ITaskAwaiter NewTask(Func<Task> function) => new TaskAwaiter(function);

        /// <summary>
        /// 内部实现类
        /// </summary>
        private class TaskAwaiter : ITaskAwaiter
        {
            public bool IsCompleted => _IsCompleted;
            public Exception Exception => _Exception;

            private delegate void CompletedEvent();
            private delegate void ErrorEvent(Exception e);
            private event CompletedEvent? Completed;
            private event ErrorEvent? Error;

            private bool _IsCompleted = false;
            private Exception _Exception = new();

            internal TaskAwaiter(Action action) => Worker(action);

            internal TaskAwaiter(Func<Task> function) => Worker(function);

            /// <summary>
            /// 返回 ITaskAwaiter 可以进一步等待并执行方法<para/>
            /// 注意事项：async () 委托的后续 OnCompleted 方法将不会进一步等待，而是直接执行，因为它是异步的
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public ITaskAwaiter OnCompleted(Action action)
            {
                Completed += () =>
                {
                    action();
                };
                return this;
            }

            /// <summary>
            /// 在捕获到异常时，将触发 Error 事件
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public ITaskAwaiter OnError(Action<Exception> action)
            {
                Error += new ErrorEvent(action);
                return this;
            }

            private void Worker(Action action)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Run(action);
                        _IsCompleted = true;
                        Completed?.Invoke();
                    }
                    catch (Exception e)
                    {
                        _Exception = e;
                        Error?.Invoke(e);
                    }
                });
            }

            private void Worker(Func<Task> function)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await function();
                        _IsCompleted = true;
                        Completed?.Invoke();
                    }
                    catch (Exception e)
                    {
                        _Exception = e;
                        Error?.Invoke(e);
                    }
                });
            }
        }
    }
}
