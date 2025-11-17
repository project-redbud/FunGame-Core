using System.Runtime.InteropServices;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public partial class INIHelper
    {
        /*
         * 声明API函数
         */
        [LibraryImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial long WritePrivateProfileString(string section, string key, string val, string filePath);
        [LibraryImport("Kernel32.dll", EntryPoint = "GetPrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial int GetPrivateProfileString(string section, string key, string def, char[] val, int size, string filePath);

        /// <summary>
        /// 默认的配置文件名称
        /// </summary>
        public const string DefaultFileName = @"FunGame.ini";

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        public static void WriteINI(string Section, string Key, string Value, string FileName = DefaultFileName)
        {
            WritePrivateProfileString(Section, Key, Value, AppDomain.CurrentDomain.BaseDirectory + FileName);
        }

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">键</param>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        /// <returns>读取到的值</returns>
        public static string ReadINI(string Section, string Key, string FileName = DefaultFileName)
        {
            char[] val = new char[General.StreamByteSize];
            _ = GetPrivateProfileString(Section, Key, "", val, General.StreamByteSize, AppDomain.CurrentDomain.BaseDirectory + FileName);
            string? read = new(val);
            return read != null ? read.Trim('\0') : "";
        }

        /// <summary>
        /// 查询ini文件是否存在
        /// </summary>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        /// <returns>是否存在</returns>
        public static bool INIFileExists(string FileName = DefaultFileName) => File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}{FileName}");

        /// <summary>
        /// 初始化ini模板文件
        /// </summary>
        public static void Init(FunGameInfo.FunGame FunGameType)
        {
            switch (FunGameType)
            {
                case FunGameInfo.FunGame.FunGame_Core:
                case FunGameInfo.FunGame.FunGame_Core_Api:
                case FunGameInfo.FunGame.FunGame_Console:
                case FunGameInfo.FunGame.FunGame_Desktop:
                    /**
                     * Config
                     */
                    WriteINI("Config", "AutoConnect", "true");
                    WriteINI("Config", "AutoLogin", "false");
                    /**
                     * Account
                     */
                    WriteINI("Account", "UserName", "");
                    WriteINI("Account", "Password", "");
                    WriteINI("Account", "AutoKey", "");
                    break;
                case FunGameInfo.FunGame.FunGame_Server:
                    /**
                     * Console
                     */
                    WriteINI("Console", "LogLevel", "INFO");
                    /**
                     * Server
                     */
                    WriteINI("Server", "Name", "FunGame Server");
                    WriteINI("Server", "Password", "");
                    WriteINI("Server", "Description", "Just Another FunGame Server.");
                    WriteINI("Server", "Notice", "This is the FunGame Server's Notice.");
                    WriteINI("Server", "Key", "");
                    WriteINI("Server", "Status", "1");
                    WriteINI("Server", "BannedList", "");
                    WriteINI("Server", "UseDesktopParameters", "true");
                    /**
                     * ServerMail
                     */
                    WriteINI("ServerMail", "OfficialMail", "");
                    WriteINI("ServerMail", "SupportMail", "");
                    /**
                     * Socket
                     */
                    WriteINI("Socket", "Port", "22222");
                    WriteINI("Socket", "UseWebSocket", "false");
                    WriteINI("Socket", "WebSocketAddress", "*");
                    WriteINI("Socket", "WebSocketPort", "22223");
                    WriteINI("Socket", "WebSocketSubUrl", "ws");
                    WriteINI("Socket", "WebSocketSSL", "false");
                    WriteINI("Socket", "MaxPlayer", "20");
                    WriteINI("Socket", "MaxConnectFailed", "0");
                    /**
                     * MySQL
                     */
                    WriteINI("MySQL", "UseMySQL", "false");
                    WriteINI("MySQL", "DBServer", "localhost");
                    WriteINI("MySQL", "DBPort", "3306");
                    WriteINI("MySQL", "DBName", "fungame");
                    WriteINI("MySQL", "DBUser", "root");
                    WriteINI("MySQL", "DBPassword", "pass");
                    /**
                     * SQLite
                     */
                    WriteINI("SQLite", "UseSQLite", "true");
                    WriteINI("SQLite", "DataSource", "FunGameDB");
                    /**
                     * Mailer
                     */
                    WriteINI("Mailer", "UseMailSender", "false");
                    WriteINI("Mailer", "MailAddress", "");
                    WriteINI("Mailer", "Name", "");
                    WriteINI("Mailer", "Password", "");
                    WriteINI("Mailer", "Host", "");
                    WriteINI("Mailer", "Port", "587");
                    WriteINI("Mailer", "SSL", "true");
                    break;
            }
        }
    }

    public class TXTHelper
    {
        /// <summary>
        /// 读取TXT文件内容
        /// </summary>
        /// <param name="filename">文件名（需要包含扩展名）</param>
        /// <param name="path">相对路径</param>
        /// <returns>内容</returns>
        public static string ReadTXT(string filename, string path = "")
        {
            if (path.Trim() != "") path = Path.Combine(path, filename);
            else path = $@"{AppDomain.CurrentDomain.BaseDirectory}{filename}";
            if (File.Exists(path))
            {
                string s = "";
                // 创建一个 StreamReader 的实例来读取文件
                using StreamReader sr = new(path);
                string? line;
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    s += line + " ";
                }
                return s;
            }
            return "";
        }

        /// <summary>
        /// 写入TXT文件内容（如不存在文件会创建）<para/>
        /// <paramref name="overwrite" /> 选项用于覆盖或追加文本
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filename">文件名（需要包含扩展名）</param>
        /// <param name="path">相对路径</param>
        /// <param name="overwrite">是否覆盖</param>
        public static void WriteTXT(string content, string filename, string path = "", bool overwrite = false)
        {
            if (path.Trim() != "")
            {
                // 不存在文件夹将创建文件夹
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                path = Path.Combine(path, filename);
            }
            else path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            // 写入内容
            StreamWriter writer = File.Exists(path) ? new(path, !overwrite, General.DefaultEncoding) : new(path, false, General.DefaultEncoding);
            writer.WriteLine(content);
            writer.Close();
        }

        /// <summary>
        /// 写入并覆盖TXT文件内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filename">文件名（需要包含扩展名）</param>
        /// <param name="path">相对路径</param>
        public static void OverwriteTXT(string content, string filename, string path = "") => WriteTXT(content, filename, path, true);

        /// <summary>
        /// 追加错误日志 默认写入logs文件夹下的当日日期.log文件
        /// </summary>
        /// <param name="msg"></param>
        public static void AppendErrorLog(string msg) => WriteTXT(DateTimeUtility.GetDateTimeToString(TimeType.General) + ": " + msg + "\r\n", DateTimeUtility.GetDateTimeToString("yyyy-MM-dd") + ".log", "logs");

        /// <summary>
        /// 追加错误日志 默认写入logs文件夹下的当日日期.log文件
        /// </summary>
        /// <param name="e"></param>
        public static void AppendErrorLog(Exception e) => AppendErrorLog(e.GetErrorInfo());
    }
}
