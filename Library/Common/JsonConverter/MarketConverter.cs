using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class MarketConverter : BaseEntityConverter<Market>
    {
        public override Market NewInstance()
        {
            return new Market("市场");
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Market result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Market.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Market.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Market.StartTime):
                    string dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime time))
                    {
                        result.StartTime = time;
                    }
                    else
                    {
                        result.StartTime = DateTime.MinValue;
                    }
                    break;
                case nameof(Market.EndTime):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out time))
                    {
                        result.EndTime = time;
                    }
                    else
                    {
                        result.EndTime = DateTime.MinValue;
                    }
                    break;
                case nameof(Market.StartTimeOfDay):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out time))
                    {
                        result.StartTimeOfDay = time;
                    }
                    else
                    {
                        result.StartTimeOfDay = DateTime.MinValue;
                    }
                    break;
                case nameof(Market.EndTimeOfDay):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out time))
                    {
                        result.EndTimeOfDay = time;
                    }
                    else
                    {
                        result.EndTimeOfDay = DateTime.MinValue;
                    }
                    break;
                case nameof(Market.MarketItems):
                    Dictionary<long, MarketItem> marketItems = NetworkUtility.JsonDeserialize<Dictionary<long, MarketItem>>(ref reader, options) ?? [];
                    foreach (long id in marketItems.Keys)
                    {
                        result.MarketItems[id] = marketItems[id];
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Market value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Market.Name), value.Name);
            writer.WriteString(nameof(Market.Description), value.Description);
            if (value.StartTime.HasValue) writer.WriteString(nameof(Market.StartTime), value.StartTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.EndTime.HasValue) writer.WriteString(nameof(Market.EndTime), value.EndTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.StartTimeOfDay.HasValue) writer.WriteString(nameof(Market.StartTimeOfDay), value.StartTimeOfDay.Value.ToString(General.GeneralDateTimeFormat));
            if (value.EndTimeOfDay.HasValue) writer.WriteString(nameof(Market.EndTimeOfDay), value.EndTimeOfDay.Value.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName(nameof(Market.MarketItems));
            JsonSerializer.Serialize(writer, value.MarketItems, options);

            writer.WriteEndObject();
        }
    }
}
