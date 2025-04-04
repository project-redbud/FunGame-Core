namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 为字符串（string）添加扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 使用 HMAC-SHA512 算法对文本进行加密<para/>
        /// </summary>
        /// <param name="text">需要加密的文本</param>
        /// <param name="key">用于加密的秘钥</param>
        /// <returns>加密后的 HMAC-SHA512 哈希值</returns>
        public static string Encrypt(this string text, string key)
        {
            return Encryption.HmacSha512(text, key.ToLower());
        }

        public static bool EqualsGuid(this string str, object? value)
        {
            if (str.ToLower().Replace("-", "").Equals(value?.ToString()?.ToLower().Replace("-", "")))
            {
                return true;
            }
            return false;
        }
    }
}
