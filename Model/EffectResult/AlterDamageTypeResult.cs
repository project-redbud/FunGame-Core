using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.AlterDamageTypeBeforeCalculation"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不转换伤害类型）。
/// </summary>
public readonly record struct AlterDamageTypeResult
{
    /// <summary>
    /// 是否按普通攻击结算的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖 <see cref="EffectContext.DamageContext.IsNormalAttack"/>。
    /// </summary>
    public bool? IsNormalAttack { get; init; }

    /// <summary>
    /// 伤害类型的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖 <see cref="EffectContext.DamageContext.DamageType"/>，
    /// 框架据此判定是否切换物理/魔法伤害算法。
    /// </summary>
    public DamageType? DamageType { get; init; }

    /// <summary>
    /// 魔法类型的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖 <see cref="EffectContext.DamageContext.MagicType"/>。
    /// </summary>
    public MagicType? MagicType { get; init; }
}
