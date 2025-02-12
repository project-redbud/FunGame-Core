using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class NovelOptionConverter : BaseEntityConverter<NovelOption>
    {
        public override NovelOption NewInstance()
        {
            return new NovelOption();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref NovelOption result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(NovelOption.Key):
                    result.Key = reader.GetString() ?? "";
                    break;
                case nameof(NovelOption.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(NovelOption.Targets):
                    result.Values[nameof(NovelOption.Targets)] = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    break;
                case nameof(NovelNode.AndPredicates):
                    result.Values[nameof(NovelNode.AndPredicates)] = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    break;
                case nameof(NovelNode.OrPredicates):
                    result.Values[nameof(NovelNode.OrPredicates)] = NetworkUtility.JsonDeserialize<List<string>>(ref reader, options) ?? [];
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, NovelOption value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(NovelOption.Key), value.Key);
            writer.WriteString(nameof(NovelOption.Name), value.Name);
            writer.WritePropertyName(nameof(NovelOption.Targets));
            JsonSerializer.Serialize(writer, value.Targets.Select(n => n.Key).ToList(), options);
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
