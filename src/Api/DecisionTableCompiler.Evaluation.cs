using FunGame.Core.Controller;
using FunGame.Core.Model.DecisionTable;

namespace FunGame.Core.Api
{
    public sealed partial class DecisionTableCompiler
    {
        private sealed partial class CompiledPolicy
        {
            /// <summary>按规则优先级与预算选择行动。</summary>
            /// <param name="context">当前只读局面。</param>
            /// <returns>匹配结果及搜索诊断。</returns>
            public DecisionEvaluationResult Evaluate(DecisionEvaluationContext context)
            {
                List<DecisionDiagnostic> diagnostics = [];
                List<(DecisionRuleConfig Rule, bool? Precondition)> rules = [.. _config.Rules.Where(r => r.Enabled)
                    .OrderByDescending(r => r.Priority).Select(r => (r, context.RejectedRules.Contains(r.Id) ? null : Precheck(context, r.When)))];
                int remainingRules = rules.Count(r => !context.RejectedRules.Contains(r.Rule.Id) && r.Precondition != false && r.Rule.Action.Type != "endTurn");
                foreach ((DecisionRuleConfig rule, bool? precondition) in rules)
                {
                    if (context.RejectedRules.Contains(rule.Id))
                    {
                        diagnostics.Add(Diagnostic(rule.Id, "execution.rejected", "当前尝试中执行重验失败。"));
                        continue;
                    }
                    if (precondition == false)
                    {
                        diagnostics.Add(Diagnostic(rule.Id, "condition.unmatched", "条件不匹配。"));
                        continue;
                    }
                    bool constant = rule.Action.Type == "endTurn";
                    int quota = constant ? 0 : DecisionEvaluationContext.AllocatePlans(context.RemainingPlans, remainingRules--);
                    if (!constant && quota == 0)
                    {
                        context.MarkExhausted();
                        diagnostics.Add(Diagnostic(rule.Id, "budget.exhausted", "方案预算已耗尽，未规划本条规则。"));
                        continue;
                    }
                    DecisionEvaluationContext scope = context.CreateScope(quota, DecisionPlanningStage.Policy);
                    IReadOnlyList<DecisionPlan> plans = _actions.TryGetValue(rule.Action.Type,
                        out Func<DecisionEvaluationContext, ActionConfig, TargetSelectorConfig?, IReadOnlyList<DecisionPlan>>? custom)
                        ? scope.RetainPlans(custom(scope, rule.Action, rule.Target))
                        : DecisionActionPlanner.PlanForRule(scope, rule.Action, rule.Target, RequiresPositions(rule.When));
                    context.ChargeScope(scope);
                    if (scope.IsTruncated) diagnostics.Add(Diagnostic(rule.Id, "budget.truncated", "搜索达到预算上限，仅评估部分候选。"));
                    IEnumerable<DecisionPlan> eligible = plans.Where(p => EvaluateCondition(scope, rule.When, p));
                    IOrderedEnumerable<DecisionPlan> ordered = rule.Action.PositionPreference switch
                    {
                        "safe" => eligible.OrderBy(p => p.Risk).ThenByDescending(p => p.TargetCount).ThenBy(p => p.MoveSteps),
                        "coverage" => eligible.OrderByDescending(p => p.TargetCount).ThenBy(p => p.MoveSteps).ThenBy(p => p.Risk),
                        _ => eligible.OrderBy(p => p.MoveSteps).ThenBy(p => p.Risk)
                    };
                    DecisionPlan? selected = ordered.FirstOrDefault();
                    if (selected == null)
                    {
                        diagnostics.Add(plans.Count == 0 ? Diagnostic(rule.Id, "plan.unavailable", "无法规划合法行动。") :
                            Diagnostic(rule.Id, "condition.unmatched", "已评估方案的条件不匹配。"));
                        continue;
                    }
                    if (selected.Decision.ActionType == Library.Constant.CharacterActionType.None)
                    {
                        diagnostics.Add(Diagnostic(rule.Id, "plan.invalid", "规划器未提供有效动作。"));
                        continue;
                    }
                    selected.Decision.RuleId = rule.Id;
                    return new(selected.Decision, rule.Id, diagnostics);
                }
                return _config.Fallback == "endTurn" ? new(new() { ActionType = Library.Constant.CharacterActionType.EndTurn, HasCandidate = true }, null, diagnostics) : new(null, null, diagnostics);
            }

            private bool? Precheck(DecisionEvaluationContext context, ConditionConfig? condition)
            {
                if (condition == null) return true;
                List<ConditionConfig>? children = condition.All ?? condition.Any;
                if (children != null)
                {
                    bool all = condition.All != null;
                    bool unknown = false;
                    foreach (ConditionConfig child in children)
                    {
                        bool? value = Precheck(context, child);
                        if (value == !all) return !all;
                        unknown |= value == null;
                    }
                    return unknown ? null : all;
                }
                if (condition.Not != null) return !Precheck(context, condition.Not);
                return condition.Type is "self.hpRatio" or "self.mp" or "self.ep" or "self.state" or "decisionPoints" or "skill.available"
                    ? EvaluateCondition(context, condition, new() { Decision = new() }) : null;
            }

            private static DecisionDiagnostic Diagnostic(string rule, string code, string message) => new(rule, message) { Code = code };
        }
    }
}
