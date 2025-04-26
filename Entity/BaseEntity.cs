using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// 实体的数字 ID
        /// </summary>
        public virtual long Id { get; set; } = 0;

        /// <summary>
        /// 实体的唯一 ID
        /// </summary>
        public virtual Guid Guid { get; set; } = Guid.Empty;

        /// <summary>
        /// 实体的名称
        /// </summary>
        public virtual string Name { get; set; } = "";

        /// <summary>
        /// 实体的当前状态（关联数据库操作）
        /// </summary>
        public EntityState EntityState { get; set; } = EntityState.Unchanged;

        /// <summary>
        /// 实体所用的游戏平衡常数
        /// </summary>
        public EquilibriumConstant GameplayEquilibriumConstant { get; set; } = General.GameplayEquilibriumConstant;

        /// <summary>
        /// 比较两个实体是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(IBaseEntity? other);

        /// <summary>
        /// 获取实体的 Id.Name
        /// </summary>
        /// <returns></returns>
        public string GetIdName()
        {
            return Id + "." + Name;
        }
    }
}
