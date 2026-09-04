using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 物品使用域上下文：角色使用物品后、使用物品事件
    /// </summary>
    public class ItemUseContext(IGamingQueue queue, Character actor, DecisionPoints? dp = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; internal set; } = dp;

        /// <summary>
        /// 使用的物品
        /// </summary>
        public Item? Item { get; internal set; } = null;

        /// <summary>
        /// 物品附带的技能
        /// </summary>
        public Skill? Skill { get; internal set; } = null;

        /// <summary>
        /// 使用目标列表
        /// </summary>
        public List<Character> Targets { get; internal set; } = [];
    }
}
