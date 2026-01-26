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
            return new OpenSkill(0, "", []);
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Skill result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Skill.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Skill.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Skill.Guid):
                    result.Guid = reader.GetGuid();
                    break;
                case nameof(Skill.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Skill.GeneralDescription):
                    result.GeneralDescription = reader.GetString() ?? "";
                    break;
                case nameof(Skill.Slogan):
                    result.Slogan = reader.GetString() ?? "";
                    break;
                case nameof(Skill.SkillType):
                    result.SkillType = (SkillType)reader.GetInt32();
                    break;
                case nameof(Skill.Level):
                    result.Level = reader.GetInt32();
                    break;
                case nameof(Skill.ExLevel):
                    result.ExLevel = reader.GetInt32();
                    break;
                case nameof(Skill.CastAnywhere):
                    result.CastAnywhere = reader.GetBoolean();
                    break;
                case nameof(Skill.CastRange):
                    result.CastRange = reader.GetInt32();
                    break;
                case nameof(Skill.CanSelectSelf):
                    result.CanSelectSelf = reader.GetBoolean();
                    break;
                case nameof(Skill.CanSelectEnemy):
                    result.CanSelectEnemy = reader.GetBoolean();
                    break;
                case nameof(Skill.CanSelectTeammate):
                    result.CanSelectTeammate = reader.GetBoolean();
                    break;
                case nameof(Skill.CanSelectTargetCount):
                    result.CanSelectTargetCount = reader.GetInt32();
                    break;
                case nameof(Skill.CanSelectTargetRange):
                    result.CanSelectTargetRange = reader.GetInt32();
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
                case nameof(Skill.FreeCostMP):
                    result.FreeCostMP = reader.GetBoolean();
                    break;
                case nameof(Skill.EPCost):
                    result.EPCost = reader.GetDouble();
                    break;
                case nameof(Skill.CostAllEP):
                    result.CostAllEP = reader.GetBoolean();
                    break;
                case nameof(Skill.MinCostEP):
                    result.MinCostEP = reader.GetDouble();
                    break;
                case nameof(Skill.FreeCostEP):
                    result.FreeCostEP = reader.GetBoolean();
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
                case nameof(Skill.InstantReset):
                    result.InstantReset = reader.GetBoolean();
                    break;
                case nameof(Skill.HardnessTime):
                    result.HardnessTime = reader.GetDouble();
                    break;
                case nameof(Skill.ExHardnessTime):
                    result.ExHardnessTime = reader.GetDouble();
                    break;
                case nameof(Skill.ExHardnessTime2):
                    result.ExHardnessTime2 = reader.GetDouble();
                    break;
                case nameof(Skill.MagicBottleneck):
                    result.MagicBottleneck = reader.GetDouble();
                    break;
                case nameof(Skill.Effects):
                    HashSet<Effect> effects = NetworkUtility.JsonDeserialize<HashSet<Effect>>(ref reader, options) ?? [];
                    foreach (Effect effect in effects)
                    {
                        result.Effects.Add(effect);
                    }
                    break;
                case nameof(Skill.Values):
                    Dictionary<string, object> values = NetworkUtility.JsonDeserialize<Dictionary<string, object>>(ref reader, options) ?? [];
                    foreach (string key in values.Keys)
                    {
                        result.Values.Add(key, values[key]);
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Skill value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Skill.Id), (int)value.Id);
            writer.WriteString(nameof(Skill.Name), value.Name);
            if (value.Guid != Guid.Empty)
            {
                writer.WritePropertyName(nameof(Skill.Guid));
                JsonSerializer.Serialize(writer, value.Guid, options);
            }
            writer.WriteNumber(nameof(Skill.SkillType), (int)value.SkillType);
            writer.WriteString(nameof(Skill.Description), value.Description);
            if (value.GeneralDescription.Length > 0) writer.WriteString(nameof(Skill.GeneralDescription), value.GeneralDescription);
            if (value.Slogan.Length > 0) writer.WriteString(nameof(Skill.Slogan), value.Slogan);
            if (value.Level > 0) writer.WriteNumber(nameof(Skill.Level), value.Level);
            if (value.ExLevel > 0) writer.WriteNumber(nameof(Skill.ExLevel), value.ExLevel);
            writer.WriteBoolean(nameof(Skill.CastAnywhere), value.CastAnywhere);
            writer.WriteNumber(nameof(Skill.CastRange), value.CastRange);
            if (value.CanSelectSelf) writer.WriteBoolean(nameof(Skill.CanSelectSelf), value.CanSelectSelf);
            if (!value.CanSelectEnemy) writer.WriteBoolean(nameof(Skill.CanSelectEnemy), value.CanSelectEnemy);
            if (value.CanSelectTeammate) writer.WriteBoolean(nameof(Skill.CanSelectTeammate), value.CanSelectTeammate);
            if (value.CanSelectTargetCount != 0) writer.WriteNumber(nameof(Skill.CanSelectTargetCount), value.CanSelectTargetCount);
            if (value.CanSelectTargetRange != 0) writer.WriteNumber(nameof(Skill.CanSelectTargetRange), value.CanSelectTargetRange);
            if (!value.Enable) writer.WriteBoolean(nameof(Skill.Enable), value.Enable);
            if (value.IsInEffect) writer.WriteBoolean(nameof(Skill.IsInEffect), value.IsInEffect);
            if (value.MPCost > 0) writer.WriteNumber(nameof(Skill.MPCost), value.MPCost);
            if (!value.FreeCostMP) writer.WriteBoolean(nameof(Skill.FreeCostMP), value.FreeCostMP);
            if (value.EPCost > 0) writer.WriteNumber(nameof(Skill.EPCost), value.EPCost);
            if (value.CostAllEP) writer.WriteBoolean(nameof(Skill.CostAllEP), value.CostAllEP);
            if (value.MinCostEP > 0) writer.WriteNumber(nameof(Skill.MinCostEP), value.MinCostEP);
            if (!value.FreeCostEP) writer.WriteBoolean(nameof(Skill.FreeCostEP), value.FreeCostEP);
            if (value.CastTime > 0) writer.WriteNumber(nameof(Skill.CastTime), value.CastTime);
            if (value.CD > 0) writer.WriteNumber(nameof(Skill.CD), value.CD);
            if (value.CurrentCD > 0) writer.WriteNumber(nameof(Skill.CurrentCD), value.CurrentCD);
            if (!value.InstantReset) writer.WriteBoolean(nameof(Skill.InstantReset), value.InstantReset);
            if (value.HardnessTime > 0) writer.WriteNumber(nameof(Skill.HardnessTime), value.HardnessTime);
            if (value.ExHardnessTime != 0) writer.WriteNumber(nameof(Skill.ExHardnessTime), value.ExHardnessTime);
            if (value.ExHardnessTime2 != 0) writer.WriteNumber(nameof(Skill.ExHardnessTime2), value.ExHardnessTime2);
            if (value.MagicBottleneck != 0) writer.WriteNumber(nameof(Skill.MagicBottleneck), value.MagicBottleneck);
            writer.WritePropertyName(nameof(Skill.Effects));
            JsonSerializer.Serialize(writer, value.Effects, options);
            writer.WritePropertyName(nameof(Skill.Values));
            JsonSerializer.Serialize(writer, value.Values, options);

            writer.WriteEndObject();
        }
    }
}
