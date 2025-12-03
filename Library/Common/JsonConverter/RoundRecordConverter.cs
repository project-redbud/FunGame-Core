using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
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
                    result.Actor = NetworkUtility.JsonDeserialize<Character>(ref reader, options) ?? Factory.GetCharacter();
                    break;
                case nameof(RoundRecord.Targets):
                    List<Character> targets = NetworkUtility.JsonDeserialize<List<Character>>(ref reader, options) ?? [];
                    result.Targets.AddRange(targets);
                    break;
                case nameof(RoundRecord.Damages):
                    Dictionary<Guid, double> damagesGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, double> kvp in damagesGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.Damages[character] = kvp.Value;
                        }
                    }
                    break;

                case nameof(RoundRecord.ActionType):
                    result.ActionType = (CharacterActionType)reader.GetInt32();
                    break;
                case nameof(RoundRecord.Skill):
                    result.Skill = NetworkUtility.JsonDeserialize<Skill>(ref reader, options);
                    break;
                case nameof(RoundRecord.SkillCost):
                    result.SkillCost = reader.GetString() ?? "";
                    break;
                case nameof(RoundRecord.Item):
                    result.Item = NetworkUtility.JsonDeserialize<Item>(ref reader, options);
                    break;
                case nameof(RoundRecord.HasKill):
                    result.HasKill = reader.GetBoolean();
                    break;
                case nameof(RoundRecord.Assists):
                    List<Character> assists = NetworkUtility.JsonDeserialize<List<Character>>(ref reader, options) ?? [];
                    result.Assists.AddRange(assists);
                    break;

                case nameof(RoundRecord.IsCritical):
                    Dictionary<Guid, bool> isCriticalGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, bool>>(ref reader, options) ?? [];
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
                    Dictionary<Guid, bool> isEvadedGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, bool>>(ref reader, options) ?? [];
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
                    Dictionary<Guid, bool> isImmuneGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, bool>>(ref reader, options) ?? [];
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
                    Dictionary<Guid, double> healsGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, double>>(ref reader, options) ?? [];
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
                    Dictionary<Guid, Effect> effectsGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, Effect>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<Guid, Effect> kvp in effectsGuid)
                    {
                        Character? character = FindCharacterByGuid(kvp.Key, result);
                        if (character != null)
                        {
                            result.Effects[character] = kvp.Value;
                        }
                    }
                    break;
                case nameof(RoundRecord.ApplyEffects):
                    Dictionary<Guid, List<EffectType>> applyEffectsGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, List<EffectType>>>(ref reader, options) ?? [];
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
                    List<string> actorCK = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    result.ActorContinuousKilling.AddRange(actorCK);
                    break;
                case nameof(RoundRecord.DeathContinuousKilling):
                    List<string> deathCK = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    result.DeathContinuousKilling.AddRange(deathCK);
                    break;
                case nameof(RoundRecord.CastTime):
                    result.CastTime = reader.GetDouble();
                    break;
                case nameof(RoundRecord.HardnessTime):
                    result.HardnessTime = reader.GetDouble();
                    break;
                case nameof(RoundRecord.RespawnCountdowns):
                    Dictionary<Guid, double> respawnCountdownGuid = NetworkUtility.JsonDeserialize<Dictionary<Guid, double>>(ref reader, options) ?? [];
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
                    List<Character> respawns = NetworkUtility.JsonDeserialize<List<Character>>(ref reader, options) ?? [];
                    result.Respawns.AddRange(respawns);
                    break;
                case nameof(RoundRecord.RoundRewards):
                    List<Skill> rewards = NetworkUtility.JsonDeserialize<List<Skill>>(ref reader, options) ?? [];
                    result.RoundRewards.AddRange(rewards);
                    break;
                case nameof(RoundRecord.OtherMessages):
                    List<string> messages = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
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
            writer.WriteNumber(nameof(RoundRecord.ActionType), (int)value.ActionType);
            writer.WritePropertyName(nameof(RoundRecord.Skill));
            JsonSerializer.Serialize(writer, value.Skill, options);
            writer.WriteString(nameof(RoundRecord.SkillCost), value.SkillCost);
            writer.WritePropertyName(nameof(RoundRecord.Item));
            JsonSerializer.Serialize(writer, value.Item, options);
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
            Character? character = record.Targets.FirstOrDefault(c => c.Guid == guid);
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
