using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 【转换战斗天赋】战技的特效：释放时把角色当前生效的战斗天赋切换到目标天赋
    /// <para>挂在 <see cref="SwitchCombatTalentSkill"/> 的 Effects 上随技能释放派发</para>
    /// </summary>
    public class SwitchCombatTalentEffect : Effect
    {
        public override void OnSkillCasted(SkillCastContext ctx)
        {
            base.OnSkillCasted(ctx);
            // 只有已学天赋 ≥ 2的角色才能转换
            if (ctx.Trigger?.Class is not CharacterClass plan || !plan.HasCombatTalentSwitch)
            {
                return;
            }
            SwitchCombatTalentSkill? switchSkill = ctx.Skill as SwitchCombatTalentSkill;
            // 1. 优先按天赋指定（同一定位掌握多个天赋时的唯一精确方式）
            if (!string.IsNullOrEmpty(switchSkill?.TargetTalentId))
            {
                Skill? byTalent = plan.AllLearnedTalents.FirstOrDefault(t => t.GetIdName() == switchSkill.TargetTalentId);
                if (byTalent is not null)
                {
                    plan.SwitchCombatTalent(byTalent, out _);
                    return;
                }
            }
            // 2. 按定位指定：切到该定位尚未激活的第一个已学天赋
            if (switchSkill?.TargetRoleType is { } role && role != RoleType.None)
            {
                plan.SwitchCombatTalent(role, out _);
                return;
            }
            // 3. 未指定：切到已学序列中的下一个（循环，可能仍是同一定位）
            Skill? next = plan.NextTalentAfter(plan.CombatTalent);
            if (next is not null)
            {
                plan.SwitchCombatTalent(next, out _);
            }
        }
    }
}
