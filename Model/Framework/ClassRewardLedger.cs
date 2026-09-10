namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 单个职业的路线图奖励账本：已结算到的职业等级、剩余选择权、已习得技能与已施加的属性
    /// <para/>按职业 IdName 挂在 <see cref="Entity.CharacterClass.RewardLedgers"/> 上，与职业记录（<see cref="Entity.Class"/> 副本）一一对应
    /// <para/>只存 IdName 等可序列化数据，不持技能引用，便于存档与复制
    /// </summary>
    public class ClassRewardLedger
    {
        /// <summary>
        /// 已结算到的职业等级（保证结算幂等：只补发 (SettledToLevel, 当前等级] 区间）
        /// </summary>
        public int SettledToLevel { get; set; } = 0;

        /// <summary>
        /// 剩余职业技能选择权（战技 / 魔法 / 爆发技 通用）
        /// </summary>
        public int PendingActiveSkillChoices { get; set; } = 0;

        /// <summary>
        /// 剩余被动选择权
        /// </summary>
        public int PendingPassiveChoices { get; set; } = 0;

        /// <summary>
        /// 剩余数值提升次数（4 / 9 级：可替代被动选择，兑换一次核心属性分配）
        /// </summary>
        public int PendingNumericBoosts { get; set; } = 0;

        /// <summary>
        /// 1 级初始分配权是否可领取（领取后置 false；额度见 <see cref="EquilibriumConstant.InitialAttributeBudget"/>）
        /// </summary>
        public bool InitialAllocationAvailable { get; set; } = false;

        /// <summary>
        /// 已习得的职业技能 IdName（来自职业池副本中的技能实例）
        /// </summary>
        public HashSet<string> LearnedSkillIds { get; set; } = [];

        /// <summary>
        /// 累计已施加到角色身上的属性分配（洗点撤销时原样扣回）
        /// </summary>
        public ClassAttributeAllocation AppliedAttribute { get; set; } = new();

        /// <summary>
        /// 已获得的流派固有被动数量（与 <see cref="Entity.SubClass.InherentPassives"/> 门槛一致，仅作记录与校验）
        /// </summary>
        public int GrantedInherentPassiveCount { get; set; } = 0;

        /// <summary>
        /// 数值提升的额度覆盖（来自路线图单级自带的 <see cref="ClassLevelUpReward.NumericBoost"/>）
        /// <para/>null 时回落到 <see cref="EquilibriumConstant.NumericBoostBudget"/>
        /// </summary>
        public ClassAttributeBudget? NumericBoostBudget { get; set; } = null;

        /// <summary>
        /// 清空账本（洗点用；属性撤销由结算器的 Revoke 负责）
        /// </summary>
        public void Reset()
        {
            SettledToLevel = 0;
            PendingActiveSkillChoices = 0;
            PendingPassiveChoices = 0;
            PendingNumericBoosts = 0;
            InitialAllocationAvailable = false;
            LearnedSkillIds.Clear();
            AppliedAttribute = new();
            GrantedInherentPassiveCount = 0;
            NumericBoostBudget = null;
        }

        /// <summary>
        /// 复制一份
        /// </summary>
        public ClassRewardLedger Copy() => new()
        {
            SettledToLevel = SettledToLevel,
            PendingActiveSkillChoices = PendingActiveSkillChoices,
            PendingPassiveChoices = PendingPassiveChoices,
            PendingNumericBoosts = PendingNumericBoosts,
            InitialAllocationAvailable = InitialAllocationAvailable,
            LearnedSkillIds = [.. LearnedSkillIds],
            AppliedAttribute = AppliedAttribute.Copy(),
            GrantedInherentPassiveCount = GrantedInherentPassiveCount,
            NumericBoostBudget = NumericBoostBudget?.Copy()
        };
    }
}
