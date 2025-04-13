using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class RoomConverter : BaseEntityConverter<Room>
    {
        public override Room NewInstance()
        {
            return Factory.GetRoom();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Room result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case RoomQuery.Column_ID:
                    result.Id = reader.GetInt64();
                    break;
                case RoomQuery.Column_Roomid:
                    result.Roomid = reader.GetString() ?? "";
                    break;
                case RoomQuery.Column_CreateTime:
                    string dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        result.CreateTime = date;
                    }
                    else result.CreateTime = General.DefaultTime;
                    break;
                case RoomQuery.Column_RoomMaster:
                    result.RoomMaster = JsonSerializer.Deserialize<User>(ref reader, options) ?? General.UnknownUserInstance;
                    break;
                case RoomQuery.Column_RoomType:
                    result.RoomType = (RoomType)reader.GetInt64();
                    break;
                case RoomQuery.Column_GameModule:
                    result.GameModule = reader.GetString() ?? "";
                    break;
                case RoomQuery.Column_GameMap:
                    result.GameMap = reader.GetString() ?? "";
                    break;
                case RoomQuery.Column_RoomState:
                    result.RoomState = (RoomState)reader.GetInt64();
                    break;
                case RoomQuery.Column_IsRank:
                    result.IsRank = reader.GetBoolean();
                    break;
                case RoomQuery.Column_Password:
                    result.Password = reader.GetString() ?? "";
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Room value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(RoomQuery.Column_ID, value.Id);
            writer.WriteString(RoomQuery.Column_Roomid, value.Roomid);
            writer.WriteString(RoomQuery.Column_CreateTime, value.CreateTime.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName(RoomQuery.Column_RoomMaster);
            JsonSerializer.Serialize(writer, value.RoomMaster, options);
            writer.WriteString(RoomQuery.Column_GameModule, value.GameModule);
            writer.WriteString(RoomQuery.Column_GameMap, value.GameMap);
            writer.WriteNumber(RoomQuery.Column_RoomType, (long)value.RoomType);
            writer.WriteNumber(RoomQuery.Column_RoomState, (long)value.RoomState);
            writer.WriteBoolean(RoomQuery.Column_IsRank, value.IsRank);
            writer.WriteString(RoomQuery.Column_Password, value.Password);
            writer.WriteEndObject();
        }
    }
}
