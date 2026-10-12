using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class UserConverter : BaseEntityConverter<User>
    {
        public override User NewInstance()
        {
            return new();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref User result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case "Id":
                    result.Id = reader.GetInt64();
                    break;
                case "Username":
                    result.Username = reader.GetString() ?? "";
                    break;
                case "RegTime":
                    string regTime = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(regTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime RegTime))
                    {
                        result.RegTime = RegTime;
                    }
                    else result.RegTime = General.DefaultTime;
                    break;
                case "LastTime":
                    string lastTime = reader.GetString() ?? "";
                    if (DateTime.TryParseExact(lastTime, General.GeneralDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime LastTime))
                    {
                        result.LastTime = LastTime;
                    }
                    else result.LastTime = General.DefaultTime;
                    break;
                case "Email":
                    result.Email = reader.GetString() ?? "";
                    break;
                case "Nickname":
                    result.NickName = reader.GetString() ?? "";
                    break;
                case "IsAdmin":
                    result.IsAdmin = reader.GetBoolean();
                    break;
                case "IsOperator":
                    result.IsOperator = reader.GetBoolean();
                    break;
                case "IsEnable":
                    result.IsEnable = reader.GetBoolean();
                    break;
                case "GameTime":
                    result.GameTime = reader.GetDouble();
                    break;
                case "AutoKey":
                    result.AutoKey = reader.GetString() ?? "";
                    break;
                case nameof(Inventory):
                    Inventory inventory = JsonService.GetObject<Inventory>(ref reader, options) ?? new(result);
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

            writer.WriteNumber("Id", value.Id);
            writer.WriteString("Username", value.Username);
            writer.WriteString("RegTime", value.RegTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString("LastTime", value.LastTime.ToString(General.GeneralDateTimeFormat));
            writer.WriteString("Email", value.Email);
            writer.WriteString("Nickname", value.NickName);
            writer.WriteBoolean("IsAdmin", value.IsAdmin);
            writer.WriteBoolean("IsOperator", value.IsOperator);
            writer.WriteBoolean("IsEnable", value.IsEnable);
            writer.WriteNumber("GameTime", value.GameTime);
            writer.WriteString("AutoKey", value.AutoKey);
            writer.WritePropertyName(nameof(Inventory));
            JsonSerializer.Serialize(writer, value.Inventory, options);

            writer.WriteEndObject();
        }
    }
}
