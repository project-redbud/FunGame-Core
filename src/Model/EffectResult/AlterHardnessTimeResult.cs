namespace FunGame.Core.Model.EffectResult;

/// <summary>
/// <see cref="Entity.Effect.AlterHardnessTimeAfterNormalAttack"/> 与
/// <see cref="Entity.Effect.AlterHardnessTimeAfterCastSkill"/> 的回读对象。<para/>
/// 返回 <c>default</c> 表示不干预（硬直时间不变、不清零）。
/// 所有属性均为"默认即无害"设计。
/// </summary>
public readonly record struct AlterHardnessTimeResult
{
    /// <summary>
    /// 硬直时间的比率修正，默认 0 = 不变。<para/>
    /// 最终硬直 = 基础硬直 × (1 + <c>Factor</c>)。<para/>
    /// [聚合: 连乘] 各特效按其 Factor 依次复合到硬直时间上（乘法可交换，顺序无关）；
    /// 对应原 <c>ctx.BaseHardnessTime *= (1 + r)</c> 的链式乘法语义。<para/>
    /// 例：-0.2 表示硬直时间减少 20%（加速），0.3 表示增加 30%（减速）。
    /// </summary>
    public double Factor { get; init; }

    /// <summary>
    /// 是否清零硬直时间并解除插队保护。<para/>
    /// [聚合: OR] 任一特效返回 true 即生效（结果为 0，<c>IsCheckProtected</c> 置 false）；
    /// 对应原特效把 <c>ctx.BaseHardnessTime</c> 直接置 0、<c>ctx.IsCheckProtected</c> 置 false 的语义。
    /// </summary>
    public bool ClearHardnessTime { get; init; }

    /// <summary>
    /// 插队保护开关覆盖，默认 null = 不干预。<para/>
    /// [聚合: 覆盖后者胜] 返回非 null 时覆盖 <see cref="EffectContext.HardnessContext.IsCheckProtected"/>。
    /// </summary>
    public bool? OverrideCheckProtected { get; init; }
}
