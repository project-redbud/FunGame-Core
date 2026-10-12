namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.BeforeApplyTrueDamage"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（正常结算真实伤害）。
/// </summary>
public readonly record struct BeforeApplyTrueDamageResult
{
    /// <summary>
    /// 是否化解本次真实伤害（结算结果改写为闪避，伤害归零）。<para/>
    /// [聚合: OR] 任一特效返回 true 即化解；对应原 <c>BeforeApplyTrueDamage</c> 返回 true 的语义。
    /// </summary>
    public bool NullifyDamage { get; init; }
}
