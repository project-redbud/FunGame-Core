using Milimoe.FunGame.Core.Interface.Sockets;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public class HeartBeat : ISocketHeartBeat
    {
        public TransmittalType TransmittalType { get; } = TransmittalType.Socket;
        public bool SendingHeartBeat => _SendingHeartBeat;
        public int HeartBeatFaileds => _HeartBeatFaileds;

        private Task? SendingHeartBeatTask;
        private bool _SendingHeartBeat = false;
        private bool _LastHeartbeatReceived = false;
        private int _HeartBeatFaileds = 0;

        private readonly Socket? _Socket = null;
        private readonly HTTPClient? _HTTPClient = null;

        public HeartBeat(Socket socket)
        {
            _Socket = socket;
            TransmittalType = TransmittalType.Socket;
        }

        public HeartBeat(HTTPClient client)
        {
            _HTTPClient = client;
            TransmittalType = TransmittalType.WebSocket;
        }

        public void StartSendingHeartBeat()
        {
            if (!FunGameInfo.FunGame_DebugMode)
            {
                _SendingHeartBeat = true;
                _Socket?.AddSocketObjectHandler(SocketObject_Handler);
                _HTTPClient?.AddSocketObjectHandler(SocketObject_Handler);
                SendingHeartBeatTask = Task.Factory.StartNew(SendHeartBeat);
            }
        }

        public void StopSendingHeartBeat()
        {
            _SendingHeartBeat = false;
            SendingHeartBeatTask?.Wait(1);
            SendingHeartBeatTask = null;
            _Socket?.RemoveSocketObjectHandler(SocketObject_Handler);
            _HTTPClient?.RemoveSocketObjectHandler(SocketObject_Handler);
        }

        private async Task SendHeartBeat()
        {
            try
            {
                await Task.Delay(100);
                if (_Socket != null)
                {
                    while (_Socket.Connected)
                    {
                        if (!SendingHeartBeat) _SendingHeartBeat = true;
                        // 发送心跳包
                        _LastHeartbeatReceived = false;
                        if (_Socket.Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                        {
                            await Task.Delay(4 * 1000);
                            if (!_LastHeartbeatReceived) AddHeartBeatFaileds();
                        }
                        else AddHeartBeatFaileds();
                        await Task.Delay(20 * 1000);
                    }
                }
                else if (_HTTPClient != null)
                {
                    while (_HTTPClient.WebSocket?.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        if (!SendingHeartBeat) _SendingHeartBeat = true;
                        // 发送心跳包
                        if (await _HTTPClient.Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                        {
                            await Task.Delay(4 * 1000);
                            AddHeartBeatFaileds();
                        }
                        else AddHeartBeatFaileds();
                        await Task.Delay(20 * 1000);
                    }
                }
                _SendingHeartBeat = false;
            }
            catch (System.Exception e)
            {
                if (_Socket != null)
                {
                    _Socket.OnConnectionLost(e);
                    _Socket.Close();
                }
                if (_HTTPClient != null)
                {
                    _HTTPClient.OnConnectionLost(e);
                    _HTTPClient.Close();
                }
            }
        }

        private void AddHeartBeatFaileds()
        {
            // 超过三次没回应心跳，服务器连接失败。
            if (_HeartBeatFaileds++ >= 3)
            {
                _Socket?.Close();
                _HTTPClient?.Close();
                throw new LostConnectException();
            }
        }

        private void SocketObject_Handler(SocketObject obj)
        {
            if (obj.SocketType == SocketMessageType.HeartBeat)
            {
                _LastHeartbeatReceived = true;
                _HeartBeatFaileds = 0;
            }
        }
    }
}
