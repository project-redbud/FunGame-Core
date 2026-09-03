using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class InventoryConverter : BaseEntityConverter<Inventory>
    {
        private const string MainCharacterIdKey = "MainCharacterId";

        public override Inventory NewInstance()
        {
            return new(new());
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Inventory result, Dictionary<string, object> convertingContext)
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
                    HashSet<Character> characters = JsonService.GetObject<HashSet<Character>>(ref reader, options) ?? [];
                    foreach (Character character in characters)
                    {
                        result.Characters.Add(character);
                    }
                    break;
                case nameof(Inventory.Items):
                    HashSet<Item> items = JsonService.GetObject<HashSet<Item>>(ref reader, options) ?? [];
                    foreach (Item item in items)
                    {
                        result.Items.Add(item);
                    }
                    break;
                case nameof(Inventory.MainCharacter):
                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        Character? legacyMainCharacter = JsonService.GetObject<Character>(ref reader, options);
                        if (legacyMainCharacter != null)
                        {
                            result.MainCharacter = legacyMainCharacter;
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        convertingContext[MainCharacterIdKey] = reader.GetInt64();
                    }
                    break;
                case nameof(Inventory.Squad):
                    HashSet<long> squad = JsonService.GetObject<HashSet<long>>(ref reader, options) ?? [];
                    foreach (long cid in squad)
                    {
                        result.Squad.Add(cid);
                    }
                    break;
                case nameof(Inventory.Training):
                    Dictionary<long, DateTime> training = JsonService.GetObject<Dictionary<long, DateTime>>(ref reader, options) ?? [];
                    foreach (long cid in training.Keys)
                    {
                        result.Training.Add(cid, training[cid]);
                    }
                    break;
            }
        }

        public override void AfterConvert(ref Inventory result, Dictionary<string, object> convertingContext)
        {
            // 按 Id 从 Characters 中取回角色
            if (convertingContext.TryGetValue(MainCharacterIdKey, out object? value) && value is long mainCharacterId)
            {
                Character? mainCharacter = result.Characters.FirstOrDefault(c => c.Id == mainCharacterId);
                if (mainCharacter != null)
                {
                    result.MainCharacter = mainCharacter;
                }
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
            writer.WriteNumber(nameof(Inventory.MainCharacter), value.Characters.Count > 0 ? value.MainCharacter.Id : 0);
            writer.WritePropertyName(nameof(Inventory.Squad));
            JsonSerializer.Serialize(writer, value.Squad, options);
            writer.WritePropertyName(nameof(Inventory.Training));
            JsonSerializer.Serialize(writer, value.Training, options);

            writer.WriteEndObject();
        }
    }
}
