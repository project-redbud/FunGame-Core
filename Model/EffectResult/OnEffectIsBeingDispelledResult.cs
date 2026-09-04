namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnEffectIsBeingDispelled"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（允许被驱散）。
/// </summary>
public readonly record struct OnEffectIsBeingDispelledResult
{
    /// <summary>
    /// 是否阻止本次驱散。<para/>
    /// [聚合: OR] 任一特效返回 true 即阻止；对应原 <c>OnEffectIsBeingDispelled</c> 返回 false 的语义。
    /// </summary>
    public bool BlockDispel { get; init; }
}
