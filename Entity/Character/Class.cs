using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Entity
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

        /// <summary>
        /// 战斗天赋池，按角色定位索引
        /// <para>战斗天赋绑定于职业而非流派：定位由已选流派提供，规划系统再由流派 <see cref="SubClass.Class"/> 反查所属职业，从此池取对应定位的天赋</para>
        /// </summary>
        public Dictionary<RoleType, HashSet<Skill>> CombatTalents { get; set; } = [];

        /// <summary>
        /// 职业模板的核心属性分配上下限（可空 = 不限）
        /// <para>与角色模板 <see cref="Character.AttributeLimit"/> 取交集后约束 1 级初始分配；数值提升不受此限制</para>
        /// </summary>
        public ClassAttributeLimit? AttributeLimit { get; set; } = null;

        /// <summary>
        /// 复制技能并保留等级状态
        /// <para>职业记录复制需要完整状态：基础等级写入副本基础，突破加成独立保留</para>
        /// </summary>
        internal static Skill CopySkillState(Skill skill)
        {
            Skill copy = skill.Copy();
            copy.Level = Math.Max(0, skill.Level - skill.ExLevel);
            copy.ExLevel = skill.ExLevel;
            return copy;
        }

        /// <summary>
        /// 复制职业定义作为玩家职业记录
        /// <para>技能实例同步深拷贝，职业记录之间互不共享</para>
        /// </summary>
        /// <returns>职业记录的副本</returns>
        public Class Copy()
        {
            Class copy = new()
            {
                Id = Id,
                Name = Name,
                Level = Level,
                Skills = [.. Skills.Select(CopySkillState)],
                Magics = [.. Magics.Select(CopySkillState)],
                PassiveSkills = [.. PassiveSkills.Select(CopySkillState)],
                SuperSkills = [.. SuperSkills.Select(CopySkillState)],
                AttributeLimit = AttributeLimit
            };
            foreach (KeyValuePair<RoleType, HashSet<Skill>> kv in CombatTalents)
            {
                copy.CombatTalents[kv.Key] = [.. kv.Value.Select(CopySkillState)];
            }
            return copy;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Class && other.GetIdName() == GetIdName();
        }
    }
}
