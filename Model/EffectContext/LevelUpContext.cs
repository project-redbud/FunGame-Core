using FunGame.Core.Entity;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 升级域上下文：技能升级、持有者升级
    /// </summary>
    public class LevelUpContext(Character actor, double level) : HookContext(null, actor)
    {
        /// <summary>
        /// 升级后的等级
        /// </summary>
        public double Level { get; internal set; } = level;
    }
}
