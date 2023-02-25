using Milimoe.FunGame.Core.Library.Constant;
using System.Runtime.InteropServices;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public partial class INIHelper
    {
        /*
         * 声明API函数
         */
        [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
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
            char[] val = new char[2048];
            _ = GetPrivateProfileString(Section, Key, "", val, 2048, Environment.CurrentDirectory.ToString() + @"\" + FileName);
            string? read = new(val);
            return read != null ? read.Trim('\0') : "";
        }

        /// <summary>
        /// 查询ini文件是否存在
        /// </summary>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        /// <returns>是否存在</returns>
        public static bool ExistINIFile(string FileName = @"FunGame.ini")
        {
            return File.Exists(Environment.CurrentDirectory.ToString() + @"\" + FileName);
        }

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
                    break;
            }
        }
    }
}
