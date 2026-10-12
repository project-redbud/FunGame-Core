namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeEvadeCheck"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不跳过闪避检定、无检定加值）。
/// 所有属性均为"默认即无害"设计。
/// </summary>
public readonly record struct BeforeEvadeCheckResult
{
    /// <summary>
    /// 跳过本次闪避检定（无视闪避，本次攻击必然命中）。<para/>
    /// [聚合: OR] 任一特效返回 true 即跳过检定；对应原 <c>BeforeEvadeCheck</c> 返回 false 的语义。
    /// </summary>
    public bool SkipEvadeCheck { get; init; }

    /// <summary>
    /// 闪避检定加值增量。<para/>
    /// [聚合: SUM] 多个特效的加值累加；对应原 <c>ctx.ThrowingBonus += x</c> 的语义。
    /// 目标角色的闪避率最终为 <c>EvadeRate + Σ ThrowingBonusDelta</c>。
    /// </summary>
    public double ThrowingBonusDelta { get; init; }
}
