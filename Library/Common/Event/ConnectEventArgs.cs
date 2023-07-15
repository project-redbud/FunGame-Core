using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ConnectEventArgs : GeneralEventArgs
    {
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 22222;
        public ConnectResult ConnectResult { get; set; } = ConnectResult.Success;

        public ConnectEventArgs(string ip = "", int port = 0, ConnectResult result = ConnectResult.Success)
        {
            if (ip.Trim() != "") ServerIP = ip;
            if (port != 0) ServerPort = port;
            ConnectResult = result;
        }
    }
}
