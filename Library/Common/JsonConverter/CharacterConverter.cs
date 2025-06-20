using System.Text.Json;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.JsonConverter
{
    public class CharacterConverter : BaseEntityConverter<Character>
    {
        public override Character NewInstance()
        {
            return Factory.GetCharacter();
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Character result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(Character.Id):
                    result.Id = reader.GetInt64();
                    break;
                case nameof(Character.Guid):
                    result.Guid = reader.GetGuid();
                    break;
                case nameof(Character.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Character.FirstName):
                    result.FirstName = reader.GetString() ?? "";
                    break;
                case nameof(Character.NickName):
                    result.NickName = reader.GetString() ?? "";
                    break;
                case nameof(Character.Profile):
                    result.Profile = NetworkUtility.JsonDeserialize<CharacterProfile>(ref reader, options) ?? new(result.Name, result.FirstName, result.NickName);
                    break;
                case nameof(Character.EquipSlot):
                    result.EquipSlot = NetworkUtility.JsonDeserialize<EquipSlot>(ref reader, options) ?? new();
                    break;
                case nameof(Character.MagicType):
                    result.MagicType = (MagicType)reader.GetInt32();
                    break;
                case nameof(Character.FirstRoleType):
                    result.FirstRoleType = (RoleType)reader.GetInt32();
                    break;
                case nameof(Character.SecondRoleType):
                    result.SecondRoleType = (RoleType)reader.GetInt32();
                    break;
                case nameof(Character.ThirdRoleType):
                    result.ThirdRoleType = (RoleType)reader.GetInt32();
                    break;
                case nameof(Character.Promotion):
                    result.Promotion = reader.GetInt32();
                    break;
                case nameof(Character.PrimaryAttribute):
                    result.PrimaryAttribute = (PrimaryAttribute)reader.GetInt32();
                    break;
                case nameof(Character.Level):
                    result.Level = reader.GetInt32();
                    break;
                case nameof(Character.LevelBreak):
                    result.LevelBreak = reader.GetInt32();
                    break;
                case nameof(Character.EXP):
                    result.EXP = reader.GetDouble();
                    break;
                case nameof(Character.IsNeutral):
                    result.IsNeutral = reader.GetBoolean();
                    break;
                case nameof(Character.IsUnselectable):
                    result.IsUnselectable = reader.GetBoolean();
                    break;
                case nameof(Character.ImmuneType):
                    result.ImmuneType = (ImmuneType)reader.GetInt32();
                    break;
                case nameof(Character.InitialHP):
                    result.InitialHP = reader.GetDouble();
                    break;
                case nameof(Character.ExHP2):
                    result.ExHP2 = reader.GetDouble();
                    break;
                case nameof(Character.ExHPPercentage):
                    result.ExHPPercentage = reader.GetDouble();
                    break;
                case nameof(Character.InitialMP):
                    result.InitialMP = reader.GetDouble();
                    break;
                case nameof(Character.ExMP2):
                    result.ExMP2 = reader.GetDouble();
                    break;
                case nameof(Character.ExMPPercentage):
                    result.ExMPPercentage = reader.GetDouble();
                    break;
                case nameof(Character.HP):
                    convertingContext[nameof(Character.HP)] = reader.GetDouble();
                    break;
                case nameof(Character.MP):
                    convertingContext[nameof(Character.MP)] = reader.GetDouble();
                    break;
                case nameof(Character.EP):
                    convertingContext[nameof(Character.EP)] = reader.GetDouble();
                    break;
                case nameof(Character.InitialATK):
                    result.InitialATK = reader.GetDouble();
                    break;
                case nameof(Character.ExATK2):
                    result.ExATK2 = reader.GetDouble();
                    break;
                case nameof(Character.ExATKPercentage):
                    result.ExATKPercentage = reader.GetDouble();
                    break;
                case nameof(Character.InitialDEF):
                    result.InitialDEF = reader.GetDouble();
                    break;
                case nameof(Character.ExDEF2):
                    result.ExDEF2 = reader.GetDouble();
                    break;
                case nameof(Character.ExDEFPercentage):
                    result.ExDEFPercentage = reader.GetDouble();
                    break;
                case nameof(Character.MDF):
                    result.MDF = NetworkUtility.JsonDeserialize<MagicResistance>(ref reader, options) ?? new();
                    break;
                case nameof(Character.PhysicalPenetration):
                    result.PhysicalPenetration = reader.GetDouble();
                    break;
                case nameof(Character.MagicalPenetration):
                    result.MagicalPenetration = reader.GetDouble();
                    break;
                case nameof(Character.CharacterState):
                    result.CharacterState = (CharacterState)reader.GetInt32();
                    break;
                case nameof(Character.InitialHR):
                    result.InitialHR = reader.GetDouble();
                    break;
                case nameof(Character.ExHR):
                    result.ExHR = reader.GetDouble();
                    break;
                case nameof(Character.InitialMR):
                    result.InitialMR = reader.GetDouble();
                    break;
                case nameof(Character.ExMR):
                    result.ExMR = reader.GetDouble();
                    break;
                case nameof(Character.ER):
                    result.ER = reader.GetDouble();
                    break;
                case nameof(Character.InitialSTR):
                    result.InitialSTR = reader.GetDouble();
                    break;
                case nameof(Character.InitialAGI):
                    result.InitialAGI = reader.GetDouble();
                    break;
                case nameof(Character.InitialINT):
                    result.InitialINT = reader.GetDouble();
                    break;
                case nameof(Character.ExSTR):
                    result.ExSTR = reader.GetDouble();
                    break;
                case nameof(Character.ExAGI):
                    result.ExAGI = reader.GetDouble();
                    break;
                case nameof(Character.ExINT):
                    result.ExINT = reader.GetDouble();
                    break;
                case nameof(Character.ExSTRPercentage):
                    result.ExSTRPercentage = reader.GetDouble();
                    break;
                case nameof(Character.ExAGIPercentage):
                    result.ExAGIPercentage = reader.GetDouble();
                    break;
                case nameof(Character.ExINTPercentage):
                    result.ExINTPercentage = reader.GetDouble();
                    break;
                case nameof(Character.STRGrowth):
                    result.STRGrowth = reader.GetDouble();
                    break;
                case nameof(Character.AGIGrowth):
                    result.AGIGrowth = reader.GetDouble();
                    break;
                case nameof(Character.INTGrowth):
                    result.INTGrowth = reader.GetDouble();
                    break;
                case nameof(Character.InitialSPD):
                    result.InitialSPD = reader.GetDouble();
                    break;
                case nameof(Character.ExSPD):
                    result.ExSPD = reader.GetDouble();
                    break;
                case nameof(Character.ExActionCoefficient):
                    result.ExActionCoefficient = reader.GetDouble();
                    break;
                case nameof(Character.ExAccelerationCoefficient):
                    result.ExAccelerationCoefficient = reader.GetDouble();
                    break;
                case nameof(Character.ExCDR):
                    result.ExCDR = reader.GetDouble();
                    break;
                case nameof(Character.ATR):
                    result.ATR = reader.GetDouble();
                    break;
                case nameof(Character.ExCritRate):
                    result.ExCritRate = reader.GetDouble();
                    break;
                case nameof(Character.ExCritDMG):
                    result.ExCritDMG = reader.GetDouble();
                    break;
                case nameof(Character.ExEvadeRate):
                    result.ExEvadeRate = reader.GetDouble();
                    break;
                case nameof(Character.Lifesteal):
                    result.Lifesteal = reader.GetDouble();
                    break;
                case nameof(Character.Shield):
                    result.Shield = NetworkUtility.JsonDeserialize<Shield>(ref reader, options) ?? new();
                    break;
                case nameof(Character.NormalAttack):
                    NormalAttack normalAttack = NetworkUtility.JsonDeserialize<NormalAttack>(ref reader, options) ?? new NormalAttack(result);
                    result.NormalAttack.Level = normalAttack.Level;
                    result.NormalAttack.ExDamage = normalAttack.ExDamage;
                    result.NormalAttack.ExDamage2 = normalAttack.ExDamage2;
                    result.NormalAttack.ExHardnessTime = normalAttack.ExHardnessTime;
                    result.NormalAttack.ExHardnessTime2 = normalAttack.ExHardnessTime2;
                    result.NormalAttack.SetMagicType(normalAttack.IsMagic, normalAttack.MagicType);
                    break;
                case nameof(Character.Skills):
                    HashSet<Skill> skills = NetworkUtility.JsonDeserialize<HashSet<Skill>>(ref reader, options) ?? [];
                    foreach (Skill skill in skills)
                    {
                        result.Skills.Add(skill);
                    }
                    break;
                case nameof(Character.Items):
                    HashSet<Item> items = NetworkUtility.JsonDeserialize<HashSet<Item>>(ref reader, options) ?? [];
                    foreach (Item item in items)
                    {
                        result.Items.Add(item);
                    }
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Character value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(Character.Id), value.Id);
            writer.WritePropertyName(nameof(Character.Guid));
            JsonSerializer.Serialize(writer, value.Guid, options);
            writer.WriteString(nameof(Character.Name), value.Name);
            writer.WriteString(nameof(Character.FirstName), value.FirstName);
            writer.WriteString(nameof(Character.NickName), value.NickName);
            writer.WritePropertyName(nameof(Character.Profile));
            JsonSerializer.Serialize(writer, value.Profile, options);
            writer.WritePropertyName(nameof(Character.EquipSlot));
            JsonSerializer.Serialize(writer, value.EquipSlot, options);
            writer.WriteNumber(nameof(Character.MagicType), (int)value.MagicType);
            writer.WriteNumber(nameof(Character.FirstRoleType), (int)value.FirstRoleType);
            writer.WriteNumber(nameof(Character.SecondRoleType), (int)value.SecondRoleType);
            writer.WriteNumber(nameof(Character.ThirdRoleType), (int)value.ThirdRoleType);
            writer.WriteNumber(nameof(Character.Promotion), value.Promotion);
            writer.WriteNumber(nameof(Character.PrimaryAttribute), (int)value.PrimaryAttribute);
            writer.WriteNumber(nameof(Character.Level), value.Level);
            writer.WriteNumber(nameof(Character.LevelBreak), value.LevelBreak);
            writer.WriteNumber(nameof(Character.EXP), value.EXP);
            writer.WriteBoolean(nameof(Character.IsNeutral), value.IsNeutral);
            writer.WriteBoolean(nameof(Character.IsUnselectable), value.IsUnselectable);
            writer.WriteNumber(nameof(Character.ImmuneType), (int)value.ImmuneType);
            writer.WriteNumber(nameof(Character.CharacterState), (int)value.CharacterState);
            writer.WriteNumber(nameof(Character.InitialHP), value.InitialHP);
            writer.WriteNumber(nameof(Character.ExHP2), value.ExHP2);
            writer.WriteNumber(nameof(Character.ExHPPercentage), value.ExHPPercentage);
            writer.WriteNumber(nameof(Character.InitialMP), value.InitialMP);
            writer.WriteNumber(nameof(Character.ExMP2), value.ExMP2);
            writer.WriteNumber(nameof(Character.ExMPPercentage), value.ExMPPercentage);
            writer.WriteNumber(nameof(Character.HP), value.HP);
            writer.WriteNumber(nameof(Character.MP), value.MP);
            writer.WriteNumber(nameof(Character.EP), value.EP);
            writer.WriteNumber(nameof(Character.InitialATK), value.InitialATK);
            writer.WriteNumber(nameof(Character.ExATK2), value.ExATK2);
            writer.WriteNumber(nameof(Character.ExATKPercentage), value.ExATKPercentage);
            writer.WriteNumber(nameof(Character.InitialDEF), value.InitialDEF);
            writer.WriteNumber(nameof(Character.ExDEF2), value.ExDEF2);
            writer.WriteNumber(nameof(Character.ExDEFPercentage), value.ExDEFPercentage);
            writer.WritePropertyName(nameof(Character.MDF));
            JsonSerializer.Serialize(writer, value.MDF, options);
            writer.WriteNumber(nameof(Character.PhysicalPenetration), value.PhysicalPenetration);
            writer.WriteNumber(nameof(Character.MagicalPenetration), value.MagicalPenetration);
            writer.WriteNumber(nameof(Character.InitialHR), value.InitialHR);
            writer.WriteNumber(nameof(Character.ExHR), value.ExHR);
            writer.WriteNumber(nameof(Character.InitialMR), value.InitialMR);
            writer.WriteNumber(nameof(Character.ExMR), value.ExMR);
            writer.WriteNumber(nameof(Character.ER), value.ER);
            writer.WriteNumber(nameof(Character.InitialSTR), value.InitialSTR);
            writer.WriteNumber(nameof(Character.InitialAGI), value.InitialAGI);
            writer.WriteNumber(nameof(Character.InitialINT), value.InitialINT);
            writer.WriteNumber(nameof(Character.ExSTR), value.ExSTR);
            writer.WriteNumber(nameof(Character.ExAGI), value.ExAGI);
            writer.WriteNumber(nameof(Character.ExINT), value.ExINT);
            writer.WriteNumber(nameof(Character.ExSTRPercentage), value.ExSTRPercentage);
            writer.WriteNumber(nameof(Character.ExAGIPercentage), value.ExAGIPercentage);
            writer.WriteNumber(nameof(Character.ExINTPercentage), value.ExINTPercentage);
            writer.WriteNumber(nameof(Character.STRGrowth), value.STRGrowth);
            writer.WriteNumber(nameof(Character.AGIGrowth), value.AGIGrowth);
            writer.WriteNumber(nameof(Character.INTGrowth), value.INTGrowth);
            writer.WriteNumber(nameof(Character.InitialSPD), value.InitialSPD);
            writer.WriteNumber(nameof(Character.ExSPD), value.ExSPD);
            writer.WriteNumber(nameof(Character.ExActionCoefficient), value.ExActionCoefficient);
            writer.WriteNumber(nameof(Character.ExAccelerationCoefficient), value.ExAccelerationCoefficient);
            writer.WriteNumber(nameof(Character.ExCDR), value.ExCDR);
            writer.WriteNumber(nameof(Character.ATR), value.ATR);
            writer.WriteNumber(nameof(Character.ExCritRate), value.ExCritRate);
            writer.WriteNumber(nameof(Character.ExCritDMG), value.ExCritDMG);
            writer.WriteNumber(nameof(Character.ExEvadeRate), value.ExEvadeRate);
            writer.WriteNumber(nameof(Character.Lifesteal), value.Lifesteal);
            writer.WritePropertyName(nameof(Character.Shield));
            JsonSerializer.Serialize(writer, value.Shield, options);
            writer.WritePropertyName(nameof(Character.NormalAttack));
            JsonSerializer.Serialize(writer, value.NormalAttack, options);
            writer.WritePropertyName(nameof(Character.Skills));
            JsonSerializer.Serialize(writer, value.Skills.Where(s => s.Guid == Guid.Empty), options);
            writer.WritePropertyName(nameof(Character.Items));
            JsonSerializer.Serialize(writer, value.Items, options);
            writer.WriteEndObject();
        }

        public override void AfterConvert(ref Character result, Dictionary<string, object> convertingContext)
        {
            if (convertingContext.TryGetValue(nameof(Character.HP), out object? value) && value is double hp)
            {
                result.HP = hp;
            }
            if (convertingContext.TryGetValue(nameof(Character.MP), out value) && value is double mp)
            {
                result.MP = mp;
            }
            if (convertingContext.TryGetValue(nameof(Character.EP), out value) && value is double ep)
            {
                result.EP = ep;
            }
        }
    }
}
