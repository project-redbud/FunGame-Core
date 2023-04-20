using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public class BaseController : ISocketHandler, IDisposable
    {
        /// <summary>
        /// 接收到的SocketObject实例
        /// </summary>
        protected virtual SocketObject Work { get; set; }

        /// <summary>
        /// 是否处于等待服务器响应的状态
        /// </summary>
        protected virtual bool Working { get; set; } = false;

        /// <summary>
        /// Socket
        /// </summary>
        private readonly Socket _Socket;

        /// <summary>
        /// 继承请调用base构造
        /// </summary>
        /// <param name="socket">Socket</param>
        public BaseController(Socket? socket)
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
        /// 调用Socket.Send()前，请设置为等待状态
        /// </summary>
        protected void SetWorking()
        {
            Working = true;
            Work = default;
        }

        /// <summary>
        /// 调用Socket.Send() == Success后，请等待任务完成
        /// </summary>
        protected void WaitForWorkDone()
        {
            while (true)
            {
                if (!Working) break;
            }
        }
    }
}
