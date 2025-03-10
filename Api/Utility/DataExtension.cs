namespace Milimoe.FunGame.Core.Api.Utility
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 从 <see cref="Dictionary{String, Object}"/> 中获取指定键的值，并将其转换为指定的类型。
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="dict">要从中获取值的字典</param>
        /// <param name="key">要获取值的键</param>
        /// <returns>转换后的值，如果键不存在或转换失败，则返回默认值。</returns>
        public static T? GetValue<T>(this Dictionary<string, object> dict, string key)
        {
            if (dict is null || !dict.TryGetValue(key, out object? value) || value is null)
            {
                return default;
            }

            try
            {
                // 如果已经是目标类型，则直接返回
                if (value is T t)
                {
                    return t;
                }

                return NetworkUtility.JsonDeserializeFromDictionary<T>(dict, key);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
