using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocket
    {
        public System.Net.Sockets.Socket Instance { get; }
        public int Runtime { get; }
        public string ServerIP { get; }
        public int ServerPort { get; }
        public string ServerName { get; }
        public string ServerNotice { get; }
        public int HeartBeatFaileds { get; }
        public bool Connected
        {
            get
            {
                return Instance != null && Instance.Connected;
            }
        }
        public bool Receiving { get; }
        public bool SendingHeartBeat { get; }
        public SocketResult Send(SocketMessageType type, params object[] objs);
        public object[] Receive();
        public void Close();
        public void StartReceiving(Task t);
    }
}
