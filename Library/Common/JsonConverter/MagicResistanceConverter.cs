using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class MDFConverter : BaseEntityConverter<MDF>
    {
        public override MDF NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref MDF result)
        {
            MagicResistance temp;
            switch (propertyName)
            {
                case nameof(MDF.None):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.None)
                    {
                        result.None.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.Starmark):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.Starmark)
                    {
                        result.Starmark.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.PurityNatural):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.PurityNatural)
                    {
                        result.PurityNatural.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.PurityContemporary):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.PurityContemporary)
                    {
                        result.PurityContemporary.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.Bright):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.Bright)
                    {
                        result.Bright.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.Shadow):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.Shadow)
                    {
                        result.Shadow.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.Element):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.Element)
                    {
                        result.Element.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.Fleabane):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.Fleabane)
                    {
                        result.Fleabane.Value = temp.Value;
                    }
                    break;
                case nameof(MDF.Particle):
                    temp = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    if (temp.MagicType == MagicType.Particle)
                    {
                        result.Particle.Value = temp.Value;
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, MDF value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(MDF.None));
            JsonSerializer.Serialize(writer, value.None, options);

            writer.WritePropertyName(nameof(MDF.Starmark));
            JsonSerializer.Serialize(writer, value.Starmark, options);

            writer.WritePropertyName(nameof(MDF.PurityNatural));
            JsonSerializer.Serialize(writer, value.PurityNatural, options);

            writer.WritePropertyName(nameof(MDF.PurityContemporary));
            JsonSerializer.Serialize(writer, value.PurityContemporary, options);

            writer.WritePropertyName(nameof(MDF.Bright));
            JsonSerializer.Serialize(writer, value.Bright, options);

            writer.WritePropertyName(nameof(MDF.Shadow));
            JsonSerializer.Serialize(writer, value.Shadow, options);

            writer.WritePropertyName(nameof(MDF.Element));
            JsonSerializer.Serialize(writer, value.Element, options);

            writer.WritePropertyName(nameof(MDF.Fleabane));
            JsonSerializer.Serialize(writer, value.Fleabane, options);

            writer.WritePropertyName(nameof(MDF.Particle));
            JsonSerializer.Serialize(writer, value.Particle, options);

            writer.WriteEndObject();
        }
    }

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
                case nameof(MagicResistance.MagicType):
                    result = new((MagicType)reader.GetInt32(), result.Value);
                    break;
                case nameof(MagicResistance.Value):
                    result.Value = reader.GetDouble();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, MagicResistance value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(MagicResistance.MagicType), (int)value.MagicType);
            writer.WriteNumber(nameof(MagicResistance.Value), value.Value);
            writer.WriteEndObject();
        }
    }
}
