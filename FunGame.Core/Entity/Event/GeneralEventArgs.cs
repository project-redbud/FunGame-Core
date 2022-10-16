using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Event
{
    public class GeneralEventArgs : EventArgs
    {
        public string EventMsg { get; set; } = "";
        public object[]? Parameters { get; set; } = null;
    }

    public class GeneralEvent<T>
    {
        public T Instance { get; set; }
        public GeneralEvent()
        {
            Instance = System.Activator.CreateInstance<T>();
        }
    }
}
