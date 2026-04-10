using System.Collections;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class ConcurrentFIFOQueue<T> : IEnumerable<T>
    {
        private System.Collections.Concurrent.ConcurrentQueue<T> Instance { get; } = [];

        public bool IsEmpty => Instance.IsEmpty;

        public int Count => Instance.Count;

        public void Clear() => Instance.Clear();

        public void Add(T obj)
        {
            Instance.Enqueue(obj);
        }

        public bool Dequeue(out T? obj)
        {
            return Instance.TryDequeue(out obj);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Instance.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
