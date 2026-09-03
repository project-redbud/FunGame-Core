namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeSkillCastWillBeInterrupted"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（允许打断）。
/// </summary>
public readonly record struct BeforeSkillCastWillBeInterruptedResult
{
    /// <summary>
    /// 是否阻止本次施法被打断。<para/>
    /// [聚合: OR] 任一特效返回 true 即阻止打断；对应原 <c>BeforeSkillCastWillBeInterrupted</c> 返回 false 的语义。
    /// </summary>
    public bool BlockInterruption { get; init; }
}
