namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IFactory<T>
    {
        public Type EntityType { get; }
        public T Create();
    }
}
