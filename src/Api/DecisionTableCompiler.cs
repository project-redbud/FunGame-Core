using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Interface;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Api
{
    /// <summary>自定义纯条件和动作规划器的注册表。</summary>
    public sealed class DecisionTableRegistry
    {
        internal Dictionary<string, Func<DecisionEvaluationContext, ConditionConfig, DecisionPlan, bool>> Conditions { get; } = [];
        internal Dictionary<string, Func<DecisionEvaluationContext, ActionConfig, TargetSelectorConfig?, IReadOnlyList<DecisionPlan>>> Actions { get; } = [];
        /// <summary>注册条件，不覆盖已有标识。</summary>
        /// <param name="id">条件标识。</param>
        /// <param name="evaluate">只读条件求值。</param>
        /// <exception cref="ArgumentException">扩展标识格式无效或已经注册。</exception>
        public void RegisterCondition(string id, Func<DecisionEvaluationContext, ConditionConfig, DecisionPlan, bool> evaluate)
        {
            if (string.IsNullOrWhiteSpace(id) || !id.Contains(':')) throw new ArgumentException("扩展标识需要 module:name。", nameof(id));
            ArgumentNullException.ThrowIfNull(evaluate);
            Conditions.Add(id, evaluate);
        }
        /// <summary>注册动作，不覆盖已有标识。</summary>
        /// <param name="id">动作标识。</param>
        /// <param name="plan">只读规划器；上下文携带本条方案额度，返回集合受该额度限制。</param>
        /// <exception cref="ArgumentException">扩展标识格式无效或已经注册。</exception>
        public void RegisterAction(string id, Func<DecisionEvaluationContext, ActionConfig, TargetSelectorConfig?, IReadOnlyList<DecisionPlan>> plan)
        {
            if (string.IsNullOrWhiteSpace(id) || !id.Contains(':')) throw new ArgumentException("扩展标识需要 module:name。", nameof(id));
            ArgumentNullException.ThrowIfNull(plan);
            Actions.Add(id, plan);
        }
    }

    /// <summary>配置校验与不可变策略编译。</summary>
    public sealed partial class DecisionTableCompiler
    {
        private static readonly HashSet<string> ActionTypes = ["normalAttack", "castSkill", "useItem", "approach", "retreat", "endTurn"];
        private static readonly HashSet<string> ConditionTypes = ["self.hpRatio", "target.hpRatio", "self.mp", "self.ep", "self.state", "skill.available", "decisionPoints", "target.exists", "distance", "plan.targetCount", "plan.risk"];
        private static readonly HashSet<string> Comparisons = ["lessThan", "atMost", "equal", "atLeast", "greaterThan"];
        private readonly DecisionTableRegistry _registry;
        private readonly HashSet<long>? _skills;
        private readonly HashSet<long>? _items;

        /// <summary>创建编译器；省略资源目录时在运行时解析资源。</summary>
        /// <param name="registry">扩展注册表。</param>
        /// <param name="skills">模组技能标识。</param>
        /// <param name="items">模组物品标识。</param>
        public DecisionTableCompiler(DecisionTableRegistry? registry = null, IEnumerable<long>? skills = null, IEnumerable<long>? items = null)
        {
            _registry = registry ?? new();
            _skills = skills == null ? null : [.. skills];
            _items = items == null ? null : [.. items];
        }

        /// <summary>校验配置，包含规则路径。</summary>
        /// <param name="config">配置。</param>
        /// <returns>错误列表。</returns>
        public IReadOnlyList<DecisionDiagnostic> Validate(DecisionTableConfig config)
        {
            List<DecisionDiagnostic> errors = [];
            if (config.SchemaVersion != 1) errors.Add(new("schemaVersion", "仅支持版本 1。"));
            if (string.IsNullOrWhiteSpace(config.Id) || !config.Id.Contains(':')) errors.Add(new("id", "需要 module:name 标识。"));
            if (config.Fallback is not ("defaultAI" or "endTurn")) errors.Add(new("fallback", "未知回退方式。"));
            if (config.Rules == null || config.Rules.Count > 256) { errors.Add(new("rules", "规则列表不可为 null，允许空表，最多 256 条。")); return errors; }
            HashSet<string> ids = [];
            for (int index = 0; index < config.Rules.Count; index++)
            {
                DecisionRuleConfig? rule = config.Rules[index];
                string path = $"rules[{index}]";
                if (rule == null) { errors.Add(new(path, "规则不能为空。")); continue; }
                if (string.IsNullOrWhiteSpace(rule.Id) || !ids.Add(rule.Id)) errors.Add(new(path + ".id", "标识为空或重复。"));
                if (rule.Trigger != "beforeDecision") errors.Add(new(path + ".trigger", "仅支持 beforeDecision。"));
                TargetSelectorConfig? target = rule.Target;
                if (target != null && (target.Relation is not ("self" or "ally" or "enemy") || target.Selector is not ("self" or "lowestHpRatio" or "highestHpRatio" or "nearest") || string.IsNullOrWhiteSpace(target.Bind))) errors.Add(new(path + ".target", "目标关系、选择器或绑定无效。"));
                ActionConfig? action = rule.Action;
                if (action == null) { errors.Add(new(path + ".action", "缺少动作。")); continue; }
                if (action.Type == null || !ActionTypes.Contains(action.Type) && !_registry.Actions.ContainsKey(action.Type)) errors.Add(new(path + ".action.type", "未知动作。"));
                if (action.Movement is not ("never" or "ifNeeded" or "required")) errors.Add(new(path + ".action.movement", "未知移动策略。"));
                if (action.PositionPreference is not ("current" or "coverage" or "safe")) errors.Add(new(path + ".action.positionPreference", "未知位置偏好。"));
                if (action.Target != null && action.Target != target?.Bind) errors.Add(new(path + ".action.target", "未定义目标绑定。"));
                if (action.Parameters == null) errors.Add(new(path + ".action.parameters", "参数对象不能为空。"));
                if (action.MaxRisk is double risk && (!double.IsFinite(risk) || risk < 0)) errors.Add(new(path + ".action.maxRisk", "风险必须是非负有限数。"));
                if (action.Type == "castSkill" && action.SkillId == null) errors.Add(new(path + ".action.skillId", "缺少技能标识。"));
                if (action.Type == "useItem" && action.ItemId == null) errors.Add(new(path + ".action.itemId", "缺少物品标识。"));
                CheckResource(action.SkillId, _skills, path + ".action.skillId", errors);
                CheckResource(action.ItemId, _items, path + ".action.itemId", errors);
                ValidateCondition(rule.When, target?.Bind, action.BindPlan, path + ".when", 0, errors);
            }
            return errors;
        }

        private void ValidateCondition(ConditionConfig? condition, string? target, string? plan, string path, int depth, List<DecisionDiagnostic> errors)
        {
            if (condition == null) return;
            if (depth > 16) { errors.Add(new(path, "条件根节点深度为 0，最多嵌套 16 级。")); return; }
            int forms = (condition.All != null ? 1 : 0) + (condition.Any != null ? 1 : 0) + (condition.Not != null ? 1 : 0) + (!string.IsNullOrEmpty(condition.Type) ? 1 : 0);
            if (forms != 1) { errors.Add(new(path, "条件只能包含一种形式。")); return; }
            List<ConditionConfig>? children = condition.All ?? condition.Any;
            if (children != null)
            {
                if (children.Count > 64) { errors.Add(new(path, "单个组合最多 64 个条件。")); return; }
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] == null) errors.Add(new($"{path}[{i}]", "条件不能为空。"));
                    else ValidateCondition(children[i], target, plan, $"{path}[{i}]", depth + 1, errors);
                }
                return;
            }
            if (condition.Not != null) { ValidateCondition(condition.Not, target, plan, path + ".not", depth + 1, errors); return; }
            if (condition.Type == null) { errors.Add(new(path + ".type", "条件标识不能为空。")); return; }
            if (condition.Parameters == null) errors.Add(new(path + ".parameters", "参数对象不能为空。"));
            if (!ConditionTypes.Contains(condition.Type) && !_registry.Conditions.ContainsKey(condition.Type)) errors.Add(new(path + ".type", "未知条件。"));
            if (!Comparisons.Contains(condition.Operator) || !double.IsFinite(condition.Value)) errors.Add(new(path, "比较方式或数值无效。"));
            if (condition.Type.EndsWith("hpRatio") && (condition.Value < 0 || condition.Value > 1)) errors.Add(new(path + ".value", "生命比例必须在 0 至 1。"));
            if ((condition.Type.StartsWith("target.") || condition.Type == "distance") && (target == null || condition.Target != target)) errors.Add(new(path + ".target", "未定义目标绑定。"));
            if (condition.Type.StartsWith("plan.") && (plan == null || condition.Target != plan)) errors.Add(new(path + ".target", "未定义方案绑定。"));
            if (condition.Type == "skill.available" && condition.SkillId == null) errors.Add(new(path + ".skillId", "缺少技能标识。"));
            CheckResource(condition.SkillId, _skills, path + ".skillId", errors);
        }

        private static void CheckResource(long? id, HashSet<long>? resources, string path, List<DecisionDiagnostic> errors)
        {
            if (id is long value && resources != null && !resources.Contains(value)) errors.Add(new(path, $"未找到资源 {value}。"));
        }

        /// <summary>校验后编译独立快照；无效配置抛出 ArgumentException。</summary>
        /// <param name="config">配置。</param>
        /// <returns>可共享的只读策略。</returns>
        public IDecisionPolicy Compile(DecisionTableConfig config)
        {
            IReadOnlyList<DecisionDiagnostic> errors = Validate(config);
            if (errors.Count > 0) throw new ArgumentException(string.Join(Environment.NewLine, errors.Select(e => $"{e.Path}：{e.Message}")), nameof(config));
            return new CompiledPolicy(DecisionTableParser.Parse(DecisionTableGenerator.GetJson(config)), _registry);
        }

        private sealed partial class CompiledPolicy : IDecisionPolicy
        {
            private readonly DecisionTableConfig _config;
            private readonly Dictionary<string, Func<DecisionEvaluationContext, ConditionConfig, DecisionPlan, bool>> _conditions;
            private readonly Dictionary<string, Func<DecisionEvaluationContext, ActionConfig, TargetSelectorConfig?, IReadOnlyList<DecisionPlan>>> _actions;
            internal CompiledPolicy(DecisionTableConfig config, DecisionTableRegistry registry)
            {
                _config = config;
                _conditions = new(registry.Conditions);
                _actions = new(registry.Actions);
            }

            private bool RequiresPositions(ConditionConfig? condition) => condition != null &&
                (condition.Type == "distance" || condition.Type != null && _conditions.ContainsKey(condition.Type) ||
                    condition.All?.Any(RequiresPositions) == true || condition.Any?.Any(RequiresPositions) == true || RequiresPositions(condition.Not));

            private bool EvaluateCondition(DecisionEvaluationContext context, ConditionConfig? condition, DecisionPlan plan)
            {
                if (condition == null) return true;
                if (condition.All != null) return condition.All.All(c => EvaluateCondition(context, c, plan));
                if (condition.Any != null) return condition.Any.Any(c => EvaluateCondition(context, c, plan));
                if (condition.Not != null) return !EvaluateCondition(context, condition.Not, plan);
                if (_conditions.TryGetValue(condition.Type, out Func<DecisionEvaluationContext, ConditionConfig, DecisionPlan, bool>? custom)) return custom(context, condition, plan);
                if (condition.Type == "target.exists") return plan.Target != null;
                if (condition.Type == "skill.available") return context.Skills.Any(s => s.Id == condition.SkillId && DecisionActionPlanner.CanUse(context, "castSkill", s));
                if (condition.Type == "distance" && (plan.Target == null || plan.Decision.TargetMoveGrid == null || context.Queue.Map?.GetCharacterCurrentGrid(plan.Target) == null)) return false;
                if (condition.Type == "plan.risk" && context.Queue.Map == null) return false;
                double value = condition.Type switch
                {
                    "self.hpRatio" => Ratio(context.Actor), "target.hpRatio" => plan.Target == null ? double.NaN : Ratio(plan.Target),
                    "self.mp" => context.Actor.MP, "self.ep" => context.Actor.EP, "self.state" => (int)context.Actor.CharacterState,
                    "decisionPoints" => context.Points.CurrentDecisionPoints, "plan.targetCount" => plan.TargetCount, "plan.risk" => plan.Risk,
                    "distance" => GameMap.CalculateManhattanDistance(plan.Decision.TargetMoveGrid!, context.Queue.Map!.GetCharacterCurrentGrid(plan.Target!)!), _ => double.NaN
                };
                return condition.Operator switch { "lessThan" => value < condition.Value, "atMost" => value <= condition.Value, "equal" => value == condition.Value, "greaterThan" => value > condition.Value, _ => value >= condition.Value };
            }

            private static double Ratio(Character character) => character.MaxHP > 0 ? character.HP / character.MaxHP : 0;
        }
    }
}
