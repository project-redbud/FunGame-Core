using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 硬直域上下文：普通攻击/释放技能后修改硬直时间
    /// </summary>
    public class HardnessContext(IGamingQueue queue, Character actor) : HookContext(queue, actor)
    {
        /// <summary>
        /// 刚刚释放的技能（普通攻击后为 null）
        /// </summary>
        public Skill? Skill { get; set; } = null;

        /// <summary>
        /// 基础硬直时间（可修改）
        /// </summary>
        public double BaseHardnessTime { get; set; } = 0;

        /// <summary>
        /// 是否使用插队保护机制（可修改）
        /// </summary>
        public bool IsCheckProtected { get; set; } = false;
    }
}
