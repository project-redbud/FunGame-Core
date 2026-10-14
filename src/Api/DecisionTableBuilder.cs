using FunGame.Core.Model.DecisionTable;

namespace FunGame.Core.Api
{
    /// <summary>只构建配置，不执行规则或读写文件。</summary>
    public sealed class DecisionTableBuilder
    {
        private readonly DecisionTableConfig _config;
        private DecisionTableBuilder(string id) => _config = new() { Id = id };
        /// <summary>创建构建器。</summary>
        /// <param name="id">策略标识。</param>
        /// <returns>构建器。</returns>
        public static DecisionTableBuilder Create(string id) => new(id);
        /// <summary>设置协议版本。</summary>
        /// <param name="version">版本。</param>
        /// <returns>构建器。</returns>
        public DecisionTableBuilder WithSchemaVersion(int version) { _config.SchemaVersion = version; return this; }
        /// <summary>设置回退方式。</summary>
        /// <param name="mode">defaultAI 或 endTurn。</param>
        /// <returns>构建器。</returns>
        public DecisionTableBuilder WithFallback(string mode) { _config.Fallback = mode; return this; }
        /// <summary>添加独立规则。</summary>
        /// <param name="id">规则标识。</param>
        /// <param name="configure">仅在构建时执行的配置回调。</param>
        /// <returns>构建器。</returns>
        public DecisionTableBuilder AddRule(string id, Action<DecisionRuleBuilder> configure)
        {
            DecisionRuleBuilder rule = new(id);
            configure(rule);
            _config.Rules.Add(rule.Config);
            return this;
        }
        /// <summary>创建经校验的独立配置快照。</summary>
        /// <param name="compiler">可选的资源与扩展校验器。</param>
        /// <returns>配置快照。</returns>
        public DecisionTableConfig Build(DecisionTableCompiler? compiler = null)
        {
            if (!TryBuild(out DecisionTableConfig? config, out IReadOnlyList<DecisionDiagnostic> diagnostics, compiler))
                throw new ArgumentException(string.Join(Environment.NewLine, diagnostics.Select(d => $"{d.Path}：{d.Message}")));
            return config!;
        }
        /// <summary>尝试构建独立快照。</summary>
        /// <param name="config">结果。</param>
        /// <param name="diagnostics">校验错误。</param>
        /// <param name="compiler">可选校验器。</param>
        /// <returns>是否成功。</returns>
        public bool TryBuild(out DecisionTableConfig? config, out IReadOnlyList<DecisionDiagnostic> diagnostics, DecisionTableCompiler? compiler = null)
        {
            diagnostics = (compiler ?? new()).Validate(_config);
            config = diagnostics.Count == 0 ? DecisionTableParser.Parse(DecisionTableGenerator.GetJson(_config)) : null;
            return config != null;
        }
    }

    /// <summary>规则配置构建器。</summary>
    public sealed class DecisionRuleBuilder
    {
        internal DecisionRuleConfig Config { get; }
        internal DecisionRuleBuilder(string id) => Config = new() { Id = id };
        /// <summary>设置优先级。</summary>
        /// <param name="value">优先级。</param>
        /// <returns>构建器。</returns>
        public DecisionRuleBuilder Priority(int value) { Config.Priority = value; return this; }
        /// <summary>设置启用状态。</summary>
        /// <param name="value">是否启用。</param>
        /// <returns>构建器。</returns>
        public DecisionRuleBuilder Enabled(bool value) { Config.Enabled = value; return this; }
        /// <summary>设置触发时机。</summary>
        /// <param name="trigger">触发标识。</param>
        /// <returns>构建器。</returns>
        public DecisionRuleBuilder On(string trigger) { Config.Trigger = trigger; return this; }
        /// <summary>设置目标描述。</summary>
        /// <param name="selector">目标。</param>
        /// <returns>构建器。</returns>
        public DecisionRuleBuilder Target(TargetSelectorConfig selector) { Config.Target = selector; return this; }
        /// <summary>设置条件。</summary>
        /// <param name="condition">条件描述。</param>
        /// <returns>构建器。</returns>
        public DecisionRuleBuilder When(ConditionConfig condition) { Config.When = condition; return this; }
        /// <summary>设置动作。</summary>
        /// <param name="action">动作描述。</param>
        /// <returns>构建器。</returns>
        public DecisionRuleBuilder Then(ActionConfig action) { Config.Action = action; return this; }
    }

    /// <summary>内置条件描述工厂。</summary>
    public static class Conditions
    {
        /// <summary>所有条件成立。</summary>
        /// <param name="conditions">子条件。</param>
        /// <returns>组合条件。</returns>
        public static ConditionConfig All(params ConditionConfig[] conditions) => new() { All = [.. conditions] };
        /// <summary>任一条件成立。</summary>
        /// <param name="conditions">子条件。</param>
        /// <returns>组合条件。</returns>
        public static ConditionConfig Any(params ConditionConfig[] conditions) => new() { Any = [.. conditions] };
        /// <summary>条件取反。</summary>
        /// <param name="condition">子条件。</param>
        /// <returns>组合条件。</returns>
        public static ConditionConfig Not(ConditionConfig condition) => new() { Not = condition };
        /// <summary>自身生命比例。</summary>
        /// <param name="comparison">比较标识。</param>
        /// <param name="value">比例。</param>
        /// <returns>条件。</returns>
        public static ConditionConfig SelfHpRatio(string comparison, double value) => Compare("self.hpRatio", comparison, value);
        /// <summary>绑定目标的生命比例。</summary>
        /// <param name="target">绑定名。</param>
        /// <param name="comparison">比较标识。</param>
        /// <param name="value">比例。</param>
        /// <returns>条件。</returns>
        public static ConditionConfig TargetHpRatio(string target, string comparison, double value) => Compare("target.hpRatio", comparison, value, target);
        /// <summary>技能可用。</summary>
        /// <param name="id">技能标识。</param>
        /// <returns>条件。</returns>
        public static ConditionConfig SkillAvailable(long id) => new() { Type = "skill.available", SkillId = id };
        /// <summary>比较内置数值条件。</summary>
        /// <param name="type">条件标识。</param>
        /// <param name="comparison">比较标识。</param>
        /// <param name="value">数值。</param>
        /// <param name="binding">目标或方案绑定。</param>
        /// <returns>条件。</returns>
        public static ConditionConfig Compare(string type, string comparison, double value, string? binding = null) => new() { Type = type, Operator = comparison, Value = value, Target = binding };
    }

    /// <summary>内置目标描述工厂。</summary>
    public static class Targets
    {
        /// <summary>自身目标。</summary>
        /// <param name="bind">绑定名。</param>
        /// <returns>描述。</returns>
        public static TargetSelectorConfig Self(string bind = "self") => new() { Relation = "self", Selector = "self", Bind = bind };
        /// <summary>友方目标，包含自身。</summary>
        /// <param name="bind">绑定名。</param>
        /// <param name="order">排序标识。</param>
        /// <returns>描述。</returns>
        public static TargetSelectorConfig Allies(string bind = "ally", string order = "lowestHpRatio") => new() { Relation = "ally", Selector = order, Bind = bind };
        /// <summary>敌方目标。</summary>
        /// <param name="bind">绑定名。</param>
        /// <param name="order">排序标识。</param>
        /// <returns>描述。</returns>
        public static TargetSelectorConfig Enemies(string bind = "enemy", string order = "lowestHpRatio") => new() { Relation = "enemy", Selector = order, Bind = bind };
    }

    /// <summary>内置动作描述工厂。</summary>
    public static class Actions
    {
        /// <summary>释放技能。</summary>
        /// <param name="id">技能标识。</param>
        /// <param name="target">绑定目标。</param>
        /// <param name="movement">移动策略。</param>
        /// <returns>描述。</returns>
        public static ActionConfig CastSkill(long id, string? target = null, string movement = "ifNeeded") => new() { Type = "castSkill", SkillId = id, Target = target, Movement = movement };
        /// <summary>普通攻击。</summary>
        /// <param name="target">绑定目标。</param>
        /// <param name="movement">移动策略。</param>
        /// <returns>描述。</returns>
        public static ActionConfig NormalAttack(string? target = null, string movement = "ifNeeded") => new() { Type = "normalAttack", Target = target, Movement = movement };
        /// <summary>使用物品。</summary>
        /// <param name="id">物品标识。</param>
        /// <param name="target">绑定目标。</param>
        /// <returns>描述。</returns>
        public static ActionConfig UseItem(long id, string? target = null) => new() { Type = "useItem", ItemId = id, Target = target };
        /// <summary>主动结束回合。</summary>
        /// <returns>描述。</returns>
        public static ActionConfig EndTurn() => new() { Type = "endTurn", Movement = "never" };
    }
}
