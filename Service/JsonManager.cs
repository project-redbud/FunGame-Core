using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Common.JsonConverter;
using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Core.Service
{
    internal class JsonManager
    {
        private static bool _IsFirst = true;

        /// <summary>
        /// 默认的JsonSerializerOptions
        /// </summary>
        internal static JsonSerializerOptions GeneralOptions { get; set; } = new()
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        
        /// <summary>
        /// 注册一个自定义转换器
        /// </summary>
        /// <param name="converter"></param>
        internal static void AddConverter(JsonConverter converter)
        {
            GeneralOptions.Converters.Add(converter);
        }

        internal static void CheckFirst()
        {
            if (_IsFirst)
            {
                _IsFirst = false;
                AddConverter(new DataSetConverter());
                AddConverter(new DateTimeConverter());
            }
        }

        /// <summary>
        /// 获取Json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static string GetString(JsonObject json)
        {
            CheckFirst();
            return JsonSerializer.Serialize(json, GeneralOptions);
        }

        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        internal static JsonObject? GetObject(string JsonString)
        {
            CheckFirst();
            return JsonSerializer.Deserialize<JsonObject>(JsonString, GeneralOptions);
        }

        /// <summary>
        /// 反序列化多个Json对象
        /// 注意必须是相同的Json对象才可以使用此方法解析
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        internal static JsonObject[] GetObjects(string JsonString)
        {
            CheckFirst();
            JsonString = "[" + JsonString.Replace("}{", "},{") + "]"; // 将Json字符串转换为数组
            return JsonSerializer.Deserialize<JsonObject[]>(JsonString, GeneralOptions) ?? Array.Empty<JsonObject>();
        }
    }
}
