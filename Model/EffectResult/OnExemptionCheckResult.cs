namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnExemptionCheck"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（正常进行豁免检定、无检定加值）。
/// </summary>
public readonly record struct OnExemptionCheckResult
{
    /// <summary>
    /// 是否跳过本次豁免检定（特效将必定命中，不再掷豁免骰）。<para/>
    /// [聚合: OR] 任一特效返回 true 即跳过；对应原 <c>OnExemptionCheck</c> 返回 false 的语义。
    /// </summary>
    public bool SkipExemptionCheck { get; init; }

    /// <summary>
    /// 豁免检定加值增量，默认 0 = 无增量。<para/>
    /// [聚合: SUM] 各特效加值累加；对应原 <c>ctx.ThrowingBonus += x</c> 语义。
    /// 最终检定值为 <c>ExemptionType 对应豁免值 + Σ ThrowingBonusDelta</c>。
    /// </summary>
    public double ThrowingBonusDelta { get; init; }
}
