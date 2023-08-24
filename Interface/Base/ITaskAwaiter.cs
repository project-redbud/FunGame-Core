namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ITaskAwaiter
    {
        public bool IsCompleted { get; }
        public Exception Exception { get; }

        public ITaskAwaiter OnCompleted(Action action);
        public ITaskAwaiter OnError(Action action);
    }
}
