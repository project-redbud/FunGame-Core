using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Service
{
    internal class ThreadManager
    {
        internal static int MAX_THREAD { get; } = 20;

        private ConcurrentDictionary<string, Task> Threads { get; } = new();

        internal Task this[string name]
        {
            get
            {
                return Threads[name];
            }
        }

        internal bool Add(string name, Task t)
        {
            return Threads.TryAdd(name, t);
        }

        internal bool Remove(string name)
        {
            return Threads.TryRemove(name, out _);
        }

        internal void Clear()
        {
            Threads.Clear();
        }
        
    }
}
