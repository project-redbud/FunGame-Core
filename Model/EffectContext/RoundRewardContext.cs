using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 回合奖励域上下文：发放（获得）、移除、夺取<para/>
    /// 框架在奖励管线的每个节点构造一次实例，同一实例依次流经 [ 队列事件 → 特效钩子 ]
    /// </summary>
    /// <param name="queue">行动顺序表；局外场景为 null</param>
    /// <param name="owner">奖励归属角色（<see cref="RoundRewardBinding.Character"/> 时为奖励表的持有者）</param>
    /// <param name="binding">奖励的绑定方式</param>
    /// <param name="turnKey">
    /// 奖励键：<see cref="RoundRewardBinding.Round"/> 时为全局回合（<see cref="IGamingQueue.TotalRound"/>）；
    /// <see cref="RoundRewardBinding.Character"/> 时为该角色的行动回合序号
    /// </param>
    public class RoundRewardContext(IGamingQueue? queue, Character owner, RoundRewardBinding binding, int turnKey) : HookContext(queue, owner)
    {
        /// <summary>
        /// 本次涉及的奖励绑定方式
        /// </summary>
        public RoundRewardBinding Binding { get; } = binding;

        /// <summary>
        /// 本次涉及的奖励键（语义由 <see cref="Binding"/> 决定）
        /// </summary>
        public int TurnKey { get; } = turnKey;

        /// <summary>
        /// 涉及的全部奖励（夺取时为全部被夺取项）
        /// </summary>
        public IReadOnlyList<Skill> Skills { get; internal set; } = [];

        /// <summary>
        /// 夺取者（仅 <see cref="Effect.OnRoundRewardStolen"/> 有值）
        /// </summary>
        public Character? Thief { get; internal set; }

        /// <summary>
        /// 原持有者（仅 <see cref="Effect.OnRoundRewardStolen"/> 有值）
        /// </summary>
        public Character? From { get; internal set; }

        /// <summary>
        /// 是否为「吟唱回合顺延到结算回合」的被动奖励
        /// </summary>
        public bool IsCarryOver { get; internal set; }
    }
}
