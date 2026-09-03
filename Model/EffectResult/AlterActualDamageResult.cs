namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.AlterActualDamageAfterCalculation"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（无增减、不化解）。
/// </summary>
public readonly record struct AlterActualDamageResult
{
    /// <summary>
    /// 实际伤害增量，默认 0 = 无增量，可为负。<para/>
    /// [聚合: SUM] 各特效增量累加；对应原钩子返回值语义。
    /// </summary>
    public double DamageDelta { get; init; }

    /// <summary>
    /// 是否化解本次伤害（将结算结果改写为闪避，伤害归零）。<para/>
    /// [聚合: OR] 任一特效返回 true 即化解；对应原 <c>ctx.IsEvaded = true</c> 语义。
    /// </summary>
    public bool IsEvaded { get; init; }
}
