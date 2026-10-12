namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnShieldBroken"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不化解剩余伤害）。
/// </summary>
public readonly record struct OnShieldBrokenResult
{
    /// <summary>
    /// 是否化解本次剩余伤害（框架将剩余伤害置 0，伤害被完全抵消）。<para/>
    /// [聚合: OR] 任一特效返回 true 即化解；对应原 <c>OnShieldBroken</c> 返回 false 的语义。
    /// </summary>
    public bool NullifyRemainingDamage { get; init; }
}
