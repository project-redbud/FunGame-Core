using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 结算上下文：结算一次职业奖励所需的全部输入
    /// </summary>
    /// <param name="character">被规划的角色</param>
    /// <param name="classRecord">职业计划中的职业记录（<see cref="CharacterClass.Classes"/> 中的副本）</param>
    /// <param name="subClass">该职业对应的流派记录，用于校验技能前置与固有被动</param>
    /// <param name="plan">所属职业规划，用于判断当前是「已习得制」还是「全量授予」</param>
    /// <param name="isFirstClass">该职业是否为角色首个职业（兼职职业不发 1 级初始分配权）</param>
    public class ClassRewardContext(Character character, Class classRecord, SubClass? subClass = null, CharacterClass? plan = null, bool isFirstClass = true)
    {
        /// <summary>
        /// 被规划的角色
        /// </summary>
        public Character Character { get; } = character;

        /// <summary>
        /// 职业记录
        /// </summary>
        public Class ClassRecord { get; } = classRecord;

        /// <summary>
        /// 对应流派记录
        /// </summary>
        public SubClass? SubClass { get; } = subClass;

        /// <summary>
        /// 游戏平衡常数
        /// </summary>
        public EquilibriumConstant Eq => Character.GameplayEquilibriumConstant;

        /// <summary>
        /// 所属职业规划（可空；用于判定职业技能挂载模式）
        /// </summary>
        public CharacterClass? Plan { get; } = plan;

        /// <summary>
        /// 该职业是否为角色首个职业（兼职职业不发 1 级初始分配权，除非平衡常数关闭该限制）
        /// </summary>
        public bool IsFirstClass { get; } = isFirstClass;

        /// <summary>
        /// 是否按「已习得制」授予职业技能（未提供 Plan 时按默认开启处理；false 时为职业池全量授予）
        /// </summary>
        public bool SkillSelectionEnabled => Plan?.SkillSelectionEnabled ?? true;
    }

    /// <summary>
    /// 一次结算的结果（供上层提示 / 事件数据使用）
    /// </summary>
    public class ClassRewardSettlementResult(bool success, string message = "")
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; } = success;

        /// <summary>
        /// 结果消息（结算器会在结算末尾写入奖励摘要）
        /// </summary>
        public string Message { get; set; } = message;

        /// <summary>
        /// 本次发放的职业技能选择权
        /// </summary>
        public int GrantedActiveSkillChoices { get; set; } = 0;

        /// <summary>
        /// 本次发放的被动选择权
        /// </summary>
        public int GrantedPassiveChoices { get; set; } = 0;

        /// <summary>
        /// 本次发放的数值提升次数
        /// </summary>
        public int GrantedNumericBoosts { get; set; } = 0;

        /// <summary>
        /// 本次是否发放了 1 级初始分配权
        /// </summary>
        public bool InitialAllocationGranted { get; set; } = false;

        /// <summary>
        /// 本次施加到角色身上的属性分配
        /// </summary>
        public ClassAttributeAllocation? GrantedAttribute { get; set; } = null;

        /// <summary>
        /// 本次提升了等级的已学技能
        /// </summary>
        public List<Skill> LeveledSkills { get; } = [];

        public static ClassRewardSettlementResult Ok(string message = "") => new(true, message);

        public static ClassRewardSettlementResult Fail(string message) => new(false, message);
    }

    /// <summary>
    /// 属性落地策略：决定一份 <see cref="ClassAttributeAllocation"/> 如何加到角色身上（以及洗点时如何扣回）
    /// <para/>核心库默认写到「初始核心属性 + 成长」；模组可替换为写到额外属性、百分比等口径
    /// </summary>
    public interface IClassAttributeApplier
    {
        /// <summary>
        /// 施加分配
        /// </summary>
        void Apply(Character character, ClassAttributeAllocation allocation);

        /// <summary>
        /// 撤销分配
        /// </summary>
        void Revoke(Character character, ClassAttributeAllocation allocation);
    }

    /// <summary>
    /// 默认属性落地：加到角色的初始核心属性（InitialSTR / InitialAGI / InitialINT）与成长（STRGrowth / AGIGrowth / INTGrowth）
    /// </summary>
    public class DefaultClassAttributeApplier : IClassAttributeApplier
    {
        /// <summary>
        /// 共享实例
        /// </summary>
        public static DefaultClassAttributeApplier Instance { get; } = new();

        /// <inheritdoc/>
        public virtual void Apply(Character character, ClassAttributeAllocation allocation)
        {
            character.InitialSTR += allocation.STR;
            character.InitialAGI += allocation.AGI;
            character.InitialINT += allocation.INT;
            character.STRGrowth += allocation.STRGrowth;
            character.AGIGrowth += allocation.AGIGrowth;
            character.INTGrowth += allocation.INTGrowth;
        }

        /// <inheritdoc/>
        public virtual void Revoke(Character character, ClassAttributeAllocation allocation)
        {
            character.InitialSTR -= allocation.STR;
            character.InitialAGI -= allocation.AGI;
            character.InitialINT -= allocation.INT;
            character.STRGrowth -= allocation.STRGrowth;
            character.AGIGrowth -= allocation.AGIGrowth;
            character.INTGrowth -= allocation.INTGrowth;
        }
    }

    /// <summary>
    /// 职业升级路线图结算器（核心扩展点）
    /// <para/>规划器只依赖本接口，模组可实现并替换整套结算口径（选择权、数值提升、技能提级规则）
    /// </summary>
    public interface IClassRewardSettler
    {
        /// <summary>
        /// 结算职业等级区间 (fromLevel, toLevel] 内的全部奖励（幂等，只补发未结算的等级）
        /// </summary>
        ClassRewardSettlementResult Settle(ClassRewardContext context, ClassRewardLedger ledger, int fromLevel, int toLevel);

        /// <summary>
        /// 消耗一次选择权习得职业池中的技能（按技能类型自动判断使用主动 / 被动选择权）
        /// </summary>
        ClassRewardSettlementResult SpendSkillChoice(ClassRewardContext context, ClassRewardLedger ledger, Skill skill);

        /// <summary>
        /// 消耗 1 级初始分配权：按额度分配核心属性，受职业与角色模板限值的交集约束
        /// </summary>
        ClassRewardSettlementResult SpendInitialAllocation(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation);

        /// <summary>
        /// 消耗一次数值提升（替代被动选择）：按额度分配核心属性，不受模板限值约束
        /// </summary>
        ClassRewardSettlementResult SpendNumericBoost(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation);

        /// <summary>
        /// 撤销该职业已发放的全部奖励（属性扣回 + 清空账本），洗点用
        /// </summary>
        ClassRewardSettlementResult Revoke(ClassRewardContext context, ClassRewardLedger ledger);

        /// <summary>
        /// 1 级初始分配额度，null 表示不由结算器提供
        /// </summary>
        ClassAttributeBudget? InitialAllocationBudget(ClassRewardContext context, ClassRewardLedger ledger);

        /// <summary>
        /// 数值提升单次额度，null 表示由上层自行决定
        /// </summary>
        ClassAttributeBudget? NumericBoostBudget(ClassRewardContext context, ClassRewardLedger ledger);
    }

    /// <summary>
    /// 默认结算器：忠实于 <see cref="ClassLevelUpReward"/> 的字段语义
    /// <para/>· 1 级发放初始分配权（额度见 <see cref="EquilibriumConstant.InitialAttributeBudget"/>，受职业与角色模板限值交集约束）
    /// <para/>· 固有被动数量仅记账（实际授予由 <see cref="CharacterClass.ApplyTo"/> 按流派门槛完成）
    /// <para/>· 选择权 / 数值提升发放为待选配额，由玩家通过规划器动作消耗
    /// <para/>· 数值提升按 <see cref="EquilibriumConstant.NumericBoostBudget"/> 发放，不受模板上下限约束
    /// <para/>· 已学职业技能按等级增量提升，魔法额外 +1，最终由技能类型上限钳制
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
                if (!context.Eq.ClassLevelUpRewards.TryGetValue(level, out ClassLevelUpReward? reward))
                {
                    ledger.SettledToLevel = level;
                    continue;
                }
                // 1 级：发放初始分配权（额度与模板限值在领取时校验，见 SpendInitialAllocation）
                // 兼职职业默认不重复发放，避免属性叠加膨胀（可由 EquilibriumConstant.InitialAllocationOnlyForFirstClass 关闭）
                bool canGrantInitial = level == 1 && (!context.Eq.InitialAllocationOnlyForFirstClass || context.IsFirstClass);
                if (canGrantInitial && !ledger.InitialAllocationAvailable)
                {
                    ledger.InitialAllocationAvailable = true;
                    result.InitialAllocationGranted = true;
                    summary.Add($"获得初始分配权（{InitialAllocationBudget(context, ledger)?.Describe() ?? "未配置额度"}）");
                }
                if (reward.InherentPassive > 0)
                {
                    ledger.GrantedInherentPassiveCount += reward.InherentPassive;
                    summary.Add($"获得流派固有被动 ×{reward.InherentPassive}");
                }
                if (reward.ActiveSkillChoices > 0)
                {
                    ledger.PendingActiveSkillChoices += reward.ActiveSkillChoices;
                    result.GrantedActiveSkillChoices += reward.ActiveSkillChoices;
                    summary.Add($"职业技能选择权 +{reward.ActiveSkillChoices}");
                }
                if (reward.PassiveChoices > 0)
                {
                    ledger.PendingPassiveChoices += reward.PassiveChoices;
                    result.GrantedPassiveChoices += reward.PassiveChoices;
                    summary.Add($"被动选择权 +{reward.PassiveChoices}");
                }
                if (reward.CanNumericBoost)
                {
                    // 数值提升替代被动选择：该档发放与被动选择权同数的次数（至少 1 次）
                    int count = Math.Max(reward.PassiveChoices, 1);
                    if (reward.NumericBoost != null)
                    {
                        // 路线图单级自带额度优先
                        ledger.NumericBoostBudget ??= reward.NumericBoost.Copy();
                    }
                    ledger.PendingNumericBoosts += count;
                    result.GrantedNumericBoosts += count;
                    summary.Add($"可用数值提升 ×{count}（可替代被动选择）");
                }
                // 已学职业技能等级提升（魔法额外 +1）；未启用选择制时职业池全量授予
                IEnumerable<Skill> learned = context.SkillSelectionEnabled
                    ? GetLearnedSkills(context.ClassRecord, ledger)
                    : AllPoolSkills(context.ClassRecord);
                foreach (Skill skill in learned.Where(s => s.IsActive))
                {
                    int delta = reward.SkillLevelUp + (skill.IsMagic ? reward.MagicExtraLevel : 0);
                    if (delta <= 0)
                    {
                        continue;
                    }
                    // Level 读写含 ExLevel，提级须先剥离额外等级，避免把突破加成折进基础等级
                    skill.Level = Math.Max(0, skill.Level - skill.ExLevel) + delta;
                    result.LeveledSkills.Add(skill);
                }
                if (reward.SkillLevelUp > 0)
                {
                    summary.Add($"已学职业技能等级 +{reward.SkillLevelUp}{(reward.MagicExtraLevel > 0 ? $"（魔法额外 +{reward.MagicExtraLevel}）" : "")}");
                }
                ledger.SettledToLevel = level;
            }
            result.Message = summary.Count == 0 ? "本档无奖励。" : string.Join("；", summary) + "。";
            return result;
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
            bool firstTime = ledger.LearnedSkillIds.Add(idName);
            if (firstTime && target.Level <= 0)
            {
                target.Level = Math.Max(1, target.Level - target.ExLevel); // 习得即 1 级
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
                return ClassRewardSettlementResult.Fail("无可用数值提升（仅 4 / 9 级档位提供，且可被被动选择替代）。");
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
            // 数值提升替代被动选择：同步抵扣一次被动选择权（若还有）
            if (ledger.PendingPassiveChoices > 0)
            {
                ledger.PendingPassiveChoices--;
            }
            ClassAttributeAllocation grant = allocation.Copy();
            AttributeApplier.Apply(context.Character, grant);
            ledger.AppliedAttribute.Add(grant);
            return ClassRewardSettlementResult.Ok($"已获得数值提升：{grant.Describe()}。");
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
