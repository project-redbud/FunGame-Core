using System.Text.Json;

namespace Milimoe.FunGame.Core.Interface.Base
{
    internal interface IEntityConverter<T>
    {
        public void ReadPropertyName(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options, ref T? result);
    }
}
