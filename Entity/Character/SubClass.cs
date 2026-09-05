using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Entity
{
    /// <summary>
    /// 子职业（流派/派别）
    /// </summary>
    public class SubClass(Class @class) : BaseEntity
    {
        /// <summary>
        /// 流派名称
        /// </summary>
        public override string Name { get; set; } = "";

        /// <summary>
        /// 所属职业
        /// </summary>
        public Class Class => @class;

        /// <summary>
        /// 职业等级
        /// </summary>
        public int Level => @class.Level;

        /// <summary>
        /// 固有被动，key = 获得所需的职业等级（设定为 1 和 6 级各 1 个）
        /// </summary>
        public Dictionary<int, HashSet<Skill>> InherentPassives { get; set; } = [];

        /// <summary>
        /// 角色定位，角色从已选流派提供的定位中挑选至多 3 个
        /// </summary>
        public HashSet<RoleType> RoleTypes { get; set; } = [];

        /// <summary>
        /// 复制流派定义作为玩家职业记录，副本绑定到 <paramref name="ownerClass"/>
        /// <para>流派的等级委托给所属职业（<see cref="Level"/>），因此副本必须绑到职业的副本上。</para>
        /// </summary>
        /// <param name="ownerClass">所属职业的副本</param>
        /// <returns>流派记录的副本</returns>
        public SubClass Copy(Class ownerClass)
        {
            SubClass copy = new(ownerClass)
            {
                Id = Id,
                Name = Name,
                RoleTypes = [.. RoleTypes]
            };
            foreach (KeyValuePair<int, HashSet<Skill>> kv in InherentPassives)
            {
                copy.InherentPassives[kv.Key] = [.. kv.Value.Select(Class.CopySkillState)];
            }
            return copy;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is SubClass && other.GetIdName() == GetIdName();
        }
    }
}
