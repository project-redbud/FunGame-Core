using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 行动域上下文：角色开始行动/行动后/决策结束，以及对应的顺序表事件
    /// </summary>
    public class ActionContext(IGamingQueue queue, Character actor, DecisionPoints? dp = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; internal set; } = dp;

        /// <summary>
        /// 行动类型
        /// </summary>
        public CharacterActionType ActionType { get; internal set; } = CharacterActionType.None;

        /// <summary>
        /// 行动/决策对应的回合记录快照（事件时使用）
        /// </summary>
        public RoundRecord? Record { get; internal set; } = null;
    }
}
