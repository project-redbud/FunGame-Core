namespace Milimoe.FunGame.Core.Api.Utility
{
    public class ConcurrentQueue<T>
    {
        public bool Lock { get; set; }

        private System.Collections.Concurrent.ConcurrentQueue<T> Instance { get; }

        public ConcurrentQueue()
        {
            Instance = new System.Collections.Concurrent.ConcurrentQueue<T>();
        }

        public void Add(T obj)
        {
            if (Lock)
            {
                return;
            }
            Lock = true;
            lock (Instance)
            {
                Instance.Enqueue(obj);
            }
        }

        public bool Delete()
        {
            bool result = false;
            lock (Instance)
            {
                result = Instance.TryDequeue(out _);
            }
            Lock = false;
            return result;
        }
    }
}
