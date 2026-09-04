using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.EffectResult;
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
        /// 单/多角色生效中的特效并集，按 <see cref="Effect.Priority"/> 降序<para/>
        /// </summary>
        private static Effect[] EffectsOf(params Character[] characters)
        {
            return [.. characters.SelectMany(c => c.Effects.Where(e => e.IsInEffect)).OrderByDescending(e => e.Priority).Distinct()];
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
        /// 触发 AlterActionTypeBeforeAction：各特效通过回读对象声明行动/覆盖，框架维护 ctx 概率与可用性最新值；
        /// 存在强制行动时短路并返回 (行动类型, 是否强制)
        /// </summary>
        private (CharacterActionType Type, bool Force) TriggerAlterActionTypeBeforeAction(Character character, DecisionContext ctx)
        {
            CharacterActionType actionType = CharacterActionType.None;
            bool forceAction = false;
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterActionTypeBeforeAction), e =>
                {
                    AlterActionTypeResult result = e.AlterActionTypeBeforeAction(ctx);
                    // 应用覆盖到 ctx（internal set：仅框架可写），后续特效可读到最新值
                    if (result.CanUseItem.HasValue) ctx.CanUseItem = result.CanUseItem.Value;
                    if (result.CanCastSkill.HasValue) ctx.CanCastSkill = result.CanCastSkill.Value;
                    if (result.PUseItem.HasValue) ctx.PUseItem = result.PUseItem.Value;
                    if (result.PCastSkill.HasValue) ctx.PCastSkill = result.PCastSkill.Value;
                    if (result.PNormalAttack.HasValue) ctx.PNormalAttack = result.PNormalAttack.Value;
                    if (result.ForceAction && result.ActionType != CharacterActionType.None)
                    {
                        forceAction = true;
                        actionType = result.ActionType;
                    }
                }, character);
                if (forceAction)
                {
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
        /// 触发 AlterHardnessTimeAfterNormalAttack：按回读对象维护 ctx.BaseHardnessTime / IsCheckProtected 最新值
        /// </summary>
        private void TriggerAlterHardnessTimeAfterNormalAttack(Character character, HardnessContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterHardnessTimeAfterNormalAttack), e =>
                {
                    ApplyAlterHardnessTimeResult(ctx, e.AlterHardnessTimeAfterNormalAttack(ctx));
                }, character);
            }
        }

        /// <summary>
        /// 将单条 <see cref="AlterHardnessTimeResult"/> 应用到硬直上下文（保持原链式执行语义）
        /// </summary>
        private static void ApplyAlterHardnessTimeResult(HardnessContext ctx, AlterHardnessTimeResult result)
        {
            if (result.ClearHardnessTime)
            {
                ctx.BaseHardnessTime = 0;
                ctx.IsCheckProtected = false;
                return;
            }
            ctx.BaseHardnessTime *= 1 + result.Factor;
            if (result.OverrideCheckProtected.HasValue)
            {
                ctx.IsCheckProtected = result.OverrideCheckProtected.Value;
            }
        }

        /// <summary>
        /// 触发 AfterCharacterStartCasting（同一次吟唱共用一份上下文）
        /// </summary>
        private void TriggerAfterCharacterStartCasting(Character character, Skill skill, List<Character> targets)
        {
            SkillCastContext ctx = new(this, character) { Skill = skill, Targets = targets };
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterStartCasting), e => e.AfterCharacterStartCasting(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AfterCharacterCastSkill（同一次施放共用一份上下文）
        /// </summary>
        private void TriggerAfterCharacterCastSkill(Character character, Skill skill, List<Character> targets)
        {
            SkillCastContext ctx = new(this, character) { Skill = skill, Targets = targets };
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterCastSkill), e => e.AfterCharacterCastSkill(ctx), character);
            }
        }

        /// <summary>
        /// 触发 AlterHardnessTimeAfterCastSkill：按回读对象维护 ctx.BaseHardnessTime / IsCheckProtected 最新值
        /// </summary>
        private void TriggerAlterHardnessTimeAfterCastSkill(Character character, HardnessContext ctx)
        {
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AlterHardnessTimeAfterCastSkill), e =>
                {
                    ApplyAlterHardnessTimeResult(ctx, e.AlterHardnessTimeAfterCastSkill(ctx));
                }, character);
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
        /// 触发 OnCharacterDecisionCompleted（同一次决策共用一份上下文）
        /// </summary>
        private void TriggerOnCharacterDecisionCompleted(Character character, DecisionPoints dp)
        {
            ActionContext ctx = new(this, character, dp);
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.OnCharacterDecisionCompleted), e => e.OnCharacterDecisionCompleted(ctx), character);
            }
        }

        #endregion

        #region 伤害结算钩子包装

        /// <summary>
        /// 触发 AlterActualDamageAfterCalculation：DamageDelta(SUM) 逐特效记入 ctx.TotalDamageBonus，IsEvaded(OR) 改写返回结果
        /// </summary>
        private DamageResult TriggerAlterActualDamageAfterCalculation(Character actor, Character enemy, DamageContext ctx, DamageResult damageResult)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.AlterActualDamageAfterCalculation), e =>
                {
                    AlterActualDamageResult result = e.AlterActualDamageAfterCalculation(ctx);
                    if (result.DamageDelta != 0) ctx.TotalDamageBonus[e] = result.DamageDelta;
                    if (result.IsEvaded)
                    {
                        damageResult = DamageResult.Evaded;
                    }
                }, actor, enemy);
            }
            return damageResult;
        }

        /// <summary>
        /// 触发 BeforeApplyTrueDamage（共用本次结算的 <paramref name="ctx"/>），NullifyDamage(OR) 聚合：任一化解则伤害结果改为闪避
        /// </summary>
        private DamageResult TriggerBeforeApplyTrueDamage(Character actor, Character enemy, DamageContext ctx, DamageResult damageResult)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeApplyTrueDamage), e =>
                {
                    BeforeApplyTrueDamageResult result = e.BeforeApplyTrueDamage(ctx);
                    if (result.NullifyDamage)
                    {
                        damageResult = DamageResult.Evaded;
                    }
                }, actor, enemy);
            }
            return damageResult;
        }

        /// <summary>
        /// 触发 OnDamageImmuneCheck（共用本次结算的 <paramref name="ctx"/>），IgnoreDamageImmunity(OR) 聚合：任一无视则免疫不生效
        /// </summary>
        private bool TriggerOnDamageImmuneCheck(Character actor, Character enemy, DamageContext ctx)
        {
            bool ignore = false;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnDamageImmuneCheck), e =>
                {
                    OnDamageImmuneCheckResult result = e.OnDamageImmuneCheck(ctx);
                    if (result.IgnoreDamageImmunity)
                    {
                        ignore = true;
                    }
                }, actor, enemy);
            }
            return ignore;
        }

        /// <summary>
        /// 触发 OnShieldBroken（同一次破碎共用 <paramref name="ctx"/>），NullifyRemainingDamage(OR) 聚合：任一特效化解则剩余伤害归零<para/>
        /// <see cref="ShieldContext.OverFlowing"/> 在分发前同步为当前剩余伤害，使特效观察到最新值。
        /// </summary>
        private double TriggerOnShieldBroken(Character actor, Character enemy, ShieldContext ctx, double remain)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnShieldBroken), e =>
                {
                    ctx.OverFlowing = remain;
                    OnShieldBrokenResult result = e.OnShieldBroken(ctx);
                    if (result.NullifyRemainingDamage)
                    {
                        WriteLine($"[ {(enemy.Effects.Contains(e) ? enemy : actor)} ] 因护盾破碎而发动了 [ {e.Skill.Name} ]，化解了本次伤害！");
                        remain = 0;
                    }
                }, enemy, actor);
            }
            return remain;
        }

        /// <summary>
        /// 触发 OnShieldNeutralizeDamage（同一次化解共用 <paramref name="ctx"/>）
        /// </summary>
        private void TriggerOnShieldNeutralizeDamage(Character actor, Character enemy, ShieldContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnShieldNeutralizeDamage), e => e.OnShieldNeutralizeDamage(ctx), enemy, actor);
            }
        }

        /// <summary>
        /// 触发 OnApplyDamage：OriginalMessage(覆盖后者胜) 应用到 ctx，供主线回读
        /// </summary>
        private void TriggerOnApplyDamage(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnApplyDamage), e =>
                {
                    OnApplyDamageResult result = e.OnApplyDamage(ctx);
                    if (result.OriginalMessage != null) ctx.OriginalMessage = result.OriginalMessage;
                }, enemy, actor);
            }
        }

        /// <summary>
        /// 触发 BeforeLifesteal，CancelLifesteal(OR) 聚合：任一特效取消则本次生命偷取不生效
        /// </summary>
        private bool TriggerBeforeLifesteal(Character actor, Character enemy, LifestealContext ctx)
        {
            bool allow = true;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeLifesteal), e => { if (e.BeforeLifesteal(ctx).CancelLifesteal) allow = false; }, actor, enemy);
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
        /// 触发 AlterEPAfterDamage（仅攻击者特效）：BaseEP 覆盖应用到 ctx，供主线回读
        /// </summary>
        private void TriggerAlterEPAfterDamage(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor))
            {
                FireEffect(effect, nameof(Effect.AlterEPAfterDamage), e =>
                {
                    AlterEPResult result = e.AlterEPAfterDamage(ctx);
                    if (result.BaseEP.HasValue) ctx.BaseEP = result.BaseEP.Value;
                }, actor);
            }
        }

        /// <summary>
        /// 触发 AlterEPAfterGetDamage（仅受击者特效）：BaseEP 覆盖应用到 ctx，供主线回读
        /// </summary>
        private void TriggerAlterEPAfterGetDamage(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(enemy))
            {
                FireEffect(effect, nameof(Effect.AlterEPAfterGetDamage), e =>
                {
                    AlterEPResult result = e.AlterEPAfterGetDamage(ctx);
                    if (result.BaseEP.HasValue) ctx.BaseEP = result.BaseEP.Value;
                }, enemy);
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
        /// 触发 AlterDamageTypeBeforeCalculation：覆盖值应用到 ctx（类型转换判定以 ctx 最新值为准）
        /// </summary>
        private void TriggerAlterDamageTypeBeforeCalculation(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.AlterDamageTypeBeforeCalculation), e =>
                {
                    AlterDamageTypeResult result = e.AlterDamageTypeBeforeCalculation(ctx);
                    if (result.IsNormalAttack.HasValue) ctx.IsNormalAttack = result.IsNormalAttack.Value;
                    if (result.DamageType.HasValue) ctx.DamageType = result.DamageType.Value;
                    if (result.MagicType.HasValue) ctx.MagicType = result.MagicType.Value;
                }, actor, enemy);
            }
        }

        /// <summary>
        /// 触发 AlterExpectedDamageBeforeCalculation（共用本次结算的 <paramref name="ctx"/>），加成逐特效记入 totalDamageBonus
        /// </summary>
        private void TriggerAlterExpectedDamageBeforeCalculation(Character actor, Character enemy, Effect[] effects, DamageContext ctx, Dictionary<Effect, double> totalDamageBonus)
        {
            foreach (Effect effect in effects)
            {
                FireEffect(effect, nameof(Effect.AlterExpectedDamageBeforeCalculation), e =>
                {
                    double damageBonus = e.AlterExpectedDamageBeforeCalculation(ctx);
                    if (damageBonus != 0) totalDamageBonus[e] = damageBonus;
                }, actor, enemy);
            }
        }

        /// <summary>
        /// 触发 BeforeEvadeCheck，收集回读对象并聚合：SkipEvadeCheck(OR) → 是否跳过检定；ThrowingBonusDelta(SUM) → 检定加值
        /// </summary>
        private (bool CheckEvade, double ThrowingBonus) TriggerBeforeEvadeCheck(Character actor, Character enemy, DamageContext ctx)
        {
            bool skipEvadeCheck = false;
            double throwingBonus = 0;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeEvadeCheck), e =>
                {
                    BeforeEvadeCheckResult result = e.BeforeEvadeCheck(ctx);
                    if (result.SkipEvadeCheck) skipEvadeCheck = true;
                    throwingBonus += result.ThrowingBonusDelta;
                }, actor, enemy);
            }
            return (!skipEvadeCheck, throwingBonus);
        }

        /// <summary>
        /// 触发 OnEvadedTriggered（共用本次结算的 <paramref name="ctx"/>），IgnoreEvaded(OR) 聚合：任一无视则本次闪避无效
        /// </summary>
        private bool TriggerOnEvadedTriggered(Character actor, Character enemy, DamageContext ctx)
        {
            bool isAlterEvaded = false;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnEvadedTriggered), e =>
                {
                    OnEvadedTriggeredResult result = e.OnEvadedTriggered(ctx);
                    if (result.IgnoreEvaded) isAlterEvaded = true;
                }, actor, enemy);
            }
            return isAlterEvaded;
        }

        /// <summary>
        /// 触发 BeforeCriticalCheck，收集回读对象并聚合：SkipCriticalCheck(OR) → 是否跳过检定；ThrowingBonusDelta(SUM) → 检定加值
        /// </summary>
        private (bool CheckCritical, double ThrowingBonus) TriggerBeforeCriticalCheck(Character actor, Character enemy, DamageContext ctx)
        {
            bool skipCriticalCheck = false;
            double throwingBonus = 0;
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.BeforeCriticalCheck), e =>
                {
                    BeforeCriticalCheckResult result = e.BeforeCriticalCheck(ctx);
                    if (result.SkipCriticalCheck) skipCriticalCheck = true;
                    throwingBonus += result.ThrowingBonusDelta;
                }, actor, enemy);
            }
            return (!skipCriticalCheck, throwingBonus);
        }

        /// <summary>
        /// 触发 OnCriticalDamageTriggered（共用本次结算的 <paramref name="ctx"/>）
        /// </summary>
        private void TriggerOnCriticalDamageTriggered(Character actor, Character enemy, DamageContext ctx)
        {
            foreach (Effect effect in EffectsOf(actor, enemy))
            {
                FireEffect(effect, nameof(Effect.OnCriticalDamageTriggered), e => e.OnCriticalDamageTriggered(ctx), actor, enemy);
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
            HookContext ctx = new(this, null);
            foreach (Effect effect in QueueEffects())
            {
                FireEffect(effect, nameof(Effect.OnGameStart), e => e.OnGameStart(ctx), characters);
            }
        }

        /// <summary>
        /// 触发 BeforeApplyRecoveryAtTimeLapsing（遍历全部特效，不过滤生效状态）：取消回复 OR 聚合；
        /// 各特效可覆盖 HR/MR（框架直接写入 ctx 供主线回读）
        /// </summary>
        private bool TriggerBeforeApplyRecoveryAtTimeLapsing(Character character, TimeLapseContext ctx)
        {
            bool allowRecovery = true;
            foreach (Effect effect in AllEffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.BeforeApplyRecoveryAtTimeLapsing), e =>
                {
                    BeforeApplyRecoveryResult result = e.BeforeApplyRecoveryAtTimeLapsing(ctx);
                    if (result.CancelRecovery) allowRecovery = false;
                    if (result.HROverride.HasValue) ctx.HR = result.HROverride.Value;
                    if (result.MROverride.HasValue) ctx.MR = result.MROverride.Value;
                }, character);
            }
            return allowRecovery;
        }

        /// <summary>
        /// 触发 BeforeHealToTarget：取消治疗 OR 聚合
        /// </summary>
        private bool TriggerBeforeHealToTarget(Character actor, Character target, HealContext ctx)
        {
            bool allow = true;
            foreach (Effect effect in EffectsOf(actor, target))
            {
                FireEffect(effect, nameof(Effect.BeforeHealToTarget), e =>
                {
                    if (e.BeforeHealToTarget(ctx).CancelHeal) allow = false;
                }, actor, target);
            }
            return allow;
        }

        /// <summary>
        /// 触发 AlterHealValueBeforeHealToTarget：治疗增量 SUM 聚合、复活请求 OR 聚合，追加明细到 healStrings，返回是否允许复活
        /// </summary>
        private bool TriggerAlterHealValueBeforeHealToTarget(Character actor, Character target, HealContext ctx, List<string> healStrings, bool canRespawn)
        {
            foreach (Effect effect in EffectsOf(actor, target))
            {
                FireEffect(effect, nameof(Effect.AlterHealValueBeforeHealToTarget), e =>
                {
                    AlterHealValueResult result = e.AlterHealValueBeforeHealToTarget(ctx);
                    if (result.AllowRespawn && !canRespawn)
                    {
                        canRespawn = true;
                    }
                    if (result.HealDelta != 0)
                    {
                        ctx.TotalHealBonus[e] = result.HealDelta;
                        healStrings.Add($"{(result.HealDelta > 0 ? " + " : " - ")}{Math.Abs(result.HealDelta):0.##}（{e.Name}）");
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
            ItemUseContext ctx = new(this, character, dp) { Item = item, Skill = skill, Targets = targets };
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.AfterCharacterUseItem), e => e.AfterCharacterUseItem(ctx), character);
            }
        }

        /// <summary>
        /// 触发 BeforeSelectTargetGrid（每特效独立上下文）
        /// </summary>
        private void TriggerBeforeSelectTargetGrid(Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> moveRange)
        {
            SelectionContext ctx = new(this, character) { Enemys = enemys, Teammates = teammates, Map = map, MoveRange = moveRange };
            foreach (Effect effect in EffectsOf(character))
            {
                FireEffect(effect, nameof(Effect.BeforeSelectTargetGrid), e => e.BeforeSelectTargetGrid(ctx), character);
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
        /// 触发 BeforeSkillCastWillBeInterrupted，BlockInterruption(OR) 聚合：任一特效阻止则打断不生效
        /// </summary>
        private bool TriggerBeforeSkillCastWillBeInterrupted(Character caster, Character interrupter, SkillCastContext ctx)
        {
            bool interruption = true;
            foreach (Effect effect in EffectsOf(caster, interrupter))
            {
                FireEffect(effect, nameof(Effect.BeforeSkillCastWillBeInterrupted), e =>
                {
                    if (e.BeforeSkillCastWillBeInterrupted(ctx).BlockInterruption) interruption = false;
                }, caster, interrupter);
            }
            return interruption;
        }

        /// <summary>
        /// 触发 OnSkillCastInterrupted
        /// </summary>
        private void TriggerOnSkillCastInterrupted(Character caster, Character interrupter, SkillCastContext ctx)
        {
            foreach (Effect effect in EffectsOf(caster, interrupter))
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
            ImmuneContext ctx = new(this, character) { Target = target, Skill = skill, Item = item };
            foreach (Effect effect in EffectsOf(character, target))
            {
                FireEffect(effect, nameof(Effect.OnImmuneCheck), e =>
                {
                    OnImmuneCheckResult result = e.OnImmuneCheck(ctx);
                    if (result.IgnoreImmunity) ignore = true;
                }, character, target);
            }
            return ignore;
        }

        /// <summary>
        /// 触发 OnExemptionCheck：SkipExemptionCheck(OR) 决定是否跳过豁免检定；ThrowingBonusDelta(SUM) 累加到 ctx.ThrowingBonus
        /// </summary>
        private bool TriggerOnExemptionCheck(Character[] characters, ImmuneContext ctx)
        {
            bool skipExemptionCheck = false;
            foreach (Effect effect in EffectsOf(characters))
            {
                FireEffect(effect, nameof(Effect.OnExemptionCheck), e =>
                {
                    OnExemptionCheckResult result = e.OnExemptionCheck(ctx);
                    if (result.SkipExemptionCheck) skipExemptionCheck = true;
                    if (result.ThrowingBonusDelta != 0) ctx.ThrowingBonus += result.ThrowingBonusDelta;
                }, characters);
            }
            return !skipExemptionCheck;
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
