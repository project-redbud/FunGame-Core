using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public class ChainActionSettlementTests
{
    [Fact]
    public void ChainSettlement_RunsAsNormalTurn()
    {
        BattleFixture f = StartChainCast();
        int starts = 0;
        int decisions = 0;
        f.Queue.TurnStartEvent += ctx => { starts++; return true; };
        f.Queue.DecideActionEvent += ctx => { decisions++; return CharacterActionType.EndTurn; };

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(1, starts);
        Assert.Equal(0, decisions);
        ActionRecord action = Assert.Single(f.Queue.LastRound.Actions);
        Assert.True(action.IsSuccess);
        Assert.Equal(CharacterActionType.CastSkill, action.ActionType);
        Assert.Same(f.B, action.Actor);
        Assert.Same(f.A, action.TurnActor);
        Assert.NotNull(action.ChainActionRequestId);
    }

    [Fact]
    public void ChainSettlement_CountsActionTurn()
    {
        BattleFixture f = StartChainCast();
        int turns = f.Queue.CharacterStatistics[f.B].ActionTurn;

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(turns + 1, f.Queue.CharacterStatistics[f.B].ActionTurn);
    }

    [Fact]
    public void ChainSettlement_GrantsAndReclaimsRoundRewards()
    {
        BattleFixture f = StartChainCast();
        f.Queue.InitRoundRewards([], bindToCharacter: true);
        Skill reward = new TestSkill(SkillType.Passive) { Name = "结算奖励", Level = 1 };
        Assert.True(f.Queue.AddRoundReward(f.B, 1, reward));
        f.Queue.TurnStartEvent += ctx =>
        {
            Assert.Contains(reward, f.B.Skills);
            return true;
        };

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Contains(reward, f.Queue.LastRound.RoundRewards);
        Assert.Contains(f.Queue.LastRound.RoundRewardEvents, r => r.Kind == RoundRewardEventKind.Gained && r.Skills.Contains(reward));
        Assert.Contains(f.Queue.LastRound.RoundRewardEvents, r => r.Kind == RoundRewardEventKind.Lost && r.Skills.Contains(reward));
        Assert.DoesNotContain(reward, f.B.Skills);
        Assert.DoesNotContain(reward, f.Queue.QueryRoundRewards(f.B, 1));
    }

    [Fact]
    public void ChainSettlement_FiresEffectTurnEnd()
    {
        BattleFixture f = StartChainCast();
        TurnProbeEffect effect = new(new TestSkill(SkillType.Passive) { Level = 1 });
        effect.AddToCharacter(f.B);

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(1, effect.ActionStarts);
        Assert.Equal(1, effect.TurnEnds);
    }

    [Fact]
    public void ChainSettlement_ClearsTurnCounters()
    {
        BattleFixture f = StartChainCast();
        DecisionPoints dp = f.Points(f.B);
        dp.ActionsTaken = 2;
        dp.DecisionPointsCost = 4;
        dp.ActionsHardnessTime.Add(99);
        dp.ActionTypes[CharacterActionType.NormalAttack] = 1;
        int budget = dp.CurrentDecisionPoints;
        f.Queue.TurnStartEvent += ctx =>
        {
            Assert.Equal(0, dp.ActionsTaken);
            Assert.Equal(0, dp.DecisionPointsCost);
            Assert.Empty(dp.ActionsHardnessTime);
            Assert.Empty(dp.ActionTypes);
            Assert.Equal(budget, dp.CurrentDecisionPoints);
            return true;
        };

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Empty(dp.ActionTypes);
        Assert.Equal(new[] { 5d }, dp.ActionsHardnessTime);
    }

    [Fact]
    public void ChainSettlement_HardnessIgnoresPreviousTurn()
    {
        BattleFixture f = StartChainCast();
        DecisionPoints dp = f.Points(f.B);
        dp.ActionsTaken = 3;
        dp.DecisionPointsCost = 6;
        dp.ActionsHardnessTime.Add(99);

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(5.5, f.Queue.HardnessTime[f.B], 3);
    }

    [Fact]
    public void ChainSettlement_TurnTakeoverDefersRelease()
    {
        BattleFixture f = StartChainCast();
        double mp = f.B.MP;
        bool takeover = true;
        int starts = 0;
        f.Queue.TurnStartEvent += ctx => { starts++; return !takeover; };
        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(1, starts);
        Assert.Equal(CharacterState.Casting, f.B.CharacterState);
        Assert.Equal(mp, f.B.MP);
        Assert.Empty(f.Queue.LastRound.Actions);

        takeover = false;
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);

        Assert.Equal(2, starts);
        Assert.True(Assert.Single(f.Queue.LastRound.Actions).IsSuccess);
        Assert.Equal(5.5, f.Queue.HardnessTime[f.B], 3);
        Assert.Equal(mp - 10, f.B.MP, 3);
    }

    [Fact]
    public void ActionRecord_HardnessIsPerAction()
    {
        BattleFixture f = new();
        ActionSink sink = new();
        f.Queue.RoundRecordSink = sink;
        f.A.NormalAttack.ExHardnessTime += 6 - f.A.NormalAttack.RealHardnessTime;
        ChainActionRequest? request = null;
        f.Queue.TurnStartEvent += ctx => { request = f.Submit(f.B); return true; };
        int decision = 0;
        f.Queue.DecideActionEvent += ctx => ++decision switch
        {
            1 => CharacterActionType.NormalAttack,
            2 => CharacterActionType.ChainAction,
            _ => CharacterActionType.EndTurn
        };
        f.Queue.SelectNormalAttackTargetsEvent += ctx => [f.D];
        f.Queue.SelectChainActionEvent += (actor, pending) => request?.Id;

        f.Run(f.A);

        Assert.Equal(6d, Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.A && a.ActionType == CharacterActionType.NormalAttack).HardnessTime);
        Assert.Equal(3d, Assert.Single(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.ChainAction).HardnessTime);
        Assert.Equal(8d, f.Queue.LastRound.HardnessTime);
        Assert.Equal(6d, Assert.Single(sink.Actions, a => a.Actor == f.A && a.ActionType == CharacterActionType.NormalAttack).HardnessTime);
        Assert.Equal(3d, Assert.Single(sink.Actions, a => a.ActionType == CharacterActionType.ChainAction).HardnessTime);
    }

    [Fact]
    public void ActionRecord_CastTimeIsSentBeforeTurnAggregation()
    {
        BattleFixture f = new();
        ActionSink sink = new();
        f.Queue.RoundRecordSink = sink;
        Skill magic = f.NewSkill(SkillType.Magic, f.A);
        magic.MPCost = 0;
        magic.CastTime = 4;
        f.Queue.DecideActionEvent += ctx => CharacterActionType.PreCastSkill;
        f.Queue.SelectSkillEvent += ctx => magic;
        f.Queue.SelectSkillTargetsEvent += ctx => [f.D];

        f.Run(f.A);

        ActionRecord action = Assert.Single(sink.Actions);
        Assert.Equal(4d, action.CastTime);
        Assert.Equal(0d, action.HardnessTime);
        Assert.Equal(4d, Assert.Single(f.Queue.LastRound.Actions).CastTime);
    }

    [Fact]
    public void PublicConfirm_LeavesCoordinatorHardnessToHost()
    {
        BattleFixture f = new();
        f.DuringTurn(() =>
        {
            Assert.True(f.Queue.ConfirmChainAction(f.A, f.Submit(f.B).Id).IsSuccess);

            Assert.Empty(f.Points(f.A).ActionsHardnessTime);
            Assert.Equal(0d, Assert.Single(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.ChainAction).HardnessTime);
            Assert.Equal(1, f.Points(f.A).ActionsTaken);
        });
    }

    [Fact]
    public void ChainSettlement_ActionHardnessExcludesOriginalQueuePosition()
    {
        BattleFixture f = StartChainCast();
        ActionSink sink = new();
        f.Queue.RoundRecordSink = sink;

        f.MakeNext(f.B);
        f.Run(f.B);

        Assert.Equal(5d, Assert.Single(sink.Actions).HardnessTime);
        Assert.Equal(5.5, f.Queue.LastRound.HardnessTime, 3);
    }

    private static BattleFixture StartChainCast()
    {
        BattleFixture f = new();
        Skill magic = f.NewSkill(SkillType.Magic, f.B);
        magic.MPCost = 10;
        f.DuringTurn(() => Assert.True(f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, magic).Id).IsCasting));
        return f;
    }

    private sealed class TurnProbeEffect(Skill skill) : Effect(skill)
    {
        public int ActionStarts { get; private set; }
        public int TurnEnds { get; private set; }

        public override void OnCharacterActionStart(ActionContext ctx) => ActionStarts++;
        public override void OnTurnEnd(TurnContext ctx) => TurnEnds++;
    }

    private sealed class ActionSink : IRoundRecordSink
    {
        public List<ActionRecord> Actions { get; } = [];

        public void Attach(Guid queueId) { }
        public void End() { }
        public void SendAction(ActionRecord action) => Actions.Add(action);
        public void SendRound(RoundRecord round) { }
        public void SendCheckpointRound(RoundRecord round) { }
        public void SendCharacterStatistics(Dictionary<Guid, CharacterStatistics> statistics) { }
        public void SendCharacters(IEnumerable<Character> characters) { }
        public void SendTeams(IEnumerable<Team> teams) { }
        public void SendQueueData(Dictionary<Guid, double> queueData) { }
        public void SendEliminatedCharacters(IEnumerable<string> characterGuids) { }
        public void SendEliminatedTeams(IEnumerable<string> teamNames) { }
    }
}
