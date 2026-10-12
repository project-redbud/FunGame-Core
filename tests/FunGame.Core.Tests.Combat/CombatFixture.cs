using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.EffectResult;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Tests.Combat;

internal static class CombatHelper
{
    public const int Seed = 19;

    public static Character NewCharacter(string name, double hp = 1000, double mp = 200, double ep = 200, double atk = 100, double def = 0, double spd = 100)
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
        character.Recovery(ep);
        return character;
    }

    /// <summary>把技能挂到角色身上并绑定队列，使技能内的特效钩子可用</summary>
    public static T Grant<T>(this T skill, Character owner, IGamingQueue queue, int level = 1) where T : Skill
    {
        skill.Character = owner;
        skill.Level = level;
        skill.GamingQueue = queue;
        foreach (Effect effect in skill.Effects) effect.GamingQueue = queue;
        owner.Skills.Add(skill);
        return skill;
    }
}

internal sealed class TestSkill(SkillType type) : Skill(type);

internal sealed class TestEffect(Skill skill) : Effect(skill);

internal sealed class FlatDamageBonusEffect(Skill skill, double bonus) : Effect(skill)
{
    public override double AlterExpectedDamageBeforeCalculation(DamageContext ctx) => bonus;
}

internal sealed class ActualDamageDeltaEffect(Skill skill, double delta) : Effect(skill)
{
    public override AlterActualDamageResult AlterActualDamageAfterCalculation(DamageContext ctx) => new() { DamageDelta = delta };
}

internal sealed class DamageTypeOverrideEffect(Skill skill, DamageType damageType, MagicType magicType) : Effect(skill)
{
    public override AlterDamageTypeResult AlterDamageTypeBeforeCalculation(DamageContext ctx)
        => new() { DamageType = damageType, MagicType = magicType };
}

internal sealed class EvadeIgnoreEffect(Skill skill) : Effect(skill)
{
    public override OnEvadedTriggeredResult OnEvadedTriggered(DamageContext ctx) => new() { IgnoreEvaded = true };
}

/// <summary>1v1 队列，覆盖伤害计算与伤害应用两个阶段</summary>
internal sealed class Duel
{
    public Character Actor = CombatHelper.NewCharacter("Actor");
    public Character Enemy = CombatHelper.NewCharacter("Enemy");
    public MixGamingQueue Queue { get; }

    public Duel()
    {
        Queue = new([Actor, Enemy], _ => { }, seed: CombatHelper.Seed);
        Queue.AddCharacter(Actor, 0, false);
        Queue.AddCharacter(Enemy, 1, false);
    }

    public double DefenseReductionFactor => Queue.GameplayEquilibriumConstant.DEFReductionFactor;

    public (DamageResult Result, double Damage) Physical(double expected, bool isNormalAttack = false, bool evade = false, bool critical = false, bool triggerEffects = false)
        => Calculate(expected, null, isNormalAttack, evade, critical, triggerEffects);

    public (DamageResult Result, double Damage) Magical(double expected, MagicType magicType = MagicType.None, bool critical = false, bool triggerEffects = false)
        => Calculate(expected, magicType, false, false, critical, triggerEffects);

    public double Deal(double damage, bool isNormalAttack = false, DamageType damageType = DamageType.Physical, MagicType magicType = MagicType.None, bool triggerEffects = false, bool ignoreImmune = false, bool calculateShield = true)
    {
        double before = Enemy.HP;
        DamageCalculationOptions options = new(Actor)
        {
            TriggerEffects = triggerEffects,
            IgnoreImmune = ignoreImmune,
            CalculateShield = calculateShield
        };
        Queue.DamageToEnemy(Actor, Enemy, damage, isNormalAttack, damageType, magicType, DamageResult.Normal, options);
        return before - Enemy.HP;
    }

    private (DamageResult Result, double Damage) Calculate(double expected, MagicType? magicType, bool isNormalAttack, bool evade, bool critical, bool triggerEffects)
    {
        DamageCalculationOptions options = new(Actor)
        {
            CalculateEvade = evade,
            CalculateCritical = critical,
            TriggerEffects = triggerEffects
        };
        DamageCalculationOptions? current = options;
        int changeCount = 0;
        double final;
        DamageResult result;
        if (magicType is MagicType type)
        {
            result = Queue.CalculateMagicalDamage(Actor, Enemy, isNormalAttack, type, expected, out final, ref changeCount, ref current);
        }
        else
        {
            result = Queue.CalculatePhysicalDamage(Actor, Enemy, isNormalAttack, expected, out final, ref changeCount, ref current);
        }
        return (result, final);
    }
}

/// <summary>能开回合的 1v1 队列，覆盖技能施放与资源结算</summary>
internal sealed class TurnBattle
{
    public Character Caster = CombatHelper.NewCharacter("Caster");
    public Character Enemy = CombatHelper.NewCharacter("Enemy");
    public TeamGamingQueue Queue { get; }

    public TurnBattle()
    {
        Queue = new([Caster, Enemy], _ => { }, seed: CombatHelper.Seed);
        Queue.AddTeam("T1", [Caster]);
        Queue.AddTeam("T2", [Enemy]);
        Queue.AddCharacter(Caster, 0, false);
        Queue.AddCharacter(Enemy, 1, false);
        foreach (DecisionPoints points in Queue.CharacterDecisionPoints.Values) points.CurrentDecisionPoints = 7;
        Queue.TurnStartEvent += ctx => { DecisionPointsAtTurnStart = Points(Caster).CurrentDecisionPoints; return true; };
    }

    /// <summary>最近一次回合开始时施法者的决策点，用于断言单回合内的消耗量</summary>
    public int DecisionPointsAtTurnStart { get; private set; }

    public DecisionPoints Points(Character character) => Queue.CharacterDecisionPoints[character];

    public TestSkill NewSkill(SkillType type, double hardness = 5, double castTime = 3, double mpCost = 0, double epCost = 10, double cd = 0)
    {
        return new TestSkill(type) { Name = type.ToString(), HardnessTime = hardness, CastTime = castTime, MPCost = mpCost, EPCost = epCost, CD = cd }
            .Grant(Caster, Queue);
    }

    /// <summary>推进到施法者回合并用给定技能行动（途中其他角色的回合按结束回合处理）</summary>
    public void TakeTurn(Skill skill, CharacterActionType actionType, Character? target = null)
    {
        AdvanceTo(Caster);
        int decisions = 0;
        GamingQueue.DecideActionEventHandler decide = ctx => ++decisions == 1 ? actionType : CharacterActionType.EndTurn;
        GamingQueue.SelectSkillEventHandler select = ctx => skill;
        GamingQueue.SelectSkillTargetsEventHandler targets = ctx => [target ?? Enemy];
        Queue.DecideActionEvent += decide;
        Queue.SelectSkillEvent += select;
        Queue.SelectSkillTargetsEvent += targets;
        try { Queue.ProcessTurn(Caster); }
        finally
        {
            Queue.DecideActionEvent -= decide;
            Queue.SelectSkillEvent -= select;
            Queue.SelectSkillTargetsEvent -= targets;
        }
    }

    public void ResolveTurn(Character actor)
    {
        AdvanceTo(actor);
        GamingQueue.DecideActionEventHandler decide = ctx => CharacterActionType.EndTurn;
        Queue.DecideActionEvent += decide;
        try { Queue.ProcessTurn(actor); }
        finally { Queue.DecideActionEvent -= decide; }
    }

    public void Run(Character expected)
    {
        Character actual = Queue.NextCharacter() ?? throw new InvalidOperationException("队列为空");
        Assert.Equal(expected.Guid, actual.Guid);
        Queue.ProcessTurn(actual);
    }

    /// <summary>推进到施法者的回合，观察本回合可选择的技能列表并结束该回合</summary>
    public List<Skill> ObserveOfferedSkills()
    {
        List<Skill> offered = [];
        AdvanceTo(Caster);
        GamingQueue.DecideActionEventHandler observe = ctx => { offered.AddRange(ctx.Skills); return CharacterActionType.EndTurn; };
        Queue.DecideActionEvent += observe;
        try { Queue.ProcessTurn(Caster); }
        finally { Queue.DecideActionEvent -= observe; }
        return offered;
    }

    public void AdvanceTo(Character actor)
    {
        for (int i = 0; i < 16; i++)
        {
            if (Queue.NextCharacter() is not Character next) throw new InvalidOperationException("队列为空");
            if (next == actor) return;
            EndTurnOf(next);
        }
        throw new InvalidOperationException($"未能在限定次数内等到 {actor.Name} 的回合");
    }

    private void EndTurnOf(Character actor)
    {
        GamingQueue.DecideActionEventHandler end = ctx => CharacterActionType.EndTurn;
        Queue.DecideActionEvent += end;
        try { Queue.ProcessTurn(actor); }
        finally { Queue.DecideActionEvent -= end; }
    }

    public void MakeNext(Character actor)
    {
        foreach (Character other in Queue.Queue.Where(c => c != actor).ToArray()) Queue.RemoveCharacterFromQueue(other);
    }
}
