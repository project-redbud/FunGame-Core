using FunGame.Core.Api.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FunGame.Core.Api.Util
{
    /// <summary>
    /// 工具类，客户端和服务器端都可以直接调用的工具方法都可以写在这里
    /// </summary>
    public class Utility
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
    }
}
