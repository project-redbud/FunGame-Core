using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class RoomConverter : JsonConverter<Room>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Room);
        }

        public override Room Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            Room room = Factory.GetRoom();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString() ?? "";
                    reader.Read();
                    switch (property)
                    {
                        case RoomQuery.Column_ID:
                            room.Id = reader.GetInt64();
                            break;

                        case RoomQuery.Column_RoomID:
                            room.Roomid = reader.GetString() ?? "";
                            break;

                        case RoomQuery.Column_CreateTime:
                            string dateString = reader.GetString() ?? "";
                            if (DateTime.TryParseExact(dateString, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime result))
                            {
                                room.CreateTime = result;
                            }
                            else room.CreateTime = General.DefaultTime;
                            break;

                        case RoomQuery.Column_RoomMaster:
                            string master = reader.GetString() ?? "";
                            room.RoomMaster = JsonSerializer.Deserialize<User>(master, options) ?? General.UnknownUserInstance;
                            break;

                        case RoomQuery.Column_RoomType:
                            room.RoomType = (RoomType)reader.GetInt64();
                            break;
                            
                        case RoomQuery.Column_GameMode:
                            room.GameMode = reader.GetString() ?? "";
                            break;
                            
                        case RoomQuery.Column_GameMap:
                            room.GameMap = reader.GetString() ?? "";
                            break;

                        case RoomQuery.Column_RoomState:
                            room.RoomState = (RoomState)reader.GetInt64();
                            break;
                            
                        case RoomQuery.Column_IsRank:
                            room.IsRank = reader.GetBoolean();
                            break;

                        case RoomQuery.Column_Password:
                            room.Password = reader.GetString() ?? "";
                            break;
                    }

                }
            }

            return room;
        }

        public override void Write(Utf8JsonWriter writer, Room value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(RoomQuery.Column_ID, value.Id);
            writer.WriteString(RoomQuery.Column_RoomID, value.Roomid);
            writer.WriteString(RoomQuery.Column_CreateTime, value.CreateTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString(RoomQuery.Column_RoomMaster, JsonSerializer.Serialize(value.RoomMaster, typeof(User), options));
            writer.WriteString(RoomQuery.Column_GameMode, value.GameMode);
            writer.WriteString(RoomQuery.Column_GameMap, value.GameMap);
            writer.WriteNumber(RoomQuery.Column_RoomType, (long)value.RoomType);
            writer.WriteNumber(RoomQuery.Column_RoomState, (long)value.RoomState);
            writer.WriteBoolean(RoomQuery.Column_IsRank, value.IsRank);
            writer.WriteString(RoomQuery.Column_Password, value.Password);
            writer.WriteEndObject();
        }
    }
}
