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
        internal static ITaskAwaiter NewTask(Action action) => new TaskAwaiter(action);

        /// <summary>
        /// 开启一个任务：调用返回对象的OnCompleted()方法可以执行后续操作，支持异步
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
            /// 返回ITaskAwaiter可以进一步调用方法<para/>
            /// 但是意义不大，前一个OnCompleted方法并不会等待下一个方法<para/>
            /// 可以理解为并行广播<para/>
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public ITaskAwaiter OnCompleted(Action action)
            {
                if (IsCompleted) action();
                else Completed += new CompletedEvent(action);
                return this;
            }

            /// <summary>
            /// 在捕获到异常时，将触发Error事件
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
