﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Milimoe.FunGame.Core.Library.Common.JsonConverter;
using Milimoe.FunGame.Core.Library.Common.Network;

namespace Milimoe.FunGame.Core.Service
{
    internal class JsonManager
    {
        private static bool _IsFirst = true;
        private readonly static JsonSerializerOptions _GeneralOptions = new()
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        /// <summary>
        /// 默认的JsonSerializerOptions
        /// </summary>
        internal static JsonSerializerOptions GeneralOptions
        {
            get
            {
                if (_IsFirst)
                {
                    // 首次使用时，向其添加自定义转换器
                    _IsFirst = false;
                    AddConverter(new DataSetConverter());
                    AddConverter(new DateTimeConverter());
                }
                return _GeneralOptions;
            }
        }
        
        /// <summary>
        /// 注册一个自定义转换器
        /// </summary>
        /// <param name="converter"></param>
        internal static void AddConverter(JsonConverter converter)
        {
            _GeneralOptions.Converters.Add(converter);
        }

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
        /// 反序列化Json对象，此方法可能无法返回正确的类型，请注意辨别
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static object? GetObject(string json)
        {
            return JsonSerializer.Deserialize<object>(json, GeneralOptions);
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
            return JsonSerializer.Deserialize<List<T>>(json, GeneralOptions) ?? new List<T>();
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
    }
}
