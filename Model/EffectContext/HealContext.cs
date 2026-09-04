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
        /// 治疗目标
        /// </summary>
        public Character? Target { get; internal set; } = target;

        /// <summary>
        /// 本次治疗的基础治疗值
        /// </summary>
        public double Heal { get; internal set; } = 0;

        /// <summary>
        /// 当前是否允许复活（每次判定前重置为 false，判定逐特效进行）
        /// </summary>
        public bool CanRespawn { get; internal set; } = false;

        /// <summary>
        /// 是否是复活治疗
        /// </summary>
        public bool IsRespawn { get; internal set; } = false;

        /// <summary>
        /// 各特效的治疗增减贡献记录
        /// </summary>
        public Dictionary<Effect, double> TotalHealBonus { get; internal set; } = [];
    }
}
