using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

internal sealed class BossMap(int size = 12) : GameMap
{
    public override string Name => "Boss";
    public override string Description => "决策表测试地图";
    public override string Version => "1";
    public override string Author => "Tests";
    public override int Length => size;
    public override int Width => size;
    public override int Height => 1;
    public override float Size => 1;
    public override GameMap InitGamingQueue(IGamingQueue queue) { BossMap map = new(size); map.Load(); return map; }
}

internal sealed class BossBattleFixture
{
    internal Character Warrior { get; } = QueueTestHelper.NewCharacter("Warrior", atk: 100);
    internal Character Mage { get; } = QueueTestHelper.NewCharacter("Mage", atk: 80);
    internal Character Healer { get; } = QueueTestHelper.NewCharacter("Healer", atk: 60);
    internal Character Boss { get; } = QueueTestHelper.NewCharacter("Boss", hp: 4000, atk: 150, spd: 110);
    internal TeamGamingQueue Queue { get; }
    internal DecisionTableConfig Config { get; }
    internal Dictionary<string, int> Hits { get; } = [];

    internal BossBattleFixture(int seed = 19, bool mapMode = false, bool usePolicy = true, int mapSize = 12)
    {
        Character[] actors = [Warrior, Mage, Healer, Boss];
        for (int i = 0; i < actors.Length; i++) actors[i].Guid = new Guid(i + 1, 0, 0, new byte[8]);
        Queue = new(_ => { }, mapMode ? new BossMap(mapSize) : null, seed: seed);
        Queue.InitCharacters([.. actors], regenerateGuids: false);
        Queue.AddTeam("challengers", [Warrior, Mage, Healer]);
        Queue.AddTeam("boss", [Boss]);
        AddSkill(Boss, 9001, SkillType.Skill, recovery: true, self: true);
        AddSkill(Boss, 9002, SkillType.Skill, area: true);
        AddSkill(Boss, 9003, SkillType.Skill);
        AddSkill(Boss, 9004, SkillType.Magic, area: true);
        AddSkill(Warrior, 8001, SkillType.Skill);
        AddSkill(Mage, 8002, SkillType.Magic, area: true);
        AddSkill(Healer, 8003, SkillType.Skill, recovery: true);
        foreach (Character actor in actors) actor.EP = 100;
        Config = CreateBossConfig();
        DecisionTableLoader loader = new(new(skills: [9001, 9002, 9003, 9004, 8001, 8002, 8003]));
        DecisionTableConfig empty = DecisionTableBuilder.Create("example:empty").Build();
        Assert.Empty(loader.Register(DecisionTableParser.Parse(DecisionTableGenerator.GetJson(empty))));
        Assert.Empty(loader.Register(DecisionTableParser.Parse(DecisionTableGenerator.GetJson(Config))));
        foreach (Character actor in actors)
        {
            actor.DecisionTableId = actor == Boss && usePolicy ? "example:boss" : "example:empty";
            Assert.Empty(loader.Bind(actor));
        }
        Queue.SetCharactersToAIControl(false, false, actors);
        Queue.InitActionQueue();
        foreach (DecisionPoints points in Queue.CharacterDecisionPoints.Values)
        {
            points.CurrentDecisionPoints = points.MaxDecisionPoints = 5;
            points.RecoverDecisionPointsPerRound = 5;
        }
        Queue.AI!.SelectionMode = AISelectionMode.Deterministic;
        Queue.AI.EvaluatePlanEvent += (_, plan) => plan.Decision.SkillToUse?.Id is 9001 or 8003
            ? plan.Decision.Targets.Sum(t => Math.Min(t.MaxHP - t.HP, t.MaxHP * .3)) : plan.Decision.SkillToUse is Skill ? plan.TargetCount * 100 : null;
        Queue.AI.PolicyEvaluatedEvent += (actor, result, _) => { if (actor == Boss && result.RuleId is string rule) Hits[rule] = Hits.GetValueOrDefault(rule) + 1; };
        if (Queue.Map is GameMap map)
        {
            map.SetCharacterCurrentGrid(Warrior, map[2, 2]!);
            map.SetCharacterCurrentGrid(Mage, map[2, 3]!);
            map.SetCharacterCurrentGrid(Healer, map[3, 2]!);
            map.SetCharacterCurrentGrid(Boss, map[5, 2]!);
        }
    }

    private void AddSkill(Character actor, long id, SkillType type, bool area = false, bool recovery = false, bool self = false)
    {
        Skill skill = new TestSkill(type) { Id = id, Name = id.ToString(), Level = 1, Character = actor, GamingQueue = Queue,
            CastRange = 3, CanSelectTargetRange = area ? 1 : 0, IsNonDirectional = area, CastTime = type == SkillType.Magic ? 3 : 0,
            CastAnywhere = false,
            CanSelectTargetCount = area ? 3 : 1,
            HardnessTime = 5, CD = 10, EPCost = 5, MPCost = 5,
            CanSelectSelf = recovery, CanSelectEnemy = !recovery, CanSelectTeammate = recovery && !self };
        skill.Effects.Add(new BattleSkillEffect(skill, recovery));
        actor.Skills.Add(skill);
    }

    internal static DecisionTableConfig CreateBossConfig()
    {
        ActionConfig cluster = Actions.CastSkill(9002);
        cluster.BindPlan = "cluster";
        cluster.PositionPreference = "coverage";
        ActionConfig channel = Actions.CastSkill(9004);
        channel.BindPlan = "channel";
        channel.PositionPreference = "safe";
        channel.MaxRisk = 0;
        return DecisionTableBuilder.Create("example:boss")
            .AddRule("emergency-recover", r => r.Priority(100).Target(Targets.Self("boss"))
                .When(Conditions.SelfHpRatio("lessThan", .3)).Then(Actions.CastSkill(9001, "boss", "never")))
            .AddRule("pressure-cluster", r => r.Priority(90).When(Conditions.Compare("plan.targetCount", "atLeast", 2, "cluster")).Then(cluster))
            .AddRule("focus-wounded", r => r.Priority(80).Target(Targets.Enemies("victim"))
                .When(Conditions.TargetHpRatio("victim", "lessThan", .25)).Then(Actions.CastSkill(9003, "victim")))
            .AddRule("safe-channel", r => r.Priority(70).When(Conditions.All(Conditions.SelfHpRatio("atLeast", .4),
                Conditions.Compare("plan.targetCount", "atLeast", 1, "channel"), Conditions.Compare("plan.risk", "atMost", 0, "channel"))).Then(channel))
            .AddRule("basic-pressure", r => r.Priority(60).Target(Targets.Enemies("victim")).Then(Actions.NormalAttack("victim"))).Build();
    }

    internal DecisionEvaluationContext Context()
    {
        List<Character> enemies = Queue.GetEnemies(Boss);
        (List<Character> allies, List<Character> selectable, List<Skill> skills, List<Item> items) = Queue.GetTurnStartNeedyList(Boss, [], enemies);
        return new() { Queue = Queue, Actor = Boss, Points = Queue.CharacterDecisionPoints[Boss], Enemies = enemies, Allies = [],
            SelectableEnemies = selectable, SelectableAllies = allies, Skills = skills, Items = items, TurnOrigin = Queue.Map?.GetCharacterCurrentGrid(Boss),
            MoveGrids = Queue.Map?.Grids.Values.ToArray() ?? [] };
    }

    internal List<string> Run(int limit = 500)
    {
        List<string> trace = [];
        for (int turn = 0; turn < limit && !Queue.GameOver; turn++)
        {
            Character actor = Queue.NextCharacter() ?? throw new InvalidOperationException("战斗未结束但队列为空。");
            Queue.ProcessTurn(actor);
            trace.AddRange(Queue.LastRound.Actions.Select(a => $"{a.Actor.Guid}:{a.ActionType}:{a.Skill?.Id}:{string.Join(',', a.Targets.Select(t => t.Guid))}:{a.IsSuccess}"));
            if (trace.Count > 2000) throw new InvalidOperationException($"seed={Queue.Seed}，动作超过 2000。");
        }
        Assert.True(Queue.GameOver, $"seed={Queue.Seed}，超过 {limit} 回合。");
        return trace;
    }

    private sealed class BattleSkillEffect(Skill skill, bool recovery) : Effect(skill)
    {
        public override void OnSkillCasted(SkillCastContext ctx)
        {
            foreach (Character target in ctx.Targets.ToArray())
            {
                if (recovery) HealToTarget(ctx.Trigger!, target, target.MaxHP * .3);
                else DamageToEnemy(ctx.Trigger!, target, DamageType.True, MagicType.None, 180);
            }
        }
    }
}
