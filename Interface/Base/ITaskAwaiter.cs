namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ITaskAwaiter
    {
        public bool IsCompleted { get; }

        public ITaskAwaiter OnCompleted(Action action);
    }
}
