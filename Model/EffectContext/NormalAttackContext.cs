using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 普通攻击域上下文：角色完成普通攻击后、普通攻击事件
    /// </summary>
    public class NormalAttackContext(IGamingQueue queue, Character actor, DecisionPoints? dp = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; set; } = dp;

        /// <summary>
        /// 普通攻击实例
        /// </summary>
        public NormalAttack? NormalAttack { get; set; } = null;

        /// <summary>
        /// 攻击目标列表
        /// </summary>
        public List<Character> Targets { get; set; } = [];
    }
}
