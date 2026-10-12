using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Api
{
    /// <summary>
    /// 默认结算器：忠实于 <see cref="ClassLevelUpReward"/> 的字段语义
    /// <para/>· 1 级发放初始分配权（额度见 <see cref="EquilibriumConstant.InitialAttributeBudget"/>，受职业与角色模板限值交集约束）
    /// <para/>· 固有被动数量仅记账（实际授予由 <see cref="CharacterClass.ApplyTo"/> 按流派门槛完成）
    /// <para/>· 选择权 / 数值提升发放为待选配额，由玩家通过规划器动作消耗
    /// <para/>· 数值提升按 <see cref="EquilibriumConstant.NumericBoostBudget"/> 发放，不受模板上下限约束；路线图单级自带额度优先，且随等级升降一并重算
    /// <para/>· 职业池中的主动技能按「绝对水位」重算（1 + 该职业等级区间的提级累计；未习得的池技能一并提升，之后习得即继承当前水位），魔法额外 +1，最终由技能类型上限钳制
    /// <para/>· <see cref="Reconcile"/> 提供可升可降的绝对对齐（草稿态调级用）；下调要回收的配额若已被使用则整体失败，不做部分回退
    /// </summary>
    public class DefaultClassRewardSettler : IClassRewardSettler
    {
        /// <summary>
        /// 共享实例
        /// </summary>
        public static DefaultClassRewardSettler Instance { get; } = new();

        /// <summary>
        /// 属性落地策略，可替换
        /// </summary>
        public IClassAttributeApplier AttributeApplier { get; set; } = DefaultClassAttributeApplier.Instance;

        /// <inheritdoc/>
        public virtual ClassRewardSettlementResult Settle(ClassRewardContext context, ClassRewardLedger ledger, int fromLevel, int toLevel)
        {
            if (toLevel <= fromLevel)
            {
                return ClassRewardSettlementResult.Ok("无需结算。");
            }
            if (toLevel > context.Eq.MaxClassLevel)
            {
                return ClassRewardSettlementResult.Fail($"职业等级 {toLevel} 超过上限 {context.Eq.MaxClassLevel}。");
            }
            ClassRewardSettlementResult result = ClassRewardSettlementResult.Ok();
            List<string> summary = [];
            for (int level = fromLevel + 1; level <= toLevel; level++)
            {
                GrantLevel(context, ledger, level, 1, result, summary);
                ledger.SettledToLevel = level;
            }
            // 池技能等级按「绝对水位」重算一次（幂等）：写成 1 + (1, toLevel] 的提级累计，
            // 这样上下调整都自洽，也不会因为技能等级上限钳制而丢失信息
            AppendPoolSkillSummary(toLevel, ApplyPoolSkillLevelsAt(context, toLevel, result), summary);
            ledger.NumericBoostBudget = ResolveNumericBoostBudget(context, toLevel);
            result.Message = summary.Count == 0 ? "本档无奖励。" : string.Join("；", summary) + "。";
            return result;
        }

        /// <inheritdoc/>
        public virtual ClassRewardSettlementResult Reconcile(ClassRewardContext context, ClassRewardLedger ledger, int targetLevel)
        {
            if (targetLevel < 1 || targetLevel > context.Eq.MaxClassLevel)
            {
                return ClassRewardSettlementResult.Fail($"目标职业等级必须在 1–{context.Eq.MaxClassLevel} 之间（当前 {targetLevel}）。");
            }
            if (targetLevel == ledger.SettledToLevel)
            {
                return ClassRewardSettlementResult.Ok("等级未变化，无需对齐。");
            }
            // 上调：复用结算（增量发放）
            if (targetLevel > ledger.SettledToLevel)
            {
                return Settle(context, ledger, ledger.SettledToLevel, targetLevel);
            }
            // 下调：先整体校验可回收量，全部满足才动手，避免部分回退留下不一致状态
            (int Active, int Passive, int Numeric, int _) = SumGrants(context, targetLevel, ledger.SettledToLevel);
            if (ledger.PendingActiveSkillChoices < Active || ledger.PendingPassiveChoices < Passive || ledger.PendingNumericBoosts < Numeric)
            {
                return ClassRewardSettlementResult.Fail($"从 {ledger.SettledToLevel} 级下调到 {targetLevel} 级需回收 职业技能选择权 ×{Active} / 被动选择权 ×{Passive} / 数值提升 ×{Numeric}，"
                    + "但其中一部分已被使用（已学技能与已分配属性无法自动追回）。请改用洗点。");
            }
            ClassRewardSettlementResult result = ClassRewardSettlementResult.Ok();
            List<string> summary = [];
            for (int level = ledger.SettledToLevel; level > targetLevel; level--)
            {
                GrantLevel(context, ledger, level, -1, result, summary);
                ledger.SettledToLevel = level - 1;
            }
            // 池技能等级绝对重算到目标水位（回退方向同样如此，避免上限钳制导致回退不准）
            AppendPoolSkillSummary(targetLevel, ApplyPoolSkillLevelsAt(context, targetLevel, result), summary);
            ledger.NumericBoostBudget = ResolveNumericBoostBudget(context, targetLevel);
            result.Message = summary.Count == 0 ? "等级已下调，该区间无奖励需要回收。" : string.Join("；", summary) + "。";
            return result;
        }

        /// <summary>
        /// 按 <paramref name="sign"/>（+1 发放 / −1 回收）处理某一职业等级的可计数奖励
        /// （职业技能选择权 / 被动选择权 / 数值提升 / 流派固有被动记录）
        /// <para/>池技能等级与数值提升额度覆盖都不在此处理，由调用方最后统一按目标等级重算
        /// （见 <see cref="ApplyPoolSkillLevelsAt"/>、<see cref="ResolveNumericBoostBudget"/>）
        /// <para/>回收方向不撤销 1 级初始分配权（等级下限为 1，该档不会被回收区间覆盖）
        /// </summary>
        /// <param name="context">结算上下文</param>
        /// <param name="ledger">奖励账本</param>
        /// <param name="level">要处理的职业等级</param>
        /// <param name="sign">+1 发放、−1 回收</param>
        /// <param name="result">累计结果</param>
        /// <param name="summary">摘要文案</param>
        private void GrantLevel(ClassRewardContext context, ClassRewardLedger ledger, int level, int sign, ClassRewardSettlementResult result, List<string> summary)
        {
            if (!context.Eq.ClassLevelUpRewards.TryGetValue(level, out ClassLevelUpReward? reward))
            {
                return;
            }
            // 1 级：发放初始分配权（额度与模板限值在领取时校验，见 SpendInitialAllocation）
            // 兼职职业默认不重复发放，避免属性叠加膨胀（可由 EquilibriumConstant.InitialAllocationOnlyForFirstClass 关闭）
            bool canGrantInitial = level == 1 && (!context.Eq.InitialAllocationOnlyForFirstClass || context.IsFirstClass);
            if (sign > 0 && canGrantInitial && !ledger.InitialAllocationAvailable)
            {
                ledger.InitialAllocationAvailable = true;
                result.InitialAllocationGranted = true;
                summary.Add($"获得初始分配权（{InitialAllocationBudget(context, ledger)?.Describe() ?? "未配置额度"}）");
            }
            if (reward.InherentPassive > 0)
            {
                ledger.GrantedInherentPassiveCount = Math.Max(0, ledger.GrantedInherentPassiveCount + sign * reward.InherentPassive);
                summary.Add(sign > 0 ? $"获得流派固有被动 ×{reward.InherentPassive}" : $"回收流派固有被动记录 ×{reward.InherentPassive}");
            }
            if (reward.ActiveSkillChoices > 0)
            {
                ledger.PendingActiveSkillChoices += sign * reward.ActiveSkillChoices;
                result.GrantedActiveSkillChoices += sign * reward.ActiveSkillChoices;
                summary.Add($"职业技能选择权 {(sign > 0 ? "+" : "-")}{reward.ActiveSkillChoices}");
            }
            if (reward.PassiveChoices > 0)
            {
                ledger.PendingPassiveChoices += sign * reward.PassiveChoices;
                result.GrantedPassiveChoices += sign * reward.PassiveChoices;
                summary.Add($"被动选择权 {(sign > 0 ? "+" : "-")}{reward.PassiveChoices}");
            }
            if (reward.CanNumericBoost)
            {
                // 数值提升替代被动选择：该档发放与被动选择权同数的次数（至少 1 次）
                int count = Math.Max(reward.PassiveChoices, 1);
                ledger.PendingNumericBoosts += sign * count;
                result.GrantedNumericBoosts += sign * count;
                summary.Add(sign > 0 ? $"可用数值提升 ×{count}（可替代被动选择）" : $"回收数值提升次数 ×{count}");
            }
            // 池技能等级不在这里逐级加减：由调用方在最后统一做一次「绝对重算」
            // （见 ApplyPoolSkillLevelsAt），避免技能等级上限钳制后回退不准、以及多次升降重复叠加
        }

        /// <summary>
        /// 取「已结算等级区间内最高一档」的路线图自带数值提升额度
        /// （没有自带额度则为 null，领取时回落到 <see cref="EquilibriumConstant.NumericBoostBudget"/>）
        /// <para/>随等级升降一并重算：下调到不含该档的等级时额度覆盖会被回收
        /// </summary>
        private static ClassAttributeBudget? ResolveNumericBoostBudget(ClassRewardContext context, int settledToLevel)
        {
            ClassAttributeBudget? budget = null;
            for (int level = 1; level <= settledToLevel; level++)
            {
                if (context.Eq.ClassLevelUpRewards.TryGetValue(level, out ClassLevelUpReward? reward) && reward.NumericBoost != null)
                {
                    budget = reward.NumericBoost.Copy();
                }
            }
            return budget;
        }

        /// <summary>
        /// 追加一条「职业池主动技能已重算到 N 级」的摘要（列出去重后的实际生效等级）
        /// </summary>
        private static void AppendPoolSkillSummary(int level, List<int> appliedLevels, List<string> summary)
        {
            if (appliedLevels.Count > 0)
            {
                summary.Add($"职业池主动技能已重算到 {level} 级（实际等级 {string.Join(" / ", appliedLevels)}）");
            }
        }

        /// <summary>
        /// 把职业池中的主动技能等级「绝对重算」到 <paramref name="level"/> 对应的水位
        /// <para/>作用范围恒为整个职业池（未习得的也一起），与已习得制 / 全量授予无关；
        /// 之后习得的技能直接继承该水位（<see cref="RoadmapSkillLevel"/>）
        /// <para/>幂等：结果只取决于 <paramref name="level"/>，与之前升降过几次无关；
        /// 写入的是基础等级，突破加成 <see cref="Skill.ExLevel"/> 单独保留
        /// </summary>
        /// <param name="context">结算上下文</param>
        /// <param name="level">目标职业等级</param>
        /// <param name="result">累计结果（记录本次被提升的技能）</param>
        /// <returns>池内主动技能实际生效的基础等级（去重升序，已含技能类型上限钳制）</returns>
        private static List<int> ApplyPoolSkillLevelsAt(ClassRewardContext context, int level, ClassRewardSettlementResult result)
        {
            HashSet<int> applied = [];
            foreach (Skill skill in AllPoolSkills(context.ClassRecord).Where(s => s.IsActive))
            {
                int target = RoadmapSkillLevel(context, level, skill);
                int current = Math.Max(0, skill.Level - skill.ExLevel);
                if (current != target)
                {
                    skill.Level = target; // setter 按技能类型上限钳制；Level 读写含 ExLevel，故只写基础等级
                    if (target > current)
                    {
                        result.LeveledSkills.Add(skill);
                    }
                }
                // 回读实际生效的基础等级（钳制后），避免摘要报出超出上限的目标值
                applied.Add(Math.Max(0, skill.Level - skill.ExLevel));
            }
            return [.. applied.OrderBy(v => v)];
        }

        /// <summary>
        /// 汇总 (fromLevel, toLevel] 区间内路线图将要发放 / 回收的配额总数
        /// </summary>
        private static (int Active, int Passive, int Numeric, int Inherent) SumGrants(ClassRewardContext context, int fromLevel, int toLevel)
        {
            int active = 0;
            int passive = 0;
            int numeric = 0;
            int inherent = 0;
            for (int level = fromLevel + 1; level <= toLevel; level++)
            {
                if (!context.Eq.ClassLevelUpRewards.TryGetValue(level, out ClassLevelUpReward? reward))
                {
                    continue;
                }
                active += reward.ActiveSkillChoices;
                passive += reward.PassiveChoices;
                numeric += reward.CanNumericBoost ? Math.Max(reward.PassiveChoices, 1) : 0;
                inherent += reward.InherentPassive;
            }
            return (active, passive, numeric, inherent);
        }

        /// <inheritdoc/>
        public virtual ClassRewardSettlementResult SpendSkillChoice(ClassRewardContext context, ClassRewardLedger ledger, Skill skill)
        {
            if (skill is null)
            {
                return ClassRewardSettlementResult.Fail("技能不能为空。");
            }
            Skill? target = FindInPool(context.ClassRecord, skill);
            if (target is null)
            {
                return ClassRewardSettlementResult.Fail($"技能【{skill.Name}】不在职业【{context.ClassRecord.Name}】的技能池中。");
            }
            bool isPassive = !target.IsActive;
            if (isPassive ? ledger.PendingPassiveChoices < 1 : ledger.PendingActiveSkillChoices < 1)
            {
                return ClassRewardSettlementResult.Fail($"剩余{(isPassive ? "被动" : "职业技能")}选择权不足。");
            }
            if (!CheckRequirement(context, target, out string? error))
            {
                return ClassRewardSettlementResult.Fail(error ?? "不满足技能前置条件。");
            }
            if (isPassive)
            {
                ledger.PendingPassiveChoices--;
            }
            else
            {
                ledger.PendingActiveSkillChoices--;
            }
            string idName = target.GetIdName();
            ledger.LearnedSkillIds.Add(idName);
            // 习得即 1 级，并自动补到当前路线图水位：不区分该技能此前是否已随池提级，
            // 一律按已结算等级区间累计的提级量取齐（幂等，只补不降），保证后学的技能与先学的一样高
            int water = RoadmapSkillLevel(context, ledger.SettledToLevel, target);
            if (Math.Max(0, target.Level - target.ExLevel) < water)
            {
                target.Level = water; // 写入基础等级（setter 按技能类型上限钳制），ExLevel 单独保留
            }
            target.Source = SkillSource.Class;
            target.AddSkillToCharacter(context.Character);
            return ClassRewardSettlementResult.Ok($"已习得{(isPassive ? "被动" : "职业技能")}【{target.Name}】。");
        }

        /// <inheritdoc/>
        public virtual ClassRewardSettlementResult SpendInitialAllocation(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation)
        {
            if (!ledger.InitialAllocationAvailable)
            {
                return ClassRewardSettlementResult.Fail("当前没有可用的 1 级初始分配权（每个职业仅在 1 级发放一次）。");
            }
            ClassAttributeBudget? budget = InitialAllocationBudget(context, ledger);
            if (budget is null)
            {
                return ClassRewardSettlementResult.Fail("未配置初始分配额度（EquilibriumConstant.InitialAttributeBudget）。");
            }
            ClassAttributeLimit? limit = ResolveInitialLimit(context);
            if (!budget.Check(allocation, limit, out string? error))
            {
                return ClassRewardSettlementResult.Fail(error ?? "初始分配不合法。");
            }
            ledger.InitialAllocationAvailable = false;
            ClassAttributeAllocation grant = allocation.Copy();
            AttributeApplier.Apply(context.Character, grant);
            ledger.AppliedAttribute.Add(grant);
            return ClassRewardSettlementResult.Ok($"已完成初始分配：{grant.Describe()}。");
        }

        /// <inheritdoc/>
        public virtual ClassRewardSettlementResult SpendNumericBoost(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation)
        {
            if (ledger.PendingNumericBoosts < 1)
            {
                return ClassRewardSettlementResult.Fail("无可用数值提升份额（仅 4 / 9 级档位按 max(该档被动选择权, 1) 发放）。");
            }
            // 严格互斥：4 / 9 级发放的是同一份「被动或数值提升」份额，两者只能取其一
            // 否则先学满被动仍可再兑换数值提升，等于用 3 份发 6 份
            if (ledger.PendingPassiveChoices < 1)
            {
                return ClassRewardSettlementResult.Fail("被动选择权已用尽：数值提升与被动共用 4 / 9 级同一份份额（互斥，取其一），没有份额就不能再兑换数值提升。");
            }
            ClassAttributeBudget? budget = NumericBoostBudget(context, ledger);
            if (budget is null)
            {
                return ClassRewardSettlementResult.Fail("未配置数值提升额度（EquilibriumConstant.NumericBoostBudget）。");
            }
            if (!budget.Check(allocation, null, out string? error))
            {
                return ClassRewardSettlementResult.Fail(error ?? "数值提升分配不合法。");
            }
            ledger.PendingNumericBoosts--;
            // 严格互斥：同步消耗同一份额的被动选择权
            ledger.PendingPassiveChoices--;
            ClassAttributeAllocation grant = allocation.Copy();
            AttributeApplier.Apply(context.Character, grant);
            ledger.AppliedAttribute.Add(grant);
            return ClassRewardSettlementResult.Ok($"已获得数值提升：{grant.Describe()}（剩余份额 {ledger.PendingPassiveChoices}）。");
        }

        /// <inheritdoc/>
        public virtual ClassRewardSettlementResult Revoke(ClassRewardContext context, ClassRewardLedger ledger)
        {
            if (!ledger.AppliedAttribute.IsEmpty)
            {
                AttributeApplier.Revoke(context.Character, ledger.AppliedAttribute.Copy());
            }
            foreach (Skill skill in GetLearnedSkills(context.ClassRecord, ledger))
            {
                skill.RemoveSkillFromCharacter(context.Character);
            }
            ledger.Reset();
            return ClassRewardSettlementResult.Ok("已撤销该职业的全部路线图奖励。");
        }

        /// <inheritdoc/>
        public virtual ClassAttributeBudget? InitialAllocationBudget(ClassRewardContext context, ClassRewardLedger ledger)
        {
            return context.Eq.InitialAttributeBudget?.Copy();
        }

        /// <inheritdoc/>
        public virtual ClassAttributeBudget? NumericBoostBudget(ClassRewardContext context, ClassRewardLedger ledger)
        {
            // 路线图单级自带额度优先，其次平衡常数默认
            return ledger.NumericBoostBudget?.Copy() ?? context.Eq.NumericBoostBudget?.Copy();
        }

        /// <summary>
        /// 初始分配的模板限值：职业模板与角色模板的交集
        /// </summary>
        protected virtual ClassAttributeLimit? ResolveInitialLimit(ClassRewardContext context)
        {
            ClassAttributeLimit? classLimit = context.ClassRecord.AttributeLimit;
            ClassAttributeLimit? characterLimit = context.Character.AttributeLimit;
            if (classLimit != null && characterLimit != null)
            {
                return classLimit.Intersect(characterLimit);
            }
            return classLimit ?? characterLimit;
        }

        /// <summary>
        /// 取职业记录中已习得的技能实例（按账本 IdName 匹配职业池副本）
        /// </summary>
        public static IEnumerable<Skill> GetLearnedSkills(Class classRecord, ClassRewardLedger ledger)
        {
            if (ledger.LearnedSkillIds.Count == 0)
            {
                return [];
            }
            return AllPoolSkills(classRecord).Where(s => ledger.LearnedSkillIds.Contains(s.GetIdName()));
        }

        /// <summary>
        /// 单级提级增量：战技 / 爆发技 +<see cref="ClassLevelUpReward.SkillLevelUp"/>，魔法再额外 +<see cref="ClassLevelUpReward.MagicExtraLevel"/>
        /// </summary>
        /// <param name="reward">路线图单级奖励</param>
        /// <param name="skill">目标技能</param>
        public static int SkillLevelDelta(ClassLevelUpReward reward, Skill skill)
        {
            return reward.SkillLevelUp + (skill.IsMagic ? reward.MagicExtraLevel : 0);
        }

        /// <summary>
        /// 按路线图推算某技能应达到的等级水位：习得基础 1 级 + (1, <paramref name="settledToLevel"/>] 区间内的提级累计
        /// <para/>用于「后来习得的技能自动升到当前路线图的最高等级」——不区分该技能是否曾提前学过
        /// <para/>职业被动按设定恒为 1 级，不参与提级
        /// </summary>
        /// <param name="context">结算上下文</param>
        /// <param name="settledToLevel">该职业已结算到的等级</param>
        /// <param name="skill">目标技能</param>
        public static int RoadmapSkillLevel(ClassRewardContext context, int settledToLevel, Skill skill)
        {
            const int baseLevel = 1;
            if (!skill.IsActive)
            {
                return baseLevel;
            }
            int level = baseLevel;
            for (int lv = 1; lv <= settledToLevel; lv++)
            {
                if (context.Eq.ClassLevelUpRewards.TryGetValue(lv, out ClassLevelUpReward? reward))
                {
                    level += SkillLevelDelta(reward, skill);
                }
            }
            return level;
        }

        /// <summary>
        /// 职业池全量技能（被动 / 战技 / 魔法 / 爆发技）
        /// </summary>
        public static IEnumerable<Skill> AllPoolSkills(Class classRecord)
        {
            return classRecord.PassiveSkills.Concat(classRecord.Skills).Concat(classRecord.Magics).Concat(classRecord.SuperSkills);
        }

        /// <summary>
        /// 在职业池中定位技能实例
        /// </summary>
        private static Skill? FindInPool(Class classRecord, Skill skill)
        {
            string idName = skill.GetIdName();
            return AllPoolSkills(classRecord).FirstOrDefault(s => s.GetIdName() == idName);
        }

        /// <summary>
        /// 校验技能前置：流派要求与属性要求
        /// </summary>
        private static bool CheckRequirement(ClassRewardContext context, Skill skill, out string? error)
        {
            error = null;
            if (skill.RequiredSubClass != null)
            {
                if (context.SubClass is null || context.SubClass.GetIdName() != skill.RequiredSubClass.GetIdName())
                {
                    error = $"技能【{skill.Name}】需要流派【{skill.RequiredSubClass.Name}】。";
                    return false;
                }
            }
            if (skill.RequiredAttribute != null)
            {
                double value = skill.RequiredAttribute switch
                {
                    PrimaryAttribute.STR => context.Character.STR,
                    PrimaryAttribute.AGI => context.Character.AGI,
                    PrimaryAttribute.INT => context.Character.INT,
                    _ => 0
                };
                if (value < skill.RequiredAttributeValue)
                {
                    error = $"技能【{skill.Name}】需要{CharacterSet.GetPrimaryAttributeName(skill.RequiredAttribute.Value)}达到 {skill.RequiredAttributeValue:0.##}（当前 {value:0.##}）。";
                    return false;
                }
            }
            return true;
        }
    }
}
