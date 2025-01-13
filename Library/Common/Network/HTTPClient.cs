using System.Net.WebSockets;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.HTTP;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class HTTPClient : IHTTPClient
    {
        public HttpClient Instance { get; }
        public ClientWebSocket? WebSocket { get; } = null;
        public SocketRuntimeType Runtime => SocketRuntimeType.Client;
        public Guid Token { get; set; } = Guid.Empty;
        public string ServerAddress { get; } = "";
        public int ServerPort { get; } = 0;
        public string ServerName { get; } = "";
        public string ServerNotice { get; } = "";
        public bool Connected => WebSocket != null && WebSocket.State == WebSocketState.Open;
        public bool Receiving => _receiving;
        private HeartBeat HeartBeat { get; }

        public event Action<System.Exception>? ConnectionLost;
        public event Action? Closed;

        private bool _receiving = false;
        private readonly HashSet<Action<SocketObject>> _boundEvents = [];

        private HTTPClient(ClientWebSocket websocket, string serverAddress, int serverPort, params object[] args)
        {
            Instance = new();
            WebSocket = websocket;
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
                    OnConnectionLost(e);
                    Close();
                    Api.Utility.TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    throw new SocketWrongInfoException();
                }
            }
        }

        public async Task<SocketResult> Send(SocketMessageType type, params object[] objs)
        {
            if (WebSocket != null)
            {
                return await HTTPManager.Send(WebSocket, new(type, Token, objs));
            }
            return SocketResult.NotSent;
        }

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T?> HttpGet<T>(string url)
        {
            HttpResponseMessage response = await Instance.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            T? result = NetworkUtility.JsonDeserialize<T>(content);
            return result;
        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<T?> HttpPost<T>(string url, string json)
        {
            HttpContent content = new StringContent(json, General.DefaultEncoding, "application/json");
            HttpResponseMessage response = await Instance.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string read = await response.Content.ReadAsStringAsync();
            T? result = NetworkUtility.JsonDeserialize<T>(read);
            return result;
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

        public void OnConnectionLost(System.Exception e)
        {
            ConnectionLost?.Invoke(e);
        }

        public void Close()
        {
            Instance.Dispose();
            _receiving = false;
            HeartBeat.StopSendingHeartBeat();
            WebSocket?.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            WebSocket?.Dispose();
            foreach (Action<SocketObject> method in _boundEvents.ToList())
            {
                RemoveSocketObjectHandler(method);
            }
            Closed?.Invoke();
            ConnectionLost = null;
            Closed = null;
        }

        private async Task StartListening(params object[] args)
        {
            if (WebSocket != null && WebSocket.State == WebSocketState.Open)
            {
                if (await HTTPManager.Send(WebSocket, new(SocketMessageType.Connect, Guid.Empty, args)) == SocketResult.Success && await HTTPManager.ReceiveMessage(this))
                {
                    _receiving = true;
                    await Receive();
                }
            }
        }
    }
}
