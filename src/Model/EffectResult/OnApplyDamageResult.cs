namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnApplyDamage"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（保留默认输出消息）。
/// </summary>
public readonly record struct OnApplyDamageResult
{
    /// <summary>
    /// 输出消息的覆盖，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖伤害输出消息；对应原 <c>ctx.OriginalMessage = v</c> 语义。
    /// </summary>
    public string? OriginalMessage { get; init; }
}
