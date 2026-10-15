using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Interface;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.DecisionTable;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Tests.Queue;

public sealed class DecisionTableRevalidationTests
{
    [Theory]
    [InlineData(false, "cooldown")]
    [InlineData(false, "resources")]
    [InlineData(false, "range")]
    [InlineData(true, "cooldown")]
    [InlineData(true, "resources")]
    [InlineData(true, "range")]
    public void PlannedSkill_InvalidatedBeforeExecutionDoesNotSpendBudget(bool chain, string invalidation)
    {
        BattleFixture fixture = new(mapMode: true);
        Character actor = chain ? fixture.B : fixture.A;
        actor.EP = 50;
        Skill skill = fixture.NewSkill(SkillType.Skill, actor);
        skill.Id = 1001;
        skill.Character = actor;
        skill.GamingQueue = fixture.Queue;
        skill.EPCost = 10;
        skill.CastRange = chain ? 3 : 4;
        DecisionTableConfig config = DecisionTableBuilder.Create("test:revalidate").WithFallback("endTurn")
            .AddRule("cast", rule => rule.Then(Actions.CastSkill(skill.Id, movement: "never"))).Build();
        DecisionTableLoader loader = new();
        Assert.Empty(loader.Register(config));
        actor.DecisionTableId = config.Id;
        Assert.Empty(loader.Bind(actor));
        double hp = fixture.D.HP;
        int? pointsBeforeFailure = null;
        void Invalidate()
        {
            switch (invalidation)
            {
                case "cooldown": skill.CurrentCD = 100; break;
                case "resources": actor.EP = 0; break;
                case "range": fixture.Queue.Map!.SetCharacterCurrentGrid(fixture.D, fixture.Queue.Map[5, 0]!); break;
            }
        }
        if (chain)
        {
            fixture.DuringTurn(() =>
            {
                IDecisionPolicy policy = actor.DecisionPolicy!;
                int actorPoints = fixture.Points(actor).CurrentDecisionPoints;
                int ownerPoints = fixture.Points(fixture.A).CurrentDecisionPoints;
                DecisionEvaluationResult result = policy.Evaluate(new()
                {
                    Queue = fixture.Queue, Actor = actor, Points = fixture.Points(actor), IsChainAction = true,
                    TurnOrigin = fixture.Queue.Map!.GetCharacterCurrentGrid(actor), Skills = [skill],
                    Enemies = [fixture.D], SelectableEnemies = [fixture.D], Allies = [fixture.A, fixture.C], SelectableAllies = [fixture.A, fixture.C]
                });
                Assert.Equal("cast", result.RuleId);
                Assert.Equal(50, actor.EP);
                Assert.Equal(actorPoints, fixture.Points(actor).CurrentDecisionPoints);
                ChainActionRequest request = fixture.Submit(actor, CharacterActionType.CastSkill, skill, Assert.Single(result.Decision!.Targets));
                Invalidate();
                Assert.False(fixture.Queue.ConfirmChainAction(fixture.A, request.Id).IsSuccess);
                Assert.Equal(actorPoints, fixture.Points(actor).CurrentDecisionPoints);
                Assert.Equal(ownerPoints, fixture.Points(fixture.A).CurrentDecisionPoints);
            });
        }
        else
        {
            fixture.Points(fixture.B).CurrentDecisionPoints = fixture.Points(fixture.C).CurrentDecisionPoints = 0;
            fixture.Queue.SetCharactersToAIControl(false, false, [actor]);
            fixture.Queue.AI!.PolicyEvaluatedEvent += (character, result, _) =>
            {
                if (character != actor || result.RuleId != "cast") return;
                Assert.Null(pointsBeforeFailure);
                Assert.Equal(50, actor.EP);
                pointsBeforeFailure = fixture.Points(actor).CurrentDecisionPoints;
                Invalidate();
            };
            fixture.Run(actor);
            Assert.NotNull(pointsBeforeFailure);
            Assert.Equal(pointsBeforeFailure.Value, fixture.Points(actor).CurrentDecisionPoints);
        }
        Assert.Equal(invalidation == "resources" ? 0 : 50, actor.EP);
        Assert.Equal(hp, fixture.D.HP);
        Assert.DoesNotContain(fixture.Queue.LastRound.Actions, action => action.Skill == skill);
        Assert.False(fixture.Points(actor).ActionTypes.ContainsKey(CharacterActionType.CastSkill));
    }
}
