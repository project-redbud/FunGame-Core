using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.Queue
{
    /// <summary>
    /// <see cref="GamingQueue"/> 的特效钩子隐式遍历分发
    /// <para/>三层结构：FireEffect 核心不变式（赋队列 + 记录触发 + 调用钩子）、集合构建器（镜像各调用点现有的 LINQ 形状）、每钩子 Trigger 包装（含返回值聚合）
    /// </summary>
    public partial class GamingQueue
    {
        #region 特效触发核心

        /// <summary>
        /// 触发单个特效钩子的核心不变式：赋队列、记录触发、调用钩子
        /// </summary>
        private void FireEffect(Effect effect, string hookName, Action<Effect> invoke, params Character?[] owners)
        {
            effect.GamingQueue = this;
            effect.RecordEffectTriggeredIfOverridden(hookName, owners);
            invoke(effect);
        }

        /// <summary>
        /// 触发单个特效钩子（局外场景，无队列实例，保持不赋 <see cref="Effect.GamingQueue"/>）
        /// </summary>
        private static void FireEffectWithoutQueue(Effect effect, string hookName, Action<Effect> invoke, params Character?[] owners)
        {
            effect.RecordEffectTriggeredIfOverridden(hookName, owners);
            invoke(effect);
        }

        #endregion

        #region 特效集合构建

        /// <summary>
        /// 单/多角色生效中的特效并集（镜像 SelectMany 调用点形状）
        /// </summary>
        private static Effect[] EffectsOf(params Character[] characters)
        {
            return [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
        }

        /// <summary>
        /// 双角色特效 Union 并集（镜像 Union 调用点形状）
        /// </summary>
        private static Effect[] UnionEffectsOf(Character character1, Character character2)
        {
            return [.. character1.Effects.Union(character2.Effects).Distinct().Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
        }

        /// <summary>
        /// 单角色全部特效（不过滤生效状态，镜像时间流逝/回合结束调用点形状）
        /// </summary>
        private static Effect[] AllEffectsOf(Character character)
        {
            return [.. character.Effects.OrderByDescending(e => e.Priority)];
        }

        /// <summary>
        /// 队列内全部角色的特效（全队列广播）
        /// </summary>
        private Effect[] QueueEffects()
        {
            return [.. _queue.SelectMany(c => c.Effects).Where(e => e.IsInEffect).OrderByDescending(e => e.Priority)];
        }

        #endregion

        #region 回合流程钩子包装

        /// <summary>
        /// 触发 OnTurnStart
        /// </summary>
        private void TriggerOnTurnStart(Character character, TurnContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.OnTurnStart), e => e.OnTurnStart(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AlterSelectListBeforeAction
        /// </summary>
        private void TriggerAlterSelectListBeforeAction(Character character, SelectionContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterSelectListBeforeAction), e => e.AlterSelectListBeforeAction(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AlterActionTypeBeforeAction，存在强制行动时短路并返回 (行动类型, 是否强制)
        /// </summary>
        private (CharacterActionType Type, bool Force) TriggerAlterActionTypeBeforeAction(Character character, DecisionContext ctx)
        {
            CharacterActionType actionType = CharacterActionType.None;
            bool forceAction = false;
            foreach (Effect effect in EffectsOf(character))
            {
                CharacterActionType forceType = CharacterActionType.None;
                bool force = false;
                FireEffect(effect, nameof(Effect.AlterActionTypeBeforeAction), e =>
                {
                    ctx.ForceAction = false;
                    forceType = e.AlterActionTypeBeforeAction(ctx);
                    force = ctx.ForceAction;
                }, character);
                if (force && forceType != CharacterActionType.None)
                {
                    forceAction = true;
                    actionType = forceType;
                    break;
                }
            }
            return (actionType, forceAction);
        }

        /// <summary>
        /// 触发 OnCharacterActionStart
        /// </summary>
        private void TriggerOnCharacterActionStart(Character character, ActionContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.OnCharacterActionStart), e => e.OnCharacterActionStart(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AfterCharacterNormalAttack
        /// </summary>
        private void TriggerAfterCharacterNormalAttack(Character character, NormalAttackContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterNormalAttack), e => e.AfterCharacterNormalAttack(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AlterHardnessTimeAfterNormalAttack
        /// </summary>
        private void TriggerAlterHardnessTimeAfterNormalAttack(Character character, HardnessContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterHardnessTimeAfterNormalAttack), e => e.AlterHardnessTimeAfterNormalAttack(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AfterCharacterStartCasting（每特效独立上下文）
        /// </summary>
        private void TriggerAfterCharacterStartCasting(Character character, Skill skill, List<Character> targets)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterStartCasting), e => e.AfterCharacterStartCasting(new SkillCastContext(this, character) { Skill = skill, Targets = targets }), character);
            }
        }

        /// <summary>
        /// 触发 AfterCharacterCastSkill（每特效独立上下文）
        /// </summary>
        private void TriggerAfterCharacterCastSkill(Character character, Skill skill, List<Character> targets)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterCastSkill), e => e.AfterCharacterCastSkill(new SkillCastContext(this, character) { Skill = skill, Targets = targets }), character);
            }
        }

        /// <summary>
        /// 触发 AlterHardnessTimeAfterCastSkill
        /// </summary>
        private void TriggerAlterHardnessTimeAfterCastSkill(Character character, HardnessContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterHardnessTimeAfterCastSkill), e => e.AlterHardnessTimeAfterCastSkill(ctx), character);
            }
        }

        /// <summary>
        /// 触发 OnCharacterActionTaken（全队列广播）
        /// </summary>
        private void TriggerOnCharacterActionTaken(Character character, ActionContext ctx)
        {
            Character[] characters = [.. _queue.Union([character])];
            Effect[] effects = [.. characters.SelectMany(c => c.Effects).Where(e => e.IsInEffect).OrderByDescending(e => e.Priority).Distinct()];
            foreach (Effect effect in effects)
            {
                FireEffect(effect, nameof(Effect.OnCharacterActionTaken), e => e.OnCharacterActionTaken(ctx), characters);
            }
        }

        /// <summary>
        /// 触发 OnCharacterDecisionCompleted（每特效独立上下文）
        /// </summary>
        private void TriggerOnCharacterDecisionCompleted(Character character, DecisionPoints dp)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.OnCharacterDecisionCompleted), e => e.OnCharacterDecisionCompleted(new ActionContext(this, character, dp)), character);
            }
        }

        #endregion

        #region 伤害结算钩子包装

        /// <summary>
        /// 触发 AlterActualDamageAfterCalculation，加成逐特效记入 ctx.TotalDamageBonus，闪避改写返回结果
        /// </summary>
        private DamageResult TriggerAlterActualDamageAfterCalculation(Character actor, Character enemy, DamageContext ctx, DamageResult damageResult)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.AlterActualDamageAfterCalculation), e =>
                {
                    double damageBonus = e.AlterActualDamageAfterCalculation(ctx);
                    if (damageBonus != 0) ctx.TotalDamageBonus[e] = damageBonus;
                    if (ctx.IsEvaded)
                    {
                        damageResult = DamageResult.Evaded;
                    }
                }, actor, enemy);
            }
            return damageResult;
        }

        /// <summary>
        /// 触发 BeforeApplyTrueDamage（每特效独立上下文），返回 true 时伤害结果改为闪避并影响后续特效的上下文
        /// </summary>
        private DamageResult TriggerBeforeApplyTrueDamage(Character actor, Character enemy, double damage, bool isNormalAttack, DamageResult damageResult)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeApplyTrueDamage), e =>
                {
                    if (e.BeforeApplyTrueDamage(new DamageContext(this, actor, enemy) { Damage = damage, IsNormalAttack = isNormalAttack, DamageResult = damageResult }))
                    {
                        damageResult = DamageResult.Evaded;
                    }
                }, actor, enemy);
            }
            return damageResult;
        }

        /// <summary>
        /// 触发 OnDamageImmuneCheck（每特效独立上下文），返回 true 表示无视免疫
        /// </summary>
        private bool TriggerOnDamageImmuneCheck(Character actor, Character enemy, bool isNormalAttack, DamageType damageType, MagicType magicType, double damage)
        {
            bool ignore = false;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnDamageImmuneCheck), e =>
                {
                    if (!e.OnDamageImmuneCheck(new DamageContext(this, actor, enemy) { IsNormalAttack = isNormalAttack, DamageType = damageType, MagicType = magicType, Damage = damage }))
                    {
                        ignore = true;
                    }
                }, actor, enemy);
            }
            return ignore;
        }

        /// <summary>
        /// 触发 OnShieldBroken（每特效独立上下文），返回 false 的特效化解本次伤害（剩余伤害归零）
        /// </summary>
        private double TriggerOnShieldBroken(Character actor, Character enemy, Func<ShieldContext> createContext, double remain)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnShieldBroken), e =>
                {
                    if (!e.OnShieldBroken(createContext()))
                    {
                        WriteLine($"[ {(enemy.Effects.Contains(e) ? enemy : actor)} ] 因护盾破碎而发动了 [ {e.Skill.Name} ]，化解了本次伤害！");
                        remain = 0;
                    }
                }, enemy, actor);
            }
            return remain;
        }

        /// <summary>
        /// 触发 OnShieldNeutralizeDamage（每特效独立上下文）
        /// </summary>
        private void TriggerOnShieldNeutralizeDamage(Character actor, Character enemy, Func<ShieldContext> createContext)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnShieldNeutralizeDamage), e => e.OnShieldNeutralizeDamage(createContext()), enemy, actor);
            }
        }

        /// <summary>
        /// 触发 OnApplyDamage
        /// </summary>
        private void TriggerOnApplyDamage(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnApplyDamage), e => e.OnApplyDamage(ctx), enemy, actor);
            }
        }

        /// <summary>
        /// 触发 BeforeLifesteal，返回 false 的特效取消生命偷取
        /// </summary>
        private bool TriggerBeforeLifesteal(Character actor, Character enemy, LifestealContext ctx)
        {
            bool allow = true;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeLifesteal), e => { if (!e.BeforeLifesteal(ctx)) allow = false; }, actor, enemy);
            }
            return allow;
        }

        /// <summary>
        /// 触发 AfterLifesteal
        /// </summary>
        private void TriggerAfterLifesteal(Character actor, Character enemy, LifestealContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.AfterLifesteal), e => e.AfterLifesteal(ctx), actor, enemy);
            }
        }

        /// <summary>
        /// 触发 AlterEPAfterDamage（仅攻击者特效）
        /// </summary>
        private void TriggerAlterEPAfterDamage(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor))
            {
                FireEffect(effect, nameof(Effect.AlterEPAfterDamage), e => e.AlterEPAfterDamage(ctx), actor);
            }
        }

        /// <summary>
        /// 触发 AlterEPAfterGetDamage（仅受击者特效）
        /// </summary>
        private void TriggerAlterEPAfterGetDamage(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(enemy))
            {
                FireEffect(effect, nameof(Effect.AlterEPAfterGetDamage), e => e.AlterEPAfterGetDamage(ctx), enemy);
            }
        }

        /// <summary>
        /// 触发 AfterDamageCalculation
        /// </summary>
        private void TriggerAfterDamageCalculation(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.AfterDamageCalculation), e => e.AfterDamageCalculation(ctx), actor, enemy);
            }
        }

        #endregion

        #region 伤害计算钩子包装

        /// <summary>
        /// 触发 AlterDamageTypeBeforeCalculation
        /// </summary>
        private void TriggerAlterDamageTypeBeforeCalculation(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.AlterDamageTypeBeforeCalculation), e => e.AlterDamageTypeBeforeCalculation(ctx), actor, enemy);
            }
        }

        /// <summary>
        /// 触发 AlterExpectedDamageBeforeCalculation（每特效独立上下文），加成逐特效记入 totalDamageBonus
        /// </summary>
        private void TriggerAlterExpectedDamageBeforeCalculation(Character actor, Character enemy, Effect[] effects, double expectedDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            foreach (Effect effect in effects)
            {
                FireEffect(effect, nameof(Effect.AlterExpectedDamageBeforeCalculation), e =>
                {
                    double damageBonus = e.AlterExpectedDamageBeforeCalculation(new DamageContext(this, actor, enemy)
                    {
                        Damage = expectedDamage,
                        IsNormalAttack = isNormalAttack,
                        DamageType = damageType,
                        MagicType = magicType,
                        TotalDamageBonus = totalDamageBonus
                    });
                    if (damageBonus != 0) totalDamageBonus[e] = damageBonus;
                }, actor, enemy);
            }
        }

        /// <summary>
        /// 触发 BeforeEvadeCheck，返回 false 的特效禁用闪避检定
        /// </summary>
        private bool TriggerBeforeEvadeCheck(Character actor, Character enemy, DamageContext ctx)
        {
            bool checkEvade = true;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeEvadeCheck), e => { if (!e.BeforeEvadeCheck(ctx)) checkEvade = false; }, actor, enemy);
            }
            return checkEvade;
        }

        /// <summary>
        /// 触发 OnEvadedTriggered（每特效独立上下文），返回 true 表示无视本次闪避
        /// </summary>
        private bool TriggerOnEvadedTriggered(Character actor, Character enemy, double dice)
        {
            bool isAlterEvaded = false;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnEvadedTriggered), e => { if (e.OnEvadedTriggered(new DamageContext(this, actor, enemy) { Dice = dice })) isAlterEvaded = true; }, actor, enemy);
            }
            return isAlterEvaded;
        }

        /// <summary>
        /// 触发 BeforeCriticalCheck，返回 false 的特效禁用暴击检定
        /// </summary>
        private bool TriggerBeforeCriticalCheck(Character actor, Character enemy, DamageContext ctx)
        {
            bool checkCritical = true;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeCriticalCheck), e => { if (!e.BeforeCriticalCheck(ctx)) checkCritical = false; }, actor, enemy);
            }
            return checkCritical;
        }

        /// <summary>
        /// 触发 OnCriticalDamageTriggered（每特效独立上下文）
        /// </summary>
        private void TriggerOnCriticalDamageTriggered(Character actor, Character enemy, double dice)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnCriticalDamageTriggered), e => e.OnCriticalDamageTriggered(new DamageContext(this, actor, enemy) { Dice = dice }), actor, enemy);
            }
        }

        #endregion

        #region 其他流程钩子包装

        /// <summary>
        /// 触发 OnGameStart（全队列广播，每特效独立上下文）
        /// </summary>
        private void TriggerOnGameStart()
        {
            Character[] characters = [.. _queue];
            foreach (Effect effect in QueueEffects())
            {
                FireEffect(effect, nameof(Effect.OnGameStart), e => e.OnGameStart(new HookContext(this, null)), characters);
            }
        }

        /// <summary>
        /// 触发 BeforeApplyRecoveryAtTimeLapsing（遍历全部特效，不过滤生效状态），返回 false 的特效阻止回复
        /// </summary>
        private bool TriggerBeforeApplyRecoveryAtTimeLapsing(Character character, TimeLapseContext ctx)
        {
            bool allowRecovery = true;
            foreach (Effect effect in AllEffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.BeforeApplyRecoveryAtTimeLapsing), e => { if (!e.BeforeApplyRecoveryAtTimeLapsing(ctx)) allowRecovery = false; }, character);
            }
            return allowRecovery;
        }

        /// <summary>
        /// 触发 BeforeHealToTarget，返回 false 的特效取消治疗
        /// </summary>
        private bool TriggerBeforeHealToTarget(Character actor, Character target, HealContext ctx)
        {
            bool allow = true;
            foreach (Effect effect in UnionEffectsOf(actor, target))
            {
                FireEffect(effect, nameof(Effect.BeforeHealToTarget), e => { if (!e.BeforeHealToTarget(ctx)) allow = false; }, actor, target);
            }
            return allow;
        }

        /// <summary>
        /// 触发 AlterHealValueBeforeHealToTarget，加成逐特效记入 ctx.TotalHealBonus 并追加明细，返回是否允许复活
        /// </summary>
        private bool TriggerAlterHealValueBeforeHealToTarget(Character actor, Character target, HealContext ctx, List<string> healStrings, bool canRespawn)
        {
            foreach (Effect effect in UnionEffectsOf(actor, target))
            {
                FireEffect(effect, nameof(Effect.AlterHealValueBeforeHealToTarget), e =>
                {
                    ctx.CanRespawn = false;
                    double healBonus = e.AlterHealValueBeforeHealToTarget(ctx);
                    if (ctx.CanRespawn && !canRespawn)
                    {
                        canRespawn = true;
                    }
                    if (healBonus != 0)
                    {
                        ctx.TotalHealBonus[e] = healBonus;
                        healStrings.Add($"{(healBonus > 0 ? " + " : " - ")}{Math.Abs(healBonus):0.##}（{e.Name}）");
                    }
                }, actor, target);
            }
            return canRespawn;
        }

        /// <summary>
        /// 触发 AfterDeathCalculation（全队列 + 击杀者广播）
        /// </summary>
        private void TriggerAfterDeathCalculation(Character killer, DeathContext ctx)
        {
            Character[] characters = [.. _queue.Union([killer])];
            Effect[] effects = [.. _queue.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).Union(killer.Effects).Distinct().OrderByDescending(e => e.Priority)];
            foreach (Effect effect in effects)
            {
                FireEffect(effect, nameof(Effect.AfterDeathCalculation), e => e.AfterDeathCalculation(ctx), characters);
            }
        }

        /// <summary>
        /// 触发 AfterCharacterUseItem（每特效独立上下文）
        /// </summary>
        private void TriggerAfterCharacterUseItem(Character character, DecisionPoints dp, Item item, Skill skill, List<Character> targets)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterUseItem), e => e.AfterCharacterUseItem(new ItemUseContext(this, character, dp) { Item = item, Skill = skill, Targets = targets }), character);
            }
        }

        /// <summary>
        /// 触发 BeforeSelectTargetGrid（每特效独立上下文）
        /// </summary>
        private void TriggerBeforeSelectTargetGrid(Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.BeforeSelectTargetGrid), e => e.BeforeSelectTargetGrid(new SelectionContext(this, character) { Enemys = enemys, Teammates = teammates, Map = map, MoveRange = moveRange }), character);
            }
        }

        /// <summary>
        /// 触发 AlterSelectListBeforeSelection
        /// </summary>
        private void TriggerAlterSelectListBeforeSelection(Character character, SelectionContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterSelectListBeforeSelection), e => e.AlterSelectListBeforeSelection(ctx), character);
            }
        }

        /// <summary>
        /// 触发 BeforeSkillCastWillBeInterrupted，返回 false 的特效阻止打断
        /// </summary>
        private bool TriggerBeforeSkillCastWillBeInterrupted(Character caster, Character interrupter, SkillCastContext ctx)
        {
            bool interruption = true;
            foreach (Effect effect in UnionEffectsOf(caster, interrupter))
            {
                FireEffect(effect, nameof(Effect.BeforeSkillCastWillBeInterrupted), e => { if (!e.BeforeSkillCastWillBeInterrupted(ctx)) interruption = false; }, caster, interrupter);
            }
            return interruption;
        }

        /// <summary>
        /// 触发 OnSkillCastInterrupted
        /// </summary>
        private void TriggerOnSkillCastInterrupted(Character caster, Character interrupter, SkillCastContext ctx)
        {
            foreach (Effect effect in UnionEffectsOf(caster, interrupter))
            {
                FireEffect(effect, nameof(Effect.OnSkillCastInterrupted), e => e.OnSkillCastInterrupted(ctx), caster, interrupter);
            }
        }

        /// <summary>
        /// 触发 OnImmuneCheck（每特效独立上下文），返回 true 表示无视免疫
        /// </summary>
        private bool TriggerOnImmuneCheck(Character character, Character target, Skill skill, Item? item)
        {
            bool ignore = false;
            foreach (Effect effect in EffectsOf(character, target))
            {
                FireEffect(effect, nameof(Effect.OnImmuneCheck), e => { if (!e.OnImmuneCheck(new ImmuneContext(this, character) { Target = target, Skill = skill, Item = item })) ignore = true; }, character, target);
            }
            return ignore;
        }

        /// <summary>
        /// 触发 OnExemptionCheck，返回 false 的特效跳过豁免检定
        /// </summary>
        private bool TriggerOnExemptionCheck(Character[] characters, ImmuneContext ctx)
        {
            bool checkExempted = true;
            foreach (Effect effect in EffectsOf(characters))
            {
                FireEffect(effect, nameof(Effect.OnExemptionCheck), e => { if (!e.OnExemptionCheck(ctx)) checkExempted = false; }, characters);
            }
            return checkExempted;
        }

        /// <summary>
        /// 触发 OnCharacterInquiry
        /// </summary>
        private void TriggerOnCharacterInquiry(Character character, InquiryContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.OnCharacterInquiry), e => e.OnCharacterInquiry(ctx), character);
            }
        }

        #endregion
    }
}
