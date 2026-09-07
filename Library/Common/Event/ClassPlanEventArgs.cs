using FunGame.Core.Entity;

namespace FunGame.Core.Library.Common.Event
{
    /// <summary>
    /// 职业规划阶段
    /// </summary>
    public enum ClassPlanPhase
    {
        /// <summary>
        /// 选择职业与流派（含兼职）
        /// </summary>
        SelectClass,

        /// <summary>
        /// 职业升级
        /// </summary>
        UpgradeClass,

        /// <summary>
        /// 选择角色定位
        /// </summary>
        SelectRoleTypes,

        /// <summary>
        /// 学习战斗天赋
        /// </summary>
        LearnTalent,

        /// <summary>
        /// 激活 / 转换战斗天赋
        /// </summary>
        ActivateTalent,

        /// <summary>
        /// 洗点
        /// </summary>
        ResetPlan,

        /// <summary>
        /// 修改默认职业与流派（20 级后）
        /// </summary>
        ChangeDefault
    }

    /// <summary>
    /// 职业规划事件参数
    /// </summary>
    /// <param name="phase">规划阶段</param>
    /// <param name="plan">操作后的职业计划</param>
    /// <param name="success">操作是否成功</param>
    /// <param name="message">结果消息（错误原因或描述）</param>
    public class ClassPlanEventArgs(ClassPlanPhase phase, CharacterClass plan, bool success = true, string message = "") : EventArgs
    {
        /// <summary>
        /// 规划阶段
        /// </summary>
        public ClassPlanPhase Phase { get; } = phase;

        /// <summary>
        /// 操作后的职业计划
        /// </summary>
        public CharacterClass Plan { get; } = plan;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; } = success;

        /// <summary>
        /// 结果消息（错误原因或描述）
        /// </summary>
        public string Message { get; } = message;
    }
}
