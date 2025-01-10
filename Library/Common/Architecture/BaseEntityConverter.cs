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
            Dictionary<string, object> convertingContext = [];

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    result ??= NewInstance();
                    string propertyName = reader.GetString() ?? "";
                    reader.Read();
                    ReadPropertyName(ref reader, propertyName, options, ref result, convertingContext);
                }
            }

            if (result != null)
            {
                AfterConvert(ref result, convertingContext);
            }

            return result;
        }

        public abstract T NewInstance();

        public abstract void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref T result, Dictionary<string, object> convertingContext);

        public virtual void AfterConvert(ref T result, Dictionary<string, object> convertingContext)
        {
            // do nothing
        }
    }
}
