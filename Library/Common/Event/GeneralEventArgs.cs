namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GeneralEventArgs : EventArgs
    {
        public string EventMsg { get; set; } = "";
        public object[] Parameters { get; set; } = Array.Empty<object>();
        public bool Cancel { get; set; } = false;

        public GeneralEventArgs(string EventMsg, object[] Parameters)
        {
            this.EventMsg = EventMsg;
            this.Parameters = Parameters;
        }

        public GeneralEventArgs(params object[] Parameters)
        {
            this.Parameters = Parameters;
        }
    }

    public class GeneralEvent<T>
    {
        public T? Instance { get; set; }
        public GeneralEvent()
        {
            Instance = (T?)Activator.CreateInstance(typeof(T?));
        }
    }
}
