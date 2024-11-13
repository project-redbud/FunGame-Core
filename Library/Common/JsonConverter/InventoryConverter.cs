using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class InventoryConverter : BaseEntityConverter<Inventory>
    {
        public override Inventory NewInstance()
        {
            return Factory.GetInventory();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Inventory result)
        {
            switch (propertyName)
            {
                case nameof(Inventory.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Inventory.Characters):
                    Dictionary<string, Character> characters = NetworkUtility.JsonDeserialize<Dictionary<string, Character>>(ref reader, options) ?? [];
                    foreach (string key in characters.Keys)
                    {
                        result.Characters[key] = characters[key];
                    }
                    break;
                case nameof(Inventory.Items):
                    Dictionary<string, Item> items = NetworkUtility.JsonDeserialize<Dictionary<string, Item>>(ref reader, options) ?? [];
                    foreach (string key in items.Keys)
                    {
                        result.Items[key] = items[key];
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Inventory value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Inventory.Name), value.Name);
            writer.WritePropertyName(nameof(Inventory.Characters));
            JsonSerializer.Serialize(writer, value.Characters, options);
            writer.WritePropertyName(nameof(Inventory.Items));
            JsonSerializer.Serialize(writer, value.Items, options);

            writer.WriteEndObject();
        }
    }
}
