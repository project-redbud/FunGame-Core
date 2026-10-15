using System.Text.Json;

namespace FunGame.Core.Model.DecisionTable
{
    /// <summary>可序列化的角色决策表。</summary>
    public sealed class DecisionTableConfig
    {
        /// <summary>协议版本。</summary>
        public int SchemaVersion { get; set; } = 1;
        /// <summary>包含模组命名空间的策略标识。</summary>
        public string Id { get; set; } = "";
        /// <summary>未命中时使用 defaultAI 或 endTurn。</summary>
        public string Fallback { get; set; } = "defaultAI";
        /// <summary>同优先级按配置顺序匹配的规则；不可为 null，允许空表，最多 256 条。</summary>
        public List<DecisionRuleConfig> Rules { get; set; } = [];
    }

    /// <summary>条件成立且行动可执行时命中的规则。</summary>
    public sealed class DecisionRuleConfig
    {
        /// <summary>表内唯一标识。</summary>
        public string Id { get; set; } = "";
        /// <summary>是否启用。</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>数值越大越先匹配。</summary>
        public int Priority { get; set; }
        /// <summary>触发时机，当前支持 beforeDecision。</summary>
        public string Trigger { get; set; } = "beforeDecision";
        /// <summary>目标筛选、排序与绑定。</summary>
        public TargetSelectorConfig? Target { get; set; }
        /// <summary>省略时无条件匹配；根节点深度为 0，最多嵌套 16 级。</summary>
        public ConditionConfig? When { get; set; }
        /// <summary>执行意图。</summary>
        public ActionConfig Action { get; set; } = new();
    }

    /// <summary>条件树或注册的条件描述。</summary>
    public sealed class ConditionConfig
    {
        /// <summary>内置或注册标识。</summary>
        public string Type { get; set; } = "";
        /// <summary>全部成立。</summary>
        public List<ConditionConfig>? All { get; set; }
        /// <summary>至少一个成立。</summary>
        public List<ConditionConfig>? Any { get; set; }
        /// <summary>取反。</summary>
        public ConditionConfig? Not { get; set; }
        /// <summary>引用的目标绑定。</summary>
        public string? Target { get; set; }
        /// <summary>技能标识。</summary>
        public long? SkillId { get; set; }
        /// <summary>比较方式：lessThan、atMost、equal、atLeast、greaterThan。</summary>
        public string Operator { get; set; } = "atLeast";
        /// <summary>比较值。</summary>
        public double Value { get; set; }
        /// <summary>扩展条件参数。</summary>
        public Dictionary<string, JsonElement> Parameters { get; set; } = [];
    }

    /// <summary>目标描述；运行时从动作合法目标中逐个尝试。</summary>
    public sealed class TargetSelectorConfig
    {
        /// <summary>self、ally 或 enemy。</summary>
        public string Relation { get; set; } = "enemy";
        /// <summary>self、lowestHpRatio、nearest 或 highestHpRatio。</summary>
        public string Selector { get; set; } = "lowestHpRatio";
        /// <summary>绑定名称。</summary>
        public string Bind { get; set; } = "target";
    }

    /// <summary>动作与位置偏好。</summary>
    public sealed class ActionConfig
    {
        /// <summary>normalAttack、castSkill、useItem、approach、retreat 或 endTurn。</summary>
        public string Type { get; set; } = "";
        /// <summary>从当前角色解析的技能标识。</summary>
        public long? SkillId { get; set; }
        /// <summary>从当前角色解析的物品标识。</summary>
        public long? ItemId { get; set; }
        /// <summary>引用的主要目标绑定。</summary>
        public string? Target { get; set; }
        /// <summary>never、ifNeeded 或 required；协同始终禁止移动。</summary>
        public string Movement { get; set; } = "ifNeeded";
        /// <summary>current、coverage 或 safe。</summary>
        public string PositionPreference { get; set; } = "current";
        /// <summary>范围动作的方案绑定名。</summary>
        public string? BindPlan { get; set; }
        /// <summary>可接受的风险上限，省略时不限。</summary>
        public double? MaxRisk { get; set; }
        /// <summary>扩展动作的可序列化参数。</summary>
        public Dictionary<string, JsonElement> Parameters { get; set; } = [];
    }

    /// <summary>定位配置错误或规则执行结果。</summary>
    /// <param name="Path">配置路径或规则标识。</param>
    /// <param name="Message">简洁原因。</param>
    public sealed record DecisionDiagnostic(string Path, string Message)
    {
        /// <summary>机器可读的运行诊断代码；未分类的配置诊断为空。</summary>
        public string Code { get; init; } = "";
    }
}
