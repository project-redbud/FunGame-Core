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

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref Character result)
        {
            switch (propertyName)
            {
                case nameof(Character.Name):
                    result.Name = reader.GetString() ?? "";
                    break;
                case nameof(Character.FirstName):
                    result.FirstName = reader.GetString() ?? "";
                    break;
                case nameof(Character.NickName):
                    result.NickName = reader.GetString() ?? "";
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
                case nameof(Character.RoleRating):
                    result.RoleRating = (RoleRating)reader.GetInt32();
                    break;
                case nameof(Character.Promotion):
                    result.Promotion = reader.GetInt32();
                    break;
                case nameof(Character.Level):
                    result.Level = reader.GetInt32();
                    break;
                case nameof(Character.EXP):
                    result.EXP = reader.GetDouble();
                    break;
                case nameof(Character.BaseHP):
                    result.BaseHP = reader.GetDouble();
                    break;
                case nameof(Character.HP):
                    result.HP = reader.GetDouble();
                    break;
                case nameof(Character.BaseMP):
                    result.BaseMP = reader.GetDouble();
                    break;
                case nameof(Character.MP):
                    result.MP = reader.GetDouble();
                    break;
                case nameof(Character.EP):
                    result.EP = reader.GetDouble();
                    break;
                case nameof(Character.BaseATK):
                    result.BaseATK = reader.GetDouble();
                    break;
                case nameof(Character.ATK):
                    result.ATK = reader.GetDouble();
                    break;
                case nameof(Character.BaseDEF):
                    result.BaseDEF = reader.GetDouble();
                    break;
                case nameof(Character.DEF):
                    result.DEF = reader.GetDouble();
                    break;
                case nameof(Character.MDF):
                    result.MDF = reader.GetDouble();
                    break;
                case nameof(Character.PhysicalPenetration):
                    result.PhysicalPenetration = reader.GetDouble();
                    break;
                case nameof(Character.MagicalPenetration):
                    result.MagicalPenetration = reader.GetDouble();
                    break;
                case nameof(Character.HR):
                    result.HR = reader.GetDouble();
                    break;
                case nameof(Character.MR):
                    result.MR = reader.GetDouble();
                    break;
                case nameof(Character.ER):
                    result.ER = reader.GetDouble();
                    break;
                case nameof(Character.BaseSTR):
                    result.BaseSTR = reader.GetDouble();
                    break;
                case nameof(Character.BaseAGI):
                    result.BaseAGI = reader.GetDouble();
                    break;
                case nameof(Character.BaseINT):
                    result.BaseINT = reader.GetDouble();
                    break;
                case nameof(Character.STR):
                    result.STR = reader.GetDouble();
                    break;
                case nameof(Character.AGI):
                    result.AGI = reader.GetDouble();
                    break;
                case nameof(Character.INT):
                    result.INT = reader.GetDouble();
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
                case nameof(Character.SPD):
                    result.SPD = reader.GetDouble();
                    break;
                case nameof(Character.AccelerationCoefficient):
                    result.AccelerationCoefficient = reader.GetDouble();
                    break;
                case nameof(Character.CDR):
                    result.CDR = reader.GetDouble();
                    break;
                case nameof(Character.ATR):
                    result.ATR = reader.GetDouble();
                    break;
                case nameof(Character.CritRate):
                    result.CritRate = reader.GetDouble();
                    break;
                case nameof(Character.CritDMG):
                    result.CritDMG = reader.GetDouble();
                    break;
                case nameof(Character.EvadeRate):
                    result.EvadeRate = reader.GetDouble();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, Character value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Character.Name), value.Name);
            writer.WriteString(nameof(Character.FirstName), value.FirstName);
            writer.WriteString(nameof(Character.NickName), value.NickName);
            writer.WriteNumber(nameof(Character.MagicType), (int)value.MagicType);
            writer.WriteNumber(nameof(Character.FirstRoleType), (int)value.FirstRoleType);
            writer.WriteNumber(nameof(Character.SecondRoleType), (int)value.SecondRoleType);
            writer.WriteNumber(nameof(Character.ThirdRoleType), (int)value.ThirdRoleType);
            writer.WriteNumber(nameof(Character.RoleRating), (int)value.RoleRating);
            writer.WriteNumber(nameof(Character.Promotion), value.Promotion);
            writer.WriteNumber(nameof(Character.Level), value.Level);
            writer.WriteNumber(nameof(Character.EXP), value.EXP);
            writer.WriteNumber(nameof(Character.BaseHP), value.BaseHP);
            writer.WriteNumber(nameof(Character.HP), value.HP);
            writer.WriteNumber(nameof(Character.BaseMP), value.BaseMP);
            writer.WriteNumber(nameof(Character.MP), value.MP);
            writer.WriteNumber(nameof(Character.EP), value.EP);
            writer.WriteNumber(nameof(Character.BaseATK), value.BaseATK);
            writer.WriteNumber(nameof(Character.ATK), value.ATK);
            writer.WriteNumber(nameof(Character.BaseDEF), value.BaseDEF);
            writer.WriteNumber(nameof(Character.DEF), value.DEF);
            writer.WriteNumber(nameof(Character.MDF), value.MDF);
            writer.WriteNumber(nameof(Character.PhysicalPenetration), value.PhysicalPenetration);
            writer.WriteNumber(nameof(Character.MagicalPenetration), value.MagicalPenetration);
            writer.WriteNumber(nameof(Character.HR), value.HR);
            writer.WriteNumber(nameof(Character.MR), value.MR);
            writer.WriteNumber(nameof(Character.ER), value.ER);
            writer.WriteNumber(nameof(Character.BaseSTR), value.BaseSTR);
            writer.WriteNumber(nameof(Character.BaseAGI), value.BaseAGI);
            writer.WriteNumber(nameof(Character.BaseINT), value.BaseINT);
            writer.WriteNumber(nameof(Character.STR), value.STR);
            writer.WriteNumber(nameof(Character.AGI), value.AGI);
            writer.WriteNumber(nameof(Character.INT), value.INT);
            writer.WriteNumber(nameof(Character.STRGrowth), value.STRGrowth);
            writer.WriteNumber(nameof(Character.AGIGrowth), value.AGIGrowth);
            writer.WriteNumber(nameof(Character.INTGrowth), value.INTGrowth);
            writer.WriteNumber(nameof(Character.SPD), value.SPD);
            writer.WriteNumber(nameof(Character.AccelerationCoefficient), value.AccelerationCoefficient);
            writer.WriteNumber(nameof(Character.CDR), value.CDR);
            writer.WriteNumber(nameof(Character.ATR), value.ATR);
            writer.WriteNumber(nameof(Character.CritRate), value.CritRate);
            writer.WriteNumber(nameof(Character.CritDMG), value.CritDMG);
            writer.WriteNumber(nameof(Character.EvadeRate), value.EvadeRate);
            writer.WriteEndObject();
        }

    }
}
