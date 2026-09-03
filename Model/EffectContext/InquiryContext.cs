using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 询问域上下文：角色询问反应
    /// </summary>
    public class InquiryContext(IGamingQueue queue, Character character, InquiryOptions options) : HookContext(queue, character)
    {
        /// <summary>
        /// 角色当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; internal set; } = null;

        /// <summary>
        /// 询问选项（框架填充，模组只读）
        /// </summary>
        public InquiryOptions Options { get; internal set; } = options;

        /// <summary>
        /// 询问答复<para/>
        /// 注意：此字段为事件系统与特效共享的 in-out 契约——事件处理器（外部注册）写入答复，
        /// <see cref="Queue.GamingQueue.Inquiry"/> 最终返回 <c>ctx.Response</c>；模组钩子建议只读。
        /// </summary>
        public InquiryResponse? Response { get; set; } = null;
    }
}
