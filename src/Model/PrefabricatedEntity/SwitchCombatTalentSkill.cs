using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 【转换战斗天赋】战技：战斗内激活另一已学天赋，并取消当前生效天赋
    /// <para>授予前提：角色存在次要定位（<see cref="CharacterClass.HasCombatTalentSwitch"/>）</para>
    /// </summary>
    public abstract class SwitchCombatTalentSkill : Skill
    {
        /// <summary>
        /// 目标定位：释放时切换到该定位尚未激活的已学天赋；null / <see cref="RoleType.None"/> 时自动选下一个已学天赋
        /// </summary>
        public RoleType? TargetRoleType { get; set; } = null;

        /// <summary>
        /// 目标天赋 IdName：指定时优先于 <see cref="TargetRoleType"/>
        /// <para>同一定位掌握多个天赋时，只有按天赋指定才能精确命中目标；未指定则按定位或自动循环</para>
        /// </summary>
        public string? TargetTalentId { get; set; } = null;

        /// <summary>
        /// 继承此构造以创建具体转换战技
        /// </summary>
        /// <param name="character"></param>
        protected SwitchCombatTalentSkill(Character? character = null) : base(SkillType.Skill, character)
        {
            Effects.Add(new SwitchCombatTalentEffect());
        }
    }
}
