using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.JsonConverter;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 创建一个Json工具类<para/>
    /// 此工具类拥有单独的序列化选项，支持添加自定义转换器 <see cref="BaseEntityConverter{T}"/><para/>
    /// <see cref="BaseEntityConverter{T}"/> 继承自 <see cref="JsonConverter"/>
    /// </summary>
    public class JsonTool
    {
        /// <summary>
        /// 序列化选项<para/>
        /// 已经默认添加了下列转换器：<para/>
        /// <see cref="DateTimeConverter"/>, <see cref="DataTableConverter"/>, <see cref="DataSetConverter"/>
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions => options;

        /// <summary>
        /// 注册一个自定义转换器，支持 <see cref="BaseEntityConverter{T}"/>
        /// </summary>
        /// <param name="converter"></param>
        public void AddConverter(JsonConverter converter)
        {
            if (!JsonSerializerOptions.Converters.Contains(converter))
                JsonSerializerOptions.Converters.Add(converter);
        }
        
        /// <summary>
        /// 注册多个自定义转换器，支持 <see cref="BaseEntityConverter{T}"/>
        /// </summary>
        /// <param name="converter"></param>
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
        public string GetString<T>(T obj) => JsonManager.GetString(obj, options);

        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T? GetObject<T>(string json) => JsonManager.GetObject<T>(json, options);

        /// <summary>
        /// 反序列化Json对象，此方法可能无法返回正确的类型，请注意辨别
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object? GetObject(string json) => JsonManager.GetObject(json, options);

        /// <summary>
        /// 反序列化Hashtable中Key对应的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetObject<T>(Hashtable table, string key) => JsonManager.GetObject<T>(table, key, options);

        /// <summary>
        /// 反序列化多个Json对象
        /// 注意必须是相同的Json对象才可以使用此方法解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public List<T> GetObjects<T>(string json) => JsonManager.GetObjects<T>(json, options);

        /// <summary>
        /// Private JsonSerializerOptions
        /// </summary>
        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters = { new DateTimeConverter(), new DataTableConverter(), new DataSetConverter() }
        };
    }
}
