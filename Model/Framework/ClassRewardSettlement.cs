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
        /// 本次净发放的职业技能选择权（下调回收时为负数）
        /// </summary>
        public int GrantedActiveSkillChoices { get; set; } = 0;

        /// <summary>
        /// 本次净发放的被动选择权（下调回收时为负数）
        /// </summary>
        public int GrantedPassiveChoices { get; set; } = 0;

        /// <summary>
        /// 本次净发放的数值提升次数（下调回收时为负数）
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
        /// 把该职业的路线图状态「绝对对齐」到 <paramref name="targetLevel"/>（可升可降）
        /// <para/>与账本原水位无关，因此草稿态（暂存调整）可以反复来回调级而不会重复叠加
        /// <para/>下调时若该区间发放的选择权 / 数值提升已被使用则整体失败，不做部分回退（请改用洗点）
        /// </summary>
        ClassRewardSettlementResult Reconcile(ClassRewardContext context, ClassRewardLedger ledger, int targetLevel);

        /// <summary>
        /// 消耗一次选择权习得职业池中的技能（按技能类型自动判断使用主动 / 被动选择权）
        /// <para/>习得后自动补到当前路线图水位：不区分是否曾随池提前提级，后学技能与先学技能同级
        /// </summary>
        ClassRewardSettlementResult SpendSkillChoice(ClassRewardContext context, ClassRewardLedger ledger, Skill skill);

        /// <summary>
        /// 消耗 1 级初始分配权：按额度分配核心属性，受职业与角色模板限值的交集约束
        /// </summary>
        ClassRewardSettlementResult SpendInitialAllocation(ClassRewardContext context, ClassRewardLedger ledger, ClassAttributeAllocation allocation);

        /// <summary>
        /// 消耗一次数值提升（替代被动选择）：按额度分配核心属性，不受模板限值约束
        /// <para/>与被动严格互斥：4 / 9 级发放的是同一份「被动或数值提升」份额
        /// <para/>要求 <see cref="ClassRewardLedger.PendingPassiveChoices"/> ≥ 1 并同时消耗两者
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
}
