using System.Text.Json;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class MagicResistanceConverter : BaseEntityConverter<MagicResistance>
    {
        public override MagicResistance NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref MagicResistance result)
        {
            switch (propertyName)
            {
                case nameof(MagicResistance.None):
                    result.None = reader.GetDouble();
                    break;
                case nameof(MagicResistance.Starmark):
                    result.Starmark = reader.GetDouble();
                    break;
                case nameof(MagicResistance.PurityNatural):
                    result.PurityNatural = reader.GetDouble();
                    break;
                case nameof(MagicResistance.PurityContemporary):
                    result.PurityContemporary = reader.GetDouble();
                    break;
                case nameof(MagicResistance.Bright):
                    result.Bright = reader.GetDouble();
                    break;
                case nameof(MagicResistance.Shadow):
                    result.Shadow = reader.GetDouble();
                    break;
                case nameof(MagicResistance.Element):
                    result.Element = reader.GetDouble();
                    break;
                case nameof(MagicResistance.Fleabane):
                    result.Fleabane = reader.GetDouble();
                    break;
                case nameof(MagicResistance.Particle):
                    result.Particle = reader.GetDouble();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, MagicResistance value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(MagicResistance.None), value.None);
            writer.WriteNumber(nameof(MagicResistance.Starmark), value.Starmark);
            writer.WriteNumber(nameof(MagicResistance.PurityNatural), value.PurityNatural);
            writer.WriteNumber(nameof(MagicResistance.PurityContemporary), value.PurityContemporary);
            writer.WriteNumber(nameof(MagicResistance.Bright), value.Bright);
            writer.WriteNumber(nameof(MagicResistance.Shadow), value.Shadow);
            writer.WriteNumber(nameof(MagicResistance.Element), value.Element);
            writer.WriteNumber(nameof(MagicResistance.Fleabane), value.Fleabane);
            writer.WriteNumber(nameof(MagicResistance.Particle), value.Particle);

            writer.WriteEndObject();
        }
    }
}
