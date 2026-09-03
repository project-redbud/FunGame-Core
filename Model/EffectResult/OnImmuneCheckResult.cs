namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.OnImmuneCheck"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不无视免疫）。
/// </summary>
public readonly record struct OnImmuneCheckResult
{
    /// <summary>
    /// 是否无视本次免疫（使本应免疫的伤害/技能生效）。<para/>
    /// [聚合: OR] 任一特效返回 true 即无视免疫；对应原 <c>OnImmuneCheck</c> 返回 false 的语义。
    /// </summary>
    public bool IgnoreImmunity { get; init; }
}
