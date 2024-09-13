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

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Effect result)
        {
            switch (propertyName)
            {
                case nameof(Effect.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Effect.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Effect value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Effect.Id), (int)value.Id);
            writer.WriteString(nameof(Effect.Name), value.Name);

            writer.WriteEndObject();
        }
    }
}
