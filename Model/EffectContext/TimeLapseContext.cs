using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 时间流逝域上下文：时间流逝回复、时间流逝时（角色版/地图格版共用，<see cref="HookContext.Trigger"/> 与 <see cref="Grid"/> 二选一）
    /// </summary>
    public class TimeLapseContext(IGamingQueue? queue, Character? character = null) : HookContext(queue, character)
    {
        /// <summary>
        /// 地图格版时间流逝时的目标格子（角色版为 null）
        /// </summary>
        public Grid? Grid { get; internal set; } = null;

        /// <summary>
        /// 流逝的时间
        /// </summary>
        public double Elapsed { get; internal set; } = 0;

        /// <summary>
        /// 本回合生命回复值
        /// </summary>
        public double HR { get; internal set; } = 0;

        /// <summary>
        /// 本回合魔法回复值
        /// </summary>
        public double MR { get; internal set; } = 0;
    }
}
