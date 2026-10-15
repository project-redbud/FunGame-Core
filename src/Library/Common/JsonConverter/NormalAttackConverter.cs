using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Library.Architecture;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Library.Common.JsonConverter
{
    public class NormalAttackConverter : BaseEntityConverter<NormalAttack>
    {
        public override NormalAttack NewInstance()
        {
            return new NormalAttack(new());
        }

        public override void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref NormalAttack result, Dictionary<string, object> convertingContext)
        {
            switch (propertyName)
            {
                case nameof(NormalAttack.CanSelectSelf):
                    result.CanSelectSelf = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.CanSelectEnemy):
                    result.CanSelectEnemy = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.CanSelectTeammate):
                    result.CanSelectTeammate = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.SelectAllEnemies):
                    result.SelectAllEnemies = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.SelectAllTeammates):
                    result.SelectAllTeammates = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.CanSelectTargetCount):
                    result.CanSelectTargetCount = reader.GetInt32();
                    break;
                case nameof(NormalAttack.CanSelectTargetRange):
                    result.CanSelectTargetRange = reader.GetInt32();
                    break;
                case nameof(NormalAttack.AttackAnywhere):
                    result.AttackAnywhere = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.AttackRange):
                    result.AttackRange = reader.GetInt32();
                    break;
                case nameof(NormalAttack.Enable):
                    result.Enable = reader.GetBoolean();
                    break;
                case nameof(NormalAttack.Level):
                    result.Level = reader.GetInt32();
                    break;
                case nameof(NormalAttack.ExLevel):
                    result.ExLevel = reader.GetInt32();
                    break;
                case nameof(NormalAttack.ExDamage):
                    result.ExDamage = reader.GetDouble();
                    break;
                case nameof(NormalAttack.ExDamage2):
                    result.ExDamage2 = reader.GetDouble();
                    break;
                case nameof(NormalAttack.ExHardnessTime):
                    result.ExHardnessTime = reader.GetDouble();
                    break;
                case nameof(NormalAttack.ExHardnessTime2):
                    result.ExHardnessTime2 = reader.GetDouble();
                    break;
                case nameof(NormalAttack.IsMagic):
                    result.SetMagicType(reader.GetBoolean(), result.MagicType);
                    break;
                case nameof(NormalAttack.MagicType):
                    result.SetMagicType(result.IsMagic, (MagicType)reader.GetInt32());
                    break;
                case nameof(NormalAttack.IgnoreImmune):
                    result.IgnoreImmune = (ImmuneType)reader.GetInt32();
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, NormalAttack value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteBoolean(nameof(NormalAttack.CanSelectSelf), value.CanSelectSelf);
            writer.WriteBoolean(nameof(NormalAttack.CanSelectEnemy), value.CanSelectEnemy);
            writer.WriteBoolean(nameof(NormalAttack.CanSelectTeammate), value.CanSelectTeammate);
            writer.WriteBoolean(nameof(NormalAttack.SelectAllEnemies), value.SelectAllEnemies);
            writer.WriteBoolean(nameof(NormalAttack.SelectAllTeammates), value.SelectAllTeammates);
            writer.WriteNumber(nameof(NormalAttack.CanSelectTargetCount), value.CanSelectTargetCount);
            writer.WriteNumber(nameof(NormalAttack.CanSelectTargetRange), value.CanSelectTargetRange);
            writer.WriteBoolean(nameof(NormalAttack.AttackAnywhere), value.ConfiguredAttackAnywhere);
            if (value.ConfiguredAttackRange is int range) writer.WriteNumber(nameof(NormalAttack.AttackRange), range);
            writer.WriteBoolean(nameof(NormalAttack.Enable), value.Enable);
            writer.WriteNumber(nameof(NormalAttack.Level), value.Level);
            if (value.ExLevel > 0) writer.WriteNumber(nameof(NormalAttack.ExLevel), value.ExLevel);
            writer.WriteNumber(nameof(NormalAttack.ExDamage), value.ExDamage);
            writer.WriteNumber(nameof(NormalAttack.ExDamage2), value.ExDamage2);
            writer.WriteNumber(nameof(NormalAttack.ExHardnessTime), value.ExHardnessTime);
            writer.WriteNumber(nameof(NormalAttack.ExHardnessTime2), value.ExHardnessTime2);
            writer.WriteBoolean(nameof(NormalAttack.IsMagic), value.IsMagic);
            writer.WriteNumber(nameof(NormalAttack.MagicType), (int)value.MagicType);
            writer.WriteNumber(nameof(NormalAttack.IgnoreImmune), (int)value.IgnoreImmune);

            writer.WriteEndObject();
        }
    }
}
