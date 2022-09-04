using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using FunGame.Core.Api.Model.Enum;
using System.Data.Common;

namespace FunGame.Core.Api.Util
{
    /// <summary>
    /// 在FunGame.Core.Api中添加新接口和新实现时，需要：
    /// 1、在这里定义类名和方法名
    /// 2、在FunGame.Core.Api.Model.Enum.CommonEnums里同步添加InterfaceType和InterfaceMethod
    /// 3、在GetClassName(int)、GetMethodName(int)中添加switch分支
    /// </summary>
    public class AssemblyHelper
    {
        /**
         * 定义类名
         */
        public const string ClientConnectInterface = "ClientConnectInterface";
        public const string ServerInterface = "ServerInterface";

        /**
         * 定义方法名
         */
        public const string RemoteServerIP = "RemoteServerIP";
        public const string DBConnection = "DBConnection";

        /// <summary>
        /// 获取实现类类名（xxInterfaceImpl）
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        private string GetClassName(int Interface)
        {
            return Interface switch
            {
                (int)CommonEnums.InterfaceType.ClientConnectInterface => ClientConnectInterface + Implement,
                (int)CommonEnums.InterfaceType.ServerInterface => ServerInterface + Implement,
                _ => "",
            };
        }

        /// <summary>
        /// 获取方法名
        /// </summary>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        private string GetMethodName(int Method)
        {
            // 通过AssemblyHelperType来获取方法名
            return Method switch
            {
                (int)CommonEnums.InterfaceMethod.RemoteServerIP => RemoteServerIP,
                (int)CommonEnums.InterfaceMethod.DBConnection => DBConnection,
                _ => "",
            };
        }

        /**
         * 定义需要反射的DLL
         */
        public const string FUNGAME_CORE = "FunGame.Core";

        /**
         * 无需二次修改的
         */
        public const string Implement = "Impl"; // 实现类的后缀
        public static string EXEDocPath = System.Environment.CurrentDirectory.ToString() + "\\"; // 程序目录
        public static string PluginDocPath = System.Environment.CurrentDirectory + "\\plugins\\"; // 插件目录

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
            string ClassName = GetClassName(Interface);
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
            string MethodName = GetMethodName(Method); // 获取方法名
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
}
