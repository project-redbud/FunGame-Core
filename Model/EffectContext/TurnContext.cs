using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 回合域上下文：回合开始/结束、决定行动
    /// <para/>列表字段（<see cref="Enemys"/> 等）为框架传入的可修改集合：模组可就地修改内容，但不能整体替换集合引用。
    /// </summary>
    public class TurnContext(IGamingQueue? queue, Character actor, DecisionPoints? dp = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; internal set; } = dp;

        /// <summary>
        /// 可选择的敌人列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Character> Enemys { get; internal set; } = [];

        /// <summary>
        /// 可选择的队友列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Character> Teammates { get; internal set; } = [];

        /// <summary>
        /// 可选择的技能列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Skill> Skills { get; internal set; } = [];

        /// <summary>
        /// 可选择的物品列表（框架填充；可就地修改内容，不可替换引用）
        /// </summary>
        public List<Item> Items { get; internal set; } = [];
    }
}
