using System.Text;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public partial class INIHelper
    {
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
            return ReadPrivateProfileString(Section, Key, "", AppDomain.CurrentDomain.BaseDirectory + FileName);
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
                    WriteINI("Console", "UseHotLoadAddons", "false");
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

        /// <summary>
        /// 读取ini文件内容
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string ReadPrivateProfileString(string section, string key, string def, string filePath)
        {
            if (!File.Exists(filePath)) return def;
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            string sectionHeader = "[" + section.Trim() + "]";
            bool inSection = false;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                // 跳过空行和注释
                if (string.IsNullOrEmpty(line) || line.StartsWith(';') || line.StartsWith('#'))
                    continue;

                // 检测节头
                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    inSection = string.Equals(line, sectionHeader, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                // 在目标节内查找键
                if (inSection)
                {
                    int eqIndex = line.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        string currentKey = line[..eqIndex].Trim();
                        if (string.Equals(currentKey, key.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            return line[(eqIndex + 1)..].Trim();
                        }
                    }
                }
            }
            return def;
        }

        /// <summary>
        /// 写入ini文件内容，如果节或键不存在则创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        private static void WritePrivateProfileString(string section, string key, string value, string filePath)
        {
            // 确保目录存在
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            List<string> lines = [];
            if (File.Exists(filePath))
                lines.AddRange(File.ReadAllLines(filePath, Encoding.UTF8));

            string sectionHeader = "[" + section.Trim() + "]";
            bool sectionFound = false;
            bool keyUpdated = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();
                if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                {
                    if (string.Equals(trimmed, sectionHeader, StringComparison.OrdinalIgnoreCase))
                    {
                        sectionFound = true;
                        // 在此节内查找键（直到下一个节或文件尾）
                        for (int j = i + 1; j < lines.Count; j++)
                        {
                            string line = lines[j].Trim();
                            if (line.StartsWith('[') && line.EndsWith(']'))
                                break; // 进入下一节，停止查找

                            if (!string.IsNullOrEmpty(line) && !line.StartsWith(';') && !line.StartsWith('#'))
                            {
                                int eqIndex = line.IndexOf('=');
                                if (eqIndex > 0)
                                {
                                    string currentKey = line[..eqIndex].Trim();
                                    if (string.Equals(currentKey, key.Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        lines[j] = key.Trim() + "=" + value; // 更新
                                        keyUpdated = true;
                                        break;
                                    }
                                }
                            }
                        }

                        // 若未找到键，则在节末尾插入
                        if (!keyUpdated)
                        {
                            int insertPos = i + 1;
                            while (insertPos < lines.Count && !(lines[insertPos].Trim().StartsWith('[') && lines[insertPos].Trim().EndsWith(']')))
                            {
                                insertPos++;
                            }
                            lines.Insert(insertPos, key.Trim() + "=" + value);
                        }
                        break;
                    }
                }
            }

            // 若未找到节，则追加新节
            if (!sectionFound)
            {
                if (lines.Count > 0 && !string.IsNullOrEmpty(lines[^1])) lines.Add(""); // 加空行分隔
                lines.Add(sectionHeader);
                lines.Add(key.Trim() + "=" + value);
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
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
