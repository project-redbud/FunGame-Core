using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Milimoe.FunGame.Core.Library.Common.JsonConverter;
using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Core.Service
{
    internal class JsonManager
    {
        /// <summary>
        /// 默认的序列化选项
        /// </summary>
        internal static JsonSerializerOptions GeneralOptions { get; } = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters = { new DateTimeConverter(), new DataTableConverter(), new DataSetConverter(), new UserConverter(), new RoomConverter(),
                new CharacterConverter(), new MagicResistanceConverter(), new EquipSlotConverter(), new SkillConverter(), new EffectConverter(), new ItemConverter(),
                new InventoryConverter(), new NormalAttackConverter()
            }
        };

        /// <summary>
        /// 获取Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string GetString<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, GeneralOptions);
        }

        /// <summary>
        /// 获取Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static string GetString<T>(T obj, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, GeneralOptions);
        }

        /// <summary>
        /// 反序列化Json对象，使用 <paramref name="reader"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// 反序列化Json对象，此方法可能无法返回正确的类型，请注意辨别
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static object? GetObject(string json)
        {
            return JsonSerializer.Deserialize<object>(json, GeneralOptions);
        }

        /// <summary>
        /// 反序列化Json对象，此方法可能无法返回正确的类型，请注意辨别
        /// </summary>
        /// <param name="json"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static object? GetObject(string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<object>(json, options);
        }

        /// <summary>
        /// 反序列化SocketObject中索引为index的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfArrayLengthException"></exception>
        internal static T? GetObject<T>(SocketObject obj, int index)
        {
            if (index >= obj.Parameters.Length) throw new IndexOutOfArrayLengthException();
            JsonElement element = (JsonElement)obj.Parameters[index];
            T? result = element.Deserialize<T>(GeneralOptions);
            return result;
        }

        /// <summary>
        /// 反序列化Hashtable中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(Hashtable table, string key)
        {
            if (table.ContainsKey(key))
            {
                JsonElement? element = (JsonElement?)table[key];
                if (element != null)
                {
                    T? result = ((JsonElement)element).Deserialize<T>(GeneralOptions);
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// 反序列化Dictionary中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out object? value))
            {
                JsonElement? element = (JsonElement?)value;
                if (element != null)
                {
                    T? result = ((JsonElement)element).Deserialize<T>(GeneralOptions);
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// 反序列化IEnumerable中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(IEnumerable<object> e, int index)
        {
            IEnumerable<JsonElement> elements = e.Cast<JsonElement>();
            if (elements.Count() > index)
            {
                JsonElement? element = (JsonElement?)elements.ElementAt(index);
                if (element != null)
                {
                    T? result = ((JsonElement)element).Deserialize<T>(GeneralOptions);
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// 反序列化IEnumerable中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="index"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(IEnumerable<object> e, int index, JsonSerializerOptions options)
        {
            IEnumerable<JsonElement> elements = e.Cast<JsonElement>();
            if (elements.Count() > index)
            {
                JsonElement? element = (JsonElement?)elements.ElementAt(index);
                if (element != null)
                {
                    T? result = ((JsonElement)element).Deserialize<T>(options);
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// 反序列化Hashtable中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(Hashtable table, string key, JsonSerializerOptions options)
        {
            if (table.ContainsKey(key))
            {
                JsonElement? element = (JsonElement?)table[key];
                if (element != null)
                {
                    T? result = ((JsonElement)element).Deserialize<T>(options);
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// 反序列化Dictionary中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static T? GetObject<T>(Dictionary<string, object> dict, string key, JsonSerializerOptions options)
        {
            if (dict.TryGetValue(key, out object? value))
            {
                JsonElement? element = (JsonElement?)value;
                if (element != null)
                {
                    T? result = ((JsonElement)element).Deserialize<T>(options);
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// 反序列化多个Json对象
        /// 注意必须是相同的Json对象才可以使用此方法解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static List<T> GetObjects<T>(string json)
        {
            json = "[" + json.Replace("}{", "},{") + "]"; // 将Json字符串转换为数组
            return JsonSerializer.Deserialize<List<T>>(json, GeneralOptions) ?? [];
        }

        /// <summary>
        /// 反序列化多个Json对象
        /// 注意必须是相同的Json对象才可以使用此方法解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static List<T> GetObjects<T>(string json, JsonSerializerOptions options)
        {
            json = "[" + json.Replace("}{", "},{") + "]"; // 将Json字符串转换为数组
            return JsonSerializer.Deserialize<List<T>>(json, options) ?? [];
        }

        /// <summary>
        /// 检查字符串是否为完整的JSON对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static bool IsCompleteJson<T>(string json)
        {
            try
            {
                // 尝试解析JSON数据，如果成功则表示接收到完整的JSON
                GetObject<T>(json);
                return true;
            }
            catch
            {
                // JSON解析失败，表示接收到的数据不完整
                return false;
            }
        }
    }
}
