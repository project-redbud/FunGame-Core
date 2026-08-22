using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 特效钩子（<see cref="Entity.Effect"/>）与行动顺序表（<see cref="Queue.GamingQueue"/>）事件的统一参数上下文基类<para/>
    /// 框架在管线节点构造一次上下文实例，同一实例依次流经 事件 → 技能分发 → 特效钩子。
    /// </summary>
    public class HookContext(IGamingQueue? queue, Character? actor)
    {
        /// <summary>
        /// 当前的行动顺序表实例；局外场景（如局外对目标触发技能效果）为 null
        /// </summary>
        public IGamingQueue? Queue { get; } = queue;

        /// <summary>
        /// 触发钩子/事件的主角色
        /// </summary>
        public Character? Actor { get; } = actor;
    }
}
