using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Controller
{
    public partial class AIController
    {
        /// <summary>默认候选选择模式。</summary>
        public AISelectionMode SelectionMode { get; set; } = AISelectionMode.Legacy;
        /// <summary>收益接近的绝对分差，必须是非负有限数。</summary>
        public double ScoreTolerance { get; set; } = 25;
        /// <summary>移动必须超过的收益增量。</summary>
        public double MovementBenefitThreshold { get; set; } = 1;
        /// <summary>返回近期位置的额外评分成本。</summary>
        public double RepeatedPositionPenalty { get; set; } = 1;
        /// <summary>语义估值扩展；返回 null 使用内置估值，回调不得改变战斗状态。</summary>
        public event Func<DecisionEvaluationContext, DecisionPlan, double?>? EvaluatePlanEvent;
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<Character, MovementHistory> _movementHistory = new();

        private sealed class MovementHistory
        {
            internal int? Current;
            internal Queue<int> Previous { get; } = new();
        }

        private IReadOnlyList<int> GetRecentPositions(Character actor, bool chain)
        {
            if (chain || _map?.GetCharacterCurrentGrid(actor) is not Grid grid) return [];
            MovementHistory history = _movementHistory.GetValue(actor, static _ => new());
            if (history.Current is int previous && previous != grid.Id)
            {
                history.Previous.Enqueue(previous);
                while (history.Previous.Count > 3) history.Previous.Dequeue();
            }
            history.Current = grid.Id;
            return history.Previous.ToArray();
        }

        private AIDecision DecidePlannedAction(DecisionEvaluationContext context, double itemWeight, double skillWeight, double attackWeight)
        {
            if (!Enum.IsDefined(SelectionMode) || !double.IsFinite(ScoreTolerance) || ScoreTolerance < 0 || !double.IsFinite(MovementBenefitThreshold) || MovementBenefitThreshold < 0 ||
                !double.IsFinite(RepeatedPositionPenalty) || RepeatedPositionPenalty < 0)
                throw new InvalidOperationException("评分阈值必须是非负有限数。");
            List<ActionConfig> actions = [];
            if (DecisionActionPlanner.CanUse(context, "normalAttack")) actions.Add(ActionsConfig("normalAttack"));
            foreach (Skill skill in context.Skills)
                if (DecisionActionPlanner.CanUse(context, "castSkill", skill)) actions.Add(new()
                {
                    Type = "castSkill", SkillId = skill.Id, PositionPreference = skill.IsMagic && skill.RealCastTime > 0 ? "safe" : "current",
                    Movement = !skill.IsMagic && skill.CanSelectSelf && !skill.CanSelectEnemy && !skill.CanSelectTeammate ? "never" : "ifNeeded"
                });
            foreach (Item item in context.Items)
                if (DecisionActionPlanner.CanUse(context, "useItem", item: item)) actions.Add(new() { Type = "useItem", ItemId = item.Id });
            List<DecisionPlan> plans = [];
            int reserve = context.RemainingPlans > 1 && !context.IsChainAction && _map != null ? 1 : 0;
            int remainingActions = actions.Count;
            foreach (ActionConfig action in actions)
            {
                int quota = DecisionEvaluationContext.AllocatePlans(context.RemainingPlans - reserve, remainingActions--);
                plans.AddRange(PlanDefaultAction(context, action, quota));
            }

            foreach (DecisionPlan plan in plans)
            {
                AIDecision decision = plan.Decision;
                double? custom = EvaluatePlanEvent?.Invoke(context, plan);
                double value = custom ?? EvaluatePlannedValue(context, plan);
                double repeated = plan.MoveSteps > 0 && decision.TargetMoveGrid is Grid grid && context.RecentPositions.Contains(grid.Id) ? RepeatedPositionPenalty : 0;
                decision.Score = double.IsFinite(value) ? value - plan.MoveSteps * 0.5 - plan.Risk - repeated : double.NegativeInfinity;
            }
            plans = [.. plans.Where(p => double.IsFinite(p.Decision.Score))];
            List<DecisionPlan> stationary = [.. plans.Where(p => p.MoveSteps == 0)];
            plans = [.. plans.Where(p => p.MoveSteps == 0 || !stationary.Any(s => s.Decision.ActionType == p.Decision.ActionType &&
                ReferenceEquals(s.Decision.SkillToUse, p.Decision.SkillToUse) && ReferenceEquals(s.Decision.ItemToUse, p.Decision.ItemToUse) &&
                s.Decision.Score + MovementBenefitThreshold >= p.Decision.Score))];
            if (plans.Count == 0)
            {
                IReadOnlyList<DecisionPlan> moves = PlanDefaultAction(context, new() { Type = "approach", Movement = "required" }, context.RemainingPlans, TargetsConfig());
                return moves.OrderBy(p => GameMap.CalculateManhattanDistance(p.Decision.TargetMoveGrid!, _map!.GetCharacterCurrentGrid(p.Target!)!))
                    .ThenBy(p => p.MoveSteps).ThenBy(p => p.Decision.TargetMoveGrid!.Id).FirstOrDefault()?.Decision ?? CreateDefaultDecision(context.TurnOrigin);
            }
            List<DecisionPlan> sorted = [.. plans.OrderByDescending(p => p.Decision.Score).ThenBy(p => p.Decision.ActionType)
                .ThenBy(p => p.Decision.TargetMoveGrid?.Id).ThenBy(p => p.Decision.SkillToUse?.Id).ThenBy(p => p.Decision.ItemToUse?.Id)
                .ThenBy(p => p.Target?.Guid).ThenBy(p => p.Center?.Id)];
            if (SelectionMode == AISelectionMode.Deterministic) return sorted[0].Decision;
            List<DecisionPlan> near = [.. sorted.Where(p => p.Decision.Score >= sorted[0].Decision.Score - ScoreTolerance)];
            AdjustProbabilitiesBasedOnContext(ref itemWeight, ref skillWeight, ref attackWeight, context.Actor, [.. context.Enemies], [.. context.Allies]);
            double Weight(CharacterActionType type) => type switch { CharacterActionType.NormalAttack => attackWeight, CharacterActionType.UseItem => itemWeight, _ => skillWeight };
            List<IGrouping<CharacterActionType, DecisionPlan>> groups = [.. near.GroupBy(p => p.Decision.ActionType).Where(g => double.IsFinite(Weight(g.Key)) && Weight(g.Key) > 0)];
            if (groups.Count == 0) return sorted[0].Decision;
            Random random = new(DeriveRandomSeed(_queue.Seed, Interlocked.Increment(ref _decisionSerial), context.TurnOrigin ?? Grid.Empty));
            double maxWeight = groups.Max(g => Weight(g.Key));
            double draw = random.NextDouble() * groups.Sum(g => Weight(g.Key) / maxWeight);
            IGrouping<CharacterActionType, DecisionPlan> chosen = groups[^1];
            foreach (IGrouping<CharacterActionType, DecisionPlan> group in groups)
            {
                draw -= Weight(group.Key) / maxWeight;
                if (draw <= 0) { chosen = group; break; }
            }
            double best = chosen.Max(p => p.Decision.Score);
            double scale = Math.Max(1, ScoreTolerance);
            double total = chosen.Sum(p => Math.Exp((p.Decision.Score - best) / scale));
            draw = random.NextDouble() * total;
            foreach (DecisionPlan plan in chosen)
            {
                draw -= Math.Exp((plan.Decision.Score - best) / scale);
                if (draw <= 0) return plan.Decision;
            }
            return chosen.Last().Decision;
        }

        private static IReadOnlyList<DecisionPlan> PlanDefaultAction(DecisionEvaluationContext context, ActionConfig action, int quota, TargetSelectorConfig? selector = null)
        {
            string path = $"defaultAI.{action.Type}";
            if ((action.SkillId ?? action.ItemId) is long) path += $"[{action.SkillId ?? action.ItemId}]";
            if (quota == 0)
            {
                context.MarkExhausted();
                context.PlanningDiagnostics.Add(new(path, "默认 AI 方案预算已耗尽。") { Code = "budget.exhausted" });
                return [];
            }
            DecisionEvaluationContext scope = context.CreateScope(quota, DecisionPlanningStage.DefaultAI);
            IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(scope, action, selector);
            context.ChargeScope(scope);
            if (scope.IsTruncated) context.PlanningDiagnostics.Add(new(path, "默认 AI 搜索达到预算上限，仅评估部分候选。") { Code = "budget.truncated" });
            return plans;
        }

        private static ActionConfig ActionsConfig(string type) => new() { Type = type };
        private static TargetSelectorConfig TargetsConfig() => new() { Relation = "enemy", Selector = "nearest" };

        private double EvaluatePlannedValue(DecisionEvaluationContext context, DecisionPlan plan)
        {
            Character actor = context.Actor;
            AIDecision decision = plan.Decision;
            if (decision.ActionType == CharacterActionType.NormalAttack) return EvaluateNormalAttack(actor, decision.Targets);
            if (decision.ItemToUse is Item item)
                return plan.TargetCount + (EvaluateItemEvent?.Invoke(actor, item, decision.Targets, item.Skills.Active!.RealMPCost + item.Skills.Active.RealEPCost) ?? 0);
            Skill skill = (Skill)decision.SkillToUse!;
            double cost = skill.IsMagic ? skill.RealMPCost : skill.RealEPCost;
            double score = plan.TargetCount + decision.Targets.Sum(t => CalculateTargetValueEvent?.Invoke(t, skill) ?? 0);
            if (skill.IsNonDirectional && _map != null)
                return score + (EvaluateNonDirectionalSkillEvent?.Invoke(actor, skill, decision.TargetMoveGrid!, context.GetRange(decision.TargetMoveGrid!, skill.CastRange, true), [.. context.Enemies], [.. context.Allies], cost) ?? 0);
            return score + (EvaluateSkillEvent?.Invoke(actor, skill, decision.Targets, cost) ?? 0);
        }
    }
}
