using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Tests.Combat;

public class EffectTests
{
    [Fact]
    public void AddToCharacter_IgnoresDuplicatedInstance()
    {
        Duel duel = new();
        Effect effect = new();

        effect.AddToCharacter(duel.Actor);
        effect.AddToCharacter(duel.Actor);

        Assert.Same(effect, Assert.Single(duel.Actor.Effects));
    }

    [Fact]
    public void AddToCharacter_KeepsDistinctInstancesApart()
    {
        Duel duel = new();

        new Effect().AddToCharacter(duel.Actor);
        new Effect().AddToCharacter(duel.Actor);

        Assert.Equal(2, duel.Actor.Effects.Count);
    }

    [Fact]
    public void RemoveFromCharacter_OnlyAffectsOwningCharacter()
    {
        Duel duel = new();
        Effect effect = new();
        effect.AddToCharacter(duel.Actor);

        effect.RemoveFromCharacter(duel.Enemy);
        Assert.Contains(effect, duel.Actor.Effects);

        effect.RemoveFromCharacter(duel.Actor);
        Assert.Empty(duel.Actor.Effects);
    }

    [Fact]
    public void IsInEffect_FollowsSkillLevelAndTemporaryDispel()
    {
        TestEffect effect = new(new TestSkill(SkillType.Passive));

        Assert.False(effect.IsInEffect);

        effect.Skill.Level = 1;
        Assert.True(effect.IsInEffect);

        effect.IsBeingTemporaryDispelled = true;
        Assert.False(effect.IsInEffect);
    }

    [Fact]
    public void DispelFlags_FollowDispelType()
    {
        Effect effect = new() { DispelType = DispelType.TemporaryWeak };
        Assert.True(effect.CanWeakDispel);
        Assert.False(effect.CanStrongDispel);
        Assert.True(effect.IsTemporaryDispel);

        effect.DispelType = DispelType.DurativeStrong;
        Assert.True(effect.CanWeakDispel);
        Assert.True(effect.CanStrongDispel);

        effect.DispelType = DispelType.None;
        Assert.False(effect.CanWeakDispel);
        Assert.False(effect.IsTemporaryDispel);
    }

    [Fact]
    public void EffectType_DrivesCharacterState()
    {
        Duel duel = new();
        Effect effect = new();
        Assert.Equal(CharacterState.Actionable, duel.Actor.CharacterState);

        effect.AddEffectTypeToCharacter(duel.Actor, [EffectType.Stun]);
        Assert.Equal(CharacterState.NotActionable, duel.Actor.CharacterState);
        Assert.Contains(EffectType.Stun, duel.Actor.CharacterEffectTypes[effect]);

        effect.RemoveEffectTypesFromCharacter(duel.Actor);
        effect.RemoveEffectStatesFromCharacter(duel.Actor);
        Assert.Equal(CharacterState.Actionable, duel.Actor.CharacterState);
        Assert.False(duel.Actor.CharacterEffectTypes.ContainsKey(effect));
        Assert.False(duel.Actor.CharacterEffectStates.ContainsKey(effect));
    }

    [Fact]
    public void ImmuneTypes_AreMergedIntoCharacterAndCleanedUp()
    {
        Duel duel = new();
        Effect effect = new();

        effect.AddImmuneTypesToCharacter(duel.Actor, [ImmuneType.Physical]);
        Assert.Equal(ImmuneType.Physical, duel.Actor.ImmuneType);

        effect.RemoveImmuneTypesFromCharacter(duel.Actor);
        Assert.Equal(ImmuneType.None, duel.Actor.ImmuneType);
    }

    [Fact]
    public void CharacterStates_FromMultipleEffectsAreMerged()
    {
        Duel duel = new();
        Effect stun = new();
        Effect silence = new();

        stun.AddEffectTypeToCharacter(duel.Actor, [EffectType.Stun]);
        silence.AddEffectTypeToCharacter(duel.Actor, [EffectType.Silence]);

        Assert.Equal(CharacterState.NotActionable, duel.Actor.CharacterState);

        stun.RemoveEffectTypesFromCharacter(duel.Actor);
        stun.RemoveEffectStatesFromCharacter(duel.Actor);
        Assert.Equal(CharacterState.SkillRestricted, duel.Actor.CharacterState);
    }

    [Fact]
    public void Exemptable_FollowsExemptionType()
    {
        Effect effect = new();
        Assert.Equal(PrimaryAttribute.None, effect.ExemptionType);
        Assert.False(effect.Exemptable);

        effect.ExemptionType = PrimaryAttribute.STR;
        Assert.True(effect.Exemptable);
    }

    [Fact]
    public void SkillSet_MapsEffectTypesToStateDispelAndDebuff()
    {
        Assert.Equal(CharacterState.NotActionable, SkillSet.GetCharacterStateByEffectType(EffectType.Freeze));
        Assert.Equal(CharacterState.SkillRestricted, SkillSet.GetCharacterStateByEffectType(EffectType.SilenceMagic));
        Assert.Equal(CharacterState.AttackRestricted, SkillSet.GetCharacterStateByEffectType(EffectType.Disarm));
        Assert.Equal(CharacterState.ActionRestricted, SkillSet.GetCharacterStateByEffectType(EffectType.Root));
        Assert.Equal(CharacterState.Actionable, SkillSet.GetCharacterStateByEffectType(EffectType.Haste));

        Assert.Equal(DispelledType.CannotBeDispelled, SkillSet.GetDispelledTypeByEffectType(EffectType.Doom));
        Assert.Equal(DispelledType.Strong, SkillSet.GetDispelledTypeByEffectType(EffectType.Stun));
    }

    [Fact]
    public void SkillEffects_SubscribeToTheOwningSkill()
    {
        Duel duel = new();
        Skill skill = new TestSkill(SkillType.Skill);
        TestEffect first = new(skill);
        TestEffect second = new(skill);

        skill.Effects.Add(first);
        skill.Effects.Add(second);
        skill.Grant(duel.Actor, duel.Queue);

        Assert.Equal(2, skill.Effects.Count);
        Assert.All(skill.Effects, effect => Assert.Same(skill, effect.Skill));
        Assert.All(skill.Effects, effect => Assert.NotNull(effect.GamingQueue));
    }
}
