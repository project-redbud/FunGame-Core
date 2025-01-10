using System.Text.Json;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IEntityConverter<T>
    {
        public T NewInstance();

        public void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref T result, Dictionary<string, object> convertingContext);

        public void AfterConvert(ref T result, Dictionary<string, object> convertingContext);
    }
}
