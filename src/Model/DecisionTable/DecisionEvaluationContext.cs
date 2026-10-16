using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Model.DecisionTable
{
    /// <summary>一次决策使用的局面；评估期间不得修改战斗状态。</summary>
    public sealed partial class DecisionEvaluationContext
    {
        /// <summary>所属队列。</summary>
        public required GamingQueue Queue { get; init; }
        /// <summary>当前角色。</summary>
        public required Character Actor { get; init; }
        /// <summary>决策点与配额。</summary>
        public required DecisionPoints Points { get; init; }
        /// <summary>回合移动范围的原点。</summary>
        public Grid? TurnOrigin { get; init; }
        /// <summary>移动候选；规划器仍验证路径。</summary>
        public IReadOnlyList<Grid> MoveGrids { get; init; } = [];
        /// <summary>可用技能实例，包括装备及临时技能。</summary>
        public IReadOnlyList<Skill> Skills { get; init; } = [];
        /// <summary>可用物品实例。</summary>
        public IReadOnlyList<Item> Items { get; init; } = [];
        /// <summary>全部敌方。</summary>
        public IReadOnlyList<Character> Enemies { get; init; } = [];
        /// <summary>全部友方。</summary>
        public IReadOnlyList<Character> Allies { get; init; } = [];
        /// <summary>允许选择的敌方。</summary>
        public IReadOnlyList<Character> SelectableEnemies { get; init; } = [];
        /// <summary>允许选择的友方。</summary>
        public IReadOnlyList<Character> SelectableAllies { get; init; } = [];
        /// <summary>是否为原地协同申请。</summary>
        public bool IsChainAction { get; init; }
        /// <summary>物品类别是否允许。</summary>
        public bool AllowItem { get; init; } = true;
        /// <summary>技能类别是否允许。</summary>
        public bool AllowSkill { get; init; } = true;
        /// <summary>普攻类别是否允许。</summary>
        public bool AllowAttack { get; init; } = true;
        /// <summary>位置完整评估预算。</summary>
        public int PositionBudget { get; init; } = 32;
        /// <summary>每个位置的施法中心预算。</summary>
        public int CenterBudget { get; init; } = 64;
        /// <summary>当前规划范围的方案上限；策略与默认 AI 分阶段独立使用。</summary>
        public int PlanBudget { get; init; } = 4096;
        /// <summary>当前行动尝试中已经执行失败的规则。</summary>
        public IReadOnlySet<string> RejectedRules { get; init; } = new HashSet<string>();
        /// <summary>当前角色近期实际离开的格子，用于降低往返移动偏好。</summary>
        public IReadOnlyList<int> RecentPositions { get; init; } = [];
        /// <summary>评估统计。</summary>
        public DecisionMetrics Metrics { get; private init; } = new();
        internal Dictionary<(int Origin, int Range, bool Include), List<Grid>> RangeCache { get; private init; } = [];
        internal Dictionary<(Character Actor, int Origin), IReadOnlyDictionary<Grid, int>> MovementCache { get; private init; } = [];
        internal Dictionary<Character, HashSet<Grid>> ThreatCache { get; private init; } = [];
        internal Dictionary<(string? Relation, int Grid), int> TargetDistanceCache { get; private init; } = [];

        internal List<DecisionDiagnostic> PlanningDiagnostics { get; private init; } = [];
        internal Dictionary<Grid, double> RiskCache { get; private init; } = [];

        internal List<Grid> GetRange(Grid origin, int range, bool include)
        {
            (int, int, bool) key = (origin.Id, range, include);
            if (!RangeCache.TryGetValue(key, out List<Grid>? result)) RangeCache[key] = result = Queue.Map?.GetGridsByRange(origin, range, include) ?? [];
            return result;
        }

        internal bool IsInRange(Grid origin, Grid target, int range) => DecisionGeometry.UsesDefaultCastRange(Queue.Map!)
            ? target.Z == origin.Z && GameMap.CalculateManhattanDistance(origin, target) <= range && ReferenceEquals(Queue.Map![target.X, target.Y, target.Z], target)
            : GetRange(origin, range, true).Contains(target);

        internal int GetMoveSteps(Character actor, Grid origin, Grid target)
        {
            if (ReferenceEquals(origin, target)) return 0;
            (Character, int) key = (actor, origin.Id);
            if (!MovementCache.TryGetValue(key, out IReadOnlyDictionary<Grid, int>? costs))
                MovementCache[key] = costs = Queue.Map?.GetMovementCosts(actor, origin) ?? new Dictionary<Grid, int>();
            return costs.TryGetValue(target, out int steps) ? steps : -1;
        }
    }

    /// <summary>绑定目标、位置和范围的单一施法方案。</summary>
    public sealed class DecisionPlan
    {
        /// <summary>队列执行输出。</summary>
        public required AIDecision Decision { get; init; }
        /// <summary>判断条件时绑定的主要目标。</summary>
        public Character? Target { get; init; }
        /// <summary>施法中心或方向参考格。</summary>
        public Grid? Center { get; init; }
        /// <summary>合法选择格。</summary>
        public IReadOnlyList<Grid> SelectedGrids { get; init; } = [];
        /// <summary>实际覆盖目标数。</summary>
        public int TargetCount => Decision.Targets.Count;
        /// <summary>位置近似威胁。</summary>
        public double Risk { get; init; }
        /// <summary>实际路径步数。</summary>
        public int MoveSteps { get; init; }
    }

    /// <summary>规则匹配结果。</summary>
    /// <param name="Decision">未命中时为空。</param>
    /// <param name="RuleId">命中规则。</param>
    /// <param name="Diagnostics">无法执行原因。</param>
    public sealed record DecisionEvaluationResult(AIDecision? Decision, string? RuleId, IReadOnlyList<DecisionDiagnostic> Diagnostics)
    {
        /// <summary>是否取得有效决策。</summary>
        public bool Matched => Decision != null;
    }

    /// <summary>单次决策计数，不跨战斗保存。</summary>
    public sealed class DecisionMetrics
    {
        /// <summary>生成的候选位置数量。</summary>
        public int PositionsGenerated { get; internal set; }
        /// <summary>完整评估的位置次数。</summary>
        public int PositionsEvaluated { get; internal set; }
        /// <summary>已生成并保留的候选方案数，不含常量结束回合。</summary>
        public int PlansEvaluated { get; internal set; }
        /// <summary>策略阶段保留的方案数。</summary>
        public int PolicyPlansEvaluated { get; internal set; }
        /// <summary>默认 AI 阶段保留的方案数。</summary>
        public int DefaultAIPlansEvaluated { get; internal set; }
        /// <summary>因位置、中心或方案上限截断的预算范围数，每个范围至多计一次。</summary>
        public int BudgetTruncations { get; internal set; }
        /// <summary>因没有方案额度而跳过规划的次数。</summary>
        public int BudgetExhaustions { get; internal set; }
        /// <summary>实际计算位置风险的次数。</summary>
        public int RiskEvaluations { get; internal set; }
        /// <summary>命中位置风险缓存的次数。</summary>
        public int RiskCacheHits { get; internal set; }
        /// <summary>生成的施法中心候选数量，按位置累计。</summary>
        public int CentersGenerated { get; internal set; }
        /// <summary>完整投影并检查覆盖的施法中心次数。</summary>
        public int CentersEvaluated { get; internal set; }
        /// <summary>决策耗时。</summary>
        public TimeSpan Elapsed { get; internal set; }

        internal DecisionMetrics Snapshot() => (DecisionMetrics)MemberwiseClone();
    }
}
