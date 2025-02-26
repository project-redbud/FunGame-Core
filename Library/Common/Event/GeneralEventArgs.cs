namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GeneralEventArgs : EventArgs
    {
        public bool Success { get; set; } = true;
        public string EventMsg { get; set; } = "";
        public Dictionary<string, object> Parameters { get; set; } = [];
        public bool Cancel { get; set; } = false;

        public GeneralEventArgs(string msg, Dictionary<string, object> args)
        {
            EventMsg = msg;
            Parameters = args;
        }

        public GeneralEventArgs(params object[] args)
        {
            int count = 0;
            foreach (object obj in args)
            {
                Parameters[count++.ToString()] = obj;
            }
        }
    }

    public class GeneralEvent<T>
    {
        public T? Instance { get; set; }
        public GeneralEvent()
        {
            Instance = Activator.CreateInstance<T>();
        }
    }
}
