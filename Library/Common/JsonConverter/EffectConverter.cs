using System.Text.Json;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class EffectConverter : BaseEntityConverter<Effect>
    {
        public override Effect NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Effect result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Effect.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Effect.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                default:
                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        result.Values[propertyName] = reader.GetDouble();
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        result.Values[propertyName] = reader.GetString() ?? "";
                    }
                    else if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                    {
                        result.Values[propertyName] = reader.GetBoolean();
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Effect value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Effect.Id), (int)value.Id);
            writer.WriteString(nameof(Effect.Name), value.Name);

            foreach (var kvp in value.Values)
            {
                switch (kvp.Value)
                {
                    case int intValue:
                        writer.WriteNumber(kvp.Key, intValue);
                        break;
                    case double doubleValue:
                        writer.WriteNumber(kvp.Key, doubleValue);
                        break;
                    case bool boolValue:
                        writer.WriteBoolean(kvp.Key, boolValue);
                        break;
                    case string strValue:
                        writer.WriteString(kvp.Key, strValue);
                        break;
                    default:
                        JsonSerializer.Serialize(writer, kvp.Value, options);
                        break;
                }
            }

            writer.WriteEndObject();
        }
    }
}
