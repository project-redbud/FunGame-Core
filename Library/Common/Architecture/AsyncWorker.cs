using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    /// <summary>
    /// 继承这个类可以获得后台等待的功能
    /// <para>参考实现 <see cref="BaseModel"/></para>
    /// </summary>
    public abstract class AsyncWorker<T>
    {
        /// <summary>
        /// 接收到的实例
        /// </summary>
        protected abstract T? Work { get; set; }

        /// <summary>
        /// 是否处于等待服务器响应的状态
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
        /// 请等待任务完成
        /// </summary>
        protected virtual void WaitForWorkDone()
        {
            while (true)
            {
                if (!Working) break;
                Thread.Sleep(100);
            }
        }
    }
}
