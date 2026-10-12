using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.AlterActionTypeBeforeAction"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（不指定行动、不强制、不改概率与可用性）。
/// 所有属性均为"默认即无害"设计。
/// </summary>
public readonly record struct AlterActionTypeResult
{
    /// <summary>
    /// 指定的行动类型，默认 <see cref="CharacterActionType.None"/> = 不指定。<para/>
    /// 仅当 <see cref="ForceAction"/> 为 true 且本属性非 None 时强制该行动（短路后续特效）。
    /// </summary>
    public CharacterActionType ActionType { get; init; }

    /// <summary>
    /// 是否强制指定行动。<para/>
    /// [聚合: 短路 OR] 任一特效返回 true 且 <see cref="ActionType"/> 非 None 时立即生效并停止后续特效。
    /// </summary>
    public bool ForceAction { get; init; }

    /// <summary>
    /// 是否允许使用物品的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖当前判定；后续特效可读 <see cref="EffectContext.DecisionContext.CanUseItem"/> 取得最新值。
    /// </summary>
    public bool? CanUseItem { get; init; }

    /// <summary>
    /// 是否允许释放技能的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜]
    /// </summary>
    public bool? CanCastSkill { get; init; }

    /// <summary>
    /// 使用物品概率的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 需要叠加时读取当前值计算，如 <c>PUseItem = ctx.PUseItem + x</c>；
    /// 对应原 <c>ctx.PUseItem = v</c> / <c>ctx.PUseItem += x</c> 的链式语义。
    /// </summary>
    public double? PUseItem { get; init; }

    /// <summary>
    /// 释放技能概率的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜]
    /// </summary>
    public double? PCastSkill { get; init; }

    /// <summary>
    /// 普通攻击概率的覆盖值，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜]
    /// </summary>
    public double? PNormalAttack { get; init; }
}
