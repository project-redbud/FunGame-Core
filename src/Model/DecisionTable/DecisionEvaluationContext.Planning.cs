namespace FunGame.Core.Model.DecisionTable
{
    public sealed partial class DecisionEvaluationContext
    {
        private readonly HashSet<DecisionPlan> _retainedPlans = [];
        private int _plansUsed;
        private DecisionPlanningStage _stage;
        internal int PlansUsed => _plansUsed;
        internal int RemainingPlans => Math.Max(0, PlanBudget - _plansUsed);
        internal bool IsTruncated { get; private set; }

        internal DecisionEvaluationContext CreateScope(int budget, DecisionPlanningStage stage) => new()
        {
            Queue = Queue, Actor = Actor, Points = Points, TurnOrigin = TurnOrigin, MoveGrids = MoveGrids,
            Skills = Skills, Items = Items, Enemies = Enemies, Allies = Allies,
            SelectableEnemies = SelectableEnemies, SelectableAllies = SelectableAllies,
            IsChainAction = IsChainAction, AllowItem = AllowItem, AllowSkill = AllowSkill, AllowAttack = AllowAttack,
            PositionBudget = PositionBudget, CenterBudget = CenterBudget, PlanBudget = Math.Max(0, budget),
            RejectedRules = RejectedRules, RecentPositions = RecentPositions, Metrics = Metrics,
            RangeCache = RangeCache, MovementCache = MovementCache, ThreatCache = ThreatCache,
            TargetDistanceCache = TargetDistanceCache, RiskCache = RiskCache, PlanningDiagnostics = PlanningDiagnostics, _stage = stage
        };

        internal static int AllocatePlans(int remaining, int count) => remaining <= 0 || count <= 0 ? 0 :
            remaining / count + (remaining % count == 0 ? 0 : 1);

        internal void ChargeScope(DecisionEvaluationContext scope) => _plansUsed += scope.PlansUsed;

        internal IReadOnlyList<DecisionPlan> RetainPlans(IReadOnlyList<DecisionPlan> plans)
        {
            List<DecisionPlan> retained = [];
            HashSet<DecisionPlan> seen = [];
            foreach (DecisionPlan plan in plans)
            {
                if (!seen.Add(plan)) continue;
                if (!_retainedPlans.Contains(plan))
                {
                    if (RemainingPlans == 0) { MarkTruncated(); break; }
                    _retainedPlans.Add(plan);
                    _plansUsed++;
                    Metrics.PlansEvaluated++;
                    if (_stage == DecisionPlanningStage.Policy) Metrics.PolicyPlansEvaluated++;
                    if (_stage == DecisionPlanningStage.DefaultAI) Metrics.DefaultAIPlansEvaluated++;
                }
                retained.Add(plan);
            }
            return retained;
        }

        internal void MarkTruncated()
        {
            if (IsTruncated) return;
            IsTruncated = true;
            Metrics.BudgetTruncations++;
        }

        internal void MarkExhausted() => Metrics.BudgetExhaustions++;
    }

    internal enum DecisionPlanningStage { Direct, Policy, DefaultAI }
}
