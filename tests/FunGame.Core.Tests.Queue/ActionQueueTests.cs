using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

public class ActionQueueTests
{
    [Fact]
    public void InitActionQueue_OrdersBySpeedDescending()
    {
        Character fast = QueueTestHelper.NewCharacter("fast", spd: 300);
        Character normal = QueueTestHelper.NewCharacter("normal", spd: 200);
        Character slow = QueueTestHelper.NewCharacter("slow", spd: 100);
        Character slower = QueueTestHelper.NewCharacter("slower", spd: 50);

        MixGamingQueue queue = QueueTestHelper.NewMix(fast, normal, slow, slower);
        Assert.Empty(queue.Queue);

        queue.InitActionQueue();

        Assert.Equal(new[] { fast, normal, slow, slower }, queue.Queue);
        Assert.Equal(0d, queue.HardnessTime[fast], 3);
        Assert.Equal(.1, queue.HardnessTime[normal], 3);
        Assert.Equal(.2, queue.HardnessTime[slow], 3);
        Assert.Equal(.3, queue.HardnessTime[slower], 3);
    }

    [Fact]
    public void InitActionQueue_SameSeedProducesSameOrder()
    {
        static string[] Order()
        {
            MixGamingQueue queue = QueueTestHelper.NewMix([.. QueueTestHelper.NewCharacters("A", "B", "C", "D")]);
            queue.InitActionQueue();
            return [.. queue.Queue.Select(c => c.Name)];
        }

        Assert.Equal(Order(), Order());
    }

    [Fact]
    public void InitActionQueue_SkipsDeadCharacters()
    {
        Character alive = QueueTestHelper.NewCharacter("alive", spd: 200);
        Character dead = QueueTestHelper.NewCharacter("dead", spd: 300);
        dead.HP = 0;

        MixGamingQueue queue = QueueTestHelper.NewMix(alive, dead);
        queue.InitActionQueue();

        Assert.Equal(new[] { alive }, queue.Queue);
    }

    [Fact]
    public void AddCharacter_InsertsBeforeGreaterHardness()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        Character b = QueueTestHelper.NewCharacter("B");
        MixGamingQueue queue = QueueTestHelper.NewMix(a, b);

        queue.AddCharacter(a, 1, false);
        queue.AddCharacter(b, .5, false);

        Assert.Equal(new[] { b, a }, queue.Queue);
    }

    [Fact]
    public void AddCharacter_ResolvesHardnessConflictWithEpsilon()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        Character b = QueueTestHelper.NewCharacter("B");
        MixGamingQueue queue = QueueTestHelper.NewMix(a, b);

        queue.AddCharacter(a, 1, false);
        queue.AddCharacter(b, 1, false);

        Assert.Equal(new[] { a, b }, queue.Queue);
        Assert.Equal(1d, queue.HardnessTime[a], 3);
        Assert.Equal(1.01, queue.HardnessTime[b], 3);
    }

    [Fact]
    public void AddCharacter_MovesExistingCharacterInsteadOfDuplicating()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        MixGamingQueue queue = QueueTestHelper.NewMix(a);

        queue.AddCharacter(a, 1, false);
        queue.AddCharacter(a, .2, false);

        Assert.Single(queue.Queue);
        Assert.Equal(.2, queue.HardnessTime[a], 3);
        Assert.Equal(a.GameplayEquilibriumConstant.InitialDecisionPoints, queue.CharacterDecisionPoints[a].CurrentDecisionPoints);
    }

    [Fact]
    public void UseQueueProtected_DefaultsToDisabled()
    {
        MixGamingQueue queue = QueueTestHelper.NewMix(QueueTestHelper.NewCharacter("A"));

        Assert.False(queue.UseQueueProtected);
        Assert.Equal(0, queue.MaxCutQueueTimes);
    }

    [Fact]
    public void RemoveCharacterFromQueue_AcceptsMultipleCharacters()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        Character b = QueueTestHelper.NewCharacter("B");
        Character c = QueueTestHelper.NewCharacter("C");
        MixGamingQueue queue = QueueTestHelper.NewMix(a, b, c);
        queue.AddCharacter(a, 0, false);
        queue.AddCharacter(b, 1, false);
        queue.AddCharacter(c, 2, false);

        queue.RemoveCharacterFromQueue(a, b);

        Assert.Equal(new[] { c }, queue.Queue);
        Assert.False(queue.HardnessTime.ContainsKey(a));
        Assert.True(queue.HardnessTime.ContainsKey(c));
    }

    [Fact]
    public void ClearQueue_EmptiesQueueAndHardness()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        MixGamingQueue queue = QueueTestHelper.NewMix(a);
        queue.AddCharacter(a, 0, false);

        queue.ClearQueue();

        Assert.Empty(queue.Queue);
        Assert.Empty(queue.HardnessTime);
        Assert.Null(queue.NextCharacter());
        Assert.Equal(0d, queue.TimeLapse(), 3);
    }

    [Fact]
    public void NextCharacter_AdvancesTimeUntilHeadIsReady()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        MixGamingQueue queue = QueueTestHelper.NewMix(a);
        queue.AddCharacter(a, 3, false);

        Character? actor = queue.NextCharacter();

        Assert.Equal(a.Guid, actor!.Guid);
        Assert.Equal(3d, queue.TotalTime, 3);
        Assert.Equal(1, queue.TotalRound);
        Assert.Empty(queue.Queue);
    }

    [Fact]
    public void NextCharacter_ArchivesPreviousRoundOnNewTurn()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        Character b = QueueTestHelper.NewCharacter("B");
        MixGamingQueue queue = QueueTestHelper.NewMix(a, b);
        queue.AddCharacter(a, 0, false);
        queue.AddCharacter(b, 1, false);

        queue.NextCharacter();
        Character? second = queue.NextCharacter();

        Assert.Equal(b.Guid, second!.Guid);
        Assert.Equal(2, queue.TotalRound);
        RoundRecord archived = Assert.Single(queue.Rounds);
        Assert.Equal(1, archived.Round);
        Assert.Equal(a.Guid, archived.Actor.Guid);
        Assert.Equal(queue.Seed, archived.Seed);
    }

    [Fact]
    public void ChangeCharacterHardnessTime_AppliesAbsoluteAndPercentageValue()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        Character outsider = QueueTestHelper.NewCharacter("outsider");
        MixGamingQueue queue = QueueTestHelper.NewMix(a, outsider);
        queue.AddCharacter(a, 2, false);

        queue.ChangeCharacterHardnessTime(a, -1, false, false);
        Assert.Equal(1d, queue.HardnessTime[a], 3);

        queue.ChangeCharacterHardnessTime(a, -.5, true, false);
        Assert.Equal(.5, queue.HardnessTime[a], 3);

        queue.ChangeCharacterHardnessTime(a, -100, false, false);
        Assert.Equal(0d, queue.HardnessTime[a], 3);

        queue.ChangeCharacterHardnessTime(outsider, -1, false, false);
        Assert.False(queue.HardnessTime.ContainsKey(outsider));
    }

    [Fact]
    public void SetCharactersToAIControl_TogglesControlFlag()
    {
        BattleFixture f = new();

        f.Queue.SetCharactersToAIControl(false, false, [f.A]);
        Assert.True(f.Queue.IsCharacterInAIControlling(f.A));
        Assert.False(f.Queue.IsCharacterInAIControlling(f.B));

        f.Queue.SetCharactersToAIControl(false, true, [f.A]);
        Assert.False(f.Queue.IsCharacterInAIControlling(f.A));
    }

    [Fact]
    public void ProcessTurn_ReturnsFalseWhileGameIsRunning()
    {
        BattleFixture f = new();
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;

        Character actor = f.Queue.NextCharacter()!;

        Assert.False(f.Queue.ProcessTurn(actor));
        Assert.False(f.Queue.GameOver);
    }

    [Fact]
    public void ProcessTurn_RejectsNestedTurnProcessing()
    {
        BattleFixture f = new();
        f.Queue.TurnStartEvent += ctx =>
        {
            Assert.Throws<InvalidOperationException>(() => f.Queue.ProcessTurn(f.A));
            return true;
        };
        f.Queue.DecideActionEvent += ctx => CharacterActionType.EndTurn;

        f.Run(f.A);
    }

    [Fact]
    public void TurnStartEvent_CanTakeOverTheWholeTurn()
    {
        BattleFixture f = new();
        int decided = 0;
        f.Queue.TurnStartEvent += ctx => false;
        f.Queue.DecideActionEvent += ctx => { decided++; return CharacterActionType.EndTurn; };

        f.Run(f.A);

        Assert.Equal(0, decided);
        Assert.Empty(f.Queue.LastRound.Actions);
    }
}
