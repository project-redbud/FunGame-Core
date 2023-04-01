namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class ConnectEventArgs : GeneralEventArgs
    {
        public string ServerIP { get; set; } = "";
        public string ServerPort { get; set; } = "";

        public ConnectEventArgs(params object[]? objs)
        {
            if (objs != null)
            {
                if (objs.Length > 0) ServerIP = (string)objs[0];
                if (objs.Length > 1) ServerPort = (string)objs[1];
            }
        }
    }
}
