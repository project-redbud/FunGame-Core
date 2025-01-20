using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class GoodsConverter : BaseEntityConverter<Goods>
    {
        public override Goods NewInstance()
        {
            return new Goods();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Goods result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Goods.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Goods.Items):
                    List<Item> items = NetworkUtility.JsonDeserialize<List<Item>>(ref reader, options) ?? [];
                    foreach (Item item in items)
                    {
                        result.Items.Add(item);
                    }
                    break;
                case nameof(Goods.Stock):
                    result.Stock = reader.GetInt32();
                    break;
                case nameof(Goods.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Goods.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Goods.Prices):
                    Dictionary<string, double> prices = NetworkUtility.JsonDeserialize<Dictionary<string, double>>(ref reader, options) ?? [];
                    foreach (string needy in prices.Keys)
                    {
                        result.Prices[needy] = prices[needy];
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Goods value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Goods.Id), value.Id);
            writer.WritePropertyName(nameof(Goods.Items));
            JsonSerializer.Serialize(writer, value.Items, options);
            writer.WriteNumber(nameof(Goods.Stock), value.Stock);
            writer.WriteString(nameof(Goods.Name), value.Name);
            writer.WriteString(nameof(Goods.Description), value.Description);
            writer.WritePropertyName(nameof(Goods.Prices));
            JsonSerializer.Serialize(writer, value.Prices, options);

            writer.WriteEndObject();
        }
    }
}
