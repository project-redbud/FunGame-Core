using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class SkillConverter : BaseEntityConverter<Skill>
    {
        public override Skill NewInstance()
        {
            return new OpenSkill(0, "");
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
                case nameof(Skill.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Skill.GeneralDescription):
                    result.GeneralDescription = reader.GetString() ?? "";
                    break;
                case nameof(Skill.Level):
                    result.Level = reader.GetInt32();
                    break;
                case nameof(Skill.SkillType):
                    result.SkillType = (SkillType)reader.GetInt32();
                    break;
                case nameof(Skill.Enable):
                    result.Enable = reader.GetBoolean();
                    break;
                case nameof(Skill.IsInEffect):
                    result.IsInEffect = reader.GetBoolean();
                    break;
                case nameof(Skill.MPCost):
                    result.MPCost = reader.GetDouble();
                    break;
                case nameof(Skill.EPCost):
                    result.EPCost = reader.GetDouble();
                    break;
                case nameof(Skill.CastTime):
                    result.CastTime = reader.GetDouble();
                    break;
                case nameof(Skill.CD):
                    result.CD = reader.GetDouble();
                    break;
                case nameof(Skill.CurrentCD):
                    result.CurrentCD = reader.GetDouble();
                    break;
                case nameof(Skill.HardnessTime):
                    result.HardnessTime = reader.GetDouble();
                    break;
                case nameof(Skill.Effects):
                    HashSet<Effect> effects = NetworkUtility.JsonDeserialize<HashSet<Effect>>(ref reader, options) ?? [];
                    foreach (Effect effect in effects)
                    {
                        result.Effects.Add(effect);
                    }
                    break;
                case nameof(Skill.OtherArgs):
                    Dictionary<string, object> others = NetworkUtility.JsonDeserialize<Dictionary<string, object>>(ref reader, options) ?? [];
                    foreach (string key in others.Keys)
                    {
                        result.OtherArgs.Add(key, others[key]);
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Skill value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Skill.Id), (int)value.Id);
            writer.WriteString(nameof(Skill.Name), value.Name);
            writer.WriteString(nameof(Skill.Description), value.Description);
            if (value.GeneralDescription.Length > 0) writer.WriteString(nameof(Skill.GeneralDescription), value.GeneralDescription);
            if (value.Level > 0) writer.WriteNumber(nameof(Skill.Level), value.Level);
            writer.WriteNumber(nameof(Skill.SkillType), (int)value.SkillType);
            if (!value.Enable) writer.WriteBoolean(nameof(Skill.Enable), value.Enable);
            if (value.IsInEffect) writer.WriteBoolean(nameof(Skill.IsInEffect), value.IsInEffect);
            if (value.MPCost > 0) writer.WriteNumber(nameof(Skill.MPCost), value.MPCost);
            if (value.EPCost > 0) writer.WriteNumber(nameof(Skill.EPCost), value.EPCost);
            if (value.CastTime > 0) writer.WriteNumber(nameof(Skill.CastTime), value.CastTime);
            if (value.CD > 0) writer.WriteNumber(nameof(Skill.CD), value.CD);
            if (value.CurrentCD > 0) writer.WriteNumber(nameof(Skill.CurrentCD), value.CurrentCD);
            if (value.HardnessTime > 0) writer.WriteNumber(nameof(Skill.HardnessTime), value.HardnessTime);
            writer.WritePropertyName(nameof(Skill.Effects));
            JsonSerializer.Serialize(writer, value.Effects, options);
            writer.WritePropertyName(nameof(Skill.OtherArgs));
            JsonSerializer.Serialize(writer, value.OtherArgs, options);

            writer.WriteEndObject();
        }
    }
}
