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
                case nameof(Store.Description):
                    result.Description = reader.GetString() ?? "";
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
                case nameof(Store.StartTimeOfDay):
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
                case nameof(Store.EndTimeOfDay):
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
                case nameof(Store.NextRefreshGoods):
                    Dictionary<long, Goods> goods2 = NetworkUtility.JsonDeserialize<Dictionary<long, Goods>>(ref reader, options) ?? [];
                    foreach (long id in goods2.Keys)
                    {
                        result.NextRefreshGoods[id] = goods2[id];
                    }
                    break;
                case nameof(Store.RefreshInterval):
                    result.RefreshInterval = Convert.ToInt32(reader.GetInt64());
                    break;
                case nameof(Store.GetNewerGoodsOnVisiting):
                    result.GetNewerGoodsOnVisiting = reader.GetBoolean();
                    break;
                case nameof(Store.GlobalStock):
                    result.GlobalStock = reader.GetBoolean();
                    break;
                case nameof(Store.ExpireTime):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out time))
                    {
                        result.ExpireTime = time;
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Store value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Store.Name), value.Name);
            writer.WriteString(nameof(Store.Description), value.Description);
            if (value.StartTime.HasValue) writer.WriteString(nameof(Store.StartTime), value.StartTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.EndTime.HasValue) writer.WriteString(nameof(Store.EndTime), value.EndTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.StartTimeOfDay.HasValue) writer.WriteString(nameof(Store.StartTimeOfDay), value.StartTimeOfDay.Value.ToString(General.GeneralDateTimeFormat));
            if (value.EndTimeOfDay.HasValue) writer.WriteString(nameof(Store.EndTimeOfDay), value.EndTimeOfDay.Value.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName(nameof(Store.Goods));
            JsonSerializer.Serialize(writer, value.Goods, options);
            writer.WriteBoolean(nameof(Store.AutoRefresh), value.AutoRefresh);
            writer.WriteString(nameof(Store.NextRefreshDate), value.NextRefreshDate.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName(nameof(Store.NextRefreshGoods));
            JsonSerializer.Serialize(writer, value.NextRefreshGoods, options);
            writer.WriteNumber(nameof(Store.RefreshInterval), value.RefreshInterval);
            writer.WriteBoolean(nameof(Store.GetNewerGoodsOnVisiting), value.GetNewerGoodsOnVisiting);
            writer.WriteBoolean(nameof(Store.GlobalStock), value.GlobalStock);
            if (value.ExpireTime.HasValue) writer.WriteString(nameof(Store.ExpireTime), value.ExpireTime.Value.ToString(General.GeneralDateTimeFormat));

            writer.WriteEndObject();
        }
    }
}
