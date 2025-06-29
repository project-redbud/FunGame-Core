using System.Text.Json;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 简易的插件配置文件生成器<para/>
    /// 仅支持部分基本类型（<see cref="long"/>, <see cref="double"/>, <see cref="decimal"/>, <see cref="string"/>, <see cref="bool"/>）及其数组（<see cref="List{T}">List&lt;long&gt;, List&lt;double&gt;, List&lt;decimal&gt;, List&lt;string&gt;, List&lt;bool&gt;</see>和<see cref="Array">long[], double[], decimal[], string[], bool[]</see>）
    /// <para/>文件会保存为：程序目录/<see cref="RootPath"/>(通常是 configs)/<see cref="PluginName"/>/<see cref="FileName"/>.json
    /// </summary>
    /// <remarks>
    /// 新建一个配置文件，文件会保存为：程序目录/<see cref="RootPath"/>(通常是 configs)/<paramref name="plugin_name"/>/<paramref name="file_name"/>.json
    /// </remarks>
    /// <param name="plugin_name"></param>
    /// <param name="file_name"></param>
    public class PluginConfig(string plugin_name, string file_name) : Dictionary<string, object>
    {
        /// <summary>
        /// 配置文件存放的根目录
        /// </summary>
        public static string RootPath { get; set; } = "configs";

        /// <summary>
        /// 插件的名称
        /// </summary>
        public string PluginName { get; set; } = plugin_name;

        /// <summary>
        /// 配置文件的名称（后缀将是.json）
        /// </summary>
        public string FileName { get; set; } = file_name;

        /// <summary>
        /// 使用索引器给指定key赋值，不存在key会新增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new object this[string key]
        {
            get => base[key];
            set
            {
                if (value != null) Add(key, value);
            }
        }

        /// <summary>
        /// 如果保存了对象，请使用此方法转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public void Parse<T>(string key)
        {
            if (TryGetValue(key, out object? value) && value != null)
            {
                T? instance = NetworkUtility.JsonDeserialize<T>(value.ToString() ?? "");
                if (instance != null)
                {
                    base[key] = instance;
                }
            }
        }

        /// <summary>
        /// 获取指定key的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object? GetValue(string key)
        {
            if (base.TryGetValue(key, out object? value) && value != null)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 使用泛型获取指定key的value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get<T>(string key)
        {
            if (TryGetValue(key, out object? value) && value != null)
            {
                if (value is T typeValue)
                {
                    return typeValue;
                }
                if (value is List<object> listValue)
                {
                    return NetworkUtility.JsonDeserialize<T>(NetworkUtility.JsonSerialize(listValue));
                }
                if (value is Dictionary<object, object> dictValue)
                {
                    return NetworkUtility.JsonDeserialize<T>(NetworkUtility.JsonSerialize(dictValue));
                }
                return NetworkUtility.JsonDeserialize<T>(value.ToString() ?? "");
            }
            return default;
        }

        /// <summary>
        /// 添加一个配置，如果已存在key会覆盖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, object value)
        {
            if (value != null)
            {
                if (TryGetValue(key, out _)) base[key] = value;
                else base.Add(key, value);
            }
        }

        /// <summary>
        /// 从配置文件中读取配置。
        /// 注意：所有保存时为数组的对象都会变成<see cref="List{T}"/>对象，并且不支持<see cref="object"/>类型
        /// </summary>
        public void LoadConfig()
        {
            string dpath = $@"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RootPath)}/{PluginName}";
            string fpath = $@"{dpath}/{FileName}.json";
            if (!Directory.Exists(dpath))
            {
                Directory.CreateDirectory(dpath);
            }
            if (File.Exists(fpath))
            {
                string json = File.ReadAllText(fpath, General.DefaultEncoding);
                Dictionary<string, object> dict = NetworkUtility.JsonDeserialize<Dictionary<string, object>>(json) ?? [];
                Clear();
                foreach (string key in dict.Keys)
                {
                    JsonElement obj = (JsonElement)dict[key];
                    AddValue(key, obj);
                }
            }
        }

        /// <summary>
        /// 将配置保存到配置文件。调用此方法会覆盖原有的.json，请注意备份
        /// </summary>
        public void SaveConfig()
        {
            string json = NetworkUtility.JsonSerialize((Dictionary<string, object>)this);
            string dpath = $@"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RootPath)}/{PluginName}";
            string fpath = $@"{dpath}/{FileName}.json";
            if (!Directory.Exists(dpath))
            {
                Directory.CreateDirectory(dpath);
            }
            using StreamWriter writer = new(fpath, false, General.DefaultEncoding);
            writer.WriteLine(json);
            writer.Flush();
        }

        /// <summary>
        /// Json反序列化的方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        private void AddValue(string key, JsonElement obj)
        {
            switch (obj.ValueKind)
            {
                case JsonValueKind.Object:
                    base.Add(key, obj);
                    break;
                case JsonValueKind.Number:
                    if (obj.ValueKind == JsonValueKind.Number && obj.TryGetInt64(out long longValue))
                    {
                        base.Add(key, longValue);
                    }
                    else if (obj.ValueKind == JsonValueKind.Number && obj.TryGetDouble(out double douValue))
                    {
                        base.Add(key, douValue);
                    }
                    else if (obj.ValueKind == JsonValueKind.Number && obj.TryGetDecimal(out decimal decValue))
                    {
                        base.Add(key, decValue);
                    }
                    break;
                case JsonValueKind.String:
                    base.Add(key, obj.GetString() ?? "");
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    base.Add(key, obj.GetBoolean());
                    break;
                case JsonValueKind.Array:
                    AddValues(key, obj.EnumerateArray());
                    break;
            }
        }

        /// <summary>
        /// JSON 数组反序列化的方法，支持特定类型数组和混合类型数组（包括嵌套对象和数组）。
        /// </summary>
        /// <param name="key">字典的键</param>
        /// <param name="obj">JSON 数组的枚举器</param>
        private void AddValues(string key, JsonElement.ArrayEnumerator obj)
        {
            List<long> longList = [];
            List<double> douList = [];
            List<decimal> decList = [];
            List<string> strList = [];
            List<bool> bolList = [];
            List<object> resultList = [];
            // 标记是否为混合类型
            bool isMixed = false;
            // 记录第一个非 null 元素的类型
            JsonValueKind? firstValueKind = null;

            foreach (JsonElement arrayElement in obj)
            {
                switch (arrayElement.ValueKind)
                {
                    case JsonValueKind.Number when arrayElement.TryGetInt64(out long longValue):
                        longList.Add(longValue);
                        resultList.Add(longValue);
                        firstValueKind ??= JsonValueKind.Number;
                        if (firstValueKind != JsonValueKind.Number) isMixed = true;
                        break;

                    case JsonValueKind.Number when arrayElement.TryGetDouble(out double doubleValue):
                        douList.Add(doubleValue);
                        resultList.Add(doubleValue);
                        firstValueKind ??= JsonValueKind.Number;
                        if (firstValueKind != JsonValueKind.Number) isMixed = true;
                        break;

                    case JsonValueKind.Number when arrayElement.TryGetDecimal(out decimal decimalValue):
                        decList.Add(decimalValue);
                        resultList.Add(decimalValue);
                        firstValueKind ??= JsonValueKind.Number;
                        if (firstValueKind != JsonValueKind.Number) isMixed = true;
                        break;

                    case JsonValueKind.String:
                        string strValue = arrayElement.GetString() ?? "";
                        strList.Add(strValue);
                        resultList.Add(strValue);
                        firstValueKind ??= JsonValueKind.String;
                        if (firstValueKind != JsonValueKind.String) isMixed = true;
                        break;

                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        bool boolValue = arrayElement.GetBoolean();
                        bolList.Add(boolValue);
                        resultList.Add(boolValue);
                        firstValueKind ??= arrayElement.ValueKind;
                        if (firstValueKind != arrayElement.ValueKind) isMixed = true;
                        break;

                    case JsonValueKind.Null:
                        break;

                    case JsonValueKind.Object:
                        Dictionary<string, object> objValue = ParseObject(arrayElement);
                        resultList.Add(objValue);
                        isMixed = true;
                        break;

                    case JsonValueKind.Array:
                        List<object> arrayValue = ParseArray(arrayElement);
                        resultList.Add(arrayValue);
                        isMixed = true;
                        break;

                    default:
                        isMixed = true;
                        break;
                }
            }

            // 根据类型一致性选择存储的列表
            if (resultList.Count > 0)
            {
                if (!isMixed)
                {
                    // 所有元素类型一致，存储到对应的特定类型列表
                    switch (firstValueKind)
                    {
                        case JsonValueKind.Number when longList.Count == resultList.Count:
                            base.Add(key, longList);
                            break;
                        case JsonValueKind.Number when douList.Count == resultList.Count:
                            base.Add(key, douList);
                            break;
                        case JsonValueKind.Number when decList.Count == resultList.Count:
                            base.Add(key, decList);
                            break;
                        case JsonValueKind.String:
                            base.Add(key, strList);
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            base.Add(key, bolList);
                            break;
                        default:
                            base.Add(key, resultList); // 包含 null 或未知情况
                            break;
                    }
                }
                else
                {
                    // 混合类型或包含对象/数组，存储为 List<object>
                    base.Add(key, resultList);
                }
            }
        }

        /// <summary>
        /// 递归解析 JSON 对象
        /// </summary>
        private Dictionary<string, object> ParseObject(JsonElement element)
        {
            Dictionary<string, object> dict = [];
            foreach (JsonProperty property in element.EnumerateObject())
            {
                object? value = property.Value.ValueKind switch
                {
                    JsonValueKind.Number when property.Value.TryGetInt64(out long longValue) => longValue,
                    JsonValueKind.Number when property.Value.TryGetDouble(out double doubleValue) => doubleValue,
                    JsonValueKind.Number when property.Value.TryGetDecimal(out decimal decimalValue) => decimalValue,
                    JsonValueKind.String => property.Value.GetString() ?? "",
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    JsonValueKind.Object => ParseObject(property.Value),
                    JsonValueKind.Array => ParseArray(property.Value),
                    _ => null
                };
                if (value != null) dict.Add(property.Name, value);
            }
            return dict;
        }

        /// <summary>
        /// 递归解析 JSON 数组
        /// </summary>
        private List<object> ParseArray(JsonElement element)
        {
            List<object> list = [];
            foreach (JsonElement arrayElement in element.EnumerateArray())
            {
                object? value = arrayElement.ValueKind switch
                {
                    JsonValueKind.Number when arrayElement.TryGetInt64(out long longValue) => longValue,
                    JsonValueKind.Number when arrayElement.TryGetDouble(out double doubleValue) => doubleValue,
                    JsonValueKind.Number when arrayElement.TryGetDecimal(out decimal decimalValue) => decimalValue,
                    JsonValueKind.String => arrayElement.GetString() ?? "",
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    JsonValueKind.Object => ParseObject(arrayElement),
                    JsonValueKind.Array => ParseArray(arrayElement),
                    _ => null
                };
                if (value != null) list.Add(value);
            }
            return list;
        }
    }
}
