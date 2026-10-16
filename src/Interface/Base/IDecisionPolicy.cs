using FunGame.Core.Model.DecisionTable;

namespace FunGame.Core.Interface.Base
{
    /// <summary>角色策略；共享实现不得保存角色的可变战斗状态。</summary>
    public interface IDecisionPolicy
    {
        /// <summary>按局面选择可执行规则。</summary>
        /// <param name="context">只读评估局面。</param>
        /// <returns>行动及诊断，未命中时行动为空。</returns>
        DecisionEvaluationResult Evaluate(DecisionEvaluationContext context);
    }
}
