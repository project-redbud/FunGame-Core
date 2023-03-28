namespace Milimoe.FunGame.Core.Api.Utility
{
    public class ConcurrentQueue<T>
    {
        private bool Lock { get; set; }

        private System.Collections.Concurrent.ConcurrentQueue<T> Instance { get; } = new();

        public async void AddAsync(T obj)
        {
            if (Lock)
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (!Lock) break;
                        Thread.Sleep(100);
                    }
                });
            }
            Lock = true;
            Instance.Enqueue(obj);
        }

        public bool Delete()
        {
            bool result = Instance.TryDequeue(out _);
            Lock = false;
            return result;
        }
    }
}
