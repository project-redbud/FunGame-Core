using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// <para>继承 AsyncAwaiter 用法：</para>
    /// <para>1、调用Socket.Send()前，请设置为等待状态：SetWorking();</para>
    /// <para>2、调用Socket.Send() == Success后，请等待任务完成：WaitForWorkDone();</para>
    /// <para>3、在其他任何地方修改Working状态，均会使任务终止。</para>
    /// </summary>
    public class SocketHandlerController : AsyncAwaiter<SocketObject>, ISocketHandler, IDisposable
    {
        /// <summary>
        /// 接收到的SocketObject实例
        /// </summary>
        protected override SocketObject Work { get; set; }

        /// <summary>
        /// Socket
        /// </summary>
        private readonly Socket _Socket;

        /// <summary>
        /// 继承请调用base构造
        /// </summary>
        /// <param name="socket">Socket</param>
        public SocketHandlerController(Socket? socket)
        {
            if (socket != null)
            {
                _Socket = socket;
                socket.BindEvent(new SocketManager.SocketReceiveHandler(SocketHandler));
            }
            else throw new SocketCreateReceivingException();
        }

        /// <summary>
        /// 继承请重写此方法
        /// </summary>
        /// <param name="SocketObject">SocketObject</param>
        public virtual void SocketHandler(SocketObject SocketObject)
        {

        }

        /// <summary>
        /// 判断是否已经Disposed
        /// </summary>
        private bool IsDisposed = false;

        /// <summary>
        /// 公开的Dispose方法
        /// </summary>
        public void Dispose()
        {
            OnDisposed();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 关闭时
        /// </summary>
        /// <param name="Disposing"></param>
        protected void Dispose(bool Disposing)
        {
            if (!IsDisposed)
            {
                if (Disposing)
                {
                    _Socket.BindEvent(new SocketManager.SocketReceiveHandler(SocketHandler), true);
                }
            }
            IsDisposed = true;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="SocketObject">SocketObject</param>
        protected delegate void DisposedEvent();

        /// <summary>
        /// <para>Controller关闭时事件</para>
        /// <para>不建议new Dispose()方法，建议使用事件</para>
        /// <para>事件会在base.Dispose()执行前触发</para>
        /// </summary>
        protected event DisposedEvent? Disposed;

        /// <summary>
        /// 触发关闭事件
        /// </summary>
        /// <param name="SocketObject">SocketObject</param>
        protected void OnDisposed()
        {
            Disposed?.Invoke();
        }
    }
}
