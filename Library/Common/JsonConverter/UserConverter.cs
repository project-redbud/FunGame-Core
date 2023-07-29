using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class UserConverter : JsonConverter<User>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(User);
        }

        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            User user = Factory.GetUser();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString() ?? "";
                    reader.Read();
                    switch (propertyName)
                    {
                        case UserQuery.Column_UID:
                            user.Id = reader.GetInt64();
                            break;
                        case UserQuery.Column_Username:
                            user.Username = reader.GetString() ?? "";
                            break;
                        case UserQuery.Column_RegTime:
                            string regTime = reader.GetString() ?? "";
                            if (DateTime.TryParseExact(regTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime RegTime))
                            {
                                user.RegTime = RegTime;
                            }
                            else user.RegTime = General.DefaultTime;
                            break;
                        case UserQuery.Column_LastTime:
                            string lastTime = reader.GetString() ?? "";
                            if (DateTime.TryParseExact(lastTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime LastTime))
                            {
                                user.LastTime = LastTime;
                            }
                            else user.LastTime = General.DefaultTime;
                            break;
                        case UserQuery.Column_Email:
                            user.Email = reader.GetString() ?? "";
                            break;
                        case UserQuery.Column_Nickname:
                            user.NickName = reader.GetString() ?? "";
                            break;
                        case UserQuery.Column_IsAdmin:
                            user.IsAdmin = reader.GetBoolean();
                            break;
                        case UserQuery.Column_IsOperator:
                            user.IsOperator = reader.GetBoolean();
                            break;
                        case UserQuery.Column_IsEnable:
                            user.IsEnable = reader.GetBoolean();
                            break;
                        case UserQuery.Column_Credits:
                            user.Credits = reader.GetDecimal();
                            break;
                        case UserQuery.Column_Materials:
                            user.Materials = reader.GetDecimal();
                            break;
                        case UserQuery.Column_GameTime:
                            user.GameTime = reader.GetDecimal();
                            break;
                        case UserQuery.Column_AutoKey:
                            user.AutoKey = reader.GetString() ?? "";
                            break;
                    }
                }
            }

            return user;
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(UserQuery.Column_UID, value.Id);
            writer.WriteString(UserQuery.Column_Username, value.Username);
            writer.WriteString(UserQuery.Column_RegTime, value.RegTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString(UserQuery.Column_LastTime, value.LastTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString(UserQuery.Column_Email, value.Email);
            writer.WriteString(UserQuery.Column_Nickname, value.NickName);
            writer.WriteBoolean(UserQuery.Column_IsAdmin, value.IsAdmin);
            writer.WriteBoolean(UserQuery.Column_IsOperator, value.IsOperator);
            writer.WriteBoolean(UserQuery.Column_IsEnable, value.IsEnable);
            writer.WriteNumber(UserQuery.Column_Credits, value.Credits);
            writer.WriteNumber(UserQuery.Column_Materials, value.Materials);
            writer.WriteNumber(UserQuery.Column_GameTime, value.GameTime);
            writer.WriteString(UserQuery.Column_AutoKey, value.AutoKey);
            writer.WriteEndObject();
        }
    }
}
