using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Model
{
    public abstract class RunTime
    {
        public abstract Socket? Socket { get; }
        public bool Connected => _Socket != null && _Socket.Connected;

        protected Task? _ReceivingTask;
        protected Socket? _Socket;
        protected bool _IsReceiving;
        protected RunTimeController? _Controller;

        public bool Disconnect()
        {
            bool result = false;

            try
            {
                result = _Socket?.Send(SocketMessageType.Disconnect, "") == SocketResult.Success;
            }
            catch (Exception e)
            {
                _Controller?.WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }

        public void Disconnected()
        {
            Disconnect();
        }

        public abstract void GetServerConnection();

        public abstract Task<ConnectResult> Connect();

        public bool Close()
        {
            try
            {
                if (_Socket != null)
                {
                    _Socket.Close();
                    _Socket = null;
                }
                if (_ReceivingTask != null && !_ReceivingTask.IsCompleted)
                {
                    _ReceivingTask.Wait(1);
                    _ReceivingTask = null;
                    _IsReceiving = false;
                }
            }
            catch (Exception e)
            {
                _Controller?.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
            return true;
        }

        public abstract void Error(Exception e);

        protected void StartReceiving()
        {
            _ReceivingTask = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                _IsReceiving = true;
                while (Connected)
                {
                    Receiving();
                }
            });
            _Socket?.StartReceiving(_ReceivingTask);
        }

        protected SocketObject[] GetServerMessage()
        {
            if (_Socket != null && _Socket.Connected)
            {
                return _Socket.ReceiveArray();
            }
            return Array.Empty<SocketObject>();
        }

        protected SocketMessageType Receiving()
        {
            if (_Socket is null) return SocketMessageType.Unknown;
            SocketMessageType result = SocketMessageType.Unknown;
            try
            {
                SocketObject[] ServerMessages = GetServerMessage();

                foreach (SocketObject ServerMessage in ServerMessages)
                {
                    SocketMessageType type = ServerMessage.SocketType;
                    object[] objs = ServerMessage.Parameters;
                    result = type;
                    switch (type)
                    {
                        case SocketMessageType.Connect:
                            if (!SocketHandler_Connect(ServerMessage)) return SocketMessageType.Unknown;
                            break;

                        case SocketMessageType.Disconnect:
                            SocketHandler_Disconnect(ServerMessage);
                            break;

                        case SocketMessageType.HeartBeat:
                            SocketHandler_HeartBeat(ServerMessage);
                            break;

                        case SocketMessageType.Unknown:
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // 报错中断服务器连接
                Error(e);
            }
            return result;
        }

        protected abstract bool SocketHandler_Connect(SocketObject ServerMessage);

        protected abstract void SocketHandler_Disconnect(SocketObject ServerMessage);

        protected abstract void SocketHandler_HeartBeat(SocketObject ServerMessage);
    }
}
