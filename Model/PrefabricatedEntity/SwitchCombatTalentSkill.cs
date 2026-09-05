using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 【转换战斗天赋】战技：战斗内激活另一已学天赋，并取消当前生效天赋
    /// <para>继承此类以提供具体战技（与 <see cref="CourageCommandSkill"/> 同款薄基类写法）。
    /// 模组为角色设好 <see cref="TargetRoleType"/> 后将其加入角色技能表即可；
    /// 决策点 / 战技配额 / EP / CD 走常规战技结算（默认值已匹配设定，无需改常数）。
    /// 授予前提：角色存在次要定位（<see cref="CharacterClass.HasCombatTalentSwitch"/>）。</para>
    /// </summary>
    public abstract class SwitchCombatTalentSkill : Skill
    {
        /// <summary>
        /// 目标定位：释放时切换到该定位的已学天赋；null / <see cref="RoleType.None"/> 时自动选
        /// 当前激活之外的第一个已学天赋
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
