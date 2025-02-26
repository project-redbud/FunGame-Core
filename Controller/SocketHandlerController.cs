using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;

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
        protected override SocketObject ReceivedObject { get; set; }

        /// <summary>
        /// Socket
        /// </summary>
        private readonly Socket? _socket;

        /// <summary>
        /// WebSocket
        /// </summary>
        private readonly HTTPClient? _webSocket;

        /// <summary>
        /// 继承请调用base构造
        /// </summary>
        /// <param name="socket">Socket</param>
        public SocketHandlerController(Socket? socket)
        {
            if (socket != null)
            {
                _socket = socket;
                socket.AddSocketObjectHandler(SocketHandler);
            }
            else throw new SocketCreateReceivingException();
        }

        /// <summary>
        /// 继承请调用base构造
        /// </summary>
        /// <param name="websocket">Socket</param>
        public SocketHandlerController(HTTPClient? websocket)
        {
            if (websocket != null)
            {
                _webSocket = websocket;
                websocket.AddSocketObjectHandler(SocketHandler);
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
        private bool _isDisposed = false;

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
            if (!_isDisposed)
            {
                if (Disposing)
                {
                    _socket?.RemoveSocketObjectHandler(SocketHandler);
                    _webSocket?.RemoveSocketObjectHandler(SocketHandler);
                }
            }
            _isDisposed = true;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
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
        protected void OnDisposed()
        {
            Disposed?.Invoke();
        }
    }
}
