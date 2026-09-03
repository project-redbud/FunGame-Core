using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.EffectResult;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 治疗域上下文：治疗结算前/治疗值修改、治疗事件
    /// </summary>
    public class HealContext(IGamingQueue? queue, Character actor, Character? target = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 治疗目标（框架填充，模组只读）
        /// </summary>
        public Character? Target { get; internal set; } = target;

        /// <summary>
        /// 本次治疗的基础治疗值（框架填充；模组只读，修改请通过 <see cref="AlterHealValueResult.HealDelta"/> 返回）
        /// </summary>
        public double Heal { get; internal set; } = 0;

        /// <summary>
        /// 当前是否允许复活（框架在触发前重置为 false，判定逐特效进行；模组禁止写入，
        /// 请通过 <see cref="AlterHealValueResult.AllowRespawn"/> 返回）
        /// </summary>
        public bool CanRespawn { get; internal set; } = false;

        /// <summary>
        /// 是否是复活治疗（治疗事件时使用；框架填充）
        /// </summary>
        public bool IsRespawn { get; internal set; } = false;

        /// <summary>
        /// 各特效的治疗增减贡献记录（框架填充；模组只读）
        /// </summary>
        public Dictionary<Effect, double> TotalHealBonus { get; internal set; } = [];
    }
}
