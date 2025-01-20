using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

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
                case nameof(Store.Goods):
                    Dictionary<long, Goods> goods = NetworkUtility.JsonDeserialize<Dictionary<long, Goods>>(ref reader, options) ?? [];
                    foreach (long id in goods.Keys)
                    {
                        result.Goods[id] = goods[id];
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Store value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Store.Name), value.Name);
            writer.WritePropertyName(nameof(Store.Goods));
            JsonSerializer.Serialize(writer, value.Goods, options);

            writer.WriteEndObject();
        }
    }
}
