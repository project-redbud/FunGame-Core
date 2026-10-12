namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.AlterEPAfterDamage"/> 与 <see cref="Entity.Effect.AlterEPAfterGetDamage"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不修改能量获取）。
/// </summary>
public readonly record struct AlterEPResult
{
    /// <summary>
    /// 能量获取值的覆盖，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 需要叠加时读取当前值计算，如 <c>BaseEP = ctx.BaseEP * 1.5</c>；
    /// 对应原 <c>ctx.BaseEP = v</c> / <c>ctx.BaseEP *= x</c> 的链式语义。
    /// </summary>
    public double? BaseEP { get; init; }
}
