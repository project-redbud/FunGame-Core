using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class UserConverter : BaseEntityConverter<User>
    {
        public override User NewInstance()
        {
            return Factory.GetUser();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref User result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case UserQuery.Column_Id:
                    result.Id = reader.GetInt64();
                    break;
                case UserQuery.Column_Username:
                    result.Username = reader.GetString() ?? "";
                    break;
                case UserQuery.Column_RegTime:
                    string regTime = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(regTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime RegTime))
                    {
                        result.RegTime = RegTime;
                    }
                    else result.RegTime = General.DefaultTime;
                    break;
                case UserQuery.Column_LastTime:
                    string lastTime = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(lastTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime LastTime))
                    {
                        result.LastTime = LastTime;
                    }
                    else result.LastTime = General.DefaultTime;
                    break;
                case UserQuery.Column_Email:
                    result.Email = reader.GetString() ?? "";
                    break;
                case UserQuery.Column_Nickname:
                    result.NickName = reader.GetString() ?? "";
                    break;
                case UserQuery.Column_IsAdmin:
                    result.IsAdmin = reader.GetBoolean();
                    break;
                case UserQuery.Column_IsOperator:
                    result.IsOperator = reader.GetBoolean();
                    break;
                case UserQuery.Column_IsEnable:
                    result.IsEnable = reader.GetBoolean();
                    break;
                case UserQuery.Column_GameTime:
                    result.GameTime = reader.GetDouble();
                    break;
                case UserQuery.Column_AutoKey:
                    result.AutoKey = reader.GetString() ?? "";
                    break;
                case nameof(Inventory):
                    Inventory inventory = NetworkUtility.JsonDeserialize<Inventory>(ref reader, options) ?? Factory.GetInventory();
                    result.Inventory.Name = inventory.Name;
                    result.Inventory.Credits = inventory.Credits;
                    result.Inventory.Materials = inventory.Materials;
                    foreach (Character character in inventory.Characters)
                    {
                        result.Inventory.Characters.Add(character);
                    }
                    foreach (Item item in inventory.Items)
                    {
                        result.Inventory.Items.Add(item);
                    }
                    result.Inventory.MainCharacter = inventory.MainCharacter;
                    foreach (long cid in inventory.Squad)
                    {
                        result.Inventory.Squad.Add(cid);
                    }
                    foreach (long cid in inventory.Training.Keys)
                    {
                        result.Inventory.Training[cid] = inventory.Training[cid];
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(UserQuery.Column_Id, value.Id);
            writer.WriteString(UserQuery.Column_Username, value.Username);
            writer.WriteString(UserQuery.Column_RegTime, value.RegTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString(UserQuery.Column_LastTime, value.LastTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString(UserQuery.Column_Email, value.Email);
            writer.WriteString(UserQuery.Column_Nickname, value.NickName);
            writer.WriteBoolean(UserQuery.Column_IsAdmin, value.IsAdmin);
            writer.WriteBoolean(UserQuery.Column_IsOperator, value.IsOperator);
            writer.WriteBoolean(UserQuery.Column_IsEnable, value.IsEnable);
            writer.WriteNumber(UserQuery.Column_GameTime, value.GameTime);
            writer.WriteString(UserQuery.Column_AutoKey, value.AutoKey);
            writer.WritePropertyName(nameof(Inventory));
            JsonSerializer.Serialize(writer, value.Inventory, options);

            writer.WriteEndObject();
        }
    }
}
