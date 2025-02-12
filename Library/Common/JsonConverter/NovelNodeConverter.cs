using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class NovelNodeConverter : BaseEntityConverter<NovelNode>
    {
        public override NovelNode NewInstance()
        {
            return new NovelNode();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref NovelNode result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(NovelNode.Key):
                    result.Key = reader.GetString() ?? "";
                    break;
                case nameof(NovelNode.Priority):
                    result.Priority = reader.GetInt32();
                    break;
                case nameof(NovelNode.Previous):
                    result.Values[nameof(NovelNode.Previous)] = reader.GetString() ?? "";
                    break;
                case nameof(NovelNode.NextNodes):
                    result.Values[nameof(NovelNode.NextNodes)] = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    break;
                case nameof(NovelNode.Options):
                    result.Options = NetworkUtility.JsonDeserialize<List<NovelOption>>(ref reader, options) ?? [];
                    break;
                case nameof(NovelNode.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(NovelNode.Name2):
                    result.Name2 = reader.GetString() ?? "";
                    break;
                case nameof(NovelNode.Content):
                    result.Content = reader.GetString() ?? "";
                    break;
                case nameof(NovelNode.PortraitImagePath):
                    result.PortraitImagePath = reader.GetString() ?? "";
                    break;
                case nameof(NovelNode.AndPredicates):
                    result.Values[nameof(NovelNode.AndPredicates)] = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    break;
                case nameof(NovelNode.OrPredicates):
                    result.Values[nameof(NovelNode.OrPredicates)] = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, NovelNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(NovelNode.Key), value.Key);
            writer.WriteNumber(nameof(NovelNode.Priority), value.Priority);
            if (value.Previous != null) writer.WriteString(nameof(NovelNode.Previous), value.Previous.Key);
            writer.WritePropertyName(nameof(NovelNode.NextNodes));
            JsonSerializer.Serialize(writer, value.NextNodes.Select(n => n.Key).ToList(), options);
            if (value.Options.Count > 0)
            {
                writer.WritePropertyName(nameof(NovelNode.Options));
                JsonSerializer.Serialize(writer, value.Options, options);
            }
            writer.WriteString(nameof(NovelNode.Name), value.Name);
            if (value.Name2 != "") writer.WriteString(nameof(NovelNode.Name2), value.Name2);
            writer.WriteString(nameof(NovelNode.Content), value.Content);
            if (value.PortraitImagePath != "") writer.WriteString(nameof(NovelNode.PortraitImagePath), value.PortraitImagePath);
            if (value.AndPredicates.Count > 0)
            {
                writer.WritePropertyName(nameof(NovelNode.AndPredicates));
                JsonSerializer.Serialize(writer, value.AndPredicates.Keys.ToList(), options);
            }
            if (value.OrPredicates.Count > 0)
            {
                writer.WritePropertyName(nameof(NovelNode.OrPredicates));
                JsonSerializer.Serialize(writer, value.OrPredicates.Keys.ToList(), options);
            }
            writer.WriteEndObject();
        }
    }
}
