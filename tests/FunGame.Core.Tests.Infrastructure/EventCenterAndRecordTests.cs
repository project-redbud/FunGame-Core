using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Model;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Infrastructure;

public class EventCenterTests
{
    private const string EventCenterTestName = "FunGame.Core.Tests 测试事件";

    [Fact]
    public void Instance_IsASingletonWithEmptyCollections()
    {
        Assert.Same(EventCenter.Instance, EventCenter.Instance);
        Assert.NotNull(EventCenter.Instance.Events);
    }

    [Fact]
    public void AddActivity_RegistersIntoNamedEventCollection()
    {
        EventCenter center = EventCenter.Instance;
        Activity activity = new(1, "活动") { Name = "活动" };

        try
        {
            center.AddActivity(EventCenterTestName, activity);

            Assert.Contains(activity, center[EventCenterTestName]);
            Assert.Same(activity, center.GetActivity(EventCenterTestName, "活动"));
            Assert.True(center.RemoveActivity(EventCenterTestName, activity));
            Assert.Empty(center[EventCenterTestName]);
        }
        finally
        {
            center.RemoveEvent(EventCenterTestName);
        }
    }

    [Fact]
    public void Indexer_ReturnsEmptySetForUnknownEvent()
    {
        Assert.Empty(EventCenter.Instance["不存在的活动"]);
    }

    [Fact]
    public void AddOrUpdateAndRemoveEvent_ReplacesWholeCollection()
    {
        EventCenter center = EventCenter.Instance;
        Activity first = new(1, "一");
        Activity second = new(2, "二");

        try
        {
            center.AddOrUpdateEvent(EventCenterTestName, [first]);
            Assert.Single(center[EventCenterTestName]);

            center.AddOrUpdateEvent(EventCenterTestName, [second]);
            Assert.Same(second, Assert.Single(center[EventCenterTestName]));

            Assert.True(center.RemoveEvent(EventCenterTestName));
            Assert.False(center.RemoveEvent(EventCenterTestName));
        }
        finally
        {
            center.RemoveEvent(EventCenterTestName);
        }
    }

    [Fact]
    public void Stores_AreRegisteredByName()
    {
        EventCenter center = EventCenter.Instance;
        Store store = new("商店");

        try
        {
            center.AddOrUpdateStore(EventCenterTestName, store);
            Assert.Same(store, center.GetStore(EventCenterTestName));
        }
        finally
        {
            Assert.True(center.RemoveStore(EventCenterTestName));
        }

        Assert.Null(center.GetStore(EventCenterTestName));
    }
}

public class RoundRecordTests
{
    [Fact]
    public void Snapshot_CopiesCollectionsButKeepsEntityReferences()
    {
        Character actor = new() { Name = "甲" };
        RoundRecord round = new(3)
        {
            Actor = actor,
            HardnessTime = 4.5
        };
        round.Damages[actor] = 100;
        round.ActionTypes.Add(Library.Constant.CharacterActionType.NormalAttack);

        RoundRecord snapshot = round.Snapshot();
        round.Damages[actor] = 50;
        round.ActionTypes.Clear();

        Assert.Equal(3, snapshot.Round);
        Assert.Same(actor, snapshot.Actor);
        Assert.Equal(4.5, snapshot.HardnessTime, 3);
        Assert.Equal(100d, snapshot.Damages[actor], 3);
        Assert.Single(snapshot.ActionTypes);
    }

    [Fact]
    public void RenderRound_ProducesHeaderForEmptyRound()
    {
        RoundRecord round = new(2) { Actor = new Character { Name = "甲" } };

        string text = RoundRecordRenderer.RenderRound(round);

        Assert.False(string.IsNullOrWhiteSpace(text));
        Assert.False(string.IsNullOrWhiteSpace(RoundRecordRenderer.RenderAll([round])));
        Assert.True(string.IsNullOrWhiteSpace(RoundRecordRenderer.RenderCharacterActions([round], round.Actor)));
    }

    [Fact]
    public void BattleStatePredictor_NeedsCheckpointToRestoreState()
    {
        RoundRecord round = new(1) { Actor = new Character { Name = "甲" } };

        Assert.Empty(BattleStatePredictor.PredictAll([round], 1));
        Assert.Null(BattleStatePredictor.Predict([round], 1, Guid.NewGuid()));
    }
}

public class SkillExtensionTests
{
    [Fact]
    public void SkillOwner_FallsBackToGenericPronoun()
    {
        OpenSkill skill = new(1, "技能", []);
        Character owner = new() { Name = "甲" };

        Assert.Equal("你", skill.SkillOwner());
        Assert.Equal("甲", skill.SkillOwner(owner));
    }

    [Fact]
    public void CharacterGetName_UsesProfileNames()
    {
        Character character = new() { Name = "甲", FirstName = "乙", NickName = "丙" };

        Assert.Equal(character.Name + character.FirstName, character.GetName());
        Assert.Contains("甲", character.GetName(full: true));
        Assert.Contains("乙", character.GetName(full: true));
    }
}
