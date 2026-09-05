using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 【转换战斗天赋】战技的特效：释放时把角色当前生效的战斗天赋切换到目标天赋
    /// <para>挂在 <see cref="SwitchCombatTalentSkill"/> 的 Effects 上随技能释放派发。</para>
    /// </summary>
    public class SwitchCombatTalentEffect : Effect
    {
        public override void OnSkillCasted(SkillCastContext ctx)
        {
            base.OnSkillCasted(ctx);
            // 只有具备次要定位（已学天赋 ≥ 2）的角色才能转换
            if (ctx.Trigger?.Class is not CharacterClass plan || !plan.HasCombatTalentSwitch)
            {
                return;
            }
            // 目标定位：技能显式指定；未指定时自动选当前激活之外的第一个已学天赋
            RoleType? specified = (ctx.Skill as SwitchCombatTalentSkill)?.TargetRoleType;
            RoleType target = specified is { } role && role != RoleType.None
                ? role
                : plan.LearnedCombatTalents.FirstOrDefault(kv => !ReferenceEquals(kv.Value, plan.CombatTalent)).Key;
            if (target == RoleType.None)
            {
                return; // 无可切换的天赋
            }
            plan.SwitchCombatTalent(target, out _);
        }
    }
}
