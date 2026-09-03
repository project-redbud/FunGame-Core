namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeCriticalCheck"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（正常进行暴击检定、无检定加值）。
/// 所有属性均为"默认即无害"设计。
/// </summary>
public readonly record struct BeforeCriticalCheckResult
{
    /// <summary>
    /// 跳过本次暴击检定（本次攻击必定不暴击）。<para/>
    /// [聚合: OR] 任一特效返回 true 即跳过检定；对应原 <c>BeforeCriticalCheck</c> 返回 false 的语义。
    /// </summary>
    public bool SkipCriticalCheck { get; init; }

    /// <summary>
    /// 暴击检定加值增量。<para/>
    /// [聚合: SUM] 多个特效的加值累加；对应原 <c>ctx.ThrowingBonus += x</c> 的语义。
    /// 施法者的暴击率最终为 <c>CritRate + Σ ThrowingBonusDelta</c>。
    /// </summary>
    public double ThrowingBonusDelta { get; init; }
}
