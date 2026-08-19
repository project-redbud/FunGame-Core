using System.Text.Json;
using FunGame.Core.Api;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class CharacterStateSnapshotConverter : BaseEntityConverter<CharacterStateSnapshot>
    {
        public override CharacterStateSnapshot NewInstance()
        {
            return new CharacterStateSnapshot();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref CharacterStateSnapshot result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(CharacterStateSnapshot.Character):
                    result.Character = CharacterRefHelper.Read(ref reader);
                    break;
                case nameof(CharacterStateSnapshot.HP):
                    result.HP = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.MaxHP):
                    result.MaxHP = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.MP):
                    result.MP = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.MaxMP):
                    result.MaxMP = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.EP):
                    result.EP = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.Attributes):
                    result.Attributes = JsonService.GetObject<Dictionary<string, string>>(ref reader, options) ?? [];
                    break;
                case nameof(CharacterStateSnapshot.HR):
                    result.HR = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.MR):
                    result.MR = reader.GetDouble();
                    break;
                case nameof(CharacterStateSnapshot.Equipments):
                    Dictionary<int, long> equips = JsonService.GetObject<Dictionary<int, long>>(ref reader, options) ?? [];
                    foreach (KeyValuePair<int, long> kvp in equips)
                    {
                        result.Equipments[(EquipSlotType)kvp.Key] = kvp.Value;
                    }
                    break;
                case nameof(CharacterStateSnapshot.EquipmentsDetail):
                    result.EquipmentsDetail.AddRange(JsonService.GetObject<List<EquipmentStateSnapshot>>(ref reader, options) ?? []);
                    break;
                case nameof(CharacterStateSnapshot.Skills):
                    result.Skills.AddRange(JsonService.GetObject<List<SkillStateSnapshot>>(ref reader, options) ?? []);
                    break;
                case nameof(CharacterStateSnapshot.Items):
                    result.Items.AddRange(JsonService.GetObject<List<ItemStateSnapshot>>(ref reader, options) ?? []);
                    break;
                case nameof(CharacterStateSnapshot.Effects):
                    result.Effects.AddRange(JsonService.GetObject<List<EffectStateSnapshot>>(ref reader, options) ?? []);
                    break;

                default:
                    reader.Skip();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, CharacterStateSnapshot value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(CharacterStateSnapshot.Character));
            CharacterRefHelper.Write(writer, value.Character);
            writer.WriteNumber(nameof(CharacterStateSnapshot.HP), value.HP);
            writer.WriteNumber(nameof(CharacterStateSnapshot.MaxHP), value.MaxHP);
            writer.WriteNumber(nameof(CharacterStateSnapshot.MP), value.MP);
            writer.WriteNumber(nameof(CharacterStateSnapshot.MaxMP), value.MaxMP);
            writer.WriteNumber(nameof(CharacterStateSnapshot.EP), value.EP);
            writer.WriteNumber(nameof(CharacterStateSnapshot.HR), value.HR);
            writer.WriteNumber(nameof(CharacterStateSnapshot.MR), value.MR);
            writer.WritePropertyName(nameof(CharacterStateSnapshot.Attributes));
            JsonSerializer.Serialize(writer, value.Attributes, options);
            writer.WritePropertyName(nameof(CharacterStateSnapshot.Equipments));
            JsonSerializer.Serialize(writer, value.Equipments.ToDictionary(kv => (int)kv.Key, kv => kv.Value), options);
            writer.WritePropertyName(nameof(CharacterStateSnapshot.EquipmentsDetail));
            JsonSerializer.Serialize(writer, value.EquipmentsDetail, options);
            writer.WritePropertyName(nameof(CharacterStateSnapshot.Skills));
            JsonSerializer.Serialize(writer, value.Skills, options);
            writer.WritePropertyName(nameof(CharacterStateSnapshot.Items));
            JsonSerializer.Serialize(writer, value.Items, options);
            writer.WritePropertyName(nameof(CharacterStateSnapshot.Effects));
            JsonSerializer.Serialize(writer, value.Effects, options);
            writer.WriteEndObject();
        }
    }
}
