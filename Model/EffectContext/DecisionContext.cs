using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 决策域上下文：行动开始前指定/修改角色的行动类型与操作触发概率
    /// </summary>
    public class DecisionContext(IGamingQueue queue, Character actor, DecisionPoints dp, CharacterState state) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints DP { get; } = dp;

        /// <summary>
        /// 角色当前的状态
        /// </summary>
        public CharacterState State { get; } = state;

        /// <summary>
        /// 是否可以使用物品
        /// </summary>
        public bool CanUseItem { get; internal set; } = true;

        /// <summary>
        /// 是否可以释放技能
        /// </summary>
        public bool CanCastSkill { get; internal set; } = true;

        /// <summary>
        /// 使用物品的触发概率
        /// </summary>
        public double PUseItem { get; internal set; } = 0;

        /// <summary>
        /// 释放技能的触发概率
        /// </summary>
        public double PCastSkill { get; internal set; } = 0;

        /// <summary>
        /// 普通攻击的触发概率
        /// </summary>
        public double PNormalAttack { get; internal set; } = 0;
    }
}
