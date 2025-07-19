using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class StoreConverter : BaseEntityConverter<Store>
    {
        public override Store NewInstance()
        {
            return new Store("商店");
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Store result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Store.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Store.StartTime):
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
                case nameof(Store.EndTime):
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
                case nameof(Store.Goods):
                    Dictionary<long, Goods> goods = NetworkUtility.JsonDeserialize<Dictionary<long, Goods>>(ref reader, options) ?? [];
                    foreach (long id in goods.Keys)
                    {
                        result.Goods[id] = goods[id];
                    }
                    break;
                case nameof(Store.AutoRefresh):
                    result.AutoRefresh = reader.GetBoolean();
                    break;
                case nameof(Store.NextRefreshDate):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out time))
                    {
                        result.NextRefreshDate = time;
                    }
                    else
                    {
                        result.NextRefreshDate = DateTime.MinValue;
                    }
                    break;
                case nameof(Store.RefreshInterval):
                    result.RefreshInterval = Convert.ToInt32(reader.GetInt64());
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Store value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Store.Name), value.Name);
            if (value.StartTime.HasValue) writer.WriteString(nameof(Store.StartTime), value.StartTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.EndTime.HasValue) writer.WriteString(nameof(Store.EndTime), value.EndTime.Value.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName(nameof(Store.Goods));
            JsonSerializer.Serialize(writer, value.Goods, options);
            writer.WriteBoolean(nameof(Store.AutoRefresh), value.AutoRefresh);
            writer.WriteString(nameof(Store.NextRefreshDate), value.NextRefreshDate.ToString(General.GeneralDateTimeFormat));
            writer.WriteNumber(nameof(Store.RefreshInterval), value.RefreshInterval);

            writer.WriteEndObject();
        }
    }
}
