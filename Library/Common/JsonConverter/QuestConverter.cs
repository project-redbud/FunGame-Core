using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class QuestConverter : BaseEntityConverter<Quest>
    {
        public override Quest NewInstance()
        {
            return new Quest();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Quest result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Quest.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Quest.Guid):
                    result.Guid = NetworkUtility.JsonDeserialize<Guid>(ref reader, options);
                    break;
                case nameof(Quest.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Quest.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Quest.Status):
                    result.Status = (QuestState)reader.GetInt32();
                    break;
                case nameof(Quest.CharacterId):
                    result.CharacterId = reader.GetInt64();
                    break;
                case nameof(Quest.RegionId):
                    result.RegionId = reader.GetInt64();
                    break;
                case nameof(Quest.CreditsAward):
                    result.CreditsAward = reader.GetDouble();
                    break;
                case nameof(Quest.MaterialsAward):
                    result.MaterialsAward = reader.GetDouble();
                    break;
                case nameof(Quest.Awards):
                    List<Item> awards = NetworkUtility.JsonDeserialize<List<Item>>(ref reader, options) ?? [];
                    foreach (Item item in awards)
                    {
                        result.Awards.Add(item);
                    }
                    break;
                case nameof(Quest.AwardsCount):
                    Dictionary<string, int> dict = NetworkUtility.JsonDeserialize<Dictionary<string, int>>(ref reader, options) ?? [];
                    foreach (string key in dict.Keys)
                    {
                        result.AwardsCount[key] = dict[key];
                    }
                    break;
                case nameof(Quest.StartTime):
                    string startTimeStr = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(startTimeStr, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startTime))
                    {
                        result.StartTime = startTime;
                    }
                    break;
                case nameof(Quest.SettleTime):
                    string settleTimeStr = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(settleTimeStr, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime settleTime))
                    {
                        result.SettleTime = settleTime;
                    }
                    break;
                case nameof(Quest.QuestType):
                    result.QuestType = (QuestType)reader.GetInt32();
                    break;
                case nameof(Quest.EstimatedMinutes):
                    result.EstimatedMinutes = reader.GetInt32();
                    break;
                case nameof(Quest.Progress):
                    result.Progress = reader.GetInt32();
                    break;
                case nameof(Quest.MaxProgress):
                    result.MaxProgress = reader.GetInt32();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Quest value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Quest.Id), value.Id);
            if (value.Guid != Guid.Empty)
            {
                writer.WritePropertyName(nameof(Quest.Guid));
                JsonSerializer.Serialize(writer, value.Guid, options);
            }
            writer.WriteString(nameof(Quest.Name), value.Name);
            writer.WriteString(nameof(Quest.Description), value.Description);
            writer.WriteNumber(nameof(Quest.Status), (int)value.Status);
            writer.WriteNumber(nameof(Quest.CharacterId), value.CharacterId);
            writer.WriteNumber(nameof(Quest.RegionId), value.RegionId);
            writer.WriteNumber(nameof(Quest.CreditsAward), value.CreditsAward);
            writer.WriteNumber(nameof(Quest.MaterialsAward), value.MaterialsAward);
            writer.WritePropertyName(nameof(Quest.Awards));
            JsonSerializer.Serialize(writer, value.Awards, options);
            writer.WritePropertyName(nameof(Quest.AwardsCount));
            JsonSerializer.Serialize(writer, value.AwardsCount, options);
            writer.WriteString(nameof(Quest.AwardsString), value.AwardsString);
            if (value.StartTime != null) writer.WriteString(nameof(Quest.StartTime), value.StartTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.SettleTime != null) writer.WriteString(nameof(Quest.SettleTime), value.SettleTime.Value.ToString(General.GeneralDateTimeFormat));
            writer.WriteNumber(nameof(Quest.QuestType), (int)value.QuestType);
            writer.WriteNumber(nameof(Quest.EstimatedMinutes), value.EstimatedMinutes);
            writer.WriteNumber(nameof(Quest.Progress), value.Progress);
            writer.WriteNumber(nameof(Quest.MaxProgress), value.MaxProgress);

            writer.WriteEndObject();
        }
    }
}
