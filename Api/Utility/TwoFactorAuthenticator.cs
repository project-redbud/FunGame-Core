using System.Security.Cryptography;
using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// Aka. 2FA 双重认证 双因素身份验证
    /// </summary>
    public class TwoFactorAuthenticator
    {
        /// <summary>
        /// SQLHelper 允许为空
        /// </summary>
        private readonly SQLHelper? SQLHelper;

        /// <summary>
        /// 不使用SQL模式
        /// </summary>
        public TwoFactorAuthenticator() { }

        /// <summary>
        /// 使用SQL模式 记录对应账号的密文到数据库中
        /// </summary>
        /// <param name="SQLHelper"></param>
        public TwoFactorAuthenticator(SQLHelper SQLHelper)
        {
            this.SQLHelper = SQLHelper;
        }

        /// <summary>
        /// 检查账号是否需要2FA
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public virtual bool IsAvailable(string username)
        {
            return true;
        }

        /// <summary>
        /// 2FA验证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string code)
        {
            // TODO
            // 使用username获取此账号记录在案的2FAKey，获取此时间戳内的验证码是否一致。
            SQLHelper?.Execute();
            return true;
        }

        /// <summary>
        /// 每30秒刷新
        /// </summary>
        private const int INTERVAL_SECONDS = 30;

        /// <summary>
        /// 6位数字2FA验证码
        /// </summary>
        private const int DIGITS = 6;

        /// <summary>
        /// ----- PUBLIC KEY -----
        /// </summary>
        private const string PUBLICKEY = "----- PUBLIC KEY -----\r\n";

        /// <summary>
        /// ----- SECRET SIGN -----
        /// </summary>
        private const string SECRETSIGN = "----- SECRET SIGN -----\r\n";

        /// <summary>
        /// 创键私钥，用于绑定账号，并生成两个文件，需要用户保存
        /// </summary>
        public void CreateSecretKey(string username)
        {
            // 秘钥文件路径
            string keypath = "authenticator.key";

            // 创建RSA实例
            using RSACryptoServiceProvider rsa = new();

            // 获取公钥和私钥
            string publickey = rsa.ToXmlString(false);
            string privatekey = rsa.ToXmlString(true);

            // 要加密的明文
            byte[] random = RandomNumberGenerator.GetBytes(10);
            string randomstring = General.DefaultEncoding.GetString(random);
            // TODO 记录对应账号的密文
            SQLHelper?.Execute();
            string plain = Base32Encode(random);

            // 加密明文，获得密文
            string secret = Encryption.RSAEncrypt(plain, publickey);

            // 保存密文到文件
            File.WriteAllText(keypath, PUBLICKEY + secret + "\r\n");

            // 保存私钥到文件
            File.AppendAllText(keypath, SECRETSIGN + privatekey);
        }

        /// <summary>
        /// 生成随机秘钥字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            StringBuilder result = new((data.Length * 8 + 4) / 5);
            int buffer = data[0];
            int next = 1;
            int bitsLeft = 8;
            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < 5)
                {
                    if (next < data.Length)
                    {
                        buffer <<= 8;
                        buffer |= data[next++] & 0xFF;
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = 5 - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                int index = 0x1F & (buffer >> (bitsLeft - 5));
                bitsLeft -= 5;
                result.Append(alphabet[index]);
            }
            return result.ToString();
        }

        /// <summary>
        /// 生成基于当前时间戳的6位数字2FA验证码
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static string GenerateCode(string secretKey)
        {
            byte[] key = Base32Decode(secretKey);
            long counter = GetCurrentCounter();
            byte[] counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes);
            }
            HMACSHA1 hmac = new(key);
            byte[] hash = hmac.ComputeHash(counterBytes);
            int offset = hash[^1] & 0x0F;
            int code = ((hash[offset] & 0x7F) << 24 |
                        (hash[offset + 1] & 0xFF) << 16 |
                        (hash[offset + 2] & 0xFF) << 8 |
                        (hash[offset + 3] & 0xFF)) % (int)Math.Pow(10, DIGITS);
            return code.ToString().PadLeft(DIGITS, '0');
        }

        /// <summary>
        /// 获取当前时间节点
        /// </summary>
        /// <returns></returns>
        private static long GetCurrentCounter()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(timeSpan.TotalSeconds / INTERVAL_SECONDS);
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte[] Base32Decode(string input)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            int length = input.Length;
            int bitsLeft = 0;
            int buffer = 0;
            int next = 0;
            byte[] result = new byte[length * 5 / 8];
            foreach (char c in input)
            {
                int value = alphabet.IndexOf(c);
                if (value < 0)
                {
                    throw new ArgumentException("Invalid base32 character: " + c);
                }
                buffer <<= 5;
                buffer |= value & 0x1F;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    result[next++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }
            return result;
        }

        /// <summary>
        /// 拆分字符串中的密文和私钥
        /// </summary>
        /// <param name="content"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool SplitKeyFile(string content, out string[] strs)
        {
            strs = content.Split(SECRETSIGN);
            if (strs.Length == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
