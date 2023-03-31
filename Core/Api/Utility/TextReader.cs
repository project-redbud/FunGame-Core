using System.IO;
using System.Runtime.InteropServices;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public partial class INIHelper
    {
        /*
         * 声明API函数
         */
        [LibraryImport("kernel32", EntryPoint = "WritePrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial long WritePrivateProfileString(string section, string key, string val, string filePath);
        [LibraryImport("Kernel32.dll", EntryPoint = "GetPrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
        private static partial int GetPrivateProfileString(string section, string key, string def, char[] val, int size, string filePath);

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        public static void WriteINI(string Section, string Key, string Value, string FileName = @"FunGame.ini")
        {
            WritePrivateProfileString(Section, Key, Value, Environment.CurrentDirectory.ToString() + @"\" + FileName);
        }

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">键</param>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        /// <returns>读取到的值</returns>
        public static string ReadINI(string Section, string Key, string FileName = @"FunGame.ini")
        {
            char[] val = new char[General.StreamByteSize];
            _ = GetPrivateProfileString(Section, Key, "", val, General.StreamByteSize, Environment.CurrentDirectory.ToString() + @"\" + FileName);
            string? read = new(val);
            return read != null ? read.Trim('\0') : "";
        }

        /// <summary>
        /// 查询ini文件是否存在
        /// </summary>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        /// <returns>是否存在</returns>
        public static bool ExistINIFile(string FileName = @"FunGame.ini") => File.Exists($@"{Environment.CurrentDirectory}\{FileName}");

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
                     * Server
                     */
                    WriteINI("Server", "Name", "FunGame Server");
                    WriteINI("Server", "Password", "");
                    WriteINI("Server", "Describe", "Just Another FunGame Server.");
                    WriteINI("Server", "Notice", "This is the FunGame Server's Notice.");
                    WriteINI("Server", "Key", "");
                    WriteINI("Server", "Status", "1");
                    WriteINI("Server", "BannedList", "");
                    /**
                     * ServerMail
                     */
                    WriteINI("ServerMail", "OfficialMail", "");
                    WriteINI("ServerMail", "SupportMail", "");
                    /**
                     * Socket
                     */
                    WriteINI("Socket", "Port", "22222");
                    WriteINI("Socket", "MaxPlayer", "20");
                    WriteINI("Socket", "MaxConnectFailed", "0");
                    /**
                     * MySQL
                     */
                    WriteINI("MySQL", "DBServer", "localhost");
                    WriteINI("MySQL", "DBPort", "3306");
                    WriteINI("MySQL", "DBName", "fungame");
                    WriteINI("MySQL", "DBUser", "root");
                    WriteINI("MySQL", "DBPassword", "pass");
                    /**
                     * Mailer
                     */
                    WriteINI("Mailer", "UseMailSender", "false");
                    WriteINI("Mailer", "MailAddress", "");
                    WriteINI("Mailer", "Name", "");
                    WriteINI("Mailer", "Password", "");
                    WriteINI("Mailer", "Host", "");
                    WriteINI("Mailer", "Port", "587");
                    WriteINI("Mailer", "OpenSSL", "true");
                    break;
            }
        }
    }

    public class TXTHelper
    {
        /// <summary>
        /// 读取TXT文件内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="path">相对路径</param>
        /// <returns>内容</returns>
        public static string ReadTXT(string filename, string path = "")
        {
            if (path.Trim() != "") path = Path.Combine(path, filename);
            else path =  $@"{Environment.CurrentDirectory}\{filename}";
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
    }
}
