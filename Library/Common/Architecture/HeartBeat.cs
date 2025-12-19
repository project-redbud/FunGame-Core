using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public class HeartBeat : ISocketHeartBeat
    {
        public TransmittalType TransmittalType { get; } = TransmittalType.Socket;
        public bool SendingHeartBeat => _sendingHeartBeat;
        public int HeartBeatFaileds => _heartBeatFaileds;

        private Task? _sendingHeartBeatTask;
        private bool _sendingHeartBeat = false;
        private bool _lastHeartbeatReceived = false;
        private int _heartBeatFaileds = 0;

        private readonly Socket? _socket = null;
        private readonly HTTPClient? _httpClient = null;

        public HeartBeat(Socket socket)
        {
            _socket = socket;
            TransmittalType = TransmittalType.Socket;
        }

        public HeartBeat(HTTPClient client)
        {
            _httpClient = client;
            TransmittalType = TransmittalType.WebSocket;
        }

        public void StartSendingHeartBeat()
        {
            if (!FunGameInfo.FunGame_DebugMode)
            {
                _sendingHeartBeat = true;
                _socket?.AddSocketObjectHandler(SocketObject_Handler);
                _httpClient?.AddSocketObjectHandler(SocketObject_Handler);
                _sendingHeartBeatTask = Task.Factory.StartNew(SendHeartBeat);
            }
        }

        public void StopSendingHeartBeat()
        {
            _sendingHeartBeat = false;
            _sendingHeartBeatTask?.Wait(1);
            _sendingHeartBeatTask = null;
            _socket?.RemoveSocketObjectHandler(SocketObject_Handler);
            _httpClient?.RemoveSocketObjectHandler(SocketObject_Handler);
        }

        private async Task SendHeartBeat()
        {
            try
            {
                await Task.Delay(100);
                if (_socket != null)
                {
                    while (_socket.Connected)
                    {
                        if (!SendingHeartBeat) _sendingHeartBeat = true;
                        // 发送心跳包
                        _lastHeartbeatReceived = false;
                        if (_socket.Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                        {
                            await Task.Delay(4 * 1000);
                            if (!_lastHeartbeatReceived) AddHeartBeatFaileds();
                        }
                        else AddHeartBeatFaileds();
                        await Task.Delay(20 * 1000);
                    }
                }
                else if (_httpClient != null)
                {
                    while (_httpClient.WebSocket?.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        if (!SendingHeartBeat) _sendingHeartBeat = true;
                        // 发送心跳包
                        _lastHeartbeatReceived = false;
                        if (await _httpClient.Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                        {
                            await Task.Delay(4 * 1000);
                            if (!_lastHeartbeatReceived) AddHeartBeatFaileds();
                        }
                        else AddHeartBeatFaileds();
                        await Task.Delay(20 * 1000);
                    }
                }
                _sendingHeartBeat = false;
            }
            catch (System.Exception e)
            {
                if (_socket != null)
                {
                    _socket.OnConnectionLost(e);
                    _socket.Close();
                }
                if (_httpClient != null)
                {
                    _httpClient.OnConnectionLost(e);
                    _httpClient.Close();
                }
            }
        }

        private void AddHeartBeatFaileds()
        {
            // 超过三次没回应心跳，服务器连接失败。
            if (_heartBeatFaileds++ >= 3)
            {
                _socket?.Close();
                _httpClient?.Close();
                throw new LostConnectException();
            }
        }

        private void SocketObject_Handler(SocketObject obj)
        {
            if (obj.SocketType == SocketMessageType.HeartBeat)
            {
                _lastHeartbeatReceived = true;
                _heartBeatFaileds = 0;
            }
        }
    }
}
