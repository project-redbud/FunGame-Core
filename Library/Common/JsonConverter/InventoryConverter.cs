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
                case nameof(Inventory.Credits):
                    result.Credits = reader.GetDouble();
                    break;
                case nameof(Inventory.Materials):
                    result.Materials = reader.GetDouble();
                    break;
                case nameof(Inventory.Characters):
                    HashSet<Character> characters = NetworkUtility.JsonDeserialize<HashSet<Character>>(ref reader, options) ?? [];
                    foreach (Character character in characters)
                    {
                        result.Characters.Add(character);
                    }
                    break;
                case nameof(Inventory.Items):
                    HashSet<Item> items = NetworkUtility.JsonDeserialize<HashSet<Item>>(ref reader, options) ?? [];
                    foreach (Item item in items)
                    {
                        result.Items.Add(item);
                    }
                    break;
                case nameof(Inventory.MainCharacter):
                    Character? mc = NetworkUtility.JsonDeserialize<Character>(ref reader, options);
                    if (mc != null)
                    {
                        result.MainCharacter = mc;
                    }
                    break;
                case nameof(Inventory.Squad):
                    HashSet<long> squad = NetworkUtility.JsonDeserialize<HashSet<long>>(ref reader, options) ?? [];
                    foreach (long cid in squad)
                    {
                        result.Squad.Add(cid);
                    }
                    break;
                case nameof(Inventory.Training):
                    Dictionary<long, DateTime> training = NetworkUtility.JsonDeserialize<Dictionary<long, DateTime>>(ref reader, options) ?? [];
                    foreach (long cid in training.Keys)
                    {
                        result.Training.Add(cid, training[cid]);
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Inventory value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Inventory.Name), value.Name);
            writer.WriteNumber(nameof(Inventory.Credits), value.Credits);
            writer.WriteNumber(nameof(Inventory.Materials), value.Materials);
            writer.WritePropertyName(nameof(Inventory.Characters));
            JsonSerializer.Serialize(writer, value.Characters, options);
            writer.WritePropertyName(nameof(Inventory.Items));
            JsonSerializer.Serialize(writer, value.Items, options);
            writer.WritePropertyName(nameof(Inventory.MainCharacter));
            JsonSerializer.Serialize(writer, value.MainCharacter, options);
            writer.WritePropertyName(nameof(Inventory.Squad));
            JsonSerializer.Serialize(writer, value.Squad, options);
            writer.WritePropertyName(nameof(Inventory.Training));
            JsonSerializer.Serialize(writer, value.Training, options);

            writer.WriteEndObject();
        }
    }
}
