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
        public System.Net.WebSockets.ClientWebSocket? Instance { get; } = null;
        public SocketRuntimeType Runtime => SocketRuntimeType.Client;
        public Guid Token { get; set; } = Guid.Empty;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public bool Connected => Instance != null && Instance.State == WebSocketState.Open;
        public bool Receiving => _receiving;
        private HeartBeat HeartBeat { get; }

        private bool _receiving = false;
        private readonly HashSet<Action<SocketObject>> _boundEvents = [];

        private HTTPClient(ClientWebSocket instance, string serverAddress, int serverPort, params object[] args)
        {
            Instance = instance;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
            HeartBeat = new(this);
            HeartBeat.StartSendingHeartBeat();
            Task.Factory.StartNew(async () => await StartListening(args));
        }

        public static async Task<HTTPClient> Connect(string serverAddress, bool ssl, int serverPort = 0, string subUrl = "", params object[] args)
        {
            Uri uri = new((ssl ? "wss://" : "ws://") + serverAddress + ":" + (serverPort != 0 ? serverPort : "") + "/" + subUrl.Trim('/') + "/");
            ClientWebSocket? socket = await HTTPManager.Connect(uri);
            if (socket != null && socket.State == WebSocketState.Open)
            {
                HTTPClient client = new(socket, serverAddress, serverPort, args);
                return client;
            }
            throw new CanNotConnectException();
        }

        public async Task Receive()
        {
            while (_receiving)
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

        public async Task<SocketResult> Send(SocketMessageType type, params object[] objs)
        {
            if (Instance != null)
            {
                return await HTTPManager.Send(Instance, new(type, Token, objs));
            }
            return SocketResult.NotSent;
        }

        public void AddSocketObjectHandler(Action<SocketObject> method)
        {
            if (_boundEvents.Add(method))
            {
                SocketManager.SocketReceive += new SocketManager.SocketReceiveHandler(method);
            }
        }

        public void RemoveSocketObjectHandler(Action<SocketObject> method)
        {
            _boundEvents.Remove(method);
            SocketManager.SocketReceive -= new SocketManager.SocketReceiveHandler(method);
        }

        public void Close()
        {
            _receiving = false;
            HeartBeat.StopSendingHeartBeat();
            Instance?.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            Instance?.Dispose();
            foreach (Action<SocketObject> method in _boundEvents.ToList())
            {
                RemoveSocketObjectHandler(method);
            }
        }

        private async Task StartListening(params object[] args)
        {
            if (Instance != null && Instance.State == WebSocketState.Open)
            {
                if (await HTTPManager.Send(Instance, new(SocketMessageType.Connect, Guid.Empty, args)) == SocketResult.Success && await HTTPManager.ReceiveMessage(this))
                {
                    _receiving = true;
                    await Receive();
                }
            }
        }
    }
}
