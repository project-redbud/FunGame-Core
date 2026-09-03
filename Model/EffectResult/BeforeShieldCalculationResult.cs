namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeShieldCalculation"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（继续护盾结算、不减伤、不改消息）。
/// </summary>
public readonly record struct BeforeShieldCalculationResult
{
    /// <summary>
    /// 是否跳过本次护盾结算。<para/>
    /// [聚合: OR] 任一特效返回 true 即跳过护盾抵消；对应原 <c>BeforeShieldCalculation</c> 返回 false 的语义。
    /// </summary>
    public bool SkipShield { get; init; }

    /// <summary>
    /// 护盾减伤值，默认 0 = 不减伤。<para/>
    /// [聚合: SUM] 各特效减伤值累加作用于本次伤害；对应原 <c>ctx.DamageReduce</c> 逐特效应用的语义。
    /// </summary>
    public double DamageReduce { get; init; }

    /// <summary>
    /// 护盾结算消息覆盖，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖输出消息。
    /// </summary>
    public string? Message { get; init; }
}
