using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 生命偷取域上下文：生命偷取前/后
    /// </summary>
    public class LifestealContext(IGamingQueue queue, Character actor, Character? enemy = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 被偷取方
        /// </summary>
        public Character? Enemy { get; internal set; } = enemy;

        /// <summary>
        /// 造成的基础伤害
        /// </summary>
        public double Damage { get; internal set; } = 0;

        /// <summary>
        /// 偷取的生命值
        /// </summary>
        public double Steal { get; internal set; } = 0;
    }
}
