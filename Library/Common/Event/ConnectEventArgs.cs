namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ConnectEventArgs : GeneralEventArgs
    {
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 22222;

        public ConnectEventArgs(params object[]? objs)
        {
            if (objs != null)
            {
                if (objs.Length > 0) ServerIP = (string)objs[0];
                if (objs.Length > 1) ServerPort = (int)objs[1];
            }
        }
    }
}
