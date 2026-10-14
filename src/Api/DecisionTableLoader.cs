using FunGame.Core.Entity;
using FunGame.Core.Interface;
using FunGame.Core.Model.DecisionTable;

namespace FunGame.Core.Api
{
    /// <summary>策略目录，成功编译后才注册；不静默替换失败配置。</summary>
    /// <param name="compiler">资源及扩展编译器。</param>
    public sealed class DecisionTableLoader(DecisionTableCompiler? compiler = null)
    {
        private readonly DecisionTableCompiler _compiler = compiler ?? new();
        private readonly Dictionary<string, IDecisionPolicy> _policies = [];
        /// <summary>注册代码策略；实现者负责按角色与战斗隔离可变状态。</summary>
        /// <param name="id">策略标识。</param>
        /// <param name="policy">策略实现。</param>
        /// <returns>标识或重复注册错误。</returns>
        public IReadOnlyList<DecisionDiagnostic> Register(string id, IDecisionPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(policy);
            if (string.IsNullOrWhiteSpace(id) || !id.Contains(':')) return [new("id", "需要 module:name 标识。")];
            if (!_policies.TryAdd(id, policy)) return [new("id", $"策略 {id} 已注册。")];
            return [];
        }
        /// <summary>注册配置；重复标识返回错误。</summary>
        /// <param name="config">配置。</param>
        /// <returns>校验错误。</returns>
        public IReadOnlyList<DecisionDiagnostic> Register(DecisionTableConfig config)
        {
            IReadOnlyList<DecisionDiagnostic> errors = _compiler.Validate(config);
            if (errors.Count > 0) return errors;
            if (_policies.ContainsKey(config.Id)) return [new("id", $"策略 {config.Id} 已注册。")];
            _policies.Add(config.Id, _compiler.Compile(config));
            return [];
        }
        /// <summary>从文件加载单个策略。</summary>
        /// <param name="path">JSON 路径。</param>
        /// <returns>加载错误。</returns>
        public IReadOnlyList<DecisionDiagnostic> Load(string path)
        {
            try
            {
                return DecisionTableParser.TryParse(File.ReadAllText(path), out DecisionTableConfig? config, out IReadOnlyList<DecisionDiagnostic> diagnostics) ? Register(config!) : diagnostics;
            }
            catch (IOException error) { return [new(path, error.Message)]; }
            catch (UnauthorizedAccessException error) { return [new(path, error.Message)]; }
        }
        /// <summary>查询编译策略。</summary>
        /// <param name="id">策略标识。</param>
        /// <returns>未注册时为空。</returns>
        public IDecisionPolicy? Get(string id) => _policies.GetValueOrDefault(id);
        /// <summary>按角色 ID 绑定策略；无 ID 时解除绑定。</summary>
        /// <param name="character">角色。</param>
        /// <returns>绑定错误。</returns>
        public IReadOnlyList<DecisionDiagnostic> Bind(Character character)
        {
            if (character.DecisionTableId == null) { character.DecisionPolicy = null; return []; }
            IDecisionPolicy? policy = Get(character.DecisionTableId);
            if (policy == null) { character.DecisionPolicy = null; return [new("decisionTableId", $"未找到策略 {character.DecisionTableId}。")]; }
            character.DecisionPolicy = policy;
            return [];
        }
    }
}
