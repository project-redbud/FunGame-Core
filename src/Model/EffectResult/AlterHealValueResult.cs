namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.AlterHealValueBeforeHealToTarget"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不加成、不请求复活）。
/// </summary>
public readonly record struct AlterHealValueResult
{
    /// <summary>
    /// 治疗值增量，默认 0 = 无增量，可为负。<para/>
    /// [聚合: SUM] 各特效增量累加到治疗值上；对应原 <c>AlterHealValueBeforeHealToTarget</c> 返回值语义。
    /// </summary>
    public double HealDelta { get; init; }

    /// <summary>
    /// 是否请求允许复活（对已死亡目标治疗时生效）。<para/>
    /// [聚合: OR] 任一特效返回 true 即允许复活；对应原 <c>ctx.CanRespawn = true</c> 语义（每特效重置后判定）。
    /// </summary>
    public bool AllowRespawn { get; init; }
}
