using FunGame.Core.Entity;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

public class TeamQueueTests
{
    [Fact]
    public void AddTeam_RegistersLivingTeams()
    {
        BattleFixture f = new();

        Assert.Equal(new[] { "T1", "T2" }, f.Queue.Teams.Keys);
        Assert.Same(f.Queue.Teams["T1"], f.Queue.GetTeam(f.A));
        Assert.Same(f.Queue.Teams["T1"], f.Queue.GetTeam(f.C));
        Assert.Same(f.Queue.Teams["T2"], f.Queue.GetTeam(f.D));
        Assert.Empty(f.Queue.EliminatedTeams);
    }

    [Fact]
    public void AddTeam_RejectsEmptyNameAndTeamWithoutLivingMember()
    {
        BattleFixture f = new();
        Character dead = QueueTestHelper.NewCharacter("dead");
        dead.HP = 0;

        f.Queue.AddTeam("", [f.A]);
        f.Queue.AddTeam("T3", [dead]);

        Assert.False(f.Queue.Teams.ContainsKey(""));
        Assert.False(f.Queue.Teams.ContainsKey("T3"));
        Assert.Equal(2, f.Queue.Teams.Count);
    }

    [Fact]
    public void GetTeam_ResolvesSummonToMasterTeam()
    {
        BattleFixture f = new();
        Character summon = QueueTestHelper.NewCharacter("summon");
        summon.Master = f.B;

        Assert.Same(f.Queue.Teams["T1"], f.Queue.GetTeam(summon));
        Assert.Null(f.Queue.GetTeamFromEliminated(summon));
    }

    [Fact]
    public void NextCharacter_RecordsTeamMapAndSeedInRound()
    {
        BattleFixture f = new();

        f.Queue.NextCharacter();

        Assert.Equal("T1", f.Queue.LastRound.TeamMap[f.A.Guid]);
        Assert.Equal("T1", f.Queue.LastRound.TeamMap[f.C.Guid]);
        Assert.Equal("T2", f.Queue.LastRound.TeamMap[f.D.Guid]);
        Assert.Equal(1, f.Queue.LastRound.Round);
        Assert.Equal(f.Queue.Seed, f.Queue.LastRound.Seed);
    }

    [Fact]
    public void GetTeammates_ExcludesSelfAndEnemies()
    {
        BattleFixture f = new();

        List<Character> teammates = f.Queue.GetTeammates(f.A);

        Assert.Equal(2, teammates.Count);
        Assert.Contains(f.B, teammates);
        Assert.Contains(f.C, teammates);
        Assert.DoesNotContain(f.D, teammates);
        Assert.Empty(f.Queue.GetTeammates(f.D));
    }

    [Fact]
    public void MixQueue_DoesNotTrackTeams()
    {
        Character a = QueueTestHelper.NewCharacter("A");
        Character b = QueueTestHelper.NewCharacter("B");
        MixGamingQueue queue = new([a, b], _ => { }, seed: QueueTestHelper.Seed);
        queue.AddCharacter(a, 0, false);
        queue.AddCharacter(b, 1, false);

        queue.NextCharacter();

        Assert.Empty(queue.LastRound.TeamMap);
        Assert.Equal(a.Guid, queue.LastRound.Actor.Guid);
    }
}
