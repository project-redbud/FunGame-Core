using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 驱散域上下文：驱散他特效、被驱散时
    /// </summary>
    public class DispelContext(IGamingQueue? queue, Character? dispeller = null, Character? target = null) : HookContext(queue, dispeller)
    {
        /// <summary>
        /// 被驱散的角色
        /// </summary>
        public Character? Target { get; internal set; } = target;

        /// <summary>
        /// 被驱散的特效
        /// </summary>
        public Effect? Effect { get; internal set; } = null;

        /// <summary>
        /// 驱散源特效（即正在执行驱散的特效）
        /// </summary>
        public Effect? DispellerEffect { get; internal set; } = null;

        /// <summary>
        /// 是否对敌方施放驱散
        /// </summary>
        public bool IsEnemy { get; internal set; } = false;
    }
}
