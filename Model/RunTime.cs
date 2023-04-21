using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public abstract class RunTime
    {
        public bool Connected => _Socket != null && _Socket.Connected;

        protected Task? _ReceivingTask;
        protected Socket? _Socket;
        protected bool _IsReceiving;

        public abstract bool Disconnect();
        public abstract bool Disconnected();
        public abstract void GetServerConnection();
        public abstract Task<ConnectResult> Connect();
        public abstract bool Close();
        public abstract void Error(Exception e);

        protected abstract void StartReceiving();
        protected abstract SocketObject[] GetServerMessage();
        protected abstract SocketMessageType Receiving();
    }
}
