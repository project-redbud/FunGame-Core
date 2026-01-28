using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色职业类
    /// </summary>
    public class Class : BaseEntity
    {
        /// <summary>
        /// 职业名称
        /// </summary>
        public override string Name { get; set; } = "";

        /// <summary>
        /// 职业等级
        /// </summary>
        public int Level
        {
            get
            {
                return Math.Max(0, field);
            }
            set
            {
                field = Math.Max(0, value);
            }
        }

        /// <summary>
        /// 职业战技
        /// </summary>
        public HashSet<Skill> Skills { get; set; } = [];

        /// <summary>
        /// 职业魔法
        /// </summary>
        public HashSet<Skill> Magics { get; set; } = [];

        /// <summary>
        /// 职业被动
        /// </summary>
        public HashSet<Skill> PassiveSkills { get; set; } = [];

        /// <summary>
        /// 职业爆发技
        /// </summary>
        public HashSet<Skill> SuperSkills { get; set; } = [];

        public override bool Equals(IBaseEntity? other)
        {
            return other is Class && other.GetIdName() == GetIdName();
        }
    }
}
