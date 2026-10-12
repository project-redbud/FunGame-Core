namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnEvadedTriggered"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（闪避生效）。
/// </summary>
public readonly record struct OnEvadedTriggeredResult
{
    /// <summary>
    /// 是否无视本次闪避（本次攻击判定为命中）。<para/>
    /// [聚合: OR] 任一特效返回 true 即无视；对应原 <c>OnEvadedTriggered</c> 返回 true 的语义。
    /// </summary>
    public bool IgnoreEvaded { get; init; }
}
