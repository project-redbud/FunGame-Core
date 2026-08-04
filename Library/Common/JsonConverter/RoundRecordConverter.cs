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
        /// <summary>
        /// 序列化时额外输出的角色引用集合属性名，用于反序列化时恢复以角色为 key 的字典
        /// </summary>
        private const string AllCharactersProperty = "AllCharacters";

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
                case AllCharactersProperty:
                    convertingContext[AllCharactersProperty] = JsonService.GetObject<List<Character>>(ref reader, options) ?? new List<Character>();
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
                    convertingContext[nameof(RoundRecord.Damages)] = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    break;

                case nameof(RoundRecord.ActionTypes):
                    List<CharacterActionType> types = JsonService.GetObject<List<CharacterActionType>>(ref reader, options) ?? [];
                    foreach (CharacterActionType type in types)
                    {
                        result.ActionTypes.Add(type);
                    }
                    break;
                case nameof(RoundRecord.Skills):
                    using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                        {
                            CharacterActionType? type = ParseActionTypeKey(property.Name);
                            if (type != null)
                            {
                                Skill? skill = property.Value.Deserialize<Skill>(options);
                                if (skill != null) result.Skills[type.Value] = skill;
                            }
                        }
                    }
                    break;
                case nameof(RoundRecord.SkillsCost):
                    // SkillsCost 的 key 以 Id.Name 字符串写入，这里按 Id.Name 匹配回 Skills 中的技能实例
                    using (JsonDocument costDoc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonProperty property in costDoc.RootElement.EnumerateObject())
                        {
                            string cost = property.Value.GetString() ?? "";
                            Skill? skill = result.Skills.Values.FirstOrDefault(s => s.GetIdName() == property.Name);
                            if (skill != null) result.SkillsCost[skill] = cost;
                        }
                    }
                    break;
                case nameof(RoundRecord.Items):
                    using (JsonDocument itemDoc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonProperty property in itemDoc.RootElement.EnumerateObject())
                        {
                            CharacterActionType? type = ParseActionTypeKey(property.Name);
                            if (type != null)
                            {
                                Item? item = property.Value.Deserialize<Item>(options);
                                if (item != null) result.Items[type.Value] = item;
                            }
                        }
                    }
                    break;
                case nameof(RoundRecord.ItemsCost):
                    // ItemsCost 的 key 以 Id.Name 字符串写入，这里按 Id.Name 匹配回 Items 中的物品实例
                    using (JsonDocument itemCostDoc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonProperty property in itemCostDoc.RootElement.EnumerateObject())
                        {
                            string cost = property.Value.GetString() ?? "";
                            Item? item = result.Items.Values.FirstOrDefault(i => i.GetIdName() == property.Name);
                            if (item != null) result.ItemsCost[item] = cost;
                        }
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
                    convertingContext[nameof(RoundRecord.IsCritical)] = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    break;
                case nameof(RoundRecord.IsEvaded):
                    convertingContext[nameof(RoundRecord.IsEvaded)] = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    break;
                case nameof(RoundRecord.IsImmune):
                    convertingContext[nameof(RoundRecord.IsImmune)] = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    break;
                case nameof(RoundRecord.Heals):
                    convertingContext[nameof(RoundRecord.Heals)] = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    break;
                case nameof(RoundRecord.Effects):
                    convertingContext[nameof(RoundRecord.Effects)] = JsonService.GetObject<Dictionary<Guid, Skill>>(ref reader, options) ?? [];
                    break;
                case nameof(RoundRecord.ApplyEffects):
                    result.ApplyEffects.Clear();
                    convertingContext[nameof(RoundRecord.ApplyEffects)] = JsonService.GetObject<Dictionary<Guid, List<EffectType>>>(ref reader, options) ?? [];
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
                    convertingContext[nameof(RoundRecord.RespawnCountdowns)] = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
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
            // 收集所有涉及的角色引用，供反序列化时恢复以角色为 key 的字典（Damages/Heals/Effects/ApplyEffects 等）
            List<Character> allCharacters = [value.Actor, .. value.Targets.Values.SelectMany(c => c), .. value.Assists, .. value.Respawns];
            allCharacters.AddRange([.. value.Damages.Keys, .. value.Heals.Keys, .. value.Effects.Keys, .. value.ApplyEffects.Keys, .. value.IsCritical.Keys, .. value.IsEvaded.Keys, .. value.IsImmune.Keys, .. value.RespawnCountdowns.Keys]);
            allCharacters = [.. allCharacters.Where(c => c != null && c.Guid != Guid.Empty).DistinctBy(c => c.Guid)];
            writer.WritePropertyName(AllCharactersProperty);
            JsonSerializer.Serialize(writer, allCharacters, options);
            writer.WritePropertyName(nameof(RoundRecord.Actor));
            JsonSerializer.Serialize(writer, value.Actor, options);
            writer.WritePropertyName(nameof(RoundRecord.Targets));
            JsonSerializer.Serialize(writer, value.Targets, options);
            writer.WritePropertyName(nameof(RoundRecord.Damages));
            JsonSerializer.Serialize(writer, value.Damages.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.ActionTypes));
            JsonSerializer.Serialize(writer, value.ActionTypes.Select(type => (int)type), options);
            writer.WritePropertyName(nameof(RoundRecord.Skills));
            JsonSerializer.Serialize(writer, value.Skills.ToDictionary(kv => (int)kv.Key, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.SkillsCost));
            JsonSerializer.Serialize(writer, value.SkillsCost.ToDictionary(kv => kv.Key.GetIdName(), kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Items));
            JsonSerializer.Serialize(writer, value.Items.ToDictionary(kv => (int)kv.Key, kv => kv.Value), options);
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

        public override void AfterConvert(ref RoundRecord result, Dictionary<string, object> convertingContext)
        {
            RoundRecord record = result;
            List<Character>? allCharacters = convertingContext.TryGetValue(AllCharactersProperty, out object? ac) ? ac as List<Character> : null;

            ResolveCharacterKeyed<double>(record, convertingContext, nameof(RoundRecord.Damages), allCharacters, (c, v) => record.Damages[c] = v);
            ResolveCharacterKeyed<bool>(record, convertingContext, nameof(RoundRecord.IsCritical), allCharacters, (c, v) => record.IsCritical[c] = v);
            ResolveCharacterKeyed<bool>(record, convertingContext, nameof(RoundRecord.IsEvaded), allCharacters, (c, v) => record.IsEvaded[c] = v);
            ResolveCharacterKeyed<bool>(record, convertingContext, nameof(RoundRecord.IsImmune), allCharacters, (c, v) => record.IsImmune[c] = v);
            ResolveCharacterKeyed<double>(record, convertingContext, nameof(RoundRecord.Heals), allCharacters, (c, v) => record.Heals[c] = v);
            ResolveCharacterKeyed<Skill>(record, convertingContext, nameof(RoundRecord.Effects), allCharacters, (c, v) => record.Effects[c] = v);
            ResolveCharacterKeyed<List<EffectType>>(record, convertingContext, nameof(RoundRecord.ApplyEffects), allCharacters, (c, v) => record.ApplyEffects[c] = v);
            ResolveCharacterKeyed<double>(record, convertingContext, nameof(RoundRecord.RespawnCountdowns), allCharacters, (c, v) => record.RespawnCountdowns[c] = v);
        }

        /// <summary>
        /// 将反序列化时暂存的 Guid 键字典解析为以角色引用为 key 的字典
        /// </summary>
        private static void ResolveCharacterKeyed<T>(RoundRecord result, Dictionary<string, object> convertingContext, string propertyName, List<Character>? allCharacters, Action<Character, T> set)
        {
            if (convertingContext.TryGetValue(propertyName, out object? raw) && raw is Dictionary<Guid, T> dict)
            {
                foreach (KeyValuePair<Guid, T> kvp in dict)
                {
                    Character? character = FindCharacterByGuid(kvp.Key, result, allCharacters);
                    if (character != null) set(character, kvp.Value);
                }
            }
        }

        /// <summary>
        /// 解析字典键为 <see cref="CharacterActionType"/>，兼容数字枚举值与字符串枚举名两种写法
        /// </summary>
        private static CharacterActionType? ParseActionTypeKey(string key)
        {
            if (int.TryParse(key, out int value)) return (CharacterActionType)value;
            if (Enum.TryParse(key, out CharacterActionType type)) return type;
            return null;
        }

        private static Character? FindCharacterByGuid(Guid guid, RoundRecord record, List<Character>? allCharacters)
        {
            if (allCharacters != null)
            {
                Character? character = allCharacters.FirstOrDefault(c => c.Guid == guid);
                if (character != null) return character;
            }

            // 兼容旧存档（无 AllCharacters 字段）：从既有字段中查找
            Character? fallback = record.Targets.Values.SelectMany(c => c).FirstOrDefault(c => c.Guid == guid);
            if (fallback != null) return fallback;
            if (record.Actor != null && record.Actor.Guid == guid) return record.Actor;
            fallback = record.Assists.FirstOrDefault(c => c.Guid == guid);
            if (fallback != null) return fallback;
            fallback = record.Respawns.FirstOrDefault(c => c.Guid == guid);
            if (fallback != null) return fallback;
            return null;
        }
    }
}
