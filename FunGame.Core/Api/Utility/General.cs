using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Milimoe.FunGame.Core.Library.Constant;

// 通用工具类，客户端和服务器端都可以直接调用的工具方法都可以写在这里
namespace Milimoe.FunGame.Core.Api.Utility
{
    #region 网络服务

    /// <summary>
    /// 网络服务工具箱
    /// </summary>
    public class NetworkUtility
    {
        /// <summary>
        /// 判断字符串是否是IP地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIP(string str)
        {
            //判断是否为IP
            return Regex.IsMatch(str, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 判断字符串是否为邮箱地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(string str)
        {
            //判断是否为Email
            return Regex.IsMatch(str, @"^(\w)+(\.\w)*@(\w)+((\.\w+)+)$");
        }

        /// <summary>
        /// 判断字符串是否是一个FunGame可接受的服务器地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ErrorType IsServerAddress(string str)
        {
            string[] strs = str.Split(':');
            string ip;
            int port;
            if (strs.Length < 2)
            {
                ip = strs[0];
                port = 22222;
            }
            else if (strs.Length < 3)
            {
                ip = strs[0];
                port = Convert.ToInt32(strs[1]);
            }
            else return ErrorType.WrongFormat;
            if (IsIP(ip) && port > 0 && port < 65536) return ErrorType.None;
            else if (!IsIP(ip) && port > 0 && port < 65536) return ErrorType.IsNotIP;
            else if (IsIP(ip) && (port <= 0 || port >= 65536)) return ErrorType.IsNotPort;
            else return ErrorType.WrongFormat;
        }

        /// <summary>
        /// 判断参数是否是一个FunGame可接受的服务器地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static ErrorType IsServerAddress(string ip, int port)
        {
            if (IsIP(ip) && port > 0 && port < 65536) return ErrorType.None;
            else if (!IsIP(ip) && port > 0 && port < 65536) return ErrorType.IsNotIP;
            else if (IsIP(ip) && (port <= 0 || port >= 65536)) return ErrorType.IsNotPort;
            else return ErrorType.WrongFormat;
        }

        /// <summary>
        /// 获取服务器的延迟
        /// </summary>
        /// <param name="addr">服务器IP地址</param>
        /// <returns></returns>
        public static int GetServerPing(string addr)
        {
            Ping pingSender = new();
            PingOptions options = new()
            {
                DontFragment = true
            };
            string data = "getserverping";
            byte[] buffer = General.DefaultEncoding.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(addr, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                return Convert.ToInt32(reply.RoundtripTime);
            }
            return -1;
        }

        /// <summary>
        /// 将JsonElement转换为泛型
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="obj">为JsonElement的对象</param>
        /// <returns></returns>
        public static T? ConvertJsonObject<T>(object obj)
        {
            return ((JsonElement)obj).ToObject<T>();
        }
    }

    /// <summary>
    /// Json工具类
    /// </summary>
    internal static class JsonUtility
    {
        /// <summary>
        /// 将JsonElement转换为泛型
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="element">JsonElement</param>
        /// <returns></returns>
        internal static T? ToObject<T>(this JsonElement element)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText());
        }
    }

    #endregion

    #region 时间服务

    /// <summary>
    /// 时间服务工具箱
    /// </summary>
    public class DateTimeUtility
    {
        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <param name="type">格式化类型</param>
        /// <returns></returns>
        public static DateTime GetDateTime(TimeType type)
        {
            DateTime now = DateTime.Now;
            if (type == TimeType.DateOnly)
                return now.Date;
            else return now;
        }

        /// <summary>
        /// 通过字符串转换为DateTime对象
        /// </summary>
        /// <param name="format">时间字符串</param>
        /// <returns>转换失败返回当前时间</returns>
        public static DateTime GetDateTime(string format)
        {
            DateTime dt = default;
            if (DateTime.TryParse(format, out dt))
            {
                return dt;
            }
            else
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 获取系统时间并转为字符串
        /// </summary>
        /// <param name="type">格式化类型</param>
        /// <returns></returns>
        public static string GetDateTimeToString(TimeType type)
        {
            DateTime now = DateTime.Now;
            return type switch
            {
                TimeType.DateOnly => now.Date.ToString(""),
                TimeType.TimeOnly => now.ToString("T"),
                TimeType.Year4 => now.Year.ToString(),
                TimeType.Year2 => "'" + now.ToString("yy"),
                TimeType.Month => now.Month.ToString(),
                TimeType.Day => now.Day.ToString(),
                TimeType.Hour => now.Hour.ToString(),
                TimeType.Minute => now.Minute.ToString(),
                TimeType.Second => now.Second.ToString(),
                _ => now.ToString("yyyy/MM/dd HH:mm:ss")
            };
        }

        /// <summary>
        /// 获取系统短日期
        /// </summary>
        /// <returns></returns>
        public static string GetNowShortTime()
        {
            DateTime now = DateTime.Now;
            return now.AddMilliseconds(-now.Millisecond).ToString("T");
        }

        /// <summary>
        /// 获取系统日期
        /// </summary>
        /// <returns></returns>
        public static string GetNowTime()
        {
            DateTime now = DateTime.Now;
            return now.AddMilliseconds(-now.Millisecond).ToString();
        }
    }

    #endregion

    #region 加密服务

    /// <summary>
    /// 使用HMACSHA512算法加密
    /// </summary>
    internal class Encryption
    {
        /// <summary>
        /// 使用HMACSHA512算法加密
        /// </summary>
        /// <param name="Message">需要加密的值</param>
        /// <param name="Key">秘钥</param>
        /// <returns></returns>
        internal static string HmacSha512(string Message, string Key)
        {
            byte[] MessageBytes = General.DefaultEncoding.GetBytes(Message);
            Key = Convert.ToBase64String(General.DefaultEncoding.GetBytes(Key));
            byte[] KeyBytes = General.DefaultEncoding.GetBytes(Key);
            HMACSHA512 Hmacsha512 = new(KeyBytes);
            byte[] Hash = Hmacsha512.ComputeHash(MessageBytes);
            string Hmac = BitConverter.ToString(Hash).Replace("-", "");
            return Hmac.ToLower();
        }
    }

    public static class StringExtension
    {
        /// <summary>
        /// 使用HMACSHA512算法加密
        /// </summary>
        /// <param name="msg">需要加密的值</param>
        /// <param name="key">秘钥</param>
        /// <returns></returns>
        public static string Encrypt(this string msg, string key)
        {
            return Encryption.HmacSha512(msg, Encryption.HmacSha512(msg, key));
        }
    }

    #endregion
}
