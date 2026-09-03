namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeLifesteal"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（允许生命偷取）。
/// </summary>
public readonly record struct BeforeLifestealResult
{
    /// <summary>
    /// 是否取消本次生命偷取。<para/>
    /// [聚合: OR] 任一特效返回 true 即取消；对应原 <c>BeforeLifesteal</c> 返回 false 的语义。
    /// </summary>
    public bool CancelLifesteal { get; init; }
}
