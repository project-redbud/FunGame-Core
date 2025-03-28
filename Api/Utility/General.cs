using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Milimoe.FunGame.Core.Library.Common.Architecture;
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
        public static bool IsIP(string str) => Regex.IsMatch(str, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        /// <summary>
        /// 判断字符串是否为邮箱地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(string str) => Regex.IsMatch(str, @"^(\w)+(\.\w)*@(\w)+((\.\w+)+)$");

        /// <summary>
        /// 判断字符串是否是正常的用户名（只有中英文和数字）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUserName(string str) => Regex.IsMatch(str, @"^[\u4e00-\u9fffA-Za-z0-9]+$");

        /// <summary>
        /// 判断字符串是否是全中文的字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChineseName(string str) => Regex.IsMatch(str, @"^[\u4e00-\u9fff]+$");

        /// <summary>
        /// 获取用户名长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetUserNameLength(string str)
        {
            int length = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9') length++;
                else length += 2;
            }
            return length;
        }

        /// <summary>
        /// 判断字符串是否是一个FunGame可接受的服务器地址<para/>
        /// 此方法可以解析域名
        /// </summary>
        /// <param name="str"></param>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        /// <returns>返回地址验证结果，同时输出服务器地址和端口号</returns>
        public static ErrorIPAddressType IsServerAddress(string str, out string addr, out int port)
        {
            addr = "";
            port = 22222;
            string ip;
            // 包含端口号时，需要先截取
            string[] strs = str.Split(':');
            if (strs.Length == 1)
            {
                addr = str;
            }
            else if (strs.Length > 1)
            {
                addr = strs[0];
                port = int.Parse(strs[1]);
            }
            else if (strs.Length > 2)
            {
                return ErrorIPAddressType.WrongFormat;
            }
            try
            {
                ip = GetIPAddress(addr);
            }
            catch
            {
                ip = strs[0];
            }
            return IsServerAddress(ip, port);
        }

        /// <summary>
        /// 判断参数是否是一个FunGame可接受的服务器地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static ErrorIPAddressType IsServerAddress(string ip, int port)
        {
            if (IsIP(ip))
            {
                if (port > 0 && port < 65536)
                {
                    return ErrorIPAddressType.None;
                }
                else
                {
                    return ErrorIPAddressType.IsNotPort;
                }
            }
            else if (port > 0 && port < 65536)
            {
                return ErrorIPAddressType.IsNotAddress;
            }
            else return ErrorIPAddressType.WrongFormat;
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
        /// 解析域名为IP地址
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="family"></param>
        /// <returns></returns>
        internal static string GetIPAddress(string domain, System.Net.Sockets.AddressFamily family = System.Net.Sockets.AddressFamily.InterNetwork)
        {
            // 如果是域名，则解析为IP地址
            IPHostEntry entrys = Dns.GetHostEntry(domain);
            // 这里使用断言，请自行添加try catch配合使用
            return entrys.AddressList.Where(addr => addr.AddressFamily == family).FirstOrDefault()!.ToString();
        }

        /// <summary>
        /// 返回目标对象的Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T obj) => Service.JsonManager.GetString(obj);

        /// <summary>
        /// 返回目标对象的Json字符串 可指定反序列化选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T obj, JsonSerializerOptions options) => Service.JsonManager.GetString(obj, options);

        /// <summary>
        /// 反序列化Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T? JsonDeserialize<T>(string json) => Service.JsonManager.GetObject<T>(json);

        /// <summary>
        /// 反序列化Json对象 使用 <paramref name="reader"/> 可指定反序列化选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T? JsonDeserialize<T>(ref Utf8JsonReader reader, JsonSerializerOptions options) => Service.JsonManager.GetObject<T>(ref reader, options);

        /// <summary>
        /// 反序列化Json对象 可指定反序列化选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T? JsonDeserialize<T>(string json, JsonSerializerOptions options) => Service.JsonManager.GetObject<T>(json, options);

        /// <summary>
        /// 反序列化Hashtable中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashtable"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? JsonDeserializeFromHashtable<T>(Hashtable hashtable, string key) => Service.JsonManager.GetObject<T>(hashtable, key);

        /// <summary>
        /// 反序列化Dictionary中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? JsonDeserializeFromDictionary<T>(Dictionary<string, object> dict, string key) => Service.JsonManager.GetObject<T>(dict, key);

        /// <summary>
        /// 反序列化IEnumerable中的Json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T? JsonDeserializeFromIEnumerable<T>(IEnumerable<object> e, int index) => Service.JsonManager.GetObject<T>(e, index);

        /// <summary>
        /// 反序列化IEnumerable中的Json对象 可指定反序列化选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="index"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T? JsonDeserializeFromIEnumerable<T>(IEnumerable<object> e, int index, JsonSerializerOptions options) => Service.JsonManager.GetObject<T>(e, index, options);

        /// <summary>
        /// 反序列化Hashtable中的Json对象 可指定反序列化选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashtable"></param>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T? JsonDeserializeFromHashtable<T>(Hashtable hashtable, string key, JsonSerializerOptions options) => Service.JsonManager.GetObject<T>(hashtable, key, options);

        // 创建一个静态 HttpClient 实例，供整个应用程序生命周期使用
        private static readonly HttpClient client = new();

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<T?> HttpGet<T>(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            T? result = JsonDeserialize<T>(content);
            return result;
        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static async Task<T?> HttpPost<T>(string url, string json)
        {
            HttpContent content = new StringContent(json, General.DefaultEncoding, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string read = await response.Content.ReadAsStringAsync();
            T? result = JsonDeserialize<T>(read);
            return result;
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
            if (type == TimeType.LongDateOnly || type == TimeType.ShortDateOnly)
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
            if (DateTime.TryParse(format, out DateTime dt))
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
                TimeType.LongDateOnly => now.Date.ToString("D"),
                TimeType.ShortDateOnly => now.Date.ToString("d"),
                TimeType.LongTimeOnly => now.ToString("T"),
                TimeType.ShortTimeOnly => now.ToString("t"),
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
        /// 获取系统时间并转为字符串
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public static string GetDateTimeToString(string format) => DateTime.Now.ToString(format);

        /// <summary>
        /// 获取系统短时间
        /// </summary>
        /// <returns>时:分:秒</returns>
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

        /// <summary>
        /// 获取 DLL 编译时间
        /// </summary>
        /// <param name="dll"></param>
        /// <returns></returns>
        public static string GetBuiltTime(string dll)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(dll);

            string month = lastWriteTime.ToString("MMM", CultureInfo.InvariantCulture);
            int day = lastWriteTime.Day;
            string time = lastWriteTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            return $"{month}. {day}, {lastWriteTime.Year} {time}";
        }

        /// <summary>
        /// 获取下一次可交易的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetTradableTime(DateTime? date = null)
        {
            date ??= DateTime.Now;
            if (date is DateTime d)
            {
                if (d.Hour < 15)
                {
                    return d.Date.AddDays(1).AddHours(15);
                }
                else
                {
                    return d.Date.AddDays(2).AddHours(9);
                }
            }
            return DateTime.MinValue;
        }
    }

    #endregion

    #region 加密服务

    /// <summary>
    /// 加密服务工具箱
    /// </summary>
    public class Encryption
    {
        /// <summary>
        /// 使用 HMAC-SHA512 算法对文本进行加密
        /// </summary>
        /// <param name="text">需要加密的文本</param>
        /// <param name="key">用于加密的秘钥</param>
        /// <returns>加密后的 HMAC-SHA512 哈希值</returns>
        public static string HmacSha512(string text, string key)
        {
            byte[] text_bytes = General.DefaultEncoding.GetBytes(text);
            key = Convert.ToBase64String(General.DefaultEncoding.GetBytes(key));
            byte[] key_bytes = General.DefaultEncoding.GetBytes(key);
            HMACSHA512 hmacsha512 = new(key_bytes);
            byte[] hash_bytes = hmacsha512.ComputeHash(text_bytes);
            string hmac = Convert.ToHexString(hash_bytes);
            return hmac.ToLower();
        }

        /// <summary>
        /// 计算文件的 SHA-256 哈希值
        /// </summary>
        /// <param name="file_path">要计算哈希值的文件路径</param>
        /// <returns>文件的 SHA-256 哈希值</returns>
        public static string FileSha256(string file_path)
        {
            using SHA256 sha256 = SHA256.Create();
            using FileStream stream = File.OpenRead(file_path);
            byte[] hash = sha256.ComputeHash(stream);
            return Convert.ToHexStringLower(hash);
        }

        /// <summary>
        /// 使用 RSA 算法加密
        /// </summary>
        /// <param name="plain_text">明文</param>
        /// <param name="plublic_key">公钥</param>
        /// <returns></returns>
        public static string RSAEncrypt(string plain_text, string plublic_key)
        {
            byte[] plain = General.DefaultEncoding.GetBytes(plain_text);
            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(plublic_key);
            byte[] encrypted = rsa.Encrypt(plain, true);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// 使用 RSA 算法解密
        /// </summary>
        /// <param name="secret_text">密文</param>
        /// <param name="private_key">私钥</param>
        /// <returns></returns>
        public static string RSADecrypt(string secret_text, string private_key)
        {
            byte[] secret = Convert.FromBase64String(secret_text);
            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(private_key);
            byte[] decrypted = rsa.Decrypt(secret, true);
            return General.DefaultEncoding.GetString(decrypted);
        }

        /// <summary>
        /// 使用 MD5 算法加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5(string text)
        {
            byte[] inputBytes = General.DefaultEncoding.GetBytes(text);
            byte[] hash = System.Security.Cryptography.MD5.HashData(inputBytes);
            return Convert.ToHexStringLower(hash);
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length = 18)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+=-`~[]\\{}|;':\",./<>?";
            byte[] data = RandomNumberGenerator.GetBytes(length);

            StringBuilder result = new(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }
            return result.ToString();
        }
    }

    #endregion

    #region 验证服务

    /// <summary>
    /// 验证码服务工具箱
    /// </summary>
    public class Verification
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string CreateVerifyCode(VerifyCodeType type, int length)
        {
            return type switch
            {
                VerifyCodeType.MixVerifyCode => MixVerifyCode(length),
                VerifyCodeType.LetterVerifyCode => LetterVerifyCode(length),
                _ => NumberVerifyCode(length),
            };
        }

        /// <summary>
        /// 数字验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string NumberVerifyCode(int length)
        {
            int[] RandMembers = new int[length];
            int[] GetNumbers = new int[length];
            string VerifyCode = "";
            //生成起始序列值  
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new(seekSeek);
            int beginSeek = seekRand.Next(0, int.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字  
            for (int i = 0; i < length; i++)
            {
                Random rand = new(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                RandMembers[i] = rand.Next(pownum, int.MaxValue);
            }
            //抽取随机数字  
            for (int i = 0; i < length; i++)
            {
                string numStr = RandMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new();
                int numPosition = rand.Next(0, numLength - 1);
                GetNumbers[i] = int.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码  
            for (int i = 0; i < length; i++)
            {
                VerifyCode += GetNumbers[i].ToString();
            }
            return VerifyCode;
        }

        /// <summary>
        /// 字母验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string LetterVerifyCode(int length)
        {
            char[] Verification = new char[length];
            char[] Dictionary = [ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' ];
            Random random = new();
            for (int i = 0; i < length; i++)
            {
                Verification[i] = Dictionary[random.Next(Dictionary.Length - 1)];
            }
            return new string(Verification);
        }

        /// <summary>
        /// 混合验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string MixVerifyCode(int length)
        {
            char[] Verification = new char[length];
            char[] Dictionary = [ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            ];
            Random random = new();
            for (int i = 0; i < length; i++)
            {
                Verification[i] = Dictionary[random.Next(Dictionary.Length - 1)];
            }
            return new string(Verification);
        }
    }

    public class WebAPIAuthenticator
    {
        /// <summary>
        /// Web API 自定义 Token 验证器
        /// </summary>
        public static event Func<string, string>? WebAPICustomBearerTokenAuthenticator;

        /// <summary>
        /// 添加自定义 Token 验证器
        /// </summary>
        /// <param name="handler"></param>
        public static void AddCustomBearerTokenHandler(Func<string, string> handler)
        {
            WebAPICustomBearerTokenAuthenticator += handler;
        }

        /// <summary>
        /// 对自定义 Token 进行验证
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string ValidateToken(string token)
        {
            string result = "";

            if (WebAPICustomBearerTokenAuthenticator != null)
            {
                foreach (Delegate handler in WebAPICustomBearerTokenAuthenticator.GetInvocationList())
                {
                    if (handler is Func<string, string> authHandler)
                    {
                        string name = authHandler.Invoke(token);
                        if (name != "")
                        {
                            return name;
                        }
                    }
                }
            }

            return result;
        }
    }

    #endregion

    #region 多线程服务

    /// <summary>
    /// 多线程服务工具箱
    /// </summary>
    public class TaskUtility
    {
        /// <summary>
        /// 开启一个异步任务。调用返回对象的 OnCompleted() 方法可以执行后续操作
        /// </summary>
        /// <param name="action"></param>
        public static TaskAwaiter NewTask(Action action) => new(Service.TaskManager.NewTask(action));

        /// <summary>
        /// 开启一个异步任务。调用返回对象的 OnCompleted() 方法可以执行后续操作
        /// </summary>
        /// <param name="task"></param>
        public static TaskAwaiter NewTask(Func<Task> task) => new(Service.TaskManager.NewTask(task));

        /// <summary>
        /// 开启一个计时器任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="milliseconds"></param>
        public static void RunTimer(Action action, int milliseconds)
        {
            Service.TaskManager.NewTask(async () =>
            {
                await Task.Delay(milliseconds);
            }).OnCompleted(action);
        }

        /// <summary>
        /// 添加任务计划，使用 <paramref name="time"/> 的时分秒。如果用 <see cref="TimeSpan"/>，请直接在 <see cref="TaskScheduler.Shared"/> 中添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public static void AddSchedulerTask(string name, DateTime time, Action action)
        {
            TaskScheduler.Shared.AddTask(name, new TimeSpan(time.Hour, time.Minute, time.Second), action);
        }

        /// <summary>
        /// 添加循环任务。如果用 <see cref="TimeSpan"/>，请直接在 <see cref="TaskScheduler.Shared"/> 中添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seconds"></param>
        /// <param name="action"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="startNow"></param>
        public static void AddRecurringTask(string name, int seconds, Action action, int hours = 0, int minutes = 0, bool startNow = false)
        {
            TaskScheduler.Shared.AddRecurringTask(name, new TimeSpan(hours, minutes, seconds), action, startNow);
        }
    }

    #endregion

    #region 计算服务

    /// <summary>
    /// 计算服务工具箱
    /// </summary>
    public class Calculation
    {
        /// <summary>
        /// 四舍五入计算
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double Round(double value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 四舍五入保留2位小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round2Digits(double value)
        {
            return Round(value, 2);
        }

        /// <summary>
        /// 四舍五入保留4位小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round4Digits(double value)
        {
            return Round(value, 4);
        }

        /// <summary>
        /// 此方法检查一个 百分比(%) 数值是否存在于 [0,1] 区间
        /// </summary>
        /// <param name="value"></param>
        /// <returns>如果低于0，则返回0；超过1则返回1。</returns>
        public static double PercentageCheck(double value)
        {
            return Math.Max(0, Math.Min(value, 1));
        }

        /// <summary>
        /// 检查 <paramref name="value"/> 是否约等于 0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsApproximatelyZero(double value, double epsilon = 1e-10)
        {
            return Math.Abs(value) < epsilon;
        }
    }

    #endregion
}
