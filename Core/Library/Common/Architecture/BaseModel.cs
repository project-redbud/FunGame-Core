using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public class BaseModel : ISocketHandler, IDisposable
    {
        /// <summary>
        /// Socket
        /// </summary>
        private readonly Socket _Socket;

        /// <summary>
        /// 继承请调用base构造
        /// </summary>
        /// <param name="socket">Socket</param>
        public BaseModel(Socket? socket)
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
    }
}
