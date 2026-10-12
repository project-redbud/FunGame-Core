namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeHealToTarget"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（允许治疗）。
/// </summary>
public readonly record struct BeforeHealToTargetResult
{
    /// <summary>
    /// 是否取消本次治疗。<para/>
    /// [聚合: OR] 任一特效返回 true 即取消；对应原 <c>BeforeHealToTarget</c> 返回 false 的语义。
    /// </summary>
    public bool CancelHeal { get; init; }
}
