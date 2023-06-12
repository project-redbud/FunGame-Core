using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public abstract class BaseEntityConverter<T> : JsonConverter<T>, IEntityConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            T? result = default;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString() ?? "";
                    ReadPropertyName(ref reader, propertyName, ref result);
                }
            }

            return result;
        }

        public abstract void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, ref T? result);
    }
}
