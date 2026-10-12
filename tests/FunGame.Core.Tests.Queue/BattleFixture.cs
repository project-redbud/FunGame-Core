using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Queue;

internal static class QueueTestHelper
{
    public const int Seed = 19;

    public static Character NewCharacter(string name, double hp = 1000, double mp = 100, double atk = 100, double def = 0, double spd = 100)
    {
        Character character = new()
        {
            Name = name,
            InitialHP = hp,
            InitialMP = mp,
            InitialATK = atk,
            InitialDEF = def,
            InitialSPD = spd,
            InitialHR = 1,
            ExActionCoefficient = -1
        };
        character.Recovery(100d);
        return character;
    }

    public static List<Character> NewCharacters(params string[] names) => [.. names.Select(name => NewCharacter(name))];

    public static MixGamingQueue NewMix(params Character[] characters) => new([.. characters], _ => { }, seed: Seed);
}

internal sealed class TestSkill(SkillType type) : Skill(type);

internal sealed class TestMap : GameMap
{
    public override string Name => "Test";
    public override string Description => "";
    public override string Version => "1";
    public override string Author => "Tests";
    public override int Length => 6;
    public override int Width => 1;
    public override int Height => 1;
    public override float Size => 1;

    public override GameMap InitGamingQueue(IGamingQueue queue)
    {
        TestMap map = new();
        map.Load();
        return map;
    }
}

internal sealed class BattleFixture
{
    public Character A = QueueTestHelper.NewCharacter("A");
    public Character B = QueueTestHelper.NewCharacter("B");
    public Character C = QueueTestHelper.NewCharacter("C");
    public Character D = QueueTestHelper.NewCharacter("D");
    public TeamGamingQueue Queue { get; }

    public BattleFixture(bool continuous = false, bool mapMode = false)
    {
        Queue = new([A, B, C, D], _ => { }, mapMode ? new TestMap() : null, seed: QueueTestHelper.Seed);
        Queue.AddTeam("T1", [A, B, C]);
        Queue.AddTeam("T2", [D]);
        Queue.AddCharacter(A, 0, false);
        Queue.AddCharacter(B, .5, false);
        Queue.AddCharacter(C, continuous ? .7 : 1.2, false);
        Queue.AddCharacter(D, .8, false);
        foreach (DecisionPoints points in Queue.CharacterDecisionPoints.Values) points.CurrentDecisionPoints = 5;
        if (Queue.Map != null)
        {
            Queue.Map.SetCharacterCurrentGrid(A, Queue.Map[0, 0]!);
            Queue.Map.SetCharacterCurrentGrid(B, Queue.Map[1, 0]!);
            Queue.Map.SetCharacterCurrentGrid(C, Queue.Map[2, 0]!);
            Queue.Map.SetCharacterCurrentGrid(D, Queue.Map[4, 0]!);
        }
    }

    public DecisionPoints Points(Character character) => Queue.CharacterDecisionPoints[character];

    public ChainActionRequest Submit(Character actor, CharacterActionType type = CharacterActionType.NormalAttack, Skill? skill = null, Character? target = null)
    {
        return Queue.SubmitChainAction(A, actor, type, [target ?? D], out string reason, skill)
            ?? throw new InvalidOperationException(reason);
    }

    public Skill NewSkill(SkillType type, Character owner)
    {
        Skill skill = new TestSkill(type) { Name = type.ToString(), Level = 1, HardnessTime = 5, CastTime = 3, EPCost = 0 };
        owner.Skills.Add(skill);
        return skill;
    }

    public void Run(Character expected)
    {
        Character actual = Queue.NextCharacter() ?? throw new InvalidOperationException("队列为空");
        Assert.Equal(expected.Guid, actual.Guid);
        Queue.ProcessTurn(actual);
    }

    public void DuringTurn(Action action, Character? actor = null)
    {
        GamingQueue.TurnStartEventHandler handler = ctx => { action(); return true; };
        GamingQueue.DecideActionEventHandler decide = ctx => CharacterActionType.EndTurn;
        Queue.TurnStartEvent += handler;
        Queue.DecideActionEvent += decide;
        try { Run(actor ?? A); }
        finally
        {
            Queue.TurnStartEvent -= handler;
            Queue.DecideActionEvent -= decide;
        }
    }

    public void MakeNext(Character actor)
    {
        foreach (Character other in Queue.Queue.Where(c => c != actor).ToArray()) Queue.RemoveCharacterFromQueue(other);
    }
}
