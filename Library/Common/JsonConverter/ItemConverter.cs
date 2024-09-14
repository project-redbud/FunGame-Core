using System.Text.Json;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class ItemConverter : BaseEntityConverter<Item>
    {
        public override Item NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Item result)
        {
            switch (propertyName)
            {
                case nameof(Item.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Item.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Item.ItemType):
                    result.ItemType = (ItemType)reader.GetInt32();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Item value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Item.Id), (int)value.Id);
            writer.WriteString(nameof(Item.Name), value.Name);
            writer.WriteNumber(nameof(Item.ItemType), (int)value.ItemType);

            writer.WriteEndObject();
        }
    }
}
