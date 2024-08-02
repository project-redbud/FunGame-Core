using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// 实体的数字ID
        /// </summary>
        public virtual long Id { get; set; } = 0;

        /// <summary>
        /// 实体的唯一ID
        /// </summary>
        public virtual Guid Guid { get; set; } = Guid.Empty;

        /// <summary>
        /// 实体的名称
        /// </summary>
        public virtual string Name { get; set; } = "";

        public abstract bool Equals(IBaseEntity? other);
    }
}
