using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Library.Common.JsonConverter
{
    /// <summary>
    /// 回合奖励事件（<see cref="RoundRewardRecord"/>）的读写辅助
    /// <para>角色按 <see cref="CharacterRefHelper"/> 的引用规则处理（只保留 Guid 与展示字段），
    /// 奖励技能按 <see cref="SkillRefHelper"/> 处理并附带描述（奖励只在本回合存在，回放端拿不到检查点描述索引）</para>
    /// </summary>
    internal static class RoundRewardRecordHelper
    {
        public static void Write(Utf8JsonWriter writer, RoundRewardRecord value)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(RoundRewardRecord.Kind), (int)value.Kind);
            writer.WriteNumber(nameof(RoundRewardRecord.Binding), (int)value.Binding);
            writer.WriteNumber(nameof(RoundRewardRecord.TurnKey), value.TurnKey);
            writer.WritePropertyName(nameof(RoundRewardRecord.Character));
            CharacterRefHelper.Write(writer, value.Character);
            writer.WritePropertyName(nameof(RoundRewardRecord.Counterpart));
            if (value.Counterpart is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                CharacterRefHelper.Write(writer, value.Counterpart);
            }
            writer.WritePropertyName(nameof(RoundRewardRecord.Skills));
            writer.WriteStartArray();
            foreach (Skill skill in value.Skills)
            {
                SkillRefHelper.Write(writer, skill, includeDescription: true);
            }
            writer.WriteEndArray();
            writer.WriteBoolean(nameof(RoundRewardRecord.IsCarryOver), value.IsCarryOver);
            writer.WriteEndObject();
        }

        public static void WriteList(Utf8JsonWriter writer, IEnumerable<RoundRewardRecord> values)
        {
            writer.WriteStartArray();
            foreach (RoundRewardRecord value in values)
            {
                Write(writer, value);
            }
            writer.WriteEndArray();
        }

        public static RoundRewardRecord Read(JsonElement element)
        {
            RoundRewardRecord record = new();
            if (element.TryGetProperty(nameof(RoundRewardRecord.Kind), out JsonElement kind) && kind.TryGetInt32(out int kindValue))
            {
                record.Kind = (RoundRewardEventKind)kindValue;
            }
            if (element.TryGetProperty(nameof(RoundRewardRecord.Binding), out JsonElement binding) && binding.TryGetInt32(out int bindingValue))
            {
                record.Binding = (RoundRewardBinding)bindingValue;
            }
            if (element.TryGetProperty(nameof(RoundRewardRecord.TurnKey), out JsonElement turnKey) && turnKey.TryGetInt32(out int turnKeyValue))
            {
                record.TurnKey = turnKeyValue;
            }
            if (element.TryGetProperty(nameof(RoundRewardRecord.Character), out JsonElement character))
            {
                record.Character = CharacterRefHelper.ReadElement(character);
            }
            if (element.TryGetProperty(nameof(RoundRewardRecord.Counterpart), out JsonElement counterpart) && counterpart.ValueKind == JsonValueKind.Object)
            {
                record.Counterpart = CharacterRefHelper.ReadElement(counterpart);
            }
            if (element.TryGetProperty(nameof(RoundRewardRecord.Skills), out JsonElement skills) && skills.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement skillElement in skills.EnumerateArray())
                {
                    Skill? skill = SkillRefHelper.ReadElement(skillElement);
                    if (skill != null) record.Skills.Add(skill);
                }
            }
            if (element.TryGetProperty(nameof(RoundRewardRecord.IsCarryOver), out JsonElement carryOver) && carryOver.ValueKind == JsonValueKind.True)
            {
                record.IsCarryOver = true;
            }
            return record;
        }

        public static List<RoundRewardRecord> ReadList(ref Utf8JsonReader reader)
        {
            List<RoundRewardRecord> list = [];
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return list;
            }
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                list.Add(Read(element));
            }
            return list;
        }
    }
}
