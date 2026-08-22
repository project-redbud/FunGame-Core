using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 行动顺序表更新域上下文
    /// </summary>
    public class QueueUpdatedContext(IGamingQueue queue, Character character, DecisionPoints? dp = null) : HookContext(queue, character)
    {
        /// <summary>
        /// 当前的行动顺序
        /// </summary>
        public List<Character> Characters { get; set; } = [];

        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; set; } = dp;

        /// <summary>
        /// 硬直时间
        /// </summary>
        public double HardnessTime { get; set; } = 0;

        /// <summary>
        /// 更新原因
        /// </summary>
        public QueueUpdatedReason Reason { get; set; } = QueueUpdatedReason.Action;

        /// <summary>
        /// 说明消息
        /// </summary>
        public string Message { get; set; } = "";
    }
}
