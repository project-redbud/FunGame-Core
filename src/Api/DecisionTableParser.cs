using System.Text.Json;
using System.Text.Json.Serialization;
using FunGame.Core.Model.DecisionTable;

namespace FunGame.Core.Api
{
    /// <summary>决策配置 JSON 解析。</summary>
    public static class DecisionTableParser
    {
        internal static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            MaxDepth = 64
        };

        /// <summary>解析配置，格式错误抛出 JsonException。</summary>
        /// <param name="json">配置 JSON。</param>
        /// <returns>配置对象。</returns>
        public static DecisionTableConfig Parse(string json) => JsonSerializer.Deserialize<DecisionTableConfig>(json, Options) ?? throw new JsonException("决策表不能为空。");

        /// <summary>解析配置并返回结构化错误。</summary>
        /// <param name="json">配置 JSON。</param>
        /// <param name="config">成功时的配置。</param>
        /// <param name="diagnostics">格式错误。</param>
        /// <returns>是否成功。</returns>
        public static bool TryParse(string json, out DecisionTableConfig? config, out IReadOnlyList<DecisionDiagnostic> diagnostics)
        {
            try { config = Parse(json); diagnostics = []; return true; }
            catch (JsonException error) { config = null; diagnostics = [new(error.Path ?? "$", error.Message)]; return false; }
        }
    }

    /// <summary>生成可独立加载的配置 JSON。</summary>
    public static class DecisionTableGenerator
    {
        /// <summary>生成 JSON。</summary>
        /// <param name="config">配置。</param>
        /// <returns>配置文本。</returns>
        public static string GetJson(DecisionTableConfig config) => JsonSerializer.Serialize(config, DecisionTableParser.Options);

        /// <summary>保存配置，默认拒绝覆盖。</summary>
        /// <param name="config">配置。</param>
        /// <param name="path">文件路径。</param>
        /// <param name="overwrite">是否覆盖。</param>
        public static void Save(DecisionTableConfig config, string path, bool overwrite = false)
        {
            using FileStream stream = new(path, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write);
            using StreamWriter writer = new(stream, new System.Text.UTF8Encoding(false));
            writer.Write(GetJson(config));
        }
    }
}
