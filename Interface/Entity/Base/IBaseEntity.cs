namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IBaseEntity : IEquatable<IBaseEntity>
    {
        public long Id { get; }
        public Guid Guid { get; }
        public string Name { get; }
    }
}
