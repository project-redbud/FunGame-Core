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
