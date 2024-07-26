using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class Socket : IClientSocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Client;
        public Guid Token { get; set; } = Guid.Empty;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public bool Connected => Instance != null && Instance.Connected;
        public bool Receiving => _Receiving;

        private Task? ReceivingTask;
        private readonly HeartBeat HeartBeat;
        private bool _Receiving = false;

        private Socket(System.Net.Sockets.Socket Instance, string ServerAddress, int ServerPort)
        {
            this.Instance = Instance;
            this.ServerAddress = ServerAddress;
            this.ServerPort = ServerPort;
            HeartBeat = new(this);
            HeartBeat.StartSendingHeartBeat();
        }

        public static Socket Connect(string Address, int Port = 22222)
        {
            System.Net.Sockets.Socket? socket = SocketManager.Connect(Address, Port);
            if (socket != null) return new Socket(socket, Address, Port);
            else throw new ConnectFailedException();
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                if (SocketManager.Send(new(type, Token, objs)) == SocketResult.Success)
                {
                    return SocketResult.Success;
                }
                return SocketResult.Fail;
            }
            return SocketResult.NotSent;
        }

        public SocketObject[] Receive()
        {
            try
            {
                SocketObject[] result = SocketManager.Receive();
                return result;
            }
            catch (System.Exception e)
            {
                Api.Utility.TXTHelper.AppendErrorLog(e.GetErrorInfo());
                throw new SocketWrongInfoException();
            }
        }

        public void BindEvent(Delegate Method, bool Remove = false)
        {
            if (!Remove)
            {
                SocketManager.SocketReceive += (SocketManager.SocketReceiveHandler)Method;
            }
            else
            {
                SocketManager.SocketReceive -= (SocketManager.SocketReceiveHandler)Method;
            }
        }

        public void Close()
        {
            HeartBeat.StopSendingHeartBeat();
            StopReceiving();
            Instance?.Close();
        }

        public void StartReceiving(Task t)
        {
            _Receiving = true;
            ReceivingTask = t;
        }

        private void StopReceiving()
        {
            _Receiving = false;
            ReceivingTask?.Wait(1);
            ReceivingTask = null;
        }
    }
}
