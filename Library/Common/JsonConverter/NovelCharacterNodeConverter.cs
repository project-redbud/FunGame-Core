using System.Text.Json;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class NovelCharacterNodeConverter : BaseEntityConverter<NovelCharacterNode>
    {
        public override NovelCharacterNode NewInstance()
        {
            return new NovelCharacterNode();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref NovelCharacterNode result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(NovelCharacterNode.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(NovelCharacterNode.PortraitImagePath):
                    result.PortraitImagePath = reader.GetString() ?? "";
                    break;
                case nameof(NovelCharacterNode.StandOut):
                    result.StandOut = reader.GetBoolean();
                    break;
                case nameof(NovelCharacterNode.PositionType):
                    result.PositionType = (PositionType)reader.GetInt32();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, NovelCharacterNode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(NovelCharacterNode.Name), value.Name);
            if (value.PortraitImagePath != "") writer.WriteString(nameof(NovelCharacterNode.PortraitImagePath), value.PortraitImagePath);
            if (value.StandOut) writer.WriteBoolean(nameof(NovelCharacterNode.StandOut), value.StandOut);
            if (value.PositionType != PositionType.Center) writer.WriteNumber(nameof(NovelCharacterNode.PositionType), (int)value.PositionType);
            writer.WriteEndObject();
        }
    }
}
