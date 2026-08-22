using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;

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
        public Character? Target { get; set; } = target;

        /// <summary>
        /// 治疗值
        /// </summary>
        public double Heal { get; set; } = 0;

        /// <summary>
        /// 是否允许复活（可修改）
        /// </summary>
        public bool CanRespawn { get; set; } = false;

        /// <summary>
        /// 是否是复活治疗（治疗事件时使用）
        /// </summary>
        public bool IsRespawn { get; set; } = false;

        /// <summary>
        /// 各特效的治疗增减贡献记录
        /// </summary>
        public Dictionary<Effect, double> TotalHealBonus { get; set; } = [];
    }
}
