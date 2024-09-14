using System.Text.Json;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class SkillConverter : BaseEntityConverter<Skill>
    {
        public override Skill NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Skill result)
        {
            switch (propertyName)
            {
                case nameof(Skill.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Skill.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Skill.SkillType):
                    result.SkillType = (SkillType)reader.GetInt32();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Skill value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Skill.Id), (int)value.Id);
            writer.WriteString(nameof(Skill.Name), value.Name);
            writer.WriteNumber(nameof(Skill.SkillType), (int)value.SkillType);

            writer.WriteEndObject();
        }
    }
}
