namespace FunGame.Core.Model.DecisionTable
{
    /// <summary>默认 AI 候选选择方式；不影响已命中的角色规则。</summary>
    public enum AISelectionMode
    {
        /// <summary>沿用分数乘类别权重的选择方式。</summary>
        Legacy,
        /// <summary>以稳定评分与排序选取最优候选。</summary>
        Deterministic,
        /// <summary>在近似收益候选中先按类别权重，再按候选收益抽样。</summary>
        WeightedNearBest
    }
}
