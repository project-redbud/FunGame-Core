using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GeneralEventArgs : EventArgs
    {
        public string EventMsg { get; set; } = "";
        public object[]? Parameters { get; set; } = null;

        public GeneralEventArgs(string EventMsg = "", object[]? Parameters = null)
        {
            this.EventMsg = EventMsg;
            this.Parameters = Parameters;
        }

        public GeneralEventArgs(params object[]? Parameters)
        {
            this.Parameters = Parameters;
        }
    }

    public class GeneralEvent<T>
    {
        public T Instance { get; set; }
        public GeneralEvent()
        {
            Instance = Activator.CreateInstance<T>();
        }
    }
}
