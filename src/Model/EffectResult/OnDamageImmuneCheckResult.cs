namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnDamageImmuneCheck"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（免疫生效）。
/// </summary>
public readonly record struct OnDamageImmuneCheckResult
{
    /// <summary>
    /// 是否无视本次伤害免疫（使本应免疫的伤害生效）。<para/>
    /// [聚合: OR] 任一特效返回 true 即无视；对应原 <c>OnDamageImmuneCheck</c> 返回 false 的语义。
    /// </summary>
    public bool IgnoreDamageImmunity { get; init; }
}
