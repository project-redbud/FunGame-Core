using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class ActivityConverter : BaseEntityConverter<Activity>
    {
        public override Activity NewInstance()
        {
            return new Activity();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Activity result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Activity.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Activity.Guid):
                    result.Guid = NetworkUtility.JsonDeserialize<Guid>(ref reader, options);
                    break;
                case nameof(Activity.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Activity.Description):
                    result.Description = reader.GetString() ?? "";
                    break;
                case nameof(Activity.StartTime):
                    string startTimeStr = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(startTimeStr, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime startTime))
                    {
                        result.StartTime = startTime;
                    }
                    result.UpdateState();
                    break;
                case nameof(Activity.EndTime):
                    string endTimeStr = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(endTimeStr, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime endTime))
                    {
                        result.EndTime = endTime;
                    }
                    result.UpdateState();
                    break;
                case nameof(Activity.Quests):
                    List<Quest> quests = NetworkUtility.JsonDeserialize<List<Quest>>(ref reader, options) ?? [];
                    foreach (Quest quest in quests)
                    {
                        result.Quests.Add(quest);
                    }
                    break;
                case nameof(Activity.Predecessor):
                    result.Predecessor = reader.GetInt64();
                    result.UpdateState();
                    break;
                case nameof(Activity.PredecessorStatus):
                    result.PredecessorStatus = (ActivityState)reader.GetInt32();
                    result.UpdateState();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Activity value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Activity.Id), value.Id);
            if (value.Guid != Guid.Empty)
            {
                writer.WritePropertyName(nameof(Activity.Guid));
                JsonSerializer.Serialize(writer, value.Guid, options);
            }
            writer.WriteString(nameof(Activity.Name), value.Name);
            writer.WriteString(nameof(Activity.Description), value.Description);
            if (value.StartTime != null) writer.WriteString(nameof(Activity.StartTime), value.StartTime.Value.ToString(General.GeneralDateTimeFormat));
            if (value.EndTime != null) writer.WriteString(nameof(Activity.EndTime), value.EndTime.Value.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName(nameof(Activity.Quests));
            JsonSerializer.Serialize(writer, value.Quests, options);
            writer.WriteNumber(nameof(Activity.Predecessor), value.Predecessor);
            writer.WriteNumber(nameof(Activity.PredecessorStatus), (int)value.PredecessorStatus);

            writer.WriteEndObject();
        }
    }
}
