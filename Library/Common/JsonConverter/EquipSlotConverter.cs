using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class EquipSlotConverter : BaseEntityConverter<EquipSlot>
    {
        public override EquipSlot NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref EquipSlot result)
        {
            Item temp;
            switch (propertyName)
            {
                case nameof(EquipSlot.MagicCardPack):
                    temp = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == Constant.EquipSlotType.MagicCardPack)
                    {
                        result.MagicCardPack = temp;
                    }
                    break;
                case nameof(EquipSlot.Weapon):
                    temp = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == Constant.EquipSlotType.Weapon)
                    {
                        result.Weapon = temp;
                    }
                    break;
                case nameof(EquipSlot.Armor):
                    temp = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == Constant.EquipSlotType.Armor)
                    {
                        result.Armor = temp;
                    }
                    break;
                case nameof(EquipSlot.Shoes):
                    temp = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == Constant.EquipSlotType.Shoes)
                    {
                        result.Shoes = temp;
                    }
                    break;
                case nameof(EquipSlot.Accessory1):
                    temp = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == Constant.EquipSlotType.Accessory1)
                    {
                        result.Accessory1 = temp;
                    }
                    break;
                case nameof(EquipSlot.Accessory2):
                    temp = NetworkUtility.JsonDeserialize<Item>(ref reader, options) ?? new();
                    if (temp.EquipSlotType == Constant.EquipSlotType.Accessory2)
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
