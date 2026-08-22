using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 移动域上下文：角色完成移动后、移动事件
    /// </summary>
    public class MoveContext(IGamingQueue queue, Character actor, DecisionPoints? dp = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; set; } = dp;

        /// <summary>
        /// 移动目标格子
        /// </summary>
        public Grid Target { get; set; } = Grid.Empty;
    }
}
