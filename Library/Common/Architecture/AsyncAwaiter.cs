namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    /// <summary>
    /// 继承这个类可以获得异步等待的功能
    /// <para>参考实现 <see cref="SocketHandlerModel"/></para>
    /// </summary>
    public abstract class AsyncAwaiter<T>
    {
        /// <summary>
        /// 接收到的实例
        /// </summary>
        protected abstract T? Work { get; set; }

        /// <summary>
        /// 是否处于等待的状态
        /// </summary>
        protected bool Working { get; set; } = false;

        /// <summary>
        /// 异步操作前，请设置为等待状态
        /// </summary>
        protected virtual void SetWorking()
        {
            Working = true;
            Work = default;
        }

        /// <summary>
        /// 等待任务完成（需要自己异步）
        /// </summary>
        protected virtual void WaitForWorkDone()
        {
            while (true)
            {
                if (!Working) break;
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 异步等待任务完成
        /// </summary>
        protected async virtual Task WaitForWorkDoneAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!Working) break;
                    Thread.Sleep(100);
                }
            });
        }
    }
}
