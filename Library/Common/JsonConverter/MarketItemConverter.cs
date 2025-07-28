using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class MarketItemConverter : BaseEntityConverter<MarketItem>
    {
        public override MarketItem NewInstance()
        {
            return new MarketItem();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref MarketItem result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(MarketItem.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(MarketItem.User):
                    result.User = reader.GetInt64();
                    break;
                case nameof(MarketItem.Username):
                    result.Username = reader.GetString() ?? "";
                    break;
                case nameof(MarketItem.Item):
                    result.Item = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? Factory.GetItem();
                    break;
                case nameof(MarketItem.Stock):
                    result.Stock = Convert.ToInt32(reader.GetInt64());
                    break;
                case nameof(MarketItem.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(MarketItem.Price):
                    result.Price = reader.GetDouble();
                    break;
                case nameof(MarketItem.CreateTime):
                    string dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime time))
                    {
                        result.CreateTime = time;
                    }
                    else
                    {
                        result.CreateTime = DateTime.MinValue;
                    }
                    break;
                case nameof(MarketItem.FinishTime):
                    dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out time))
                    {
                        result.FinishTime = time;
                    }
                    else
                    {
                        result.FinishTime = DateTime.MinValue;
                    }
                    break;
                case nameof(MarketItem.Status):
                    result.Status = (MarketItemState)reader.GetInt32();
                    break;
                case nameof(MarketItem.Buyers):
                    result.Buyers = NetworkUtility.JsonDeserialize<HashSet<long>>(ref reader, options) ?? [];
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, MarketItem value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(MarketItem.Id), value.Id);
            writer.WriteNumber(nameof(MarketItem.User), value.User);
            writer.WriteString(nameof(MarketItem.Username), value.Username);
            writer.WritePropertyName(nameof(MarketItem.Item));
            JsonSerializer.Serialize(writer, value.Item, options);
            writer.WriteNumber(nameof(MarketItem.Stock), value.Stock);
            writer.WriteString(nameof(MarketItem.Name), value.Name);
            writer.WriteNumber(nameof(MarketItem.Price), value.Price);
            writer.WriteString(nameof(MarketItem.CreateTime), value.CreateTime.ToString(General.GeneralDateTimeFormat));
            if (value.FinishTime.HasValue) writer.WriteString(nameof(MarketItem.FinishTime), value.FinishTime.Value.ToString(General.GeneralDateTimeFormat));
            writer.WriteNumber(nameof(MarketItem.Status), (int)value.Status);
            writer.WritePropertyName(nameof(MarketItem.Buyers));
            JsonSerializer.Serialize(writer, value.Buyers, options);

            writer.WriteEndObject();
        }
    }
}
