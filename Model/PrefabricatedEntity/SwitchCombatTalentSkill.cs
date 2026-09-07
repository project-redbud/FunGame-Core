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
        /// 目标定位：释放时切换到该定位的已学天赋；null / <see cref="RoleType.None"/> 时自动选当前激活之外的第一个已学天赋
        /// </summary>
        public RoleType? TargetRoleType { get; set; } = null;

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
