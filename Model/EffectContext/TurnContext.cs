using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 回合域上下文：回合开始/结束、决定行动
    /// </summary>
    public class TurnContext(IGamingQueue? queue, Character actor, DecisionPoints? dp = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; set; } = dp;

        /// <summary>
        /// 可选择的敌人列表
        /// </summary>
        public List<Character> Enemys { get; set; } = [];

        /// <summary>
        /// 可选择的队友列表
        /// </summary>
        public List<Character> Teammates { get; set; } = [];

        /// <summary>
        /// 可选择的技能列表
        /// </summary>
        public List<Skill> Skills { get; set; } = [];

        /// <summary>
        /// 可选择的物品列表
        /// </summary>
        public List<Item> Items { get; set; } = [];
    }
}
