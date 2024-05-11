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
        private int _HeartBeatFaileds = 0;

        private readonly Socket? _Socket = null;
        private readonly HTTPClient? _HTTPClient = null;

        public HeartBeat(Socket socket)
        {
            _Socket = socket;
            this.TransmittalType = TransmittalType.Socket;
        }

        public HeartBeat(HTTPClient client)
        {
            _HTTPClient = client;
            this.TransmittalType = TransmittalType.WebSocket;
        }

        public void StartSendingHeartBeat()
        {
            if (!FunGameInfo.FunGame_DebugMode)
            {
                _SendingHeartBeat = true;
                SendingHeartBeatTask = Task.Factory.StartNew(SendHeartBeat);
            }
        }

        public void StopSendingHeartBeat()
        {
            _SendingHeartBeat = false;
            SendingHeartBeatTask?.Wait(1);
            SendingHeartBeatTask = null;
        }

        private async Task SendHeartBeat()
        {
            await Task.Delay(100);
            if (_Socket != null)
            {
                while (_Socket.Connected)
                {
                    if (!SendingHeartBeat) _SendingHeartBeat = true;
                    // 发送心跳包
                    if (_Socket.Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                    {
                        await Task.Delay(4 * 000);
                        AddHeartBeatFaileds();
                    }
                    else AddHeartBeatFaileds();
                    await Task.Delay(20 * 000);
                }
            }
            else if (_HTTPClient != null)
            {
                while (_HTTPClient.Instance?.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    if (!SendingHeartBeat) _SendingHeartBeat = true;
                    // 发送心跳包
                    if (await _HTTPClient.Send(SocketMessageType.HeartBeat) == SocketResult.Success)
                    {
                        await Task.Delay(4 * 000);
                        AddHeartBeatFaileds();
                    }
                    else AddHeartBeatFaileds();
                    await Task.Delay(20 * 000);
                }
            }
            _SendingHeartBeat = false;
        }

        private void AddHeartBeatFaileds()
        {
            // 超过三次没回应心跳，服务器连接失败。
            if (_HeartBeatFaileds++ >= 3)
                throw new LostConnectException();
        }
    }
}
