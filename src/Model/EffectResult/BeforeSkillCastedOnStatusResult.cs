namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeSkillCastedOnStatus"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（保留目标）。
/// </summary>
public readonly record struct BeforeSkillCastedOnStatusResult
{
    /// <summary>
    /// 是否将当前目标从技能目标集合中移除。<para/>
    /// [聚合: OR] 任一特效返回 true 即移除该目标；对应原 <c>BeforeSkillCastedOnStatus</c> 返回 false 的语义。
    /// </summary>
    public bool RemoveFromTargets { get; init; }
}
