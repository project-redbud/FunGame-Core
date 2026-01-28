using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
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
        /// 固有被动
        /// </summary>
        public HashSet<Skill> InherentPassives { get; set; } = [];

        /// <summary>
        /// 角色定位
        /// </summary>
        public HashSet<RoleType> RoleTypes { get; set; } = [];

        /// <summary>
        /// 战斗天赋
        /// </summary>
        public HashSet<Skill> CombatTalents { get; set; } = [];

        public override bool Equals(IBaseEntity? other)
        {
            return other is SubClass && other.GetIdName() == GetIdName();
        }
    }
}
