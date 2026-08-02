using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class RoundRecordConverter : BaseEntityConverter<RoundRecord>
    {
        public override RoundRecord NewInstance()
        {
            return new RoundRecord(0);
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref RoundRecord result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(RoundRecord.Round):
                    result.Round = reader.GetInt32();
                    break;
                case nameof(RoundRecord.Actor):
                    result.Actor = JsonService.GetObject<Character>(ref reader, options) ?? new();
                    break;
                case nameof(RoundRecord.Targets):
                    Dictionary<CharacterActionType, List<Character>> targets = JsonService.GetObject<Dictionary<CharacterActionType, List<Character>>>(ref reader, options) ?? [];
                    foreach (CharacterActionType type in targets.Keys)
                    {
                        result.Targets[type] = targets[type];
                    }
                    break;
                case nameof(RoundRecord.Damages):
                    Dictionary<Guid, double> damagesGuid = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, double> kvp in damagesGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.Damages[character] = kvp.Value;
                        }
                    }
                    break;

                case nameof(RoundRecord.ActionTypes):
                    List<CharacterActionType> types = JsonService.GetObject<List<CharacterActionType>>(ref reader, options) ?? [];
                    foreach (CharacterActionType type in types)
                    {
                        result.ActionTypes.Add(type);
                    }
                    break;
                case nameof(RoundRecord.Skills):
                    Dictionary<CharacterActionType, Skill> skills = JsonService.GetObject<Dictionary<CharacterActionType, Skill>>(ref reader, options) ?? [];
                    foreach (CharacterActionType type in skills.Keys)
                    {
                        result.Skills[type] = skills[type];
                    }
                    break;
                case nameof(RoundRecord.SkillsCost):
                    Dictionary<Skill, string> skillsCost = JsonService.GetObject<Dictionary<Skill, string>>(ref reader, options) ?? [];
                    foreach (Skill skill in skillsCost.Keys)
                    {
                        result.SkillsCost[skill] = skillsCost[skill];
                    }
                    break;
                case nameof(RoundRecord.Items):
                    Dictionary<CharacterActionType, Item> items = JsonService.GetObject<Dictionary<CharacterActionType, Item>>(ref reader, options) ?? [];
                    foreach (CharacterActionType type in items.Keys)
                    {
                        result.Items[type] = items[type];
                    }
                    break;
                case nameof(RoundRecord.ItemsCost):
                    Dictionary<Item, string> itemsCost = JsonService.GetObject<Dictionary<Item, string>>(ref reader, options) ?? [];
                    foreach (Item item in itemsCost.Keys)
                    {
                        result.ItemsCost[item] = itemsCost[item];
                    }
                    break;
                case nameof(RoundRecord.HasKill):
                    result.HasKill = reader.GetBoolean();
                    break;
                case nameof(RoundRecord.Assists):
                    List<Character> assists = JsonService.GetObject<List<Character>>(ref reader, options) ?? [];
                    result.Assists.AddRange(assists);
                    break;

                case nameof(RoundRecord.IsCritical):
                    Dictionary<Guid, bool> isCriticalGuid = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, bool> kvp in isCriticalGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.IsCritical[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.IsEvaded):
                    Dictionary<Guid, bool> isEvadedGuid = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, bool> kvp in isEvadedGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.IsEvaded[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.IsImmune):
                    Dictionary<Guid, bool> isImmuneGuid = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, bool> kvp in isImmuneGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.IsImmune[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.Heals):
                    Dictionary<Guid, double> healsGuid = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, double> kvp in healsGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.Heals[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.Effects):
                    Dictionary<Guid, Skill> effectsGuid = JsonService.GetObject<Dictionary<Guid, Skill>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, Skill> kvp in effectsGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.Effects[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.ApplyEffects):
                    Dictionary<Guid, List<EffectType>> applyEffectsGuid = JsonService.GetObject<Dictionary<Guid, List<EffectType>>>(ref reader, options) ?? [];
                    result.ApplyEffects.Clear();
                    foreach (KeyValuePair<Guid, List<EffectType>> kvp in applyEffectsGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.ApplyEffects[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.ActorContinuousKilling):
                    List<string> actorCK = JsonService.GetObject<List<string>>(ref reader, options) ?? [];
                    result.ActorContinuousKilling.AddRange(actorCK);
                    break;
                case nameof(RoundRecord.DeathContinuousKilling):
                    List<string> deathCK = JsonService.GetObject<List<string>>(ref reader, options) ?? [];
                    result.DeathContinuousKilling.AddRange(deathCK);
                    break;
                case nameof(RoundRecord.CastTime):
                    result.CastTime = reader.GetDouble();
                    break;
                case nameof(RoundRecord.HardnessTime):
                    result.HardnessTime = reader.GetDouble();
                    break;
                case nameof(RoundRecord.RespawnCountdowns):
                    Dictionary<Guid, double> respawnCountdownGuid = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, double> kvp in respawnCountdownGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.RespawnCountdowns[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.Respawns):
                    List<Character> respawns = JsonService.GetObject<List<Character>>(ref reader, options) ?? [];
                    result.Respawns.AddRange(respawns);
                    break;
                case nameof(RoundRecord.RoundRewards):
                    List<Skill> rewards = JsonService.GetObject<List<Skill>>(ref reader, options) ?? [];
                    result.RoundRewards.AddRange(rewards);
                    break;
                case nameof(RoundRecord.OtherMessages):
                    List<string> messages = JsonService.GetObject<List<string>>(ref reader, options) ?? [];
                    result.OtherMessages.AddRange(messages);
                    break;

                default:
                    reader.Skip();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, RoundRecord value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(RoundRecord.Round), value.Round);
            writer.WritePropertyName(nameof(RoundRecord.Actor));
            JsonSerializer.Serialize(writer, value.Actor, options);
            writer.WritePropertyName(nameof(RoundRecord.Targets));
            JsonSerializer.Serialize(writer, value.Targets, options);
            writer.WritePropertyName(nameof(RoundRecord.Damages));
            JsonSerializer.Serialize(writer, value.Damages.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.ActionTypes));
            JsonSerializer.Serialize(writer, value.ActionTypes.Select(type => (int)type), options);
            writer.WritePropertyName(nameof(RoundRecord.Skills));
            JsonSerializer.Serialize(writer, value.Skills.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.SkillsCost));
            JsonSerializer.Serialize(writer, value.SkillsCost.ToDictionary(kv => kv.Key.GetIdName(), kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Items));
            JsonSerializer.Serialize(writer, value.Items.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.ItemsCost));
            JsonSerializer.Serialize(writer, value.ItemsCost.ToDictionary(kv => kv.Key.GetIdName(), kv => kv.Value), options);
            writer.WriteBoolean(nameof(RoundRecord.HasKill), value.HasKill);
            writer.WritePropertyName(nameof(RoundRecord.Assists));
            JsonSerializer.Serialize(writer, value.Assists, options);
            writer.WritePropertyName(nameof(RoundRecord.IsCritical));
            JsonSerializer.Serialize(writer, value.IsCritical.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.IsEvaded));
            JsonSerializer.Serialize(writer, value.IsEvaded.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.IsImmune));
            JsonSerializer.Serialize(writer, value.IsImmune.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Heals));
            JsonSerializer.Serialize(writer, value.Heals.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Effects));
            JsonSerializer.Serialize(writer, value.Effects.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.ApplyEffects));
            JsonSerializer.Serialize(writer, value.ApplyEffects.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.ActorContinuousKilling));
            JsonSerializer.Serialize(writer, value.ActorContinuousKilling, options);
            writer.WritePropertyName(nameof(RoundRecord.DeathContinuousKilling));
            JsonSerializer.Serialize(writer, value.DeathContinuousKilling, options);
            writer.WriteNumber(nameof(RoundRecord.CastTime), value.CastTime);
            writer.WriteNumber(nameof(RoundRecord.HardnessTime), value.HardnessTime);
            writer.WritePropertyName(nameof(RoundRecord.RespawnCountdowns));
            JsonSerializer.Serialize(writer, value.RespawnCountdowns.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Respawns));
            JsonSerializer.Serialize(writer, value.Respawns, options);
            writer.WritePropertyName(nameof(RoundRecord.RoundRewards));
            JsonSerializer.Serialize(writer, value.RoundRewards, options);
            writer.WritePropertyName(nameof(RoundRecord.OtherMessages));
            JsonSerializer.Serialize(writer, value.OtherMessages, options);
            writer.WriteEndObject();
        }

        private static Character? FindCharacterByGuid(Guid guid, RoundRecord record)
        {
            Character? character = record.Targets.Values.SelectMany(c => c).FirstOrDefault(c => c.Guid == guid);
            if (character != null) return character;
            if (record.Actor != null && record.Actor.Guid == guid) return record.Actor;
            character = record.Assists.FirstOrDefault(c => c.Guid == guid);
            if (character != null) return character;
            character = record.Respawns.FirstOrDefault(c => c.Guid == guid);
            if (character != null) return character;
            return null;
        }
    }
}
