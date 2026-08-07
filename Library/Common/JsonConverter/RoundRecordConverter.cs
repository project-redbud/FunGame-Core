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
                    List<Character> allCharacters = CharacterRefHelper.ReadList(ref reader);
                    result.AllCharacters.AddRange(allCharacters);
                    convertingContext[AllCharactersProperty] = allCharacters;
                    break;
                case nameof(RoundRecord.Actor):
                    result.Actor = CharacterRefHelper.Read(ref reader);
                    break;
                case nameof(RoundRecord.Targets):
                    using (JsonDocument targetDoc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonProperty property in targetDoc.RootElement.EnumerateObject())
                        {
                            CharacterActionType? type = ParseActionTypeKey(property.Name);
                            if (type != null)
                            {
                                List<Character> list = [];
                                foreach (JsonElement element in property.Value.EnumerateArray())
                                {
                                    list.Add(CharacterRefHelper.ReadElement(element));
                                }
                                result.Targets[type.Value] = list;
                            }
                        }
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
                                Skill? skill = SkillRefHelper.ReadElement(property.Value);
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
                                Item? item = ItemRefHelper.ReadElement(property.Value);
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
                    result.Assists.AddRange(CharacterRefHelper.ReadList(ref reader));
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
                    Dictionary<Guid, Skill> effects = [];
                    using (JsonDocument effectDoc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonProperty property in effectDoc.RootElement.EnumerateObject())
                        {
                            if (Guid.TryParse(property.Name, out Guid guid))
                            {
                                Skill? skill = SkillRefHelper.ReadElement(property.Value);
                                if (skill != null) effects[guid] = skill;
                            }
                        }
                    }
                    convertingContext[nameof(RoundRecord.Effects)] = effects;
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
                    result.Respawns.AddRange(CharacterRefHelper.ReadList(ref reader));
                    break;
                case nameof(RoundRecord.RoundRewards):
                    using (JsonDocument rewardDoc = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (JsonElement element in rewardDoc.RootElement.EnumerateArray())
                        {
                            Skill? skill = SkillRefHelper.ReadElement(element);
                            if (skill != null) result.RoundRewards.Add(skill);
                        }
                    }
                    break;
                case nameof(RoundRecord.OtherMessages):
                    List<string> messages = JsonService.GetObject<List<string>>(ref reader, options) ?? [];
                    result.OtherMessages.AddRange(messages);
                    break;
                case nameof(RoundRecord.Actions):
                    result.Actions.AddRange(JsonService.GetObject<List<ActionRecord>>(ref reader, options) ?? []);
                    break;
                case nameof(RoundRecord.Checkpoint):
                    result.Checkpoint = JsonService.GetObject<List<CharacterStateSnapshot>>(ref reader, options);
                    break;
                case nameof(RoundRecord.TotalTime):
                    result.TotalTime = reader.GetDouble();
                    break;
                case nameof(RoundRecord.GameResult):
                    result.GameResult.AddRange(JsonService.GetObject<List<RankingEntry>>(ref reader, options) ?? []);
                    break;
                case nameof(RoundRecord.TeamMap):
                    result.TeamMap = JsonService.GetObject<Dictionary<Guid, string>>(ref reader, options) ?? [];
                    break;
                case nameof(RoundRecord.CharacterStatistics):
                    convertingContext[nameof(RoundRecord.CharacterStatistics)] = JsonService.GetObject<Dictionary<Guid, CharacterStatistics>>(ref reader, options) ?? [];
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
            // 收集角色引用：优先使用显式设置的全角色清单（开局时写入），否则动态收集本回合出现的角色
            List<Character> allCharacters;
            if (value.AllCharacters.Count > 0)
            {
                allCharacters = [.. value.AllCharacters.Where(c => c != null && c.Guid != Guid.Empty).DistinctBy(c => c.Guid)];
            }
            else
            {
                allCharacters = [value.Actor, .. value.Targets.Values.SelectMany(c => c), .. value.Assists, .. value.Respawns];
                allCharacters.AddRange([.. value.Damages.Keys, .. value.Heals.Keys, .. value.Effects.Keys, .. value.ApplyEffects.Keys, .. value.IsCritical.Keys, .. value.IsEvaded.Keys, .. value.IsImmune.Keys, .. value.RespawnCountdowns.Keys]);
                allCharacters = [.. allCharacters.Where(c => c != null && c.Guid != Guid.Empty).DistinctBy(c => c.Guid)];
            }
            writer.WritePropertyName(AllCharactersProperty);
            CharacterRefHelper.WriteList(writer, allCharacters);
            writer.WritePropertyName(nameof(RoundRecord.Actor));
            CharacterRefHelper.Write(writer, value.Actor);
            writer.WritePropertyName(nameof(RoundRecord.Targets));
            writer.WriteStartObject();
            foreach (KeyValuePair<CharacterActionType, List<Character>> kv in value.Targets)
            {
                writer.WritePropertyName(((int)kv.Key).ToString());
                CharacterRefHelper.WriteList(writer, kv.Value);
            }
            writer.WriteEndObject();
            writer.WritePropertyName(nameof(RoundRecord.Damages));
            JsonSerializer.Serialize(writer, value.Damages.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.ActionTypes));
            JsonSerializer.Serialize(writer, value.ActionTypes.Select(type => (int)type), options);
            writer.WritePropertyName(nameof(RoundRecord.Skills));
            writer.WriteStartObject();
            foreach (KeyValuePair<CharacterActionType, Skill> kv in value.Skills)
            {
                writer.WritePropertyName(((int)kv.Key).ToString());
                SkillRefHelper.Write(writer, kv.Value);
            }
            writer.WriteEndObject();
            writer.WritePropertyName(nameof(RoundRecord.SkillsCost));
            JsonSerializer.Serialize(writer, value.SkillsCost.ToDictionary(kv => kv.Key.GetIdName(), kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Items));
            writer.WriteStartObject();
            foreach (KeyValuePair<CharacterActionType, Item> kv in value.Items)
            {
                writer.WritePropertyName(((int)kv.Key).ToString());
                ItemRefHelper.Write(writer, kv.Value);
            }
            writer.WriteEndObject();
            writer.WritePropertyName(nameof(RoundRecord.ItemsCost));
            JsonSerializer.Serialize(writer, value.ItemsCost.ToDictionary(kv => kv.Key.GetIdName(), kv => kv.Value), options);
            writer.WriteBoolean(nameof(RoundRecord.HasKill), value.HasKill);
            writer.WritePropertyName(nameof(RoundRecord.Assists));
            CharacterRefHelper.WriteList(writer, value.Assists);
            writer.WritePropertyName(nameof(RoundRecord.IsCritical));
            JsonSerializer.Serialize(writer, value.IsCritical.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.IsEvaded));
            JsonSerializer.Serialize(writer, value.IsEvaded.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.IsImmune));
            JsonSerializer.Serialize(writer, value.IsImmune.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Heals));
            JsonSerializer.Serialize(writer, value.Heals.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(RoundRecord.Effects));
            writer.WriteStartObject();
            foreach (KeyValuePair<Character, Skill> kv in value.Effects)
            {
                writer.WritePropertyName(kv.Key.Guid.ToString());
                SkillRefHelper.Write(writer, kv.Value);
            }
            writer.WriteEndObject();
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
            CharacterRefHelper.WriteList(writer, value.Respawns);
            writer.WritePropertyName(nameof(RoundRecord.RoundRewards));
            writer.WriteStartArray();
            foreach (Skill skill in value.RoundRewards)
            {
                SkillRefHelper.Write(writer, skill);
            }
            writer.WriteEndArray();
            writer.WritePropertyName(nameof(RoundRecord.OtherMessages));
            JsonSerializer.Serialize(writer, value.OtherMessages, options);
            writer.WritePropertyName(nameof(RoundRecord.Actions));
            JsonSerializer.Serialize(writer, value.Actions, options);
            writer.WritePropertyName(nameof(RoundRecord.Checkpoint));
            if (value.Checkpoint != null)
            {
                JsonSerializer.Serialize(writer, value.Checkpoint, options);
            }
            else
            {
                writer.WriteNullValue();
            }
            writer.WriteNumber(nameof(RoundRecord.TotalTime), value.TotalTime);
            writer.WritePropertyName(nameof(RoundRecord.GameResult));
            JsonSerializer.Serialize(writer, value.GameResult, options);
            writer.WritePropertyName(nameof(RoundRecord.TeamMap));
            JsonSerializer.Serialize(writer, value.TeamMap, options);
            writer.WritePropertyName(nameof(RoundRecord.CharacterStatistics));
            JsonSerializer.Serialize(writer, value.CharacterStatistics.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
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
            ResolveCharacterKeyed<CharacterStatistics>(record, convertingContext, nameof(RoundRecord.CharacterStatistics), allCharacters, (c, v) => record.CharacterStatistics[c] = v);
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
