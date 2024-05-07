using System.Net.WebSockets;
using Milimoe.FunGame.Core.Interface.HTTP;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class HTTPClient : IHTTPClient
    {
        public ClientWebSocket? Instance { get; } = null;
        public HeartBeat HeartBeat { get; }
        public SocketRuntimeType Runtime => SocketRuntimeType.Client;
        public Guid Token { get; } = Guid.Empty;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";

        private bool _Listening = false;

        private HTTPClient(ClientWebSocket Instance, string ServerAddress, int ServerPort, params object[] args)
        {
            this.Instance = Instance;
            this.ServerAddress = ServerAddress;
            this.ServerPort = ServerPort;
            HeartBeat = new(this);
            HeartBeat.StartSendingHeartBeat();
            Task.Factory.StartNew(async () => await StartListening(args));
        }

        public static async Task<HTTPClient> Connect(string ServerAddress, int ServerPort, bool SSL, string SubDirectory = "", params object[] args)
        {
            string ServerIP = Api.Utility.NetworkUtility.GetIPAddress(ServerAddress);
            Uri uri = new((SSL ? "wss://" : "ws://") + ServerIP + ":" + ServerPort + "/" + SubDirectory);
            ClientWebSocket? socket = await HTTPManager.Connect(uri);
            if (socket != null && socket.State == WebSocketState.Open)
            {
                HTTPClient client = new(socket, ServerAddress, ServerPort, args);
                return client;
            }
            throw new CanNotConnectException();
        }

        private async Task StartListening(params object[] args)
        {
            if (Instance != null && Instance.State == WebSocketState.Open)
            {
                if (await HTTPManager.Send(Instance, new(SocketMessageType.Connect, Guid.Empty, args)) == SocketResult.Success && await HTTPManager.ReceiveMessage(this))
                {
                    _Listening = true;
                    await Receive();
                }
            }
        }

        public async Task Receive()
        {
            while (_Listening)
            {
                try
                {
                    await HTTPManager.ReceiveMessage(this);
                }
                catch (System.Exception e)
                {
                    Close();
                    Api.Utility.TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    throw new SocketWrongInfoException();
                }
            }
        }

        public SocketResult Send(SocketMessageType type, params object[] objs)
        {
            throw new NotImplementedException();
        }

        public virtual SocketObject SocketObject_Handler(SocketObject objs)
        {
            return new(SocketMessageType.Unknown, Guid.Empty);
        }

        public void Close()
        {
            _Listening = false;
            HeartBeat.StopSendingHeartBeat();
            Instance?.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            Instance?.Dispose();
        }
    }
}
