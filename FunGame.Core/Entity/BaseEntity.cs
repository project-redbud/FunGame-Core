using Milimoe.FunGame.Core.Interface.Entity;
using System.Collections;

namespace Milimoe.FunGame.Core.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        public long Id { get; set; } = 0;
        public Guid Guid { get; set; } = Guid.Empty;
        public string Name { get; set; } = "";

        public abstract bool Equals(IBaseEntity? other);
        public abstract IEnumerator<IBaseEntity> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
