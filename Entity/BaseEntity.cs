using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        public virtual long Id { get; set; } = 0;
        public virtual Guid Guid { get; set; } = Guid.Empty;
        public virtual string Name { get; set; } = "";

        public abstract bool Equals(IBaseEntity? other);
    }
}
