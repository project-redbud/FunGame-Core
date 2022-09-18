using FunGame.Core.Api.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FunGame.Core.Api.Util
{
    #region 通用工具类

    /// <summary>
    /// 通用工具类，客户端和服务器端都可以直接调用的工具方法都可以写在这里
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

    #endregion

    #region 读写INI文件工具类

    public class INIHelper
    {
        /*
         * 声明API函数
         */
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        public static void WriteINI(string Section, string Key, string Value, string FileName = @"FunGame.ini")
        {
            WritePrivateProfileString(Section, Key, Value, System.Environment.CurrentDirectory.ToString() + @"\" + FileName);
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
            StringBuilder str = new(256);
            _ = GetPrivateProfileString(Section, Key, "", str, 256, System.Environment.CurrentDirectory.ToString() + @"\" + FileName);
            return str.ToString();
        }

        /// <summary>
        /// 查询ini文件是否存在
        /// </summary>
        /// <param name="FileName">文件名，缺省为FunGame.ini</param>
        /// <returns>是否存在</returns>
        public static bool ExistINIFile(string FileName = @"FunGame.ini")
        {
            return File.Exists(System.Environment.CurrentDirectory.ToString() + @"\" + FileName);
        }

        /// <summary>
        /// 初始化ini模板文件
        /// </summary>
        public static void Init(FunGameEnums.FunGame FunGameType)
        {
            switch(FunGameType)
            {
                case FunGameEnums.FunGame.FunGame_Core:
                case FunGameEnums.FunGame.FunGame_Core_Api:
                case FunGameEnums.FunGame.FunGame_Console:
                case FunGameEnums.FunGame.FunGame_Desktop:
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
                case FunGameEnums.FunGame.FunGame_Server:
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

    #endregion

    #region 接口反射工具类

    /// <summary>
    /// 在FunGame.Core.Api中添加新接口和新实现时，需要：
    /// 在FunGame.Core.Api.Model.Enum.CommonEnums里同步添加InterfaceType、InterfaceMethod
    /// </summary>
    public class ReflectionHelper
    {
        /**
         * 定义需要反射的DLL
         */
        public const string FUNGAME_CORE = "FunGame.Core";

        /**
         * 无需二次修改的
         */
        public static string EXEDocPath = System.Environment.CurrentDirectory.ToString() + "\\"; // 程序目录
        public static string PluginDocPath = System.Environment.CurrentDirectory.ToString() + "\\plugins\\"; // 插件目录

        ////////////////////////////////////////////////////////////////////
        /////////////// * 下 面 是 工 具 类 实 现 * ////////////////
        ///////////////////////////////////////////////////////////////////

        /**
         * 定义反射变量
         */
        private Assembly? Assembly;
        private Type? Type;
        private MethodInfo? Method;
        private object? Instance;

        /// <summary>
        /// 获取FunGame.Core.dll中接口的实现方法
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        private Type? GetFunGameCoreImplement(int Interface)
        {
            // 通过类名获取获取命名空间+类名称
            string ClassName = EnumHelper.GetImplementClassName(Interface);
            List<Type>? Classes = null;
            if (Assembly != null)
            {
                Classes = Assembly.GetTypes().Where(w =>
                    w.Namespace == "FunGame.Core.Implement" &&
                    w.Name.Contains(ClassName)
                ).ToList();
                if (Classes != null && Classes.Count > 0)
                    return Classes[0];
                else return null;
            }
            else return null;
        }

        /// <summary>
        /// 公开方法：获取FUNGAME.CORE.DLL中指定方法的返回值
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        public object? GetFunGameCoreValue(int Interface, int Method)
        {
            Assembly = Assembly.LoadFile(EXEDocPath + @FUNGAME_CORE + ".dll");
            Type = GetFunGameCoreImplement(Interface); // 通过类名获取获取命名空间+类名称
            string MethodName = EnumHelper.GetImplementMethodName(Method); // 获取方法名
            if (Assembly != null && Type != null) this.Method = Type.GetMethod(MethodName); // 从Type中查找方法名
            else return null;
            Instance = Assembly.CreateInstance(Type.Namespace + "." + Type.Name);
            if (Instance != null && this.Method != null)
            {
                object? value = this.Method.Invoke(Instance, Array.Empty<object>()); // 实例方法的调用
                if (value != null)
                    return value;
                else return null;
            }
            else return null;
        }
    }

    #endregion

    #region 枚举反射工具类

    public class EnumHelper
    {
        /// <summary>
        /// 获取实现类类名
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        public static string GetImplementClassName(int Interface)
        {
            foreach (string str in System.Enum.GetNames(typeof(InterfaceType)))
            {
                InterfaceType temp = (InterfaceType)System.Enum.Parse(typeof(InterfaceType), Interface.ToString(), true);
                if (temp.ToString() == str)
                    return temp + "Impl";
            }
            return "";
        }

        /// <summary>
        /// 获取实现类的方法名
        /// </summary>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        public static string GetImplementMethodName(int Method)
        {
            foreach (string str in System.Enum.GetNames(typeof(InterfaceMethod)))
            {
                InterfaceMethod temp = (InterfaceMethod)System.Enum.Parse(typeof(InterfaceMethod), Method.ToString(), true);
                if (temp.ToString() == str)
                    return temp.ToString();
            }
            return "";
        }

        /// <summary>
        /// 获取Socket枚举名
        /// </summary>
        /// <param name="SocketType">Socket枚举</param>
        /// <returns></returns>
        public static string GetSocketTypeName(int SocketType)
        {
            foreach (string str in System.Enum.GetNames(typeof(SocketMessageType)))
            {
                SocketMessageType temp = (SocketMessageType)System.Enum.Parse(typeof(SocketMessageType), SocketType.ToString(), true);
                if (temp.ToString() == str)
                    return temp.ToString();
            }
            return "";
        }
    }

    #endregion
}
