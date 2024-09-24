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

        private Socket(System.Net.Sockets.Socket instance, string serverAddress, int serverPort)
        {
            this.Instance = instance;
            this.ServerAddress = serverAddress;
            this.ServerPort = serverPort;
            HeartBeat = new(this);
            HeartBeat.StartSendingHeartBeat();
        }

        public static Socket Connect(string address, int port = 22222)
        {
            System.Net.Sockets.Socket? socket = SocketManager.Connect(address, port);
            if (socket != null) return new Socket(socket, address, port);
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

        public void BindEvent(Delegate method, bool remove = false)
        {
            if (!remove)
            {
                SocketManager.SocketReceive += (SocketManager.SocketReceiveHandler)method;
            }
            else
            {
                SocketManager.SocketReceive -= (SocketManager.SocketReceiveHandler)method;
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
