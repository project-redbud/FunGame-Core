using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// Json工具类<para/>
    /// 此工具类拥有单独的序列化选项，支持添加自定义转换器 <see cref="BaseEntityConverter{T}"/><para/>
    /// <see cref="BaseEntityConverter{T}"/> 继承自 <see cref="JsonConverter"/>
    /// </summary>
    public class JsonTool
    {
        /// <summary>
        /// 默认的序列化选项
        /// </summary>
        public static JsonSerializerOptions JsonSerializerOptions => JsonManager.GeneralOptions;
        
        /// <summary>
        /// 序列化选项
        /// </summary>
        public JsonSerializerOptions Options { get; set; } = new();

        /// <summary>
        /// 创建一个Json工具类<para/>
        /// 此工具类拥有单独的序列化选项，支持添加自定义转换器 <see cref="BaseEntityConverter{T}"/><para/>
        /// <see cref="BaseEntityConverter{T}"/> 继承自 <see cref="JsonConverter"/>
        /// </summary>
        public JsonTool()
        {
            Options.WriteIndented = JsonSerializerOptions.WriteIndented;
            Options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            Options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            foreach (JsonConverter converter in JsonSerializerOptions.Converters)
            {
                Options.Converters.Add(converter);
            }
        }

        /// <summary>
        /// 注册一个自定义转换器，支持 <see cref="BaseEntityConverter{T}"/>
        /// </summary>
        /// <param name="converter"></param>
        public void AddConverter(JsonConverter converter)
        {
            if (!Options.Converters.Contains(converter))
                Options.Converters.Add(converter);
        }

        /// <summary>
        /// 注册多个自定义转换器，支持 <see cref="BaseEntityConverter{T}"/>
        /// </summary>
        /// <param name="converters"></param>
        public void AddConverters(IEnumerable<JsonConverter> converters)
        {
            foreach (JsonConverter converter in converters)
            {
                AddConverter(converter);
            }
        }

        /// <summary>
        /// 获取Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetString<T>(T obj) => JsonManager.GetString(obj, Options);

        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T? GetObject<T>(string json) => JsonManager.GetObject<T>(json, Options);

        /// <summary>
        /// 反序列化Json对象，此方法可能无法返回正确的类型，请注意辨别
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object? GetObject(string json) => JsonManager.GetObject(json, Options);

        /// <summary>
        /// 反序列化Hashtable中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetObject<T>(Hashtable table, string key) => JsonManager.GetObject<T>(table, key, Options);

        /// <summary>
        /// 反序列化Dictionary中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetObject<T>(Dictionary<string, object> dict, string key) => JsonManager.GetObject<T>(dict, key, Options);

        /// <summary>
        /// 反序列化IEnumerable中的Json对象 可指定反序列化选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public T? JsonDeserializeFromIEnumerable<T>(IEnumerable<object> e, int index) => JsonManager.GetObject<T>(e, index, Options);

        /// <summary>
        /// 反序列化多个Json对象
        /// 注意必须是相同的Json对象才可以使用此方法解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public List<T> GetObjects<T>(string json) => JsonManager.GetObjects<T>(json, Options);
    }
}
