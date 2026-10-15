using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.EffectContext;

namespace FunGame.Core.Tests.Queue;

public class AllTargetSelectionTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void AllTargets_ActionStartRevalidatesPermissions(bool normalAttack, bool mapMode)
    {
        BattleFixture fixture = new(mapMode: mapMode);
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.Id = 1001;
        skill.EPCost = 10;
        fixture.A.EP = 50;
        skill.SelectAllEnemies = skill.SelectAllTeammates = true;
        skill.CanSelectTeammate = true;
        fixture.A.NormalAttack.SelectAllEnemies = fixture.A.NormalAttack.SelectAllTeammates = true;
        fixture.A.NormalAttack.CanSelectTeammate = true;
        fixture.A.Effects.Add(new ChangeTargetPermissions(skill, fixture.A.NormalAttack));
        DecisionTableConfig config = DecisionTableBuilder.Create("test:all").WithFallback("endTurn")
            .AddRule("all", r => r.Then(normalAttack ? Actions.NormalAttack() : Actions.CastSkill(skill.Id))).Build();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(config));
        fixture.A.DecisionTableId = config.Id;
        Assert.Empty(loader.Bind(fixture.A));
        fixture.Queue.SetCharactersToAIControl(false, false, [fixture.A]);
        fixture.Queue.AI!.SelectionMode = AISelectionMode.Deterministic;
        fixture.Run(fixture.A);
        Assert.Equal(50, fixture.A.EP);
        Assert.DoesNotContain(fixture.Queue.LastRound.Actions, action => action.ActionType == CharacterActionType.NormalAttack || action.Skill == skill);
    }

    private sealed class ChangeTargetPermissions(Skill skill, NormalAttack attack) : Effect(skill)
    {
        public override void OnCharacterActionStart(ActionContext ctx)
        {
            Skill.CanSelectTeammate = false;
            attack.CanSelectTeammate = false;
        }
    }

    [Fact]
    public void AllTargets_CopyAndRespawnPreserveDeclarations()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        actor.NormalAttack.SelectAllTeammates = true;
        actor.NormalAttack.CanSelectTeammate = actor.NormalAttack.CanSelectSelf = true;
        actor.NormalAttack.AttackRange = 2;
        Character original = actor.Copy();
        actor.NormalAttack.SelectAllTeammates = false;
        actor.NormalAttack.CanSelectSelf = false;
        actor.Respawn(original);
        Assert.True(actor.NormalAttack.SelectAllTeammates && actor.NormalAttack.CanSelectSelf);
        actor.NormalAttack.SelectAllTeammates = false;
        Assert.Equal(2, actor.NormalAttack.AttackRange);
        Skill skill = new TestSkill(SkillType.Skill)
        {
            SelectAllEnemies = true, CastRange = 2, CastAnywhere = false, IsNonDirectional = true
        };
        Skill copy = skill.Copy();
        Assert.True(copy.SelectAllEnemies && copy.CastAnywhere);
        Assert.False(copy.IsNonDirectional);
        copy.SelectAllEnemies = false;
        Assert.False(copy.CastAnywhere);
        Assert.True(copy.IsNonDirectional);
        Assert.Equal(2, copy.CastRange);
    }

    [Fact]
    public void AllAllies_DeadTargetsStillRespectPredicatesAndSelfPermission()
    {
        BattleFixture fixture = new();
        fixture.C.HP = 0;
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.SelectAllTeammates = skill.AllowSelectDead = true;
        skill.CanSelectTeammate = true;
        skill.SelectTargetPredicates.Add(target => target != fixture.B);
        Assert.Equal([fixture.C], skill.SelectTargets(fixture.A, [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B]));
    }

    [Theory]
    [InlineData(AISelectionMode.Legacy)]
    [InlineData(AISelectionMode.Deterministic)]
    [InlineData(AISelectionMode.WeightedNearBest)]
    public void AllAttack_AiUsesGlobalUnion(AISelectionMode mode)
    {
        BattleFixture fixture = new(mapMode: true);
        NormalAttack attack = fixture.A.NormalAttack;
        attack.GamingQueue = fixture.Queue;
        attack.SelectAllEnemies = attack.SelectAllTeammates = true;
        attack.CanSelectTeammate = attack.CanSelectSelf = true;
        AIController ai = new(fixture.Queue, fixture.Queue.Map) { SelectionMode = mode };
        AIDecision decision = ai.DecideAIAction(fixture.A, fixture.Points(fixture.A), fixture.Queue.Map![0, 0],
            [.. fixture.Queue.Map.Grids.Values], [], [], [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], 0, 0, 1);
        Assert.True(decision.HasCandidate);
        Assert.True(new HashSet<Character> { fixture.A, fixture.B, fixture.C, fixture.D }.SetEquals(decision.Targets));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AllTargets_ChainRejectsSubsetAndRevalidatesUnion(bool normalAttack)
    {
        BattleFixture fixture = new(continuous: true, mapMode: true);
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.B);
        skill.SelectAllEnemies = skill.SelectAllTeammates = true;
        skill.CanSelectTeammate = skill.CanSelectSelf = true;
        NormalAttack attack = fixture.B.NormalAttack;
        attack.SelectAllEnemies = attack.SelectAllTeammates = true;
        attack.CanSelectTeammate = attack.CanSelectSelf = true;
        fixture.DuringTurn(() =>
        {
            CharacterActionType type = normalAttack ? CharacterActionType.NormalAttack : CharacterActionType.CastSkill;
            Skill? selected = normalAttack ? null : skill;
            Assert.Null(fixture.Queue.SubmitChainAction(fixture.A, fixture.B, type, [fixture.D], out _, selected));
            ChainActionRequest request = fixture.Queue.SubmitChainAction(fixture.A, fixture.B, type,
                [fixture.A, fixture.B, fixture.C, fixture.D], out string reason, selected) ?? throw new InvalidOperationException(reason);
            if (normalAttack) attack.CanSelectSelf = false;
            else skill.CanSelectSelf = false;
            Assert.False(fixture.Queue.ConfirmChainAction(fixture.A, request.Id).IsSuccess);
            Assert.Equal(5, fixture.Points(fixture.B).CurrentDecisionPoints);
            request = fixture.Queue.SubmitChainAction(fixture.A, fixture.B, type,
                [fixture.A, fixture.C, fixture.D], out reason, selected) ?? throw new InvalidOperationException(reason);
            Assert.True(fixture.Queue.ConfirmChainAction(fixture.A, request.Id).IsSuccess);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AllTargets_ManualConfirmationResolvesCompleteSet(bool normalAttack)
    {
        BattleFixture fixture = new(mapMode: true);
        if (normalAttack)
        {
            fixture.A.NormalAttack.SelectAllEnemies = fixture.A.NormalAttack.SelectAllTeammates = true;
            fixture.A.NormalAttack.CanSelectTeammate = true;
            fixture.Queue.SelectNormalAttackTargetsEvent += _ => [fixture.D];
            List<Character> targets = fixture.Queue.SelectTargets(fixture.A, fixture.A.NormalAttack,
                [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], []);
            Assert.True(new HashSet<Character> { fixture.B, fixture.C, fixture.D }.SetEquals(targets));
        }
        else
        {
            Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
            skill.SelectAllEnemies = skill.SelectAllTeammates = true;
            skill.CanSelectTeammate = true;
            fixture.Queue.SelectSkillTargetsEvent += _ => [fixture.D];
            List<Character> targets = fixture.Queue.SelectTargets(fixture.A, skill,
                [fixture.D], [fixture.B, fixture.C], [fixture.D], [fixture.B, fixture.C], []);
            Assert.True(new HashSet<Character> { fixture.B, fixture.C, fixture.D }.SetEquals(targets));
        }
    }

    [Fact]
    public void AllTargets_SkillJsonRestoresMaskedDeclarations()
    {
        Skill skill = new TestSkill(SkillType.Skill)
        {
            SelectAllEnemies = true, SelectAllTeammates = true, CanSelectTeammate = true,
            CanSelectSelf = true, CastAnywhere = false, CastRange = 2, IsNonDirectional = true,
            AllowSelectDead = true, AllowSelectNoCharacterGrid = true, SkillRangeType = SkillRangeType.Circle
        };
        Skill decoded = JsonService.GetObject<Skill>(JsonService.GetString<Skill>(skill))!;
        Assert.True(decoded.CastAnywhere);
        Assert.False(decoded.IsNonDirectional);
        Assert.True(decoded.SelectAllEnemies && decoded.SelectAllTeammates && decoded.CanSelectSelf && decoded.CanSelectTeammate);
        decoded.SelectAllEnemies = decoded.SelectAllTeammates = false;
        Assert.False(decoded.CastAnywhere);
        Assert.Equal(2, decoded.CastRange);
        Assert.True(decoded.IsNonDirectional && decoded.AllowSelectDead && decoded.AllowSelectNoCharacterGrid);
        Assert.Equal(SkillRangeType.Circle, decoded.SkillRangeType);
    }

    [Fact]
    public void AllAttack_JsonAndCopyRetainDynamicDefaultRange()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        actor.NormalAttack.SelectAllEnemies = true;
        Character decoded = JsonService.GetObject<Character>(JsonService.GetString(actor))!;
        Character copied = actor.Copy();
        foreach (Character candidate in new[] { decoded, copied })
        {
            Assert.True(candidate.NormalAttack.AttackAnywhere);
            candidate.NormalAttack.SelectAllEnemies = false;
            Assert.False(candidate.NormalAttack.AttackAnywhere);
            candidate.ExATR = 7;
            Assert.Equal(candidate.ATR, candidate.NormalAttack.AttackRange);
        }
        actor.NormalAttack.AttackRange = 2;
        decoded = JsonService.GetObject<Character>(JsonService.GetString(actor))!;
        decoded.NormalAttack.SelectAllEnemies = false;
        decoded.ExATR = 7;
        Assert.Equal(2, decoded.NormalAttack.AttackRange);
    }

    [Fact]
    public void Magic_GlobalRuleTracksSkillTypeAndConfiguredRange()
    {
        Skill skill = new TestSkill(SkillType.Magic) { CastAnywhere = false, CastRange = 2 };
        Assert.True(skill.CastAnywhere);
        skill.SkillType = SkillType.Skill;
        Assert.False(skill.CastAnywhere);
        Assert.Equal(2, skill.CastRange);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void AllTargets_PlanContainsExactlyDeclaredGroups(bool allies, bool self)
    {
        BattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.GamingQueue = fixture.Queue;
        skill.SelectAllEnemies = true;
        skill.SelectAllTeammates = allies;
        skill.CanSelectTeammate = true;
        skill.CanSelectSelf = self;
        skill.IsNonDirectional = true;
        skill.CastRange = 1;
        DecisionEvaluationContext context = new()
        {
            Queue = fixture.Queue, Actor = fixture.A, Points = fixture.Points(fixture.A), Skills = [skill],
            Enemies = [fixture.D], SelectableEnemies = [fixture.D], Allies = [fixture.B, fixture.C],
            SelectableAllies = [fixture.B, fixture.C], MoveGrids = [.. fixture.Queue.Map!.Grids.Values]
        };
        IReadOnlyList<DecisionPlan> plans = DecisionActionPlanner.Plan(context, Actions.CastSkill(skill.Id), null);
        List<Character> expected = [fixture.D];
        if (allies) expected.AddRange([fixture.B, fixture.C]);
        if (allies && self) expected.Add(fixture.A);
        Assert.True(skill.CastAnywhere);
        Assert.False(skill.IsNonDirectional);
        Assert.NotEmpty(plans);
        Assert.All(plans, plan => Assert.True(expected.ToHashSet().SetEquals(plan.Decision.Targets)));
        Assert.Equal(1, context.Metrics.PositionsEvaluated);
        Assert.Equal(0, context.Metrics.CentersGenerated);
    }

    [Fact]
    public void AllEnemies_DoesNotSpreadToPermittedAllies()
    {
        BattleFixture fixture = new(mapMode: true);
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.GamingQueue = fixture.Queue;
        skill.SelectAllEnemies = true;
        skill.CanSelectTeammate = true;
        skill.CanSelectTargetRange = 5;
        Assert.Equal([fixture.D], skill.SelectTargetsByCanSelectTargetRange(fixture.A, [fixture.D], [fixture.B, fixture.C], [fixture.D]));
    }

    [Theory]
    [InlineData(false, 2)]
    [InlineData(true, 3)]
    public void AllAllies_CountRespectsSelfPermission(bool self, int expected)
    {
        BattleFixture fixture = new();
        Skill skill = fixture.NewSkill(SkillType.Skill, fixture.A);
        skill.SelectAllTeammates = true;
        skill.CanSelectTeammate = true;
        skill.CanSelectSelf = self;
        Assert.Equal(expected, skill.RealCanSelectTargetCount([fixture.D], [fixture.B, fixture.C]));
        fixture.A.NormalAttack.SelectAllTeammates = true;
        fixture.A.NormalAttack.CanSelectTeammate = true;
        fixture.A.NormalAttack.CanSelectSelf = self;
        Assert.Equal(expected, fixture.A.NormalAttack.RealCanSelectTargetCount([fixture.D], [fixture.B, fixture.C]));
    }

    [Fact]
    public void AllTargets_CharacterJsonPreservesSelectionPermissions()
    {
        Character actor = QueueTestHelper.NewCharacter("actor");
        actor.NormalAttack.SelectAllEnemies = actor.NormalAttack.SelectAllTeammates = true;
        actor.NormalAttack.CanSelectEnemy = false;
        actor.NormalAttack.CanSelectTeammate = actor.NormalAttack.CanSelectSelf = true;
        Character decoded = JsonService.GetObject<Character>(JsonService.GetString(actor))!;
        Assert.True(decoded.NormalAttack.SelectAllEnemies);
        Assert.True(decoded.NormalAttack.SelectAllTeammates);
        Assert.False(decoded.NormalAttack.CanSelectEnemy);
        Assert.True(decoded.NormalAttack.CanSelectTeammate);
        Assert.True(decoded.NormalAttack.CanSelectSelf);
    }
}
