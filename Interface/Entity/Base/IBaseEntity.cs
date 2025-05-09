namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IBaseEntity : IEquatable<IBaseEntity>
    {
        /// <summary>
        /// 实体的数字标识符
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// 实体的唯一标识符
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// 实体的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取实体的 Id.Name
        /// </summary>
        /// <returns></returns>
        public string GetIdName();
    }
}
