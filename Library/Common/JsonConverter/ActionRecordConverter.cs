using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class ActionRecordConverter : BaseEntityConverter<ActionRecord>
    {
        /// <summary>
        /// 序列化时额外输出的角色引用集合属性名，用于反序列化时恢复以角色为 key 的字典
        /// </summary>
        private const string AllCharactersProperty = "AllCharacters";

        public override ActionRecord NewInstance()
        {
            return new ActionRecord(0);
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref ActionRecord result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(ActionRecord.Round):
                    result.Round = reader.GetInt32();
                    break;
                case AllCharactersProperty:
                    convertingContext[AllCharactersProperty] = CharacterRefHelper.ReadList(ref reader);
                    break;
                case nameof(ActionRecord.Actor):
                    result.Actor = CharacterRefHelper.Read(ref reader);
                    break;
                case nameof(ActionRecord.ActionIndex):
                    result.ActionIndex = reader.GetInt32();
                    break;
                case nameof(ActionRecord.ActionType):
                    result.ActionType = (CharacterActionType)reader.GetInt32();
                    break;
                case nameof(ActionRecord.Skill):
                    result.Skill = SkillRefHelper.Read(ref reader);
                    break;
                case nameof(ActionRecord.Item):
                    result.Item = ItemRefHelper.Read(ref reader);
                    break;
                case nameof(ActionRecord.Cost):
                    result.Cost = reader.GetString() ?? "";
                    break;
                case nameof(ActionRecord.MPCost):
                    result.MPCost = reader.GetDouble();
                    break;
                case nameof(ActionRecord.EPCost):
                    result.EPCost = reader.GetDouble();
                    break;
                case nameof(ActionRecord.SkillCD):
                    result.SkillCD = reader.GetDouble();
                    break;
                case nameof(ActionRecord.DecisionPointsCost):
                    result.DecisionPointsCost = reader.GetDouble();
                    break;
                case nameof(ActionRecord.Targets):
                    result.Targets.AddRange(CharacterRefHelper.ReadList(ref reader));
                    break;
                case nameof(ActionRecord.Damages):
                    convertingContext[nameof(ActionRecord.Damages)] = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    break;
                case nameof(ActionRecord.IsCritical):
                    convertingContext[nameof(ActionRecord.IsCritical)] = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    break;
                case nameof(ActionRecord.IsEvaded):
                    convertingContext[nameof(ActionRecord.IsEvaded)] = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    break;
                case nameof(ActionRecord.IsImmune):
                    convertingContext[nameof(ActionRecord.IsImmune)] = JsonService.GetObject<Dictionary<Guid, bool>>(ref reader, options) ?? [];
                    break;
                case nameof(ActionRecord.Heals):
                    convertingContext[nameof(ActionRecord.Heals)] = JsonService.GetObject<Dictionary<Guid, double>>(ref reader, options) ?? [];
                    break;
                case nameof(ActionRecord.ApplyEffects):
                    convertingContext[nameof(ActionRecord.ApplyEffects)] = JsonService.GetObject<Dictionary<Guid, List<EffectType>>>(ref reader, options) ?? [];
                    break;
                case nameof(ActionRecord.Messages):
                    result.Messages.AddRange(JsonService.GetObject<List<string>>(ref reader, options) ?? []);
                    break;
                case nameof(ActionRecord.IsSuccess):
                    result.IsSuccess = reader.GetBoolean();
                    break;
                case nameof(ActionRecord.FailReason):
                    result.FailReason = reader.GetString() ?? "";
                    break;
                case nameof(ActionRecord.CastTime):
                    result.CastTime = reader.GetDouble();
                    break;
                case nameof(ActionRecord.HardnessTime):
                    result.HardnessTime = reader.GetDouble();
                    break;

                default:
                    reader.Skip();
                    break;
            }
        }

        public override void AfterConvert(ref ActionRecord result, Dictionary<string, object> convertingContext)
        {
            ActionRecord record = result;
            List<Character>? allCharacters = convertingContext.TryGetValue(AllCharactersProperty, out object? ac) ? ac as List<Character> : null;

            ResolveKeyed<double>(record, convertingContext, nameof(ActionRecord.Damages), allCharacters, (c, v) => record.Damages[c] = v);
            ResolveKeyed<bool>(record, convertingContext, nameof(ActionRecord.IsCritical), allCharacters, (c, v) => record.IsCritical[c] = v);
            ResolveKeyed<bool>(record, convertingContext, nameof(ActionRecord.IsEvaded), allCharacters, (c, v) => record.IsEvaded[c] = v);
            ResolveKeyed<bool>(record, convertingContext, nameof(ActionRecord.IsImmune), allCharacters, (c, v) => record.IsImmune[c] = v);
            ResolveKeyed<double>(record, convertingContext, nameof(ActionRecord.Heals), allCharacters, (c, v) => record.Heals[c] = v);
            ResolveKeyed<List<EffectType>>(record, convertingContext, nameof(ActionRecord.ApplyEffects), allCharacters, (c, v) => record.ApplyEffects[c] = v);
        }

        private static void ResolveKeyed<T>(ActionRecord record, Dictionary<string, object> convertingContext, string propertyName, List<Character>? allCharacters, Action<Character, T> set)
        {
            if (convertingContext.TryGetValue(propertyName, out object? raw) && raw is Dictionary<Guid, T> dict)
            {
                foreach (KeyValuePair<Guid, T> kvp in dict)
                {
                    Character? character = FindCharacterByGuid(kvp.Key, record, allCharacters);
                    if (character != null) set(character, kvp.Value);
                }
            }
        }

        private static Character? FindCharacterByGuid(Guid guid, ActionRecord record, List<Character>? allCharacters)
        {
            if (allCharacters != null)
            {
                Character? character = allCharacters.FirstOrDefault(c => c.Guid == guid);
                if (character != null) return character;
            }

            // 兼容旧数据（无 AllCharacters 字段）：从 Targets/Actor 中查找
            Character? fallback = record.Targets.FirstOrDefault(c => c.Guid == guid);
            if (fallback != null) return fallback;
            if (record.Actor != null && record.Actor.Guid == guid) return record.Actor;
            return null;
        }

        public override void Write(Utf8JsonWriter writer, ActionRecord value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(ActionRecord.Round), value.Round);
            // 收集所有涉及的角色引用（含仅出现在结果字典 key 中的角色，如反弹/反击伤害目标）
            List<Character> allCharacters = [value.Actor, .. value.Targets, .. value.Damages.Keys, .. value.Heals.Keys, .. value.IsCritical.Keys, .. value.IsEvaded.Keys, .. value.IsImmune.Keys];
            allCharacters.AddRange([.. value.ApplyEffects.Keys]);
            allCharacters = [.. allCharacters.Where(c => c != null && c.Guid != Guid.Empty).DistinctBy(c => c.Guid)];
            writer.WritePropertyName(AllCharactersProperty);
            CharacterRefHelper.WriteList(writer, allCharacters);
            writer.WritePropertyName(nameof(ActionRecord.Actor));
            CharacterRefHelper.Write(writer, value.Actor);
            writer.WriteNumber(nameof(ActionRecord.ActionIndex), value.ActionIndex);
            writer.WriteNumber(nameof(ActionRecord.ActionType), (int)value.ActionType);
            writer.WritePropertyName(nameof(ActionRecord.Skill));
            SkillRefHelper.Write(writer, value.Skill);
            writer.WritePropertyName(nameof(ActionRecord.Item));
            ItemRefHelper.Write(writer, value.Item);
            writer.WriteString(nameof(ActionRecord.Cost), value.Cost);
            writer.WriteNumber(nameof(ActionRecord.MPCost), value.MPCost);
            writer.WriteNumber(nameof(ActionRecord.EPCost), value.EPCost);
            writer.WriteNumber(nameof(ActionRecord.SkillCD), value.SkillCD);
            writer.WriteNumber(nameof(ActionRecord.DecisionPointsCost), value.DecisionPointsCost);
            writer.WritePropertyName(nameof(ActionRecord.Targets));
            CharacterRefHelper.WriteList(writer, value.Targets);
            writer.WritePropertyName(nameof(ActionRecord.Damages));
            JsonSerializer.Serialize(writer, value.Damages.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(ActionRecord.IsCritical));
            JsonSerializer.Serialize(writer, value.IsCritical.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(ActionRecord.IsEvaded));
            JsonSerializer.Serialize(writer, value.IsEvaded.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(ActionRecord.IsImmune));
            JsonSerializer.Serialize(writer, value.IsImmune.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(ActionRecord.Heals));
            JsonSerializer.Serialize(writer, value.Heals.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(ActionRecord.ApplyEffects));
            JsonSerializer.Serialize(writer, value.ApplyEffects.ToDictionary(kv => kv.Key.Guid, kv => kv.Value), options);
            writer.WritePropertyName(nameof(ActionRecord.Messages));
            JsonSerializer.Serialize(writer, value.Messages, options);
            writer.WriteBoolean(nameof(ActionRecord.IsSuccess), value.IsSuccess);
            writer.WriteString(nameof(ActionRecord.FailReason), value.FailReason);
            writer.WriteNumber(nameof(ActionRecord.CastTime), value.CastTime);
            writer.WriteNumber(nameof(ActionRecord.HardnessTime), value.HardnessTime);
            writer.WriteEndObject();
        }
    }
}
