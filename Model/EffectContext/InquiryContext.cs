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
        public DecisionPoints? DP { get; set; } = null;

        /// <summary>
        /// 询问选项
        /// </summary>
        public InquiryOptions Options { get; set; } = options;

        /// <summary>
        /// 询问答复（事件处理器或钩子可修改）
        /// </summary>
        public InquiryResponse? Response { get; set; } = null;
    }
}
