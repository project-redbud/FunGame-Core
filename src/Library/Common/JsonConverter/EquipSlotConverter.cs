using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class EquipSlotConverter : BaseEntityConverter<EquipSlot>
    {
        public override EquipSlot NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref EquipSlot result, Dictionary<string, object> convertingContext)
        {
            Item temp;
            switch (propertyName)
            {
                case nameof(EquipSlot.MagicCardPack):
                    temp = JsonService.GetObject<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == EquipSlotType.MagicCardPack)
                    {
                        result.MagicCardPack = temp;
                    }
                    break;
                case nameof(EquipSlot.Weapon):
                    temp = JsonService.GetObject<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == EquipSlotType.Weapon)
                    {
                        result.Weapon = temp;
                    }
                    break;
                case nameof(EquipSlot.Armor):
                    temp = JsonService.GetObject<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == EquipSlotType.Armor)
                    {
                        result.Armor = temp;
                    }
                    break;
                case nameof(EquipSlot.Shoes):
                    temp = JsonService.GetObject<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == EquipSlotType.Shoes)
                    {
                        result.Shoes = temp;
                    }
                    break;
                case nameof(EquipSlot.Accessory1):
                    temp = JsonService.GetObject<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == EquipSlotType.Accessory1)
                    {
                        result.Accessory1 = temp;
                    }
                    break;
                case nameof(EquipSlot.Accessory2):
                    temp = JsonService.GetObject<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == EquipSlotType.Accessory2)
                    {
                        result.Accessory2 = temp;
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, EquipSlot value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(value.MagicCardPack));
            JsonSerializer.Serialize(writer, value.MagicCardPack, options);

            writer.WritePropertyName(nameof(value.Weapon));
            JsonSerializer.Serialize(writer, value.Weapon, options);

            writer.WritePropertyName(nameof(value.Armor));
            JsonSerializer.Serialize(writer, value.Armor, options);

            writer.WritePropertyName(nameof(value.Shoes));
            JsonSerializer.Serialize(writer, value.Shoes, options);

            writer.WritePropertyName(nameof(value.Accessory1));
            JsonSerializer.Serialize(writer, value.Accessory1, options);

            writer.WritePropertyName(nameof(value.Accessory2));
            JsonSerializer.Serialize(writer, value.Accessory2, options);

            writer.WriteEndObject();
        }
    }
}
