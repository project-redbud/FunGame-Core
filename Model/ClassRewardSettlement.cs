using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model
{
    /// <summary>
    /// 结算上下文：结算一次职业奖励所需的全部输入
    /// </summary>
    /// <param name="character">被规划的角色</param>
    /// <param name="classRecord">职业计划中的职业记录（<see cref="CharacterClass.Classes"/> 中的副本）</param>
    /// <param name="subClass">该职业对应的流派记录，用于校验技能前置与固有被动</param>
    /// <param name="plan">所属职业规划，用于判断当前是「已习得制」还是「全量授予」</param>
    public class ClassRewardContext(Character character, Class classRecord, SubClass? subClass = null, CharacterClass? plan = null)
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
        /// 是否按「已习得制」授予职业技能（false 时为职业池全量授予）
        /// </summary>
        public bool SkillSelectionEnabled => Plan?.SkillSelectionEnabled ?? false;
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
        /// 消耗一次数值提升（替代被动选择），施加一份核心属性分配
        /// </summary>
        ClassRewardSettlementResult SpendNumericBoost(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation);

        /// <summary>
        /// 撤销该职业已发放的全部奖励（属性扣回 + 清空账本），洗点用
        /// </summary>
        ClassRewardSettlementResult Revoke(ClassRewardContext context, ClassRewardLedger ledger);

        /// <summary>
        /// 数值提升的额度（每次可分配的量），null 表示不限制，由上层自行决定
        /// </summary>
        ClassAttributeAllocation? NumericBoostAllowance(ClassRewardContext context, ClassRewardLedger ledger);
    }

    /// <summary>
    /// 默认结算器：忠实于 <see cref="ClassLevelUpReward"/> 的字段语义
    /// <para/>· 1 级施加职业初始属性分配 <see cref="Class.InitialAllocation"/>
    /// <para/>· 固有被动数量仅记账（实际授予由 <see cref="CharacterClass.ApplyTo"/> 按流派门槛完成）
    /// <para/>· 选择权 / 数值提升发放为待选配额，由玩家通过规划器动作消耗
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
                // 1 级：职业初始核心属性 / 成长分配
                if (level == 1 && !context.ClassRecord.InitialAllocation.IsEmpty)
                {
                    ClassAttributeAllocation initial = context.ClassRecord.InitialAllocation.Copy();
                    AttributeApplier.Apply(context.Character, initial);
                    ledger.AppliedAttribute.Add(initial);
                    result.GrantedAttribute = (result.GrantedAttribute ?? new()).Copy();
                    result.GrantedAttribute.Add(initial);
                    summary.Add($"初始分配：{initial.Describe()}");
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
                        ledger.NumericBoostAllowance ??= new();
                        ledger.NumericBoostAllowance.Add(reward.NumericBoost);
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
        public virtual ClassRewardSettlementResult SpendNumericBoost(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation)
        {
            if (allocation is null || allocation.IsEmpty)
            {
                return ClassRewardSettlementResult.Fail("数值提升的分配不能为空。");
            }
            if (ledger.PendingNumericBoosts < 1)
            {
                return ClassRewardSettlementResult.Fail("无可用数值提升（仅 4 / 9 级档位提供，且可被被动选择消耗替代）。");
            }
            ClassAttributeAllocation? allowance = NumericBoostAllowance(context, ledger);
            if (allowance != null && Exceeds(allocation, allowance))
            {
                return ClassRewardSettlementResult.Fail($"分配超出额度（上限：{allowance.Describe()}）。");
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
        public virtual ClassAttributeAllocation? NumericBoostAllowance(ClassRewardContext context, ClassRewardLedger ledger)
        {
            // 路线图单级自带额度优先，其次平衡常数默认，都没有则不限（由上层决定分配量）
            return ledger.NumericBoostAllowance?.Copy() ?? context.Eq.DefaultNumericBoostAllocation?.Copy();
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

        /// <summary>
        /// 分配是否超出额度（逐项比较，忽略未配置的项）
        /// </summary>
        private static bool Exceeds(ClassAttributeAllocation allocation, ClassAttributeAllocation allowance)
        {
            return (allowance.STR > 0 && allocation.STR > allowance.STR)
                || (allowance.AGI > 0 && allocation.AGI > allowance.AGI)
                || (allowance.INT > 0 && allocation.INT > allowance.INT)
                || (allowance.STRGrowth > 0 && allocation.STRGrowth > allowance.STRGrowth)
                || (allowance.AGIGrowth > 0 && allocation.AGIGrowth > allowance.AGIGrowth)
                || (allowance.INTGrowth > 0 && allocation.INTGrowth > allowance.INTGrowth);
        }
    }
}
