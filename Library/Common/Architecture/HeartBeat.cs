using System.Diagnostics;
using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    internal class HeartBeat : ISocketHeartBeat
    {
        public TransmittalType TransmittalType { get; } = TransmittalType.Socket;
        public bool SendingHeartBeat => _sendingHeartBeat;
        public int HeartBeatFaileds => _heartBeatFaileds;
        public int Ping => Math.Min(_ping, 999);

        private Task? _sendingHeartBeatTask;
        private bool _sendingHeartBeat = false;
        private int _heartBeatFaileds = 0;
        private string _currentProbeId = "";
        private long _lastSentTimestamp;
        private int _ping = 0;

        private readonly Lock _probeLock = new();
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
                _sendingHeartBeatTask = Task.Run(SendHeartBeat);
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
                while (_sendingHeartBeat)
                {
                    // 发送心跳包
                    string probeId = Guid.NewGuid().ToString();
                    lock (_probeLock)
                    {
                        _currentProbeId = probeId;
                        _lastSentTimestamp = Stopwatch.GetTimestamp();
                    }
                    try
                    {
                        bool sendSuccess = false;
                        if (_socket != null)
                        {
                            sendSuccess = _socket.Connected && _socket.Send(SocketMessageType.HeartBeat, _currentProbeId) == SocketResult.Success;
                        }
                        else if (_httpClient != null)
                        {
                            sendSuccess = _httpClient.WebSocket?.State == System.Net.WebSockets.WebSocketState.Open && await _httpClient.Send(SocketMessageType.HeartBeat, _currentProbeId) == SocketResult.Success;
                        }
                        if (!sendSuccess)
                        {
                            throw new ConnectFailedException();
                        }
                        else
                        {
                            await Task.Delay(4 * 1000);
                            lock (_probeLock)
                            {
                                if (_currentProbeId == probeId) throw new TimeOutException();
                            }
                        }
                    }
                    catch
                    {
                        AddHeartBeatFaileds();
                    }
                    await Task.Delay(20 * 1000);
                }
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
            finally
            {
                _sendingHeartBeat = false;
            }
        }

        private void AddHeartBeatFaileds()
        {
            _currentProbeId = "";
            _ping = 999;
            // 超过三次没回应心跳，服务器连接失败。
            if (++_heartBeatFaileds >= 3)
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
                string receivedId = "";
                if (obj.Length > 0)
                {
                    receivedId = obj.GetParam<string>(0) ?? "";
                }
                lock (_probeLock)
                {
                    if (receivedId == _currentProbeId && !string.IsNullOrEmpty(_currentProbeId))
                    {
                        long elapsedTicks = Stopwatch.GetTimestamp() - _lastSentTimestamp;
                        double rttMs = elapsedTicks * 1000.0 / Stopwatch.Frequency;
                        _ping = (int)Math.Round(rttMs);
                        _heartBeatFaileds = 0;
                        _currentProbeId = "";
                    }
                }
            }
        }
    }
}
