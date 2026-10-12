namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeApplyRecoveryAtTimeLapsing"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（允许回复、不修改回复量）。
/// </summary>
public readonly record struct BeforeApplyRecoveryResult
{
    /// <summary>
    /// 是否取消本次时间流逝回复。<para/>
    /// [聚合: OR] 任一特效返回 true 即取消；对应原 <c>BeforeApplyRecoveryAtTimeLapsing</c> 返回 false 的语义。
    /// </summary>
    public bool CancelRecovery { get; init; }

    /// <summary>
    /// 生命回复值的覆盖，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖本回合 HR 回复量。
    /// </summary>
    public double? HROverride { get; init; }

    /// <summary>
    /// 魔法回复值的覆盖，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖本回合 MR 回复量。
    /// </summary>
    public double? MROverride { get; init; }
}
