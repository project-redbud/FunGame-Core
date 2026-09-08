using System.Text.Json;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Library.Common.JsonConverter
{
    /// <summary>
    /// 询问概要（<see cref="InquiryRecord"/>）的读写辅助，供回合记录与操作记录的转换器共用
    /// <para>其中的角色按 <see cref="CharacterRefHelper"/> 的引用规则处理，只保留 Guid 与展示字段</para>
    /// </summary>
    internal static class InquiryRecordHelper
    {
        public static void Write(Utf8JsonWriter writer, InquiryRecord value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(InquiryRecord.Character));
            CharacterRefHelper.Write(writer, value.Character);
            writer.WriteString(nameof(InquiryRecord.Topic), value.Topic);
            writer.WriteString(nameof(InquiryRecord.Description), value.Description);
            writer.WriteNumber(nameof(InquiryRecord.InquiryType), (int)value.InquiryType);
            writer.WritePropertyName(nameof(InquiryRecord.Choices));
            JsonSerializer.Serialize(writer, value.Choices, options);
            writer.WritePropertyName(nameof(InquiryRecord.Selected));
            JsonSerializer.Serialize(writer, value.Selected, options);
            writer.WriteString(nameof(InquiryRecord.TextResult), value.TextResult);
            writer.WriteNumber(nameof(InquiryRecord.NumberResult), value.NumberResult);
            writer.WriteBoolean(nameof(InquiryRecord.Cancel), value.Cancel);
            writer.WriteNumber(nameof(InquiryRecord.Source), (int)value.Source);
            writer.WriteEndObject();
        }

        public static void WriteList(Utf8JsonWriter writer, IEnumerable<InquiryRecord> values, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (InquiryRecord value in values)
            {
                Write(writer, value, options);
            }
            writer.WriteEndArray();
        }

        public static InquiryRecord Read(JsonElement element, JsonSerializerOptions options)
        {
            InquiryRecord record = new();
            if (element.TryGetProperty(nameof(InquiryRecord.Character), out JsonElement character))
            {
                record.Character = CharacterRefHelper.ReadElement(character);
            }
            if (element.TryGetProperty(nameof(InquiryRecord.Topic), out JsonElement topic))
            {
                record.Topic = topic.GetString() ?? "";
            }
            if (element.TryGetProperty(nameof(InquiryRecord.Description), out JsonElement description))
            {
                record.Description = description.GetString() ?? "";
            }
            if (element.TryGetProperty(nameof(InquiryRecord.InquiryType), out JsonElement type) && type.TryGetInt32(out int inquiryType))
            {
                record.InquiryType = (InquiryType)inquiryType;
            }
            if (element.TryGetProperty(nameof(InquiryRecord.Choices), out JsonElement choices))
            {
                record.Choices = JsonSerializer.Deserialize<Dictionary<string, string>>(choices.GetRawText(), options) ?? [];
            }
            if (element.TryGetProperty(nameof(InquiryRecord.Selected), out JsonElement selected))
            {
                record.Selected = JsonSerializer.Deserialize<List<string>>(selected.GetRawText(), options) ?? [];
            }
            if (element.TryGetProperty(nameof(InquiryRecord.TextResult), out JsonElement text))
            {
                record.TextResult = text.GetString() ?? "";
            }
            if (element.TryGetProperty(nameof(InquiryRecord.NumberResult), out JsonElement number) && number.TryGetDouble(out double numberResult))
            {
                record.NumberResult = numberResult;
            }
            if (element.TryGetProperty(nameof(InquiryRecord.Cancel), out JsonElement cancel) && cancel.ValueKind == JsonValueKind.True)
            {
                record.Cancel = true;
            }
            if (element.TryGetProperty(nameof(InquiryRecord.Source), out JsonElement source) && source.TryGetInt32(out int sourceValue))
            {
                record.Source = (InquiryResponseSource)sourceValue;
            }
            return record;
        }

        public static List<InquiryRecord> ReadList(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            List<InquiryRecord> list = [];
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                list.Add(Read(element, options));
            }
            return list;
        }
    }
}
