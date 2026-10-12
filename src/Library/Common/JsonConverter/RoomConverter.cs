using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class RoomConverter : BaseEntityConverter<Room>
    {
        public override Room NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Room result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case "Id":
                    result.Id = reader.GetInt64();
                    break;
                case "Roomid":
                    result.Roomid = reader.GetString() ?? "";
                    break;
                case "CreateTime":
                    string dateString = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        result.CreateTime = date;
                    }
                    else result.CreateTime = General.DefaultTime;
                    break;
                case "RoomMaster":
                    result.RoomMaster = JsonSerializer.Deserialize<User>(ref reader, options) ?? General.UnknownUserInstance;
                    break;
                case "RoomType":
                    result.RoomType = (RoomType)reader.GetInt64();
                    break;
                case "GameModule":
                    result.GameModule = reader.GetString() ?? "";
                    break;
                case "GameMap":
                    result.GameMap = reader.GetString() ?? "";
                    break;
                case "RoomState":
                    result.RoomState = (RoomState)reader.GetInt64();
                    break;
                case "IsRank":
                    result.IsRank = reader.GetBoolean();
                    break;
                case "Password":
                    result.Password = reader.GetString() ?? "";
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Room value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Id", value.Id);
            writer.WriteString("Roomid", value.Roomid);
            writer.WriteString("CreateTime", value.CreateTime.ToString(General.GeneralDateTimeFormat));
            writer.WritePropertyName("RoomMaster");
            JsonSerializer.Serialize(writer, value.RoomMaster, options);
            writer.WriteString("GameModule", value.GameModule);
            writer.WriteString("GameMap", value.GameMap);
            writer.WriteNumber("RoomType", (long)value.RoomType);
            writer.WriteNumber("RoomState", (long)value.RoomState);
            writer.WriteBoolean("IsRank", value.IsRank);
            writer.WriteString("Password", value.Password);
            writer.WriteEndObject();
        }
    }
}
