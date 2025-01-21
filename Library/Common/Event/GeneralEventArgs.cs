namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GeneralEventArgs : EventArgs
    {
        public string EventMsg { get; set; } = "";
        public Dictionary<string, object> Parameters { get; set; } = [];
        public bool Cancel { get; set; } = false;

        public GeneralEventArgs(string EventMsg, Dictionary<string, object> Parameters)
        {
            this.EventMsg = EventMsg;
            this.Parameters = Parameters;
        }

        public GeneralEventArgs(params object[] Parameters)
        {
            int count = 0;
            foreach (object obj in Parameters)
            {
                this.Parameters[count++.ToString()] = obj;
            }
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
