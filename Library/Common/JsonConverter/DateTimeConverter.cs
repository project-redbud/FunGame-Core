using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            string date = reader.GetString() ?? "";

            if (DateTime.TryParseExact(date, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(General.GeneralDateTimeFormat));
        }
    }
}
