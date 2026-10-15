using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Interface;
using FunGame.Core.Model.DecisionTable;

namespace FunGame.Core.Tests.Queue;

public sealed class DecisionTableBoundaryTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(256)]
    [InlineData(257)]
    public void RuleCount_AllowsEmptyAndDiagnosesOnlyOverflow(int count)
    {
        DecisionTableConfig config = new() { Id = "test:count", Rules = [.. Enumerable.Range(0, count)
            .Select(index => new DecisionRuleConfig { Id = $"rule-{index}", Action = Actions.EndTurn() })] };
        DecisionTableCompiler compiler = new();
        if (count <= 256)
        {
            Assert.Empty(compiler.Validate(config));
            compiler.Compile(config);
        }
        else
        {
            DecisionDiagnostic error = Assert.Single(compiler.Validate(config));
            Assert.Equal("rules", error.Path);
            Assert.Contains("最多 256 条", error.Message);
            Assert.Throws<ArgumentException>(() => compiler.Compile(config));
        }
    }

    [Fact]
    public void NullRules_DiagnosticDistinguishesNullFromEmpty()
    {
        DecisionTableConfig config = DecisionTableParser.Parse("""{"id":"test:null","rules":null}""");
        DecisionDiagnostic error = Assert.Single(new DecisionTableCompiler().Validate(config));
        Assert.Equal("rules", error.Path);
        Assert.Contains("不可为 null", error.Message);
        Assert.Contains("允许空表", error.Message);
    }

    [Theory]
    [InlineData(false, 64)]
    [InlineData(false, 65)]
    [InlineData(true, 64)]
    [InlineData(true, 65)]
    public void CompositeChildren_EnforcesPerNodeLimit(bool any, int count)
    {
        List<ConditionConfig> children = [.. Enumerable.Range(0, count).Select(_ => Conditions.SelfHpRatio("atLeast", 0))];
        ConditionConfig condition = any ? new() { Any = children } : new() { All = children };
        DecisionTableConfig config = Config(condition);
        DecisionTableCompiler compiler = new();
        if (count == 64)
        {
            Assert.Empty(compiler.Validate(config));
            compiler.Compile(config);
        }
        else
        {
            DecisionDiagnostic error = Assert.Single(compiler.Validate(config));
            Assert.Equal("rules[0].when", error.Path);
            Assert.Contains("64", error.Message);
        }
    }

    [Theory]
    [InlineData("not", 16)]
    [InlineData("not", 17)]
    [InlineData("all", 16)]
    [InlineData("all", 17)]
    [InlineData("any", 16)]
    [InlineData("any", 17)]
    public void ConditionDepth_CountsRootAsZero(string form, int depth)
    {
        ConditionConfig condition = Conditions.SelfHpRatio("atLeast", 0);
        string path = "rules[0].when";
        for (int index = 0; index < depth; index++)
        {
            condition = form switch { "not" => new() { Not = condition }, "all" => new() { All = [condition] }, _ => new() { Any = [condition] } };
            path += form == "not" ? ".not" : "[0]";
        }
        DecisionTableConfig config = DecisionTableParser.Parse(DecisionTableGenerator.GetJson(Config(condition)));
        DecisionTableCompiler compiler = new();
        if (depth == 16)
        {
            Assert.Empty(compiler.Validate(config));
            compiler.Compile(config);
        }
        else
        {
            DecisionDiagnostic error = Assert.Single(compiler.Validate(config));
            Assert.Equal(path, error.Path);
            Assert.Contains("根节点深度为 0", error.Message);
            Assert.Contains("最多嵌套 16 级", error.Message);
        }
    }

    [Theory]
    [InlineData(64)]
    [InlineData(65)]
    public void JsonDepth_IsIndependentOfConditionTreeLimit(int depth)
    {
        int arrays = depth - 5;
        string json = "{\"id\":\"test:json-depth\",\"rules\":[{\"id\":\"one\",\"action\":{\"type\":\"endTurn\",\"parameters\":{\"nested\":" +
            new string('[', arrays) + "0" + new string(']', arrays) + "}}}]}";
        bool parsed = DecisionTableParser.TryParse(json, out DecisionTableConfig? config, out IReadOnlyList<DecisionDiagnostic> errors);
        Assert.Equal(depth == 64, parsed);
        if (parsed)
        {
            Assert.Empty(errors);
            Assert.Empty(new DecisionTableCompiler().Validate(config!));
            new DecisionTableCompiler().Compile(config!);
        }
        else
        {
            Assert.Null(config);
            Assert.Single(errors);
            Assert.Throws<JsonException>(() => DecisionTableParser.Parse(json));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("bad\0name.json")]
    public void Loader_InvalidPathReturnsInputDiagnostic(string? path)
    {
        DecisionTableLoader loader = new();
        DecisionDiagnostic error = Assert.Single(loader.Load(path!));
        Assert.Equal(path ?? "path", error.Path);
        Assert.False(string.IsNullOrWhiteSpace(error.Message));
        Assert.Null(loader.Get("test:loaded"));
    }

    [Fact]
    public void Loader_FileFailuresAndDuplicateConfigurationKeepRegisteredPolicy()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"fungame-decisions-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "policy.json");
        try
        {
            DecisionTableLoader loader = new();
            DecisionDiagnostic missing = Assert.Single(loader.Load(path));
            Assert.Equal(path, missing.Path);
            File.WriteAllText(path, "{");
            Assert.NotEmpty(loader.Load(path));
            File.WriteAllText(path, DecisionTableGenerator.GetJson(new() { Id = "test:loaded" }));
            Assert.Empty(loader.Load(path));
            IDecisionPolicy original = loader.Get("test:loaded")!;
            Assert.NotNull(original);
            Assert.Equal("id", Assert.Single(loader.Load(path)).Path);
            Assert.Same(original, loader.Get("test:loaded"));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
            Directory.Delete(directory);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Registry_DuplicateExtensionThrowsWithoutReplacingOriginal(bool action)
    {
        DecisionTableRegistry registry = new();
        if (action)
        {
            registry.RegisterAction("test:custom", (_, _, _) => []);
            Assert.Throws<ArgumentException>(() => registry.RegisterAction("test:custom", (_, _, _) => throw new InvalidOperationException()));
        }
        else
        {
            registry.RegisterCondition("test:custom", (_, _, _) => true);
            Assert.Throws<ArgumentException>(() => registry.RegisterCondition("test:custom", (_, _, _) => throw new InvalidOperationException()));
        }
        DecisionTableConfig config = Config(action ? null : new() { Type = "test:custom" });
        if (action) config.Rules[0].Action.Type = "test:custom";
        BattleFixture fixture = new();
        DecisionEvaluationResult result = new DecisionTableCompiler(registry).Compile(config).Evaluate(new()
        {
            Queue = fixture.Queue, Actor = fixture.A, Points = fixture.Points(fixture.A)
        });
        Assert.Equal(!action, result.Matched);
    }

    private static DecisionTableConfig Config(ConditionConfig? condition) => new()
    {
        Id = "test:boundary", Rules = [new() { Id = "one", When = condition, Action = Actions.EndTurn() }]
    };
}
