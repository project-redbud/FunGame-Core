using System.Text;
using System.Text.Json;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 简易的插件配置文件生成器<para/>
    /// 仅支持部分基本类型（<see cref="long"/>, <see cref="decimal"/>, <see cref="string"/>, <see cref="bool"/>）及其数组（<see cref="List{T}">List&lt;long&gt;, List&lt;decimal&gt;, List&lt;string&gt;, List&lt;bool&gt;</see>和<see cref="Array">long[], decimal[], string[], bool[]</see>）
    /// <para/>文件会保存为：程序目录/configs/<see cref="PluginName"/>/<see cref="FileName"/>.json
    /// </summary>
    /// <remarks>
    /// 新建一个配置文件，文件会保存为：程序目录/configs/<see cref="PluginName"/>/<see cref="FileName"/>.json
    /// </remarks>
    /// <param name="plugin_name"></param>
    /// <param name="file_name"></param>
    public class PluginConfig(string plugin_name, string file_name) : Dictionary<string, object>
    {
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
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/{PluginName}";
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
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/{PluginName}";
            string fpath = $@"{dpath}/{FileName}.json";
            if (!Directory.Exists(dpath))
            {
                Directory.CreateDirectory(dpath);
            }
            using StreamWriter writer = new(fpath, false, Encoding.Unicode);
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
        /// Json数组反序列化的方法。不支持<see cref="object"/>数组。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        private void AddValues(string key, JsonElement.ArrayEnumerator obj)
        {
            List<long> longList = [];
            List<decimal> decList = [];
            List<string> strList = [];
            List<bool> bolList = [];
            foreach (JsonElement array_e in obj)
            {
                if (array_e.ValueKind == JsonValueKind.Number && array_e.TryGetInt64(out long longValue))
                {
                    longList.Add(longValue);
                }
                else if (array_e.ValueKind == JsonValueKind.Number && array_e.TryGetDecimal(out decimal decValue))
                {
                    decList.Add(decValue);
                }
                else if (array_e.ValueKind == JsonValueKind.String)
                {
                    strList.Add(array_e.GetString() ?? "");
                }
                else if (array_e.ValueKind == JsonValueKind.True || array_e.ValueKind == JsonValueKind.False)
                {
                    bolList.Add(array_e.GetBoolean());
                }
            }
            if (longList.Count > 0) base.Add(key, longList);
            if (decList.Count > 0) base.Add(key, decList);
            if (strList.Count > 0) base.Add(key, strList);
            if (bolList.Count > 0) base.Add(key, bolList);
        }
    }
}
