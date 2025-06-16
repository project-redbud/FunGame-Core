using System.Text.Json;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class ShieldConverter : BaseEntityConverter<Shield>
    {
        public override Shield NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Shield result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Shield.Physical):
                    result.Physical = reader.GetDouble();
                    break;
                case nameof(Shield.None):
                    result.None = reader.GetDouble();
                    break;
                case nameof(Shield.Starmark):
                    result.Starmark = reader.GetDouble();
                    break;
                case nameof(Shield.PurityNatural):
                    result.PurityNatural = reader.GetDouble();
                    break;
                case nameof(Shield.PurityContemporary):
                    result.PurityContemporary = reader.GetDouble();
                    break;
                case nameof(Shield.Bright):
                    result.Bright = reader.GetDouble();
                    break;
                case nameof(Shield.Shadow):
                    result.Shadow = reader.GetDouble();
                    break;
                case nameof(Shield.Element):
                    result.Element = reader.GetDouble();
                    break;
                case nameof(Shield.Aster):
                    result.Aster = reader.GetDouble();
                    break;
                case nameof(Shield.SpatioTemporal):
                    result.SpatioTemporal = reader.GetDouble();
                    break;
                case nameof(Shield.Mix):
                    result.Mix = reader.GetDouble();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Shield value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Shield.Physical), value.Physical);
            writer.WriteNumber(nameof(Shield.None), value.None);
            writer.WriteNumber(nameof(Shield.Starmark), value.Starmark);
            writer.WriteNumber(nameof(Shield.PurityNatural), value.PurityNatural);
            writer.WriteNumber(nameof(Shield.PurityContemporary), value.PurityContemporary);
            writer.WriteNumber(nameof(Shield.Bright), value.Bright);
            writer.WriteNumber(nameof(Shield.Shadow), value.Shadow);
            writer.WriteNumber(nameof(Shield.Element), value.Element);
            writer.WriteNumber(nameof(Shield.Aster), value.Aster);
            writer.WriteNumber(nameof(Shield.SpatioTemporal), value.SpatioTemporal);
            writer.WriteNumber(nameof(Shield.Mix), value.Mix);

            writer.WriteEndObject();
        }
    }
}
