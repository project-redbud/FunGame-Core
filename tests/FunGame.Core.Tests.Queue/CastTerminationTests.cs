using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.EffectResult;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

public class CastTerminationTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Interrupt_UpdatesStateAndUsesRemainingTime(bool chain)
    {
        BattleFixture f = StartCast(chain);
        f.Queue.InterruptCasting(f.B, f.D);

        Assert.Equal(CharacterState.Actionable, f.B.CharacterState);
        Assert.Equal(chain ? 5.5 : 5d, f.Queue.HardnessTime[f.B], 3);
        ActionRecord failure = Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.B && !a.IsSuccess);
        Assert.NotNull(failure.Skill);
        Assert.Equal(chain, failure.ChainActionRequestId != null);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void TargetLoss_UsesPhaseAndRemainingTime(bool chain, bool inTurn)
    {
        BattleFixture f = StartCast(chain);
        f.Queue.AddCharacter(f.B, 1, false);
        if (inTurn)
        {
            f.Queue.AddCharacter(f.A, 0, false);
            f.DuringTurn(() => f.Queue.InterruptCasting(f.D));
        }
        else
        {
            f.Queue.InterruptCasting(f.D);
        }

        Assert.Equal(CharacterState.Actionable, f.B.CharacterState);
        Assert.Equal(chain ? 1.5 : 1d, f.Queue.HardnessTime[f.B], 3);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ResourceFailure_CompletesAttemptThenAllowsDecision(bool chain)
    {
        BattleFixture f = StartCast(chain);
        Skill magic = f.B.Skills.Single(s => s.SkillType == SkillType.Magic);
        magic.MPCost = 10000;
        f.Points(f.B).CurrentDecisionPoints = 3;
        int decisions = 0;
        f.B.NormalAttack.ExHardnessTime += 2 - f.B.NormalAttack.RealHardnessTime;
        f.Queue.DecideActionEvent += ctx => ++decisions == 1 ? CharacterActionType.NormalAttack : CharacterActionType.EndTurn;
        f.Queue.SelectNormalAttackTargetsEvent += ctx => [f.D];
        f.Queue.AddCharacter(f.B, 0, false);

        f.Run(f.B);

        Assert.Equal(2, decisions);
        Assert.Equal(2, f.Points(f.B).CurrentDecisionPoints);
        Assert.Equal(3d, Assert.Single(f.Queue.LastRound.Actions, a => !a.IsSuccess).HardnessTime, 3);
        Assert.Equal(2d, Assert.Single(f.Queue.LastRound.Actions, a => a.ActionType == CharacterActionType.NormalAttack).HardnessTime, 3);
        Assert.Equal(chain ? 3.5 : 6d, f.Queue.LastRound.HardnessTime, 3);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TurnStartInterrupt_UsesOneFailureAttemptWithoutBudgetRecovery(bool chain)
    {
        BattleFixture f = StartCast(chain);
        f.Points(f.B).CurrentDecisionPoints = 3;
        f.Queue.AddCharacter(f.B, 0, false);
        f.Queue.TurnStartEvent += ctx => { f.Queue.InterruptCasting(f.B, f.D); return true; };
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        CaptureSink sink = new();
        f.Queue.RoundRecordSink = sink;

        f.Run(f.B);

        Assert.Equal(3, f.Points(f.B).CurrentDecisionPoints);
        Assert.Equal(3d, Assert.Single(sink.Actions, a => !a.IsSuccess).HardnessTime, 3);
        Assert.Equal(chain ? 3.5 : 3d, f.Queue.LastRound.HardnessTime, 3);
        Assert.Single(f.Queue.Queue, c => c == f.B);
    }

    [Fact]
    public void ActionStartInterrupt_DoesNotLoseOrDuplicateFailure()
    {
        BattleFixture f = StartCast(true);
        CallbackEffect effect = new(new TestSkill(SkillType.Passive) { Level = 1 }, ctx =>
        {
            if (ctx.ActionType == CharacterActionType.CastSkill) f.Queue.InterruptCasting(f.B, f.D);
        });
        effect.AddToCharacter(f.B);
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        f.Queue.AddCharacter(f.B, 0, false);
        CaptureSink sink = new();
        f.Queue.RoundRecordSink = sink;

        f.Run(f.B);

        Assert.Equal(3d, Assert.Single(sink.Actions).HardnessTime, 3);
        Assert.False(sink.Actions[0].IsSuccess);
        Assert.Equal(3.5, f.Queue.LastRound.HardnessTime, 3);
    }

    [Fact]
    public void InterruptDuringOtherAction_PreservesAttackerRecord()
    {
        BattleFixture f = StartCast(true);
        f.Queue.AddCharacter(f.A, 0, false);
        f.Queue.DecideActionEvent += ctx => f.Points(f.A).ActionsTaken == 0 ? CharacterActionType.NormalAttack : CharacterActionType.EndTurn;
        f.Queue.SelectNormalAttackTargetsEvent += ctx => [f.D];
        f.Queue.CharacterNormalAttackEvent += ctx => f.Queue.InterruptCasting(f.B, f.A);
        CaptureSink sink = new();
        f.Queue.RoundRecordSink = sink;

        f.Run(f.A);

        Assert.True(Assert.Single(sink.Actions, a => a.Actor == f.A).IsSuccess);
        Assert.False(Assert.Single(sink.Actions, a => a.Actor == f.B).IsSuccess);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RemoveCaster_ClearsContextsAndDeferredRewards(bool preRelease)
    {
        BattleFixture f = new();
        f.Queue.InitRoundRewards([], bindToCharacter: true);
        Skill reward = new TestSkill(SkillType.Passive) { Level = 1 };
        f.Queue.AddRoundReward(f.B, 1, reward);
        StartNormalCast(f);
        if (preRelease)
        {
            f.Points(f.B).CurrentDecisionPoints = 5;
            f.Queue.SetCharacterPreCastSuperSkill(f.B, f.NewSkill(SkillType.SuperSkill, f.B));
        }

        f.Queue.RemoveCharacterFromQueue(f.B);

        Assert.DoesNotContain(reward, f.B.Skills);
        Assert.Equal(CharacterState.Actionable, f.B.CharacterState);
        f.D.HP = 0;
        Assert.Null(Record.Exception(() => f.Queue.DeathCalculation(f.A, f.D)));
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);
        Assert.Empty(f.Queue.LastRound.Actions);
    }

    [Fact]
    public void PreReleaseOverride_DoesNotSendIntermediateQueue()
    {
        BattleFixture f = StartCast(true);
        f.Points(f.B).CurrentDecisionPoints = 5;
        CaptureSink sink = new();
        f.Queue.RoundRecordSink = sink;

        f.Queue.SetCharacterPreCastSuperSkill(f.B, f.NewSkill(SkillType.SuperSkill, f.B));

        Assert.Empty(sink.Actions);
        Assert.Empty(sink.QueueUpdates);
        Assert.Equal(CharacterState.PreCastSuperSkill, f.B.CharacterState);
    }

    [Fact]
    public void BlockedInterrupt_PreservesCastAndDoesNotNotify()
    {
        BattleFixture f = StartCast(true);
        BlockingEffect effect = new(new TestSkill(SkillType.Passive) { Level = 1 });
        effect.AddToCharacter(f.B);
        int notifications = 0;
        f.Queue.InterruptCastingEvent += ctx => notifications++;

        f.Queue.InterruptCasting(f.B, f.D);

        Assert.Equal(CharacterState.Casting, f.B.CharacterState);
        Assert.Equal(5d, f.Queue.HardnessTime[f.B], 3);
        Assert.Equal(0, notifications);
    }

    [Fact]
    public void SuccessfulZeroHardnessCast_DoesNotApplyFailureFloor()
    {
        BattleFixture f = StartCast(true);
        Skill magic = f.B.Skills.Single(s => s.SkillType == SkillType.Magic);
        magic.HardnessTime = 0;
        f.Queue.AddCharacter(f.B, 0, false);

        f.Run(f.B);

        Assert.True(Assert.Single(f.Queue.LastRound.Actions).IsSuccess);
        Assert.Equal(.5, f.Queue.LastRound.HardnessTime, 3);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReDecision_NewCastDefersOriginalHardness(bool interruptNewCast)
    {
        BattleFixture f = StartCast(true);
        f.B.Skills.Single(s => s.SkillType == SkillType.Magic).MPCost = 10000;
        Skill nextMagic = f.NewSkill(SkillType.Magic, f.B);
        nextMagic.MPCost = 0;
        nextMagic.CastTime = 4;
        nextMagic.HardnessTime = 2;
        f.Points(f.B).CurrentDecisionPoints = 3;
        GamingQueue.DecideActionEventHandler decide = ctx => CharacterActionType.PreCastSkill;
        GamingQueue.SelectSkillEventHandler select = ctx => nextMagic;
        f.Queue.DecideActionEvent += decide;
        f.Queue.SelectSkillEvent += select;
        f.Queue.SelectSkillTargetsEvent += ctx => [f.D];
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);
        f.Queue.DecideActionEvent -= decide;
        f.Queue.SelectSkillEvent -= select;

        Assert.Equal(4d, f.Queue.LastRound.CastTime, 3);
        Assert.Equal(4d, f.Queue.HardnessTime[f.B], 3);
        if (interruptNewCast)
        {
            f.Queue.InterruptCasting(f.B, f.D);
            Assert.Equal(4.5, f.Queue.HardnessTime[f.B], 3);
        }
        else
        {
            f.Queue.AddCharacter(f.B, 0, false);
            f.Run(f.B);
            Assert.Equal(3.5, f.Queue.LastRound.HardnessTime, 3);
            Assert.Equal(2d, Assert.Single(f.Queue.LastRound.Actions).HardnessTime, 3);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TargetDeath_UsesPhaseAndClearsCast(bool inTurn)
    {
        BattleFixture f = StartCast(false);
        f.Queue.AddCharacter(f.B, 5, false);
        if (inTurn)
        {
            f.Queue.AddCharacter(f.A, 0, false);
            f.DuringTurn(() => { f.D.HP = 0; f.Queue.DeathCalculation(f.A, f.D); });
        }
        else
        {
            f.D.HP = 0;
            f.Queue.DeathCalculation(f.A, f.D);
        }

        Assert.Equal(CharacterState.Actionable, f.B.CharacterState);
        Assert.Equal(inTurn ? 3d : 5d, f.Queue.HardnessTime[f.B], 3);
        Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.B && !a.IsSuccess);
    }

    [Fact]
    public void PartialTargetLoss_KeepsRemainingTargetAndCast()
    {
        BattleFixture f = new();
        Skill magic = f.NewSkill(SkillType.Magic, f.B);
        magic.CanSelectTeammate = true;
        magic.CanSelectTargetCount = 2;
        magic.CastTime = 5;
        f.DuringTurn(() =>
        {
            ChainActionRequest? request = f.Queue.SubmitChainAction(f.A, f.B, CharacterActionType.PreCastSkill,
                [f.C, f.D], out string reason, magic);
            Assert.NotNull(request);
            Assert.True(f.Queue.ConfirmChainAction(f.A, request.Id).IsCasting, reason);
        });
        f.Queue.AddCharacter(f.A, 10, false);
        f.Queue.InterruptCasting(f.D);

        Assert.Equal(CharacterState.Casting, f.B.CharacterState);
        Assert.Equal(5d, f.Queue.HardnessTime[f.B], 3);
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);
        Assert.Equal(new[] { f.C }, Assert.Single(f.Queue.LastRound.Actions).Targets);
    }

    [Fact]
    public void Interrupt_RecalculatesControlState()
    {
        BattleFixture f = StartCast(false);
        Effect control = new();
        control.AddEffectStatesToCharacter(f.B, [CharacterState.NotActionable]);

        f.Queue.InterruptCasting(f.B, f.D);

        Assert.Equal(CharacterState.NotActionable, f.B.CharacterState);
        Assert.Single(f.Queue.LastRound.Actions, a => !a.IsSuccess);
    }

    [Fact]
    public void RewardRecovery_ReentryDoesNotRepeatTermination()
    {
        BattleFixture f = new();
        f.Queue.InitRoundRewards([], bindToCharacter: true);
        Skill reward = new TestSkill(SkillType.Passive) { Level = 1 };
        f.Queue.AddRoundReward(f.B, 1, reward);
        StartNormalCast(f);
        int rewardsLost = 0;
        int interruptions = 0;
        f.Queue.RoundRewardLostBeforeEvent += ctx =>
        {
            rewardsLost++;
            f.Queue.InterruptCasting(f.B, f.D);
        };
        f.Queue.InterruptCastingEvent += ctx => interruptions++;

        f.Queue.InterruptCasting(f.B, f.D);
        f.Queue.InterruptCasting(f.B, f.D);

        Assert.Equal(1, rewardsLost);
        Assert.Equal(1, interruptions);
        Assert.Single(f.Queue.LastRound.Actions, a => !a.IsSuccess);
        Assert.DoesNotContain(reward, f.B.Skills);
    }

    [Fact]
    public void NonDefaultFailureHardness_IsIndependentOfConfirmation()
    {
        BattleFixture f = StartCast(true);
        f.Queue.GameplayEquilibriumConstant = new EquilibriumConstant { CastFailureHardnessTime = 6, ChainActionHardnessTime = 9 };
        f.Queue.AddCharacter(f.A, 0, false);
        f.DuringTurn(() => f.Queue.InterruptCasting(f.D));

        Assert.Equal(5.5, f.Queue.HardnessTime[f.B], 3);
        BattleFixture g = StartCast(true);
        g.Queue.GameplayEquilibriumConstant = new EquilibriumConstant { CastFailureHardnessTime = 6 };
        g.B.Skills.Single(s => s.SkillType == SkillType.Magic).MPCost = 10000;
        g.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        g.Queue.AddCharacter(g.B, 0, false);
        g.Run(g.B);
        Assert.Equal(6.5, g.Queue.LastRound.HardnessTime, 3);
        Assert.Equal(6d, Assert.Single(g.Queue.LastRound.Actions).HardnessTime, 3);
    }

    [Fact]
    public void TimeLapse_TargetLossRestoresOriginalPositionAndKeepsQueueSorted()
    {
        BattleFixture f = StartCast(true);
        f.Queue.AddCharacter(f.C, 1, false);
        f.Queue.AddCharacter(f.D, 2, false);
        TimeCallbackEffect effect = new(new TestSkill(SkillType.Passive) { Level = 1 }, () => f.Queue.InterruptCasting(f.D));
        effect.AddToCharacter(f.B);

        f.Queue.TimeLapse();

        Assert.Equal(1d, f.Queue.TotalTime, 3);
        Assert.Equal(4.5, f.Queue.HardnessTime[f.B], 3);
        Assert.Equal(CharacterState.Actionable, f.B.CharacterState);
        double[] times = [.. f.Queue.Queue.Select(c => f.Queue.HardnessTime[c])];
        Assert.Equal(times.Order(), times);
    }

    [Fact]
    public void Death_ClearsPreReleaseStateAndRewardsWithoutFailure()
    {
        BattleFixture f = new();
        f.Queue.InitRoundRewards([], bindToCharacter: true);
        Skill reward = new TestSkill(SkillType.Passive) { Level = 1 };
        f.Queue.AddRoundReward(f.B, 1, reward);
        StartNormalCast(f);
        f.Points(f.B).CurrentDecisionPoints = 5;
        f.Queue.SetCharacterPreCastSuperSkill(f.B, f.NewSkill(SkillType.SuperSkill, f.B));
        f.B.HP = 0;

        f.Queue.DeathCalculation(f.D, f.B);

        Assert.NotEqual(CharacterState.PreCastSuperSkill, f.B.CharacterState);
        Assert.DoesNotContain(reward, f.B.Skills);
        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => !a.IsSuccess);
        f.Queue.SetCharacterRespawn(f.B);
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);
        Assert.Empty(f.Queue.LastRound.Actions);
    }

    [Fact]
    public void PreRelease_RejectedBudgetPreservesOriginalCast()
    {
        BattleFixture f = StartCast(true);
        f.Points(f.B).CurrentDecisionPoints = 0;

        f.Queue.SetCharacterPreCastSuperSkill(f.B, f.NewSkill(SkillType.SuperSkill, f.B));

        Assert.Equal(CharacterState.Casting, f.B.CharacterState);
        Assert.Equal(5d, f.Queue.HardnessTime[f.B], 3);
        Assert.DoesNotContain(f.Queue.LastRound.Actions, a => !a.IsSuccess);
    }

    [Fact]
    public void PreReleasedSuperSkill_CannotBeInterrupted()
    {
        BattleFixture f = StartCast(true);
        f.Points(f.B).CurrentDecisionPoints = 5;
        f.Queue.SetCharacterPreCastSuperSkill(f.B, f.NewSkill(SkillType.SuperSkill, f.B));
        double hardness = f.Queue.HardnessTime[f.B];

        f.Queue.InterruptCasting(f.B, f.D);

        Assert.Equal(CharacterState.PreCastSuperSkill, f.B.CharacterState);
        Assert.Equal(hardness, f.Queue.HardnessTime[f.B], 3);
    }

    [Fact]
    public void ImmuneCheckInterrupt_CannotReleaseCancelledMagic()
    {
        BattleFixture f = StartCast(true);
        double mp = f.B.MP;
        f.D.ImmuneType = ImmuneType.Magical;
        new ImmuneInterruptEffect(new TestSkill(SkillType.Passive) { Level = 1 }, () => f.Queue.InterruptCasting(f.B, f.D)).AddToCharacter(f.B);
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);

        Assert.Equal(mp, f.B.MP, 3);
        Assert.Equal(3d, Assert.Single(f.Queue.LastRound.Actions).HardnessTime, 3);
        Assert.False(f.Queue.LastRound.Actions[0].IsSuccess);
    }

    [Fact]
    public void ReDecision_NewCastInterruptedAtStartCompletesFailure()
    {
        BattleFixture f = StartCast(true);
        f.B.Skills.Single(s => s.SkillType == SkillType.Magic).MPCost = 10000;
        Skill nextMagic = f.NewSkill(SkillType.Magic, f.B);
        nextMagic.MPCost = 0;
        nextMagic.CastTime = 4;
        nextMagic.Effects.Add(new CastingInterruptEffect(nextMagic, () => f.Queue.InterruptCasting(f.B, f.D)));
        f.Points(f.B).CurrentDecisionPoints = 3;
        int decisions = 0;
        f.Queue.DecideActionEvent += ctx => ++decisions == 1 ? CharacterActionType.PreCastSkill : CharacterActionType.EndTurn;
        f.Queue.SelectSkillEvent += ctx => nextMagic;
        f.Queue.SelectSkillTargetsEvent += ctx => [f.D];
        CaptureSink sink = new();
        f.Queue.RoundRecordSink = sink;
        f.Queue.AddCharacter(f.B, 0, false);
        f.Run(f.B);

        Assert.Equal(2, sink.Actions.Count);
        Assert.All(sink.Actions, a => { Assert.False(a.IsSuccess); Assert.Equal(3d, a.HardnessTime, 3); });
        Assert.Equal(3.5, f.Queue.LastRound.HardnessTime, 3);
    }

    [Fact]
    public void ChainCastInterruptedAtStart_UsesCastTimeBeforeQueueReplacement()
    {
        BattleFixture f = new();
        Skill magic = f.NewSkill(SkillType.Magic, f.B);
        magic.CastTime = 5;
        magic.Effects.Add(new CastingInterruptEffect(magic, () => f.Queue.InterruptCasting(f.B, f.D)));

        f.DuringTurn(() =>
        {
            Assert.False(f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, magic).Id).IsCasting);
            Assert.Equal(5.5, f.Queue.HardnessTime[f.B], 3);
            Assert.False(Assert.Single(f.Queue.LastRound.Actions, a => a.Actor == f.B).IsSuccess);
        });
    }

    [Fact]
    public void MissingContext_CompletesDiagnosedFailure()
    {
        BattleFixture f = new();
        f.B.CharacterState = CharacterState.Casting;
        f.Queue.AddCharacter(f.A, 10, false);
        f.Queue.AddCharacter(f.B, 0, false);
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;
        CaptureSink sink = new();
        f.Queue.RoundRecordSink = sink;

        f.Run(f.B);

        ActionRecord failure = Assert.Single(sink.Actions);
        Assert.False(failure.IsSuccess);
        Assert.Contains("上下文缺失", failure.FailReason);
        Assert.Equal(3d, failure.HardnessTime, 3);
    }

    internal static BattleFixture StartCast(bool chain)
    {
        BattleFixture f = new();
        if (chain)
        {
            Skill magic = f.NewSkill(SkillType.Magic, f.B);
            magic.CastTime = 5;
            magic.MPCost = 10;
            f.DuringTurn(() => f.Queue.ConfirmChainAction(f.A, f.Submit(f.B, CharacterActionType.PreCastSkill, magic).Id));
            f.Queue.AddCharacter(f.A, 10, false);
        }
        else
        {
            StartNormalCast(f);
        }
        return f;
    }

    private static void StartNormalCast(BattleFixture f)
    {
        Skill magic = f.NewSkill(SkillType.Magic, f.B);
        magic.CastTime = 5;
        magic.MPCost = 10;
        f.Queue.AddCharacter(f.A, 10, false);
        f.Queue.AddCharacter(f.B, 0, false);
        GamingQueue.DecideActionEventHandler decide = ctx => CharacterActionType.PreCastSkill;
        GamingQueue.SelectSkillEventHandler select = ctx => magic;
        GamingQueue.SelectSkillTargetsEventHandler targets = ctx => [f.D];
        f.Queue.DecideActionEvent += decide;
        f.Queue.SelectSkillEvent += select;
        f.Queue.SelectSkillTargetsEvent += targets;
        try { f.Run(f.B); }
        finally
        {
            f.Queue.DecideActionEvent -= decide;
            f.Queue.SelectSkillEvent -= select;
            f.Queue.SelectSkillTargetsEvent -= targets;
        }
    }

    private sealed class CallbackEffect(Skill skill, Action<ActionContext> callback) : Effect(skill)
    {
        public override void OnCharacterActionStart(ActionContext ctx) => callback(ctx);
    }

    private sealed class BlockingEffect(Skill skill) : Effect(skill)
    {
        public override BeforeSkillCastWillBeInterruptedResult BeforeSkillCastWillBeInterrupted(SkillCastContext ctx) => new() { BlockInterruption = true };
    }

    private sealed class TimeCallbackEffect(Skill skill, Action callback) : Effect(skill)
    {
        public override void OnTimeElapsed(TimeLapseContext ctx) => callback();
    }

    private sealed class ImmuneInterruptEffect(Skill skill, Action callback) : Effect(skill)
    {
        public override OnImmuneCheckResult OnImmuneCheck(ImmuneContext ctx)
        {
            callback();
            return new() { IgnoreImmunity = true };
        }
    }

    private sealed class CastingInterruptEffect(Skill skill, Action callback) : Effect(skill)
    {
        public override void OnSkillCasting(SkillCastContext ctx) => callback();
    }

    internal sealed class CaptureSink : IRoundRecordSink
    {
        public List<ActionRecord> Actions { get; } = [];
        public List<Dictionary<Guid, double>> QueueUpdates { get; } = [];

        public void Attach(Guid queueId) { }
        public void End() { }
        public void SendAction(ActionRecord action) => Actions.Add(action);
        public void SendRound(RoundRecord round) { }
        public void SendCheckpointRound(RoundRecord round) { }
        public void SendCharacterStatistics(Dictionary<Guid, CharacterStatistics> statistics) { }
        public void SendCharacters(IEnumerable<Character> characters) { }
        public void SendTeams(IEnumerable<Team> teams) { }
        public void SendQueueData(Dictionary<Guid, double> queueData) => QueueUpdates.Add(queueData);
        public void SendEliminatedCharacters(IEnumerable<string> characterGuids) { }
        public void SendEliminatedTeams(IEnumerable<string> teamNames) { }
    }
}
