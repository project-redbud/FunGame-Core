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
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString() ?? "";
                    switch (propertyName)
                    {
                        case "Id":
                            user.Id = reader.GetInt64();
                            break;
                        case "Username":
                            user.Username = reader.GetString() ?? "";
                            break;
                        case "Password":
                            user.Password = reader.GetString() ?? "";
                            break;
                        case "RegTime":
                            string regTime = reader.GetString() ?? "";
                            if (DateTime.TryParseExact(regTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime RegTime))
                            {
                                user.RegTime = RegTime;
                            }
                            else user.RegTime = DateTime.MinValue;
                            break;
                        case "LastTime":
                            string lastTime = reader.GetString() ?? "";
                            if (DateTime.TryParseExact(lastTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime LastTime))
                            {
                                user.LastTime = LastTime;
                            }
                            else user.LastTime = DateTime.MinValue;
                            break;
                        case "Email":
                            user.Email = reader.GetString() ?? "";
                            break;
                        case "NickName":
                            user.NickName = reader.GetString() ?? "";
                            break;
                        case "IsAdmin":
                            user.IsAdmin = reader.GetBoolean();
                            break;
                        case "IsOperator":
                            user.IsOperator = reader.GetBoolean();
                            break;
                        case "IsEnable":
                            user.IsEnable = reader.GetBoolean();
                            break;
                        case "Credits":
                            user.Credits = reader.GetDecimal();
                            break;
                        case "Materials":
                            user.Materials = reader.GetDecimal();
                            break;
                        case "GameTime":
                            user.GameTime = reader.GetDecimal();
                            break;
                        case "AutoKey":
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
            writer.WriteString(UserQuery.Column_Password, value.Password);
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
